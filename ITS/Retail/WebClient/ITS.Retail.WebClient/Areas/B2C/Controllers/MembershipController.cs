using DevExpress.Data.Filtering;
using DevExpress.Web.Mvc;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.Model.NonPersistant;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.WebClient.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ITS.Retail.WebClient.Areas.B2C.Controllers
{
    public class MembershipController : BaseController
    {

        UnitOfWork uow;

        protected void GenerateUnitOfWork()
        {

            if (Session["uow"] == null)
            {
                uow = XpoHelper.GetNewUnitOfWork();
                Session["uow"] = uow;
            }
            else
            {
                uow = (UnitOfWork)Session["uow"];
            }
        }

        public ActionResult SignUp()
        {
            if (IsUserLoggedIn)
            {
                return Request.UrlReferrer == null ? new RedirectResult("~/B2C") : new RedirectResult(Request.UrlReferrer.ToString());
            }

            GenerateUnitOfWork();
            ViewBag.MetaDescription = Resources.CreateAccount;
            ViewBag.Title = Resources.CreateAccount;
            return View(new CustomerUser());
        }

        public ActionResult ValidateSignUp([ModelBinder(typeof(RetailModelBinder))] CustomerUser customerUser)
        {
            if (IsUserLoggedIn)
            {
                return Request.UrlReferrer == null ? new RedirectResult("~/B2C") : new RedirectResult(Request.UrlReferrer.ToString());
            }

            GenerateUnitOfWork();            

            string result = string.Empty;
            if (customerUser.IsValid(out result) == false)
            {
                Session["Danger"] = result;
                return View("SignUp", customerUser);
            }

            if (UserHelper.CheckPasswordStrength(customerUser.Password) == false)
            {
                Session["Danger"] = Resources.PasswordDoesNotMeetRequirements;
                return View("SignUp", customerUser);
            }

            if (CustomerHelper.IsValidB2CCustomer(customerUser, out result) == false)
            {
                Session["Danger"] = result;
                return View("SignUp", customerUser);
            }

            using (UnitOfWork uow2 = XpoHelper.GetNewUnitOfWork())
            {
                try
                {
                    CompanyNew Owner = uow2.GetObjectByKey<CompanyNew>(CurrentCompany.Oid);
                    User user = customerUser.SaveB2CUser(uow2, UserHelper.EncodePassword(customerUser.Password), Owner);
                    VerifyEmailToken verifyEmail = null;
                    verifyEmail = UserHelper.GenerateUserToken<VerifyEmailToken>(user);
                    verifyEmail.Save();
                    uow2.CommitTransaction();

                    if (CurrentCompany.OwnerApplicationSettings != null && !string.IsNullOrEmpty(CurrentCompany.OwnerApplicationSettings.SmtpEmailAddress)
                        && !string.IsNullOrEmpty(CurrentCompany.OwnerApplicationSettings.SmtpHost)
                        && !string.IsNullOrEmpty(CurrentCompany.OwnerApplicationSettings.SmtpPassword)
                        && !string.IsNullOrEmpty(CurrentCompany.OwnerApplicationSettings.SmtpUsername)
                        )
                    {
                        SignUpEmailTemplate signUpEmailTemplate = new SignUpEmailTemplate();
                        signUpEmailTemplate.WelcomeMessage = Resources.WelcomeB2C + " " + user.FullName;
                        signUpEmailTemplate.URL = Url.Action("VerifyEmail", "User", new { token = verifyEmail.TokenString }, this.Request.Url.Scheme);
                        string emailBody = RenderViewToString(signUpEmailTemplate);
                        MailHelper.SendMailMessage(CurrentCompany.OwnerApplicationSettings.SmtpEmailAddress,
                            new List<string>() { user.Email },
                            signUpEmailTemplate.WelcomeMessage, emailBody, CurrentCompany.OwnerApplicationSettings.SmtpHost,
                            username: CurrentCompany.OwnerApplicationSettings.SmtpUsername,
                            password: CurrentCompany.OwnerApplicationSettings.SmtpPassword,
                            port: (string.IsNullOrEmpty(CurrentCompany.OwnerApplicationSettings.SmtpPort) ? "25" : CurrentCompany.OwnerApplicationSettings.SmtpPort),
                            enableSSL: CurrentCompany.OwnerApplicationSettings.SmtpUseSSL
                            );
                    }
                    Session["CustomerUser"] = null;
                    Session["Success"] = Resources.AccountSuccessfullyCreated + Environment.NewLine + Resources.PleaseCheckYourEmail;
                }
                catch (Exception exception)
                {
                    Session["Danger"] = exception.GetFullMessage();
                    uow2.RollbackTransaction();
                }
                finally
                {
                    uow.Dispose();
                    uow = null;
                    Session["uow"] = null;
                }
            }

            return Request.UrlReferrer == null ? new RedirectResult("~/B2C") : new RedirectResult(Request.UrlReferrer.ToString());
        }

        public ActionResult SignIn()
        {
            if (IsUserLoggedIn)
            {
                return Request.UrlReferrer == null ? new RedirectResult("~/B2C") : new RedirectResult(Request.UrlReferrer.ToString());
            }

            try
            {
                string email = Request["UserEmail"];
                string password = Request["UserPassword"];
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || CustomerHelper.IsValidEmail(email) == false)
                {
                    throw new Exception(Resources.Login_Failed);
                }
                GenerateUnitOfWork();

                CriteriaOperator crop = CriteriaOperator.And(new BinaryOperator("Email", email),
                                                             new BinaryOperator("Password", UserHelper.EncodePassword(password)),
                                                             new BinaryOperator("IsB2CUser", true),
                                                             new BinaryOperator("IsActive", true)//,
                                                             //new BinaryOperator("IsApproved", true)
                                                            );
                User user = uow.FindObject<User>(crop);
                if (user == null)
                {
                    Danger = Resources.Login_Failed;
                }
                else if (user.IsApproved == false)
                {
                    Danger = Resources.PleaseCheckyourEmailForApproval;
                    string verifyEmailTokenString = string.Empty;

                    using(UnitOfWork uow2 = XpoHelper.GetNewUnitOfWork())
                    {
                        VerifyEmailToken verifyEmail = uow2.FindObject<VerifyEmailToken>( new BinaryOperator("User",user.Oid) );
                        if (verifyEmail == null || verifyEmail.IsValid==false)
                        {
                            User userOnUOW2 = uow2.GetObjectByKey<User>(user.Oid);
                            verifyEmail = UserHelper.GenerateUserToken<VerifyEmailToken>(userOnUOW2);
                            verifyEmail.Save();
                            uow2.CommitTransaction();
                        }
                        verifyEmailTokenString = verifyEmail.TokenString;
                    }

                    if (CurrentCompany.OwnerApplicationSettings != null && !string.IsNullOrEmpty(CurrentCompany.OwnerApplicationSettings.SmtpEmailAddress)
                       && !string.IsNullOrEmpty(CurrentCompany.OwnerApplicationSettings.SmtpHost)
                       && !string.IsNullOrEmpty(CurrentCompany.OwnerApplicationSettings.SmtpPassword)
                       && !string.IsNullOrEmpty(CurrentCompany.OwnerApplicationSettings.SmtpUsername)
                       )
                    {
                        SignUpEmailTemplate signUpEmailTemplate = new SignUpEmailTemplate();
                        signUpEmailTemplate.WelcomeMessage = Resources.WelcomeB2C + " " + user.FullName;
                        signUpEmailTemplate.URL = Url.Action("VerifyEmail", "User", new { token = verifyEmailTokenString }, this.Request.Url.Scheme);
                        string emailBody = RenderViewToString(signUpEmailTemplate);
                        MailHelper.SendMailMessage(CurrentCompany.OwnerApplicationSettings.SmtpEmailAddress,
                            new List<string>() { user.Email },
                            signUpEmailTemplate.WelcomeMessage, emailBody, CurrentCompany.OwnerApplicationSettings.SmtpHost,
                            username: CurrentCompany.OwnerApplicationSettings.SmtpUsername,
                            password: CurrentCompany.OwnerApplicationSettings.SmtpPassword,
                            port: (string.IsNullOrEmpty(CurrentCompany.OwnerApplicationSettings.SmtpPort) ? "25" : CurrentCompany.OwnerApplicationSettings.SmtpPort),
                            enableSSL: CurrentCompany.OwnerApplicationSettings.SmtpUseSSL
                            );
                    }
                }
                else
                {
                    CurrentUser = user;
                    if( ShoppingCart != null )
                    {
                        ShoppingCart.Customer = ShoppingCart.Session.GetObjectByKey<Customer>(CurrentCustomer.Oid);
                    }
                }


            }
            catch (Exception exception)
            {
                Session["Danger"] = exception.Message;
            }

            string redirectLink = "~/B2C";
            if (redirectLink != null && !Request.UrlReferrer.PathAndQuery.Contains("B2C/Membership/SignUp") && !Request.UrlReferrer.PathAndQuery.Contains("B2C/User/ForgotPassword"))
            {
                redirectLink = Request.UrlReferrer.ToString();
            }
            return new RedirectResult(redirectLink);
        }


        public ActionResult Logout()
        {
            BaseController.CurrentUser = null;

            return Request.UrlReferrer == null ? new RedirectResult("~/B2C") : new RedirectResult(Request.UrlReferrer.ToString());
        }

        public ActionResult Profile()
        {
            if (!IsUserLoggedIn)
            {
                Danger = Resources.PleaseLogin;
                return new RedirectResult("~/B2C/User/Login");
            }

            ViewBag.Title = Resources.Profile;
            ViewBag.MetaDescription = Resources.Profile;
            return View();
        }

        public ActionResult ProfileCallbackPanel()
        {
            return PartialView();
        }

        public ActionResult ProfileForm()
        {
            return PartialView();
        }        

        public JsonResult UpdateProfile()
        {

            if (!IsUserLoggedIn)
            {
                return Json(new {success = false});
            }

            ViewBag.Title = Resources.Profile;
            ViewBag.MetaDescription = Resources.Profile;
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                try
                {
                    if (string.IsNullOrEmpty(Request["Email"]) || string.IsNullOrEmpty(Request["FirstName"]) || string.IsNullOrEmpty(Request["LastName"]))
                    {
                        Danger = Resources.PLEASE_FILL_ALL_REQUIRED_FIELDS;
                    }
                    else if (CustomerHelper.IsValidEmail(Request["Email"]) == false)
                    {
                        Danger = Resources.InvalidEmail;
                    }
                    else
                    {
                        User user = uow.GetObjectByKey<User>(CurrentUser.Oid);
                        //user.FullName = Request["FullName"];
                        user.Email = Request["Email"];
                        user.Save();

                        Trader trader = uow.GetObjectByKey<Trader>(CurrentCustomer.Trader.Oid);
                        trader.FirstName = Request["FirstName"];
                        trader.LastName = Request["LastName"];

                        uow.CommitTransaction();

                        CurrentUser.Reload();
                        CurrentCustomer.Reload();

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

        // Addresses
        public ActionResult Address()
        {
            if (!IsUserLoggedIn)
            {
                Danger = Resources.PleaseLogin;
                return new RedirectResult("~/B2C/User/Login");
            }
            ViewBag.Title = Resources.Addresses;
            ViewBag.MetaDescription = Resources.Addresses;
            return View("Address");
        }

        public ActionResult AddressesCallbackPanel()
        {
            return View();
        }
        public ActionResult Addresses()
        {
            return PartialView();
        }
        // END OF Addresse

        public JsonResult AddAddress()
        {
            //using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            UnitOfWork uow = (UnitOfWork)(CurrentCustomer.Session);
            {

                try
                {
                    Address address = new Address(uow);
                    address.Trader = CurrentCustomer.Trader;//uow.GetObjectByKey<Trader>(CurrentCustomer.Trader.Oid);
                    address.IsDefault = false;
                    address.City = Request["City"];
                    address.Street = Request["Street"];
                    address.Region = Request["Region"];
                    address.POBox = Request["POBox"];
                    address.PostCode = Request["PostCode"];

                    Phone phone = new Phone(address.Session);
                    address.Phones.Add(phone);
                    phone.Number = Request["Phone"];
                    phone.Save();

                    Phone fax = new Phone(address.Session);
                    address.Phones.Add(fax);
                    fax.Number = Request["Fax"];
                    fax.Save();


                    if (AddressHelper.IsValidB2CAddress(address) == false)
                    {
                        throw new Exception(Resources.PLEASE_FILL_ALL_REQUIRED_FIELDS);
                    }
                    address.Save();

                    uow.CommitTransaction();
                    CurrentUser.Reload();
                    CurrentCustomer.Trader.Reload();

                    TempData["ActiveAddress"] = CurrentCustomer.Trader.Addresses.Count;
                    Success = Resources.SavedSuccesfully;
                }
                catch (Exception exception)
                {
                    uow.RollbackTransaction();
                    TempData["ActiveAddress"] = 0;
                    TempData["AddAddress"] = true;
                    Danger = exception.GetFullMessage();
                }
            }
            return Json(new { success = string.IsNullOrEmpty(Danger) });
        }

        public JsonResult UpdateAddress()
        {
            Guid addressGuid = Guid.Empty;
            string prefix = "UpdateAddress_";
            string senderName = Request.Params.AllKeys.Where(key => key.StartsWith(prefix)).FirstOrDefault();
            if (string.IsNullOrEmpty(senderName) == false && Guid.TryParse(senderName.Replace(prefix, ""), out addressGuid))
            {
                Address address = CurrentCustomer.Trader.Addresses.Where(addr => addr.Oid == addressGuid).FirstOrDefault();
                if (address != null)
                {
                    //using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                    UnitOfWork uow = (UnitOfWork)(CurrentCustomer.Session);
                    {
                        try
                        {
                            string suffix = "_" + addressGuid.ToString();

                            address = uow.GetObjectByKey<Address>(address.Oid);

                            address.City = Request["City" + suffix];
                            address.Street = Request["Street" + suffix];
                            address.Region = Request["Region" + suffix];
                            address.POBox = Request["POBox" + suffix];
                            address.PostCode = Request["PostCode" + suffix];
                            if (Request["DefaultAddress" + suffix] == "C")
                            {
                                Address previousDefaultAdress = CurrentCustomer.DefaultAddress;//uow.GetObjectByKey<Address>(CurrentCustomer.DefaultAddress.Oid);
                                if (previousDefaultAdress != null)
                                {
                                    previousDefaultAdress.IsDefault = false;
                                    previousDefaultAdress.Save();
                                }
                                Customer customer = CurrentCustomer;//uow.GetObjectByKey<Customer>(CurrentCustomer.Oid);
                                customer.DefaultAddress = address;
                                customer.Save();
                            }
                            address.IsDefault = Request["DefaultAddress" + suffix] == "C";          
                            

                            string phoneNumber = Request["Phone" + suffix];
                            Phone phone = address.Phones.FirstOrDefault();
                            if (string.IsNullOrEmpty(phoneNumber))
                            {
                                if(phone!=null)
                                {
                                    phone.Delete();
                                }
                            }
                            else
                            {                                
                                if (phone == null)
                                {
                                    phone = new Phone(address.Session);
                                    address.Phones.Add(phone);
                                }
                                phone.Number = phoneNumber;
                                phone.Save();
                            }


                            string faxNumber = Request["Fax" + suffix];
                            Phone fax = address.Phones.Count > 1 ? address.Phones[1] : null;
                            if (string.IsNullOrEmpty(faxNumber))
                            {
                                if (fax != null)
                                {
                                    fax.Delete();
                                }
                            }
                            else
                            {                                
                                if (fax == null)
                                {
                                    fax = new Phone(address.Session);
                                    address.Phones.Add(fax);
                                }
                                fax.Number = faxNumber;
                                fax.Save();
                            }


                            if (AddressHelper.IsValidB2CAddress(address) == false)
                            {
                                throw new Exception(Resources.PLEASE_FILL_ALL_REQUIRED_FIELDS);
                            }
                            address.Save();

                            uow.CommitTransaction();
                            Success = Resources.SavedSuccesfully;


                        }
                        catch (Exception exception)
                        {                            
                            uow.RollbackTransaction();
                            Danger = exception.GetFullMessage();
                        }
                    }
                    CurrentUser.Reload();

                    int counter = 0;
                    foreach( Address currentAddress in CurrentCustomer.Trader.Addresses){
                        counter++;
                        if(currentAddress.Oid == address.Oid){
                            TempData["ActiveAddress"] = counter;
                        }
                    
                    }
                    
                }
                else
                {
                    TempData["ActiveAddress"] = 1;
                    Danger = Resources.AnErrorOccurred;
                }
            }
            else
            {
                TempData["ActiveAddress"] = 1;
                Danger = Resources.AnErrorOccurred;
            }

            return Json(new { success = string.IsNullOrEmpty(Danger) });
        }

        public JsonResult DeleteAddress(Guid? AddressOid)
        {
            if(AddressOid != null)
            {

                if (CurrentCustomer.Trader.Addresses.Count == 1)
                {
                    Danger = Resources.CannotDeleteObject;
                }
                else
                {
                    try
                    {
                        using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                        {
                            Address customerAddress =  CurrentCustomer.Trader.Addresses.Where(addr => addr.Oid == AddressOid).FirstOrDefault();
                            if (customerAddress.IsDefault)
                            {
                                Danger = Resources.CannotDeleteObject;
                            }
                            else
                            {
                                CurrentCustomer.Trader.Addresses.Remove(customerAddress);
                                CurrentCustomer.Session.CommitTransaction();
                                CurrentUser.Reload();
                                Success = Resources.SuccesfullyDeleted;
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        Danger = exception.GetFullMessage();
                    }
                }
            }
            else
            {
                Danger = Resources.AnErrorOccurred;
            }


            return Json(new { success = string.IsNullOrEmpty(Danger) });
        }
    }
}
