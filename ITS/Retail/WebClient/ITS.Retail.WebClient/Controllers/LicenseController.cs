#if (_RETAIL_WEBCLIENT || _RETAIL_DUAL) && _LICENSED
using DevExpress.Xpo;
using ITS.Licensing.Client.Core;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.WebClient.AuxillaryClasses;
using ITS.Retail.WebClient.Extensions;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.WebClient.ViewModel;
using System;
using System.Web.Mvc;

namespace ITS.Retail.WebClient.Controllers
{
    [LicenseOverride]
    public class LicenseController : Controller
    {
        [LocalhostOnlyAuthorize]
        public ActionResult Index()
        {
            LicenseStatusViewModel licenseStatusViewModel = new LicenseStatusViewModel();
            if (System.IO.File.Exists(MvcApplication.LICENSE_FILE) && MvcApplication.USES_LICENSE)
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    int currentUsersCount = MvcApplication.ActiveUsersValidator.NumberOfActiveUsers;
                    int currentPeripheralUsersCount = MvcApplication.ActiveUsersValidator.NumberOfActivePeripheralUsers;
                    int currentTabletSmartPhoneUsersCount = MvcApplication.ActiveUsersValidator.NumberOfActiveTabletSmartPhoneUsers;
                    string machineID = MvcApplication.GetMachineID();
                    Guid assemblyID = MvcApplication.GetAssemblyID();
                    Version currentVersion = MvcApplication.GetVersion();
                    try
                    {
                        GetLicenseStatusResult result = MvcApplication.LicenseManager.GetLicenseStatus(currentUsersCount,
                                                                                                       currentPeripheralUsersCount,
                                                                                                       currentTabletSmartPhoneUsersCount,
                                                                                                       machineID,
                                                                                                       assemblyID,
                                                                                                       currentVersion
                                                                                                       );
                        licenseStatusViewModel.LicenseStatus = result.LicenseStatus;
                        MvcApplication.SetLicenseStatus(result.LicenseStatus);
                        licenseStatusViewModel.ErrorMessage = result.InvalidStatusReason;
                    }
                    catch (Exception ex)
                    {
                        licenseStatusViewModel.ErrorMessage = ex.Message;
                        //MvcApplication.Log.Error(ex, "Error getting License Status");
                        MvcApplication.WRMLogModule.Log(ex);
                    }
                    //read file after communicating with the server
                    LicenseInfo licenseInfo = MvcApplication.LicenseManager.ReadStoredLicenseInfo();
                    licenseStatusViewModel.StartDate = licenseInfo.StartDate;
                    licenseStatusViewModel.EndDate = licenseInfo.EndDate;
                    licenseStatusViewModel.CurrentUsers = currentUsersCount;
                    licenseStatusViewModel.MaxUsers = licenseInfo.MaxUsers;
                    licenseStatusViewModel.MaxPeripheralUsers = licenseInfo.MaxPeripheralsUsers;
                    licenseStatusViewModel.MaxTabletSmartPhoneUsers = licenseInfo.MaxTabletSmartPhoneUsers;
                    licenseStatusViewModel.LicenseType = licenseInfo.LicenseType;
                    licenseStatusViewModel.DaysToAlertBeforeExpiration = licenseInfo.DaysToAlertBeforeExpiration;
                    licenseStatusViewModel.GreyZoneDays = licenseInfo.GreyZoneDays;
                }
            }
            return View(licenseStatusViewModel);
        }

        [LocalhostOnlyAuthorize]
        public ActionResult Activate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [LocalhostOnlyAuthorize]
        public ActionResult Activate(LicenseActivationViewModel model)
        {
            if (ModelState.IsValid && MvcApplication.USES_LICENSE)
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    string machineID = MvcApplication.GetMachineID();
                    string machineName = Platform.PlatformConstants.HEADQUARTERS;
                    if ( MvcApplication.ApplicationInstance != eApplicationInstance.RETAIL )
                    {
                        machineName = String.Format("({0}) {1}", StoreControllerAppiSettings.CurrentStore.Code, StoreControllerAppiSettings.CurrentStore.Name);
                    }
                    Guid assemblyID = MvcApplication.GetAssemblyID();
                    Version currentVersion = MvcApplication.GetVersion();
                    try
                    {
                        MvcApplication.LicenseManager.RequestActivation(machineName, machineID, model.SerialNumber, assemblyID, currentVersion);
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        model.ActivationError = ex.Message;
                        //MvcApplication.Log.Error(ex, "Error during activation");
                        MvcApplication.WRMLogModule.Log(ex);
                        return View(model);
                    }
                }
            }

            return View(model);
        }

        public ActionResult NotActivated()
        {
            return View();
        }

    }
}
#endif