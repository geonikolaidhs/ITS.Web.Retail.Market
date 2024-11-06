using ITS.Retail.WebClient.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ITS.Retail.ResourcesLib;
using ITS.Retail.Model;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model.NonPersistant;
using ITS.Retail.Platform.Enumerations;
using DevExpress.Xpo.DB;

namespace ITS.Retail.WebClient.Areas.B2C.Controllers
{
    public class UserController : BaseController
    {

        public ActionResult ResetPassword()
        {
            if (!IsUserLoggedIn)
            {
                Danger = Resources.PleaseLogin;
                return new RedirectResult("~/B2C/User/Login");
            }
            ViewBag.Title = Resources.UpdatePassword;
            ViewBag.MetaDescription = Resources.UpdatePassword;
            return View();
        }

        public ActionResult ResetPasswordCallbackPanel()
        {
            return PartialView();
        }

        public ActionResult ResetPasswordForm()
        {
            return PartialView();
        }

        public JsonResult ChangePassword()
        {
            if (!IsUserLoggedIn)
            {
                return Json(new { success = false });
            }

            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                try
                {
                    if (!IsUserLoggedIn)
                    {
                       throw new Exception(Resources.PleaseLogin);
                    }
                    string oldPassword = Request["OldPassword"];
                    string resetPassword = Request["ResetPassword"];
                    string confirmResetPassword = Request["ConfirmResetPassword"];
                    if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(resetPassword) || string.IsNullOrEmpty(confirmResetPassword))
                    {
                        Danger = Resources.PLEASE_FILL_ALL_REQUIRED_FIELDS;
                    }
                    else if (UserHelper.EncodePassword(oldPassword) != CurrentUser.Password)
                    {
                        Danger = Resources.InvalidPassword;
                    }
                    else if (resetPassword != confirmResetPassword)
                    {
                        Danger = Resources.PasswordsDoNotMatch;
                    }
                    else if (UserHelper.CheckPasswordStrength(resetPassword) == false)
                    {
                        Danger = Resources.PasswordDoesNotMeetRequirements;
                    }
                    else if (oldPassword == resetPassword)
                    {
                        Danger = Resources.NewPasswordCannotBeTheSameAsTheOldPassword;
                    }
                    else
                    {
                        User user = uow.GetObjectByKey<User>(CurrentUser.Oid);
                        user.Password = UserHelper.EncodePassword(resetPassword);
                        user.Save();
                        uow.CommitTransaction();
                        CurrentUser.Reload();
                        Success = Resources.SavedSuccesfully;
                    }
                }
                catch (Exception exception)
                {
                    uow.RollbackTransaction();
                    Danger = exception.GetFullMessage();
                }
            }
            return Json(new { success = string.IsNullOrEmpty(Danger) });
        }


        //[Authorize]
        public ActionResult ForgotPassword()
        {
            if (IsUserLoggedIn)
            {
                return Request.UrlReferrer == null ? new RedirectResult("~/B2C") : new RedirectResult(Request.UrlReferrer.ToString());
            }
            if (Request.HttpMethod == "POST")
            {
                string email = Request["email"];
                if (String.IsNullOrEmpty(email)){
                    Session["Danger"] = Resources.EmailIsRequired;
                }
                else if (CustomerHelper.IsValidEmail(email) == false)
                {
                    Session["Danger"] = Resources.InvalidEmail;
                }
                else
                {
                    CriteriaOperator criteria = CriteriaOperator.And(new BinaryOperator("IsB2CUser", true),
                                                                     new BinaryOperator("IsActive", true),
                                                                     new BinaryOperator("Email", email)
                                                                    );

                    using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                    {
                        User user = uow.FindObject<User>(criteria);
                        if (user == null)
                        {
                            Session["Danger"] = Resources.InvalidEmail;
                        }
                        else
                        {
                            ForgotPasswordToken forgotPasswordToken = UserHelper.GenerateUserToken<ForgotPasswordToken>(user);
                            forgotPasswordToken.Save();
                            uow.CommitTransaction();
                            if (CurrentCompany.OwnerApplicationSettings != null && !string.IsNullOrEmpty(CurrentCompany.OwnerApplicationSettings.SmtpEmailAddress)
                        && !string.IsNullOrEmpty(CurrentCompany.OwnerApplicationSettings.SmtpHost)
                        && !string.IsNullOrEmpty(CurrentCompany.OwnerApplicationSettings.SmtpPassword)
                        && !string.IsNullOrEmpty(CurrentCompany.OwnerApplicationSettings.SmtpUsername)
                        )
                            {
                                ForgotPasswordEmailTemplate forgotPasswordEmailTemplate = new ForgotPasswordEmailTemplate();
                                forgotPasswordEmailTemplate.WelcomeMessage = Resources.WelcomeB2C + " " + user.FullName;
                                forgotPasswordEmailTemplate.URL = Url.Action("GeneratePassword", "User", new { token = forgotPasswordToken.TokenString }, this.Request.Url.Scheme);
                                string emailBody = RenderViewToString(forgotPasswordEmailTemplate);
                                MailHelper.SendMailMessage(CurrentCompany.OwnerApplicationSettings.SmtpEmailAddress,
                                    new List<string>() { user.Email },
                                    forgotPasswordEmailTemplate.WelcomeMessage, emailBody, CurrentCompany.OwnerApplicationSettings.SmtpHost,
                                    username: CurrentCompany.OwnerApplicationSettings.SmtpUsername,
                                    password: CurrentCompany.OwnerApplicationSettings.SmtpPassword,
                                    port: (string.IsNullOrEmpty(CurrentCompany.OwnerApplicationSettings.SmtpPort) ? "25" : CurrentCompany.OwnerApplicationSettings.SmtpPort),
                                    enableSSL: CurrentCompany.OwnerApplicationSettings.SmtpUseSSL
                                    );
                            }

                            Session["Success"] = Resources.PleaseCheckYourEmailForForgotPasswordToken;
                        }
                    }
                }
            }
            ViewBag.Title = Resources.ForgotYourPassword;
            ViewBag.MetaDescription = Resources.ForgotYourPassword;
            return View();
        }

        public ActionResult GeneratePassword(string token)
        {
            if (IsUserLoggedIn)
            {
                return Request.UrlReferrer == null ? new RedirectResult("~/B2C") : new RedirectResult(Request.UrlReferrer.ToString());
            }

            GenerateUnitOfWork();

            if(string.IsNullOrEmpty(token)==false)
            {
                ForgotPasswordToken forgotPasswordToken = uow.FindObject<ForgotPasswordToken>(new BinaryOperator("TokenString",token));
                if (forgotPasswordToken!=null && forgotPasswordToken.IsValid)
                {
                    try
                    {
                        string passphrase = "";
                        forgotPasswordToken.User.Password = UserHelper.GeneratePassword(out passphrase);
                        forgotPasswordToken.User.Save();
                        User user = forgotPasswordToken.User;
                        forgotPasswordToken.Delete();
                        uow.CommitTransaction();

                        if (CurrentCompany.OwnerApplicationSettings != null && !string.IsNullOrEmpty(CurrentCompany.OwnerApplicationSettings.SmtpEmailAddress)
                       && !string.IsNullOrEmpty(CurrentCompany.OwnerApplicationSettings.SmtpHost)
                       && !string.IsNullOrEmpty(CurrentCompany.OwnerApplicationSettings.SmtpPassword)
                       && !string.IsNullOrEmpty(CurrentCompany.OwnerApplicationSettings.SmtpUsername)
                       )
                        {
                            GeneratePasswordEmailTemplate generatePasswordEmailTemplate = new GeneratePasswordEmailTemplate();
                            generatePasswordEmailTemplate.WelcomeMessage = Resources.WelcomeB2C + " " + user.FullName;
                            generatePasswordEmailTemplate.Password = passphrase;
                            string emailBody = RenderViewToString(generatePasswordEmailTemplate);
                            MailHelper.SendMailMessage(CurrentCompany.OwnerApplicationSettings.SmtpEmailAddress,
                                new List<string>() { user.Email },
                                generatePasswordEmailTemplate.WelcomeMessage, emailBody, CurrentCompany.OwnerApplicationSettings.SmtpHost,
                                username: CurrentCompany.OwnerApplicationSettings.SmtpUsername,
                                password: CurrentCompany.OwnerApplicationSettings.SmtpPassword,
                                port: (string.IsNullOrEmpty(CurrentCompany.OwnerApplicationSettings.SmtpPort) ? "25" : CurrentCompany.OwnerApplicationSettings.SmtpPort),
                                enableSSL: CurrentCompany.OwnerApplicationSettings.SmtpUseSSL
                                );
                        }


                        Success = Resources.PleaseCheckYourEmail;
                    }
                    catch (Exception exception)
                    {
                        string errorMesage = exception.GetFullMessage();
                        Danger = errorMesage;
                        uow.RollbackTransaction();
                    }
                    finally
                    {
                        DisposeUnitOfWork();
                    }
                }
                else
                {
                    Session["Danger"] = Resources.AnErrorOccurred;
                    return new RedirectResult("~/B2C/User/ForgotPassword");
                }
            }

            return Request.UrlReferrer == null ? new RedirectResult("~/B2C") : new RedirectResult(Request.UrlReferrer.ToString());
        }

        public ActionResult VerifyEmail(string token)
        {
            if (IsUserLoggedIn)
            {
                return Request.UrlReferrer == null ? new RedirectResult("~/B2C") : new RedirectResult(Request.UrlReferrer.ToString());
            }

            GenerateUnitOfWork();

            if(string.IsNullOrEmpty(token)==false)
            {
                VerifyEmailToken verifyEmailToken = uow.FindObject<VerifyEmailToken>(new BinaryOperator("TokenString", token));
                if (verifyEmailToken != null && verifyEmailToken.IsValid)
                {
                    try
                    {
                        verifyEmailToken.User.IsActive = true;
                        verifyEmailToken.User.IsApproved = true;
                        verifyEmailToken.User.Save();
                        verifyEmailToken.Delete();
                        uow.CommitTransaction();
                        Session["Success"] = Resources.YourAccountIsActivated;
                    }
                    catch (Exception exception)
                    {
                        string errorMesage = exception.GetFullMessage();
                        Session["Danger"] = errorMesage;
                        uow.RollbackTransaction();
                    }
                    finally
                    {
                        DisposeUnitOfWork();
                    }
                }
                else
                {
                    Session["Danger"] = Resources.AnErrorOccurred;
                }
            }
            else
            {
                Session["Danger"] = Resources.AnErrorOccurred;
                return new RedirectResult("~/B2C/User/ForgotPassword");
            }
            return Request.UrlReferrer == null ? new RedirectResult("~/B2C") : new RedirectResult(Request.UrlReferrer.ToString());
        }

        public ActionResult Login()
        {
            if(IsUserLoggedIn)
            {
                //fix for case 5605
                return Request.UrlReferrer == null || Request.UrlReferrer.ToString().EndsWith("/User/Login") ? new RedirectResult("~/B2C") : new RedirectResult(Request.UrlReferrer.ToString());
            }

            ViewBag.Title = Resources.LoginUser;
            ViewBag.MetaDescription = Resources.LoginUser;
            return View("Login");
        }

        public ActionResult OrderHistory()
        {
            if (!IsUserLoggedIn)
            {
                Danger = Resources.PleaseLogin;
                return new RedirectResult("~/B2C/User/Login");
            }

            ViewBag.Title = Resources.Order;
            ViewBag.MetaDescription = Resources.Order;

            return View(GetCurrentUserOrders());
        }

        public ActionResult OrderDetailsCallbackPanel(Guid? documentGuid)
        { 
            if(documentGuid==null){
                documentGuid = Guid.Empty;
            }

            DocumentHeader documentHeader = XpoHelper.GetNewUnitOfWork().FindObject<DocumentHeader>(new BinaryOperator("Oid", (Guid)documentGuid, BinaryOperatorType.Equal));

            ViewBag.DocumentHeader = documentHeader;
            return PartialView();
        } 

        public ActionResult Grid()
        {
            return PartialView(GetCurrentUserOrders());
        }

        private XPCollection<DocumentHeader> GetCurrentUserOrders()
        {
            Guid wishlidtOid = CurrentUser != null && CurrentUser.WishList != null ? CurrentUser.WishList.Oid : Guid.Empty;
            CriteriaOperator criteria = CriteriaOperator.And(new BinaryOperator("Customer.Oid", CurrentCustomer.Oid),
                                                              new BinaryOperator("Source", DocumentSource.B2C),
                                                              new BinaryOperator("IsCanceled", false),
                                                              new NotOperator(new BinaryOperator("Oid", wishlidtOid))
                                                            );
            XPCollection<DocumentHeader> documentHeaders = new XPCollection<DocumentHeader>(XpoHelper.GetNewUnitOfWork(), criteria, new SortProperty("FinalizedDate",SortingDirection.Descending));
            return documentHeaders;
        }

    }
}
