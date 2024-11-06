#if (_RETAIL_WEBCLIENT || _RETAIL_DUAL) && _LICENSED

using DevExpress.Xpo;
using ITS.Retail.WebClient.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.Model;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Common.ViewModel;
using ITS.Retail.Common;
using ITS.Retail.ResourcesLib;
using ITS.Licensing.Client.Core;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.Model.NonPersistant;

namespace ITS.Retail.WebClient.Controllers
{
    public class LicenseManagementController : BaseController
    {
        [Display(ShowSettings = true)]
        public ActionResult Index()
        {
            if ( !UserHelper.IsAdmin(CurrentUser) )
            {
                return new RedirectResult("~/Home/Index");
            }
            CreateViewBags();
            return View(GetUserDistribution());
        }

        private void CreateViewBags()
        {
            LicenseInfo licenseInfo = MvcApplication.LicenseManager.ReadStoredLicenseInfo();
            ViewBag.UserLimit = licenseInfo.MaxUsers;
            ViewBag.PeripheralsUserLimit = licenseInfo.MaxPeripheralsUsers;
            ViewBag.TabletSmartPhoneUserLimit = licenseInfo.MaxTabletSmartPhoneUsers;
        }

        [Display(ShowSettings = true)]
        public ActionResult Save()
        {
            string maxConnectedUsersPrefix = "ConnectedUsers";
            string maxPeripheralUsersPrefix = "Peripheral";
            string maxTabletSmartPhoneUsersPrefix = "TabletSmartPhone";
            string licenseServerInstancePrefix = "hidden" + maxConnectedUsersPrefix;
            CreateViewBags();

            int countDistributedUsers = 0;
            int countPeripheralDistributedUsers = 0;
            int countTabletSmartPhoneDistributedUsers = 0;

            IEnumerable<string> storeControllers = Request.Params.AllKeys.ToList().Where(parameter => parameter.StartsWith(maxConnectedUsersPrefix)).Select(param => param.Replace(maxConnectedUsersPrefix,""));
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                CriteriaOperator masterLicenceCriteria = CriteriaOperator.And( new BinaryOperator("Description", Platform.PlatformConstants.HEADQUARTERS),
                                                                               new BinaryOperator("Server", Guid.Empty)
                                                                             );//RetailHelper.ApplyOwnerCriteria
                LicenseUserDistribution masterUserDistribution = uow.FindObject<LicenseUserDistribution>(masterLicenceCriteria);
                if (masterUserDistribution == null)
                {
                    masterUserDistribution = new LicenseUserDistribution(uow)
                    {
                        Description = Platform.PlatformConstants.HEADQUARTERS,
                        Server = Guid.Empty
                    };
                }

                foreach (string storeController in storeControllers)
                {
                    Guid serverGuid;
                    if (Guid.TryParse(storeController, out serverGuid))
                    {
                        LicenseUserDistribution licenseUserDistribution = uow.FindObject<LicenseUserDistribution>( new BinaryOperator("Server",serverGuid));
                        if (licenseUserDistribution == null)
                        {
                            licenseUserDistribution = new LicenseUserDistribution(uow);
                        }

                        ServerLicenseInfo storeControllerLicenseInfo = licenseUserDistribution.ServerLicenseInfo;
                        if ( storeControllerLicenseInfo == null )
                        {
                            storeControllerLicenseInfo = new ServerLicenseInfo();
                        }
                        storeControllerLicenseInfo.StartDate = MvcApplication.LicenseStatusViewModel.StartDate;
                        storeControllerLicenseInfo.EndDate = MvcApplication.LicenseStatusViewModel.EndDate;                        
                        storeControllerLicenseInfo.DaysToAlertBeforeExpiration = MvcApplication.LicenseStatusViewModel.DaysToAlertBeforeExpiration;
                        storeControllerLicenseInfo.GreyZoneDays = MvcApplication.LicenseStatusViewModel.GreyZoneDays;

                        int maxConnectedUsers;
                        if (int.TryParse(Request[maxConnectedUsersPrefix + storeController], out maxConnectedUsers))
                        {
                            if( maxConnectedUsers <= 0 )
                            {
                                Session["Error"] = Resources.InvalidValue + " " + maxConnectedUsers;
                                return View("Index", GetUserDistribution());
                            }
                            storeControllerLicenseInfo.MaxConnectedUsers = maxConnectedUsers;
                        }
                        countDistributedUsers += storeControllerLicenseInfo.MaxConnectedUsers;

                        int maxPeripheralUsers;
                        if (int.TryParse(Request[maxPeripheralUsersPrefix + storeController], out maxPeripheralUsers))
                        {
                            if (maxPeripheralUsers <= -1)
                            {
                                Session["Error"] = Resources.InvalidValue + " " + maxPeripheralUsers;
                                return View("Index", GetUserDistribution());
                            }
                            storeControllerLicenseInfo.MaxPeripheralsUsers = maxPeripheralUsers;
                        }
                        countPeripheralDistributedUsers += storeControllerLicenseInfo.MaxPeripheralsUsers;

                        int maxTabletSmartPhoneUsers;
                        if (int.TryParse(Request[maxTabletSmartPhoneUsersPrefix + storeController], out maxTabletSmartPhoneUsers))
                        {
                            if (maxTabletSmartPhoneUsers <= -1)
                            {
                                Session["Error"] = Resources.InvalidValue + " " + maxTabletSmartPhoneUsers;
                                return View("Index", GetUserDistribution());
                            }
                            storeControllerLicenseInfo.MaxTabletSmartPhoneUsers = maxTabletSmartPhoneUsers;
                        }
                        countTabletSmartPhoneDistributedUsers += storeControllerLicenseInfo.MaxTabletSmartPhoneUsers;

                        LicenseServerInstance licenseServerInstance;
                        if (Enum.TryParse<LicenseServerInstance>(Request[licenseServerInstancePrefix + storeController], out licenseServerInstance))
                        {
                            licenseUserDistribution.LicenseServerInstance = licenseServerInstance;
                        }
                        licenseUserDistribution.Server = serverGuid;
                        StoreControllerSettings storeControllerSettings = uow.GetObjectByKey<StoreControllerSettings>(serverGuid);
                        if (storeControllerSettings != null)
                        {
                            licenseUserDistribution.Description = storeControllerSettings.Store.Name;
                        }

                        licenseUserDistribution.SetInfo(storeControllerLicenseInfo);
                        licenseUserDistribution.Save();
                    }
                }

                LicenseInfo licenseInfo = MvcApplication.LicenseManager.ReadStoredLicenseInfo();
                int MaximumAllowedUsers = licenseInfo.MaxUsers;
                int MaximumAllowedPeripheralUsers = licenseInfo.MaxPeripheralsUsers;
                int MaximumAllowedTabletSmartPhoneUsers = licenseInfo.MaxTabletSmartPhoneUsers;

                ServerLicenseInfo masterLicenseInfo = masterUserDistribution.ServerLicenseInfo;
                if (masterLicenseInfo == null)
                {
                    masterLicenseInfo = new ServerLicenseInfo();
                }

                masterLicenseInfo.MaxConnectedUsers = MaximumAllowedUsers - countDistributedUsers;
                masterLicenseInfo.MaxPeripheralsUsers = MaximumAllowedPeripheralUsers - countPeripheralDistributedUsers;
                masterLicenseInfo.MaxTabletSmartPhoneUsers = MaximumAllowedTabletSmartPhoneUsers - countTabletSmartPhoneDistributedUsers;                

                Session["Error"] = String.Empty;
                if (masterLicenseInfo.MaxConnectedUsers >= 0 && masterLicenseInfo.MaxConnectedUsers < 1)
                {
                    Session["Error"] += String.Format(Resources.AtLeastOneWebUserMustRemainForHeadQuartersButRemain, masterLicenseInfo.MaxConnectedUsers) + Environment.NewLine;
                }
                else if( countDistributedUsers > MaximumAllowedUsers )
                {
                    Session["Error"] += String.Format(Resources.MaximumAllowedUsersAreAndCounted, MaximumAllowedUsers, countDistributedUsers) + Environment.NewLine;
                }

                if (masterLicenseInfo.MaxPeripheralsUsers < 0)
                {
                    Session["Error"] += String.Format(Resources.MaximumAllowedPeripheralUsersAreAndCounted, MaximumAllowedPeripheralUsers, countPeripheralDistributedUsers) + Environment.NewLine;
                }
                if (masterLicenseInfo.MaxPeripheralsUsers < 0)
                {
                    Session["Error"] += String.Format(Resources.MaximumAllowedTabletSmartPhoneUsersAreAndCounted, MaximumAllowedTabletSmartPhoneUsers, countTabletSmartPhoneDistributedUsers) + Environment.NewLine;
                }

                if ((Session["Error"] == null || String.IsNullOrEmpty(Session["Error"].ToString()) || String.IsNullOrWhiteSpace(Session["Error"].ToString()))
                    && masterLicenseInfo.MaxConnectedUsers >= 1 //One must remain for master
                    && masterLicenseInfo.MaxPeripheralsUsers >= 0 //Sum does not overcome license total
                    && masterLicenseInfo.MaxTabletSmartPhoneUsers >= 0 //Sum does not overcome license total
                   )
                {
                    masterUserDistribution.SetInfo(masterLicenseInfo);
                    masterUserDistribution.Save();

                    XpoHelper.CommitTransaction(uow);
                    Session["Notice"] = Resources.SavedSuccesfully;
                }
            }
            return new RedirectResult("~/LicenseManagement/Index");
        }

        private List<LicenseUserDistributionViewModel> GetUserDistribution()
        {
            List<LicenseUserDistributionViewModel> userDistributions = new List<LicenseUserDistributionViewModel>();

            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                foreach (LicenseUserDistribution distribution in new XPCollection<LicenseUserDistribution>(uow, new BinaryOperator("LicenseServerInstance", LicenseServerInstance.STORE_CONTROLLER)))
                {
                    LicenseUserDistributionViewModel distributionViewModel = new LicenseUserDistributionViewModel();
                    distributionViewModel.LoadPersistent(distribution);
                    userDistributions.Add(distributionViewModel);
                }

                List<Guid> storeControllersWithDefinedUsers = userDistributions.Select(licenseDistribution => licenseDistribution.Server).ToList<Guid>();

                XPCollection<StoreControllerSettings> storeControllers = new XPCollection<StoreControllerSettings>(uow,
                                                                                                                   BaseController.ApplyOwnerCriteria(
                                                                                                                                   new NotOperator(
                                                                                                                                       new InOperator("Oid",
                                                                                                                                                       storeControllersWithDefinedUsers)
                                                                                                                                                      ),
                                                                                                                                   typeof(StoreControllerSettings)
                                                                                                                                   )
                                                                                                                    );
                foreach( StoreControllerSettings storeController in storeControllers )
                {
                    LicenseUserDistributionViewModel userDistribution = new LicenseUserDistributionViewModel();
                    userDistribution.Server = storeController.Oid;
                    userDistribution.LicenseServerInstance = LicenseServerInstance.STORE_CONTROLLER;
                    userDistribution.Description = storeController.Store.Name;
                    userDistributions.Add(userDistribution);                    
                }

                LicenseUserDistribution headquartersLicenseUserDistribution = uow.FindObject<LicenseUserDistribution>(LicenseUserDistributionHelper.GetHeadquartersDistributionCriteria());
                if ( headquartersLicenseUserDistribution != null )
                {
                    LicenseUserDistributionViewModel userDistribution = new LicenseUserDistributionViewModel();
                    userDistribution.MaxConnectedUsers = headquartersLicenseUserDistribution.MaxConnectedUsers;
                    userDistribution.MaxPeripheralsUsers = headquartersLicenseUserDistribution.MaxPeripheralsUsers;
                    userDistribution.MaxTabletSmartPhoneUsers = headquartersLicenseUserDistribution.MaxTabletSmartPhoneUsers;
                    userDistribution.Server = Guid.Empty;
                    userDistribution.LicenseServerInstance = LicenseServerInstance.MASTER_OR_DUAL;
                    userDistribution.Description = Platform.PlatformConstants.HEADQUARTERS;
                    userDistributions.Add(userDistribution);
                }
            }

            return userDistributions;
        }
    }
}

#endif