using System;
using System.Web.Mvc;
using ITS.Retail.Model;
using DevExpress.Data.Filtering;
using ITS.Retail.WebClient.Providers;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using DevExpress.Web.Mvc;
using ITS.Retail.WebClient.Extensions;
using WebMatrix.WebData;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Common;
using ITS.Retail.WebClient.AuxillaryClasses;
using ITS.Retail.WebClient.ViewModel;

namespace ITS.Retail.WebClient.Controllers
{
   

    [LicensedAuthorize]
    public class LoginController : BaseController
    {
        [ActionLog(LogLevel = LogLevel.Basic)]
        [OutputCache(VaryByParam = "ReturnUrl", Duration = 3600)]
        public ActionResult Index(string ReturnUrl)
        {
            DevExpressHelper.Theme = "ITSTheme1";
            MvcApplication.ActiveUsersValidator.RemoveApplicationUser(this.Session.SessionID);
            WebSecurity.Logout();
            return View(model: ReturnUrl);
        }

        [ActionLog(LogLevel = LogLevel.Basic), HttpPost]
        public ActionResult Index([ModelBinder(typeof(RetailModelBinder))] LoginBindingModel user)
        {
            DevExpressHelper.Theme = "ITSTheme1";
            Session.Clear();
            Session["userOwner"] = null;
            try
            {
                if (WebSecurity.Login(user.Username, user.Password, user.RememberMe.Value))
                {
                    User currentUser = XpoSession.FindObject<User>(new BinaryOperator("UserName", user.Username));
                    string message = "";
                    if (UserHelper.UserCanLoginToCurrentStore(currentUser, MvcApplication.ApplicationInstance, StoreControllerAppiSettings.CurrentStoreOid, out message))
                    {
                        Guid serverID = MvcApplication.ApplicationInstance == eApplicationInstance.RETAIL ? Guid.Empty : StoreControllerAppiSettings.StoreControllerOid;
                        if(MvcApplication.USES_LICENSE)
                        {
                            int licenseManagerMaxUseres = MvcApplication.ReadStoredLicenseInfo().MaxUsers;

                            int maximumAllowedUsers = MvcApplication.ApplicationInstance == eApplicationInstance.DUAL_MODE
                                                      ? licenseManagerMaxUseres
                                                      : LicenseUserDistributionHelper.GetMaximumAllowedUsers(MvcApplication.LicenseServerInstanceType, serverID, MvcApplication.ApplicationInstance, licenseManagerMaxUseres);

                            Session["Error"] = "";
                            if (MvcApplication.LicenseStatusViewModel.InGreyZone)
                            {
                                Session["Error"] = MvcApplication.LicenseStatusViewModel.GreyZoneMessage;
                            }

                            if (MvcApplication.ActiveUsersValidator.FreeConnectionSlotExists(maximumAllowedUsers))
                            {
                                if (currentUser.TermsAccepted)
                                {
                                    return new RedirectResult(user.returnUrl ?? "~/");
                                }
                                return new RedirectResult("~/User/TermsAndConditions");
                            }
                            else
                            {
                                Session["Error"] += string.Format(Resources.MaximumAllowedUsersAreAndCounted, maximumAllowedUsers, MvcApplication.ActiveUsersValidator.NumberOfActiveUsers);
                                return View();
                            }
                        }
                        else
                        {
                            if (currentUser.TermsAccepted)
                            {
                                return new RedirectResult(user.returnUrl ?? "~/");
                            }
                            return new RedirectResult("~/User/TermsAndConditions");
                        }
                    }
                    else
                    {
                        WebSecurity.Logout();
                        Session["Error"] = message;
                        return View("Index");
                    }
                }
                Session["Error"] = Resources.Login_Failed;
                return View();

            }
            catch (Exception e)
            {

                Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                return View("Index");
            }
        }

        [AllowAnonymous]
        [OutputCache(Duration = 3600)]
        public ActionResult LoginStylesAndStaticScripts()
        {
            return PartialView();
        }

    }
}
