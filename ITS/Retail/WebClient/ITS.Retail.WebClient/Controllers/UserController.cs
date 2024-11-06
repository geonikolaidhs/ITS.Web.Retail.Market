using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.Model;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.Retail.Common;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.WebClient.Providers;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;
using ITS.Retail.Common.Helpers;
using System.Text;

namespace ITS.Retail.WebClient.Controllers
{
    public class UserController : BaseObjController<User>
    {
        //
        // GET: /User/

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

        protected override GenericViewRuleset GenerateGenericViewRuleset()
        {
            GenericViewRuleset ruleset = base.GenerateGenericViewRuleset();
            ruleset.ShowDetails = false;
            ruleset.PropertiesToIgnore.Add("Key");
            ruleset.PropertiesToIgnore.Add("Password");
            ruleset.PropertiesToIgnore.Add("PasswordAnswer");
            ruleset.PropertiesToIgnore.Add("AssociatedSuppliers");
            ruleset.PropertiesToIgnore.Add("TermsAccepted");
            ruleset.PropertiesToIgnore.Add("CentralStore");
            ruleset.PropertiesToIgnore.Add("Email");
            ruleset.PropertiesToIgnore.Add("PasswordQuestion");
            ruleset.PropertiesToIgnore.Add("FailedPasswordAnswerAttemptCount");
            ruleset.PropertiesToIgnore.Add("FailedPasswordAnswerAttemptWindowStart");
            ruleset.PropertiesToIgnore.Add("IsLockedOut");
            ruleset.PropertiesToIgnore.Add("LastActivityDate");
            ruleset.PropertiesToIgnore.Add("LastLockedOutDate");
            ruleset.PropertiesToIgnore.Add("FailedPasswordAttemptCount");
            ruleset.PropertiesToIgnore.Add("FailedPasswordAttemptWindowStart");
            ruleset.PropertiesToIgnore.Add("Comment");
            ruleset.PropertiesToIgnore.Add("LastLoginDate");
            ruleset.PropertiesToIgnore.Add("IsPOSUser");
            ruleset.PropertiesToIgnore.Add("IsB2CUser");
            ruleset.PropertiesToIgnore.Add("WishList");
            ruleset.PropertiesToIgnore.Add("TaxCode");

            ruleset.NumberOfColumns = 3;

            return ruleset;
        }


        [Security(ReturnsPartial = false, OverrideSecurity = true)]
        public new ActionResult Profile(string ID)
        {
            ViewBag.ShowPOSPwd = UserHelper.UsesPOSCredentials(CurrentUser);
            this.ToolbarOptions.ForceVisible = false;
            if (ID == null)
            {
                if (UserHelper.IsCustomer(CurrentUser))
                {
                    return new RedirectResult("~/Customer/UpdateProfile");
                }
                else if (UserHelper.IsCompanyUser(CurrentUser))
                {
                    return new RedirectResult("~/Company/UpdateProfile");
                }
            }

            if (Request.HttpMethod != "POST")
            {
                Session["Notice"] = Resources.PasswordRules;
                return View(CurrentUser);
            }

            string oldpassword = UserHelper.EncodePassword(Request["OldPassword"]);
            if (String.IsNullOrWhiteSpace(Request["Password"]) || String.IsNullOrWhiteSpace(Request["PasswordConfirm"]))
            {
                Session["Error"] = Resources.EmptyPasword;
                return View(CurrentUser);
            }
            string newpassword = UserHelper.EncodePassword(Request["Password"]);
            string conpassword = UserHelper.EncodePassword(Request["PasswordConfirm"]);
            if (newpassword != conpassword)
            {
                Session["Error"] = Resources.PasswordsDoNotMatch;
                return View(CurrentUser);
            }
            else if (oldpassword != CurrentUser.Password)
            {
                Session["Error"] = Resources.InvalidPassword;
                return View(CurrentUser);
            }

            if (!UserHelper.CheckPasswordStrength(Request["Password"]))
            {
                Session["Error"] = Resources.PasswordDoesNotMeetRequirements;
                return View(CurrentUser);
            }
            
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                User user = uow.GetObjectByKey<User>(CurrentUser.Oid);
                user.Password = newpassword;
                user.Save();
                XpoHelper.CommitTransaction(uow);
            }

            Session["Notice"] = Resources.SuccessMessage;
            Session["PasswordMustChange"] = null;
            return View(CurrentUser);
        }

        [Security(ReturnsPartial = false, OverrideSecurity = true)]
        public ActionResult ProfilePOS()
        {
            ViewBag.ShowPOSPwd = UserHelper.UsesPOSCredentials(CurrentUser);
            string oldPOSpassword = Request["OldPOSPassword"];

            if (String.IsNullOrWhiteSpace(Request["POSPassword"]) || String.IsNullOrWhiteSpace(Request["POSPasswordConfirm"]))
            {
                Session["Error"] = Resources.EmptyPOSPassword;
                return View("Profile", CurrentUser);
            }
            string newPOSpassword = Request["POSPassword"];
            string conPOSpassword = Request["POSPasswordConfirm"];

            if (newPOSpassword != conPOSpassword)
            {
                Session["Error"] = Resources.POSPasswordsDoNotMatch;
                return View("Profile", CurrentUser);
            }
            else if (oldPOSpassword != CurrentUser.POSPassword)
            {
                Session["Error"] = Resources.InvalidPOSPassword;
                return View("Profile", CurrentUser);
            }
 
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                User user = uow.GetObjectByKey<User>(CurrentUser.Oid);
                user.POSPassword = newPOSpassword;
                user.Save();
                XpoHelper.CommitTransaction(uow);
            }

            Session["Notice"] = Resources.SuccessMessage;
            Session["PasswordMustChange"] = null;
            return View("Profile", CurrentUser);
        }

        [Security(ReturnsPartial = false, OverrideSecurity = true)]
        public ActionResult TermsAndConditions()
        {
            this.ToolbarOptions.ForceVisible = false;
            ViewData["Terms"] = OwnerApplicationSettings.ApplicationTerms;
            return View();
        }

        [Security(ReturnsPartial = false, OverrideSecurity = true)]
        public ActionResult TermsFormSubmit(bool TermsAccepted)
        {
            if (TermsAccepted)
            {
                User current = CurrentUser;
                current.TermsAccepted = true;
                current.Save();
                XpoHelper.CommitTransaction(current.Session);
                if (Session["PasswordMustChange"] != null && ((bool)Session["PasswordMustChange"]) == true)
                {
                    return RedirectToAction("Profile", "User", new { ID = "changepassword" });
                }
                return new RedirectResult("../Home/Index");
            }
            else
            {
                Session.Clear();
                return new RedirectResult("../Login/Index");
            }
        }

        [Security(ReturnsPartial = false), Display(ShowSettings = false)]
        public ActionResult Index()
        {
            GenerateUnitOfWork();
            Session["UserFilter"] = CriteriaOperator.Parse("Oid='" + Guid.Empty + "'", "");
            this.ToolbarOptions.FilterButton.Visible = false;
            this.ToolbarOptions.ExportToButton.Visible = false;
            this.ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            this.ToolbarOptions.NewButton.OnClick = "AddNewCustomV2";
            this.ToolbarOptions.EditButton.OnClick = "EditSelectedRowsCustomV2";
            this.ToolbarOptions.ViewButton.Visible = true;
            this.ToolbarOptions.ViewButton.OnClick = "ShowGenericView";
            this.ToolbarOptions.OptionsButton.Visible = false;

            this.CustomJSProperties.AddJSProperty("editAction", "EditView");
            this.CustomJSProperties.AddJSProperty("editIDParameter", "UserID");
            this.CustomJSProperties.AddJSProperty("gridName", "grdUsers");
            ViewBag.RoleComboBox = GetList<Role>(uow, new BinaryOperator("Type", eRoleType.SystemAdministrator, BinaryOperatorType.NotEqual));
            return View("Index", new XPCollection<User>(uow, (CriteriaOperator)Session["UserFilter"]));

        }

        public override ActionResult Grid()
        {

            GenerateUnitOfWork();
            Guid UserID = Request["UserID"] == null || Request["UserID"] == "null" || Request["UserID"] == "-1" ? Guid.Empty : Guid.Parse(Request["UserID"]);
            if (Request["DXCallbackArgument"].Contains("SEARCH"))
            {
                CriteriaOperator searchCriteria;
                if (Request.HttpMethod == "POST")
                {
                    string name = (Request["name"] == null || Request["name"] == "null") ? "" : Request["name"];
                    string customer = (Request["customer"] == null || Request["customer"] == "null") ? "" : Request["customer"];
                    string supplier = (Request["supplier"] == null || Request["supplier"] == "null") ? "" : Request["supplier"];
                    string taxCode = (Request["taxCode"] == null || Request["taxCode"] == "null") ? "" : Request["taxCode"];
                    string role = (Request["role"] == null || Request["role"] == "null") ? "" : Request["role"];

                    CriteriaOperator fullNameFilter = null;
                    if (name != null && name.Trim() != "")
                    {
                        fullNameFilter = CriteriaOperator.Or(new BinaryOperator("UserName", "%" + name + "%", BinaryOperatorType.Like),
                                                             new BinaryOperator("FullName", "%" + name + "%", BinaryOperatorType.Like));
                    }


                    CriteriaOperator customerFilter = null;
                    if (customer != null && customer.Trim() != "")
                    {
                        XPCollection<Customer> selectedCustomers = GetList<Customer>(uow, CriteriaOperator.Or(new BinaryOperator("Code", "%" + customer + "%", BinaryOperatorType.Like),
                                                                                                                new BinaryOperator("CompanyName", "%" + customer + "%", BinaryOperatorType.Like),
                                                                                                                new BinaryOperator("Trader.FirstName", "%" + customer + "%", BinaryOperatorType.Like),
                                                                                                                new BinaryOperator("Trader.LastName", "%" + customer + "%", BinaryOperatorType.Like)));
                        var customerOids = from cust in selectedCustomers
                                           select cust.Oid;
                        customerFilter = new ContainsOperator("UserTypeAccesses", new InOperator("EntityOid", customerOids.ToList()));
                    }

                    CriteriaOperator supplierFilter = null;
                    if (supplier != null && supplier.Trim() != "")
                    {
                        XPCollection<CompanyNew> selectedSuppliers = GetList<CompanyNew>(uow, CriteriaOperator.Or(new BinaryOperator("Code", "%" + supplier + "%", BinaryOperatorType.Like),
                                                                                                                new BinaryOperator("CompanyName", "%" + supplier + "%", BinaryOperatorType.Like),
                                                                                                                new BinaryOperator("Trader.FirstName", "%" + supplier + "%", BinaryOperatorType.Like),
                                                                                                                new BinaryOperator("Trader.LastName", "%" + supplier + "%", BinaryOperatorType.Like)));
                        var supplierOids = from sup in selectedSuppliers
                                           select sup.Oid;
                        supplierFilter = new ContainsOperator("UserTypeAccesses", new InOperator("EntityOid", supplierOids.ToList()));
                    }



                    CriteriaOperator taxCodeFilter = null;
                    if (taxCode != null && taxCode.Trim() != "")
                    {
                        taxCodeFilter = new BinaryOperator("TaxCode", "%" + taxCode + "%", BinaryOperatorType.Like);
                    }

                    CriteriaOperator roleFilter = null;
                    if (role != null && role.Trim() != "")
                    {
                        roleFilter = new BinaryOperator("Role.Description", "%" + role + "%", BinaryOperatorType.Like);
                    }


                    CriteriaOperator isActiveFilter = null;
                    int isActive;
                    if (Int32.TryParse(Request["isActive"].ToString(), out isActive))
                    {
                        isActiveFilter = new BinaryOperator("IsActive", isActive);
                    }

                    searchCriteria = CriteriaOperator.And(fullNameFilter, customerFilter, supplierFilter,
                                    taxCodeFilter,
                                    roleFilter,
                                    isActiveFilter);

                    Session["UserFilter"] = searchCriteria;
                }
                else
                {
                    searchCriteria = CriteriaOperator.Parse("Oid='" + Guid.Empty + "'");
                    Session["UserFilter"] = searchCriteria;
                }
            }
            GridFilter = (CriteriaOperator)Session["UserFilter"];
            return base.Grid();
        }

        public override ActionResult LoadEditPopup()
        {
            base.LoadEditPopup();

            ViewBag.Title = Resources.EditUser;

            ActionResult rt = PartialView("LoadEditPopup");
            return rt;
        }

        public ActionResult EditView(string Oid)
        {
            GenerateUnitOfWork();
            Guid userGuid = (Oid == null || Oid == "null" || Oid == "-1") ? Guid.Empty : Guid.Parse(Oid);
            if (userGuid == Guid.Empty && TableCanInsert == false)
            {
                return new RedirectResult("~/Login");
            }
            else if (userGuid != Guid.Empty && TableCanUpdate == false)
            {
                return new RedirectResult("~/Login");
            }

            User user;
            ViewData["EditMode"] = true;

            if (Session["UnsavedUser"] == null)
            {
                if (userGuid != Guid.Empty)
                {
                    user = uow.FindObject<User>(new BinaryOperator("Oid", userGuid));
                    Session["IsNewUser"] = false;
                }
                else
                {
                    user = new User(uow);
                    Session["IsNewUser"] = true;
                }
                Session["IsRefreshed"] = false;
            }
            else
            {
                if (userGuid != Guid.Empty && (Session["UnsavedUser"] as User).Oid == userGuid)
                {
                    Session["IsRefreshed"] = true;
                    user = (User)Session["UnsavedUser"];
                }
                else if (userGuid == Guid.Empty)
                {
                    Session["IsRefreshed"] = false;
                    user = (User)Session["UnsavedUser"];
                }
                else
                {
                    uow.ReloadChangedObjects();
                    uow.RollbackTransaction();
                    Session["IsRefreshed"] = false;
                    user = uow.FindObject<User>(new BinaryOperator("Oid", userGuid));
                }
            }

            BinaryOperator criteria = !UserHelper.IsSystemAdmin(CurrentUser) ? new BinaryOperator("Type", eRoleType.SystemAdministrator, BinaryOperatorType.NotEqual) : null;
            ViewBag.RoleComboBox = GetList<Role>(uow, criteria);
            ViewData["UserID"] = user.Oid.ToString();
            Session["UnsavedUser"] = user;
            if (Session["UnsavedUser"] != null && (Session["UnsavedUser"] as User).Role != null)
            {
                ViewBag.IsPOS = UserHelper.UsesPOSCredentials((Session["UnsavedUser"] as User));
            }
            XPCollection<CompanyNew> getUserSupplier = BOApplicationHelper.GetUserEntities<CompanyNew>(uow, user);
            Session["SelectedSupplier"] = ViewData["UserCompanyAssociation"] = getUserSupplier.Count == 0 ? null : getUserSupplier.First();
            XPCollection<Customer> getUserCustomer = BOApplicationHelper.GetUserEntities<Customer>(uow, (User)Session["UnsavedUser"]);
            Session["SelectedCustomer"] = ViewData["UserCustomer"] = getUserCustomer.Count == 0 ? null : getUserCustomer.First();
            Session["UserStores"] = EffectiveOwner == null ? null : GetList<Store>(XpoHelper.GetNewUnitOfWork());
            ViewData["InitialSelectedStores"] = user != null ? string.Join(",", BOApplicationHelper.GetUserEntities<Store>(user.Session, user).Select(x => x.Oid)) : String.Empty;
            return PartialView("EditView", user);
        }

        private void CopyUserFromViewModel(User user, User ct)
        {
            user.FullName = ct.FullName;
            user.POSUserName = ct.POSUserName;
            user.POSPassword = ct.POSPassword;
            user.POSUserLevel = ct.POSUserLevel;
            Guid roleGuid = Guid.Empty;
            Guid.TryParse(Request["Role_VI"], out roleGuid);
            user.Role = user.Session.GetObjectByKey<Role>( roleGuid );
            user.IsCentralStore = Request["IsCentralStore"] == "C";
            user.IsActive = Request["IsActive"] == "C";
            user.IsApproved = Request["IsApproved"] == "C";
            user.UserName = ct.UserName;
        }

        public new JsonResult Save([ModelBinder(typeof(RetailModelBinder))] User ct)
        {
            GenerateUnitOfWork();
            Guid userGuid = Guid.Empty;
            ePOSUserLevel posuserlevel;
            bool correctUserGuid = Request["UserID"] != null && Guid.TryParse(Request["UserID"].ToString(), out userGuid);
            bool correctPassword = true;
            User user = (User)Session["UnsavedUser"];

            if (EffectiveOwner == null)
            {
                Session["Error2"] = Resources.UserMustHaveOwner;
                CopyUserFromViewModel(user, ct);
                Session["UnsavedUser"] = user;
                return Json(new { error = Session["Error2"] });
            }

            if (String.IsNullOrWhiteSpace(Request["UserName"]))
            {
                Session["Error2"] = Resources.PleaseProvideUserName;
                CopyUserFromViewModel(user, ct);
                Session["UnsavedUser"] = user;
                return Json(new { error = Session["Error2"] });
            }
            else
            {
                User existingUser = uow.FindObject<User>(new BinaryOperator("UserName", Request["UserName"]));
                if (existingUser != null && existingUser.Oid != user.Oid)
                {
                    Session["Error2"] = Resources.DuplicateUser;
                    CopyUserFromViewModel(user, ct);
                    Session["UnsavedUser"] = user;
                    return Json(new { error = Session["Error2"] });
                }
            }

            if (!String.IsNullOrWhiteSpace(Request["Password"]))
            {
                correctPassword = UserHelper.CheckPasswordStrength(Request["Password"]);
                if (!correctPassword)
                {
                    Session["Error2"] = Resources.PasswordDoesNotMeetRequirements;
                    CopyUserFromViewModel(user, ct);
                    Session["UnsavedUser"] = user;
                    return Json(new { error = Session["Error2"] });
                }
                user.Password = UserHelper.EncodePassword(Request["Password"]);
            }

            if (correctUserGuid && correctPassword)
            {

                if (user != null)
                {
                    try
                    {

#if(!DEBUG && _HASLICENCE)
                        #region ManageUserKey
                        ITSLicense.UserAccessType userAccessType = ITSLicense.ConvertToUserAccessType(Request["UserType"].ToString());
                        if (user.Key == null || user.Key == "")
                        {
                        #region Generate new key
                            if (user.IsActive)
                            {
                                user.Key = ITSLicense.GenerateUserKey(MvcApplication.license.SerialNumber, userAccessType);
                            }
                        #endregion
                        #region User Is inactive
                            else
                            {
                                user.Key = ITSLicense.GenerateUserKey(MvcApplication.license.SerialNumber, ITSLicense.UserAccessType.None);
                            }
                        #endregion
                        }
                        #region User Access Type has changed
                        else if( !userAccessType.Equals( MvcApplication.license.GetUserType(user.Key) ) )
                        {
                            user.Key = ITSLicense.GenerateUserKey(MvcApplication.license.SerialNumber, userAccessType);
                        }
                        #endregion
                        #endregion
#endif
                        user.TaxCode = Request["TaxCode"];
                        user.FullName = Request["FullName"];
                        user.IsActive = Request["IsActive"] != null && Request["IsActive"].ToString().ToUpper() == "C";
                        user.UserName = Request["UserName"];
                        user.IsCentralStore = Request["IsCentralStore"] != null && Request["IsCentralStore"].ToString().ToUpper() == "C";
                        user.IsApproved = Request["IsApproved"] != null && Request["IsApproved"].ToString().ToUpper() == "C";
                        user.Role = uow.FindObject<Role>(new BinaryOperator("Oid", (Request.Params["Role_VI"] == null || Request.Params["Role_VI"] == "") ? Guid.Empty : Guid.Parse(Request.Params["Role_VI"])));
                        user.POSUserName = user.IsPOSUser ? Request["POSUserName"] : String.Empty;
                        user.POSPassword = user.IsPOSUser ? Request["POSPassword"] : String.Empty;
                        user.POSUserLevel = Enum.TryParse(Request["POSUserLevel_VI"], out posuserlevel) ? posuserlevel : ePOSUserLevel.LEVEL0;

                        CriteriaOperator crop;

                        if (UserHelper.IsTrader(user))
                        {
                            if (String.IsNullOrWhiteSpace(Request["CustomerComboBox_VI"]))
                            {
                                Session["Error2"] = Resources.PleaseSelectACustomer;
                                return Json(new { error = Session["Error2"] });
                            }
                            Customer my_Customer = GetObjectByArgument<Customer>(user.Session, "CustomerComboBox_VI") as Customer;
                            crop = CriteriaOperator.And(new BinaryOperator("User.Oid", user.Oid),
                                                        new BinaryOperator("EntityType", typeof(Customer).ToString()));

                            XPCollection<UserTypeAccess> existingCustomers = GetList<UserTypeAccess>(uow, crop);
                            uow.Delete(existingCustomers);
                            XpoHelper.CommitTransaction(uow);
                            UserTypeAccess userTypeAccess = new UserTypeAccess(user.Session);
                            userTypeAccess.IsActive = true;
                            userTypeAccess.User = user;
                            userTypeAccess.EntityOid = my_Customer.Oid;
                            userTypeAccess.EntityType = typeof(Customer).ToString();
                            userTypeAccess.Save();
                        }
                        else if (UserHelper.UsesPOSCredentials(user))
                        {
                            if (!String.IsNullOrWhiteSpace(user.POSUserName))
                            {
                                User existingPOSUser = uow.FindObject<User>(new BinaryOperator("POSUserName", user.POSUserName));
                                if (existingPOSUser != null && existingPOSUser.Oid != user.Oid)
                                {
                                    Session["Error2"] = Resources.ExistingPOSUser;
                                    return Json(new { error = Session["Error2"] });
                                }
                            }
                            //XPCollection<CompanyNew> userSuppliers = BOApplicationHelper.GetUserEntities<CompanyNew>(user.Session, user);
                            crop = CriteriaOperator.And(new BinaryOperator("User.Oid", user.Oid),
                                                        new BinaryOperator("EntityOid", EffectiveOwner.Oid));
                            UserTypeAccess ownerUserTypeAccess = uow.FindObject<UserTypeAccess>(crop);
                            
                            if(ownerUserTypeAccess == null)
                            {
                                UserTypeAccess userTypeAccess = new UserTypeAccess(user.Session);
                                userTypeAccess.IsActive = true;
                                userTypeAccess.User = user;
                                userTypeAccess.EntityOid = EffectiveOwner.Oid;
                                userTypeAccess.EntityType = typeof(CompanyNew).ToString();
                                userTypeAccess.Save();
                            }
                            //uow.Delete(existingSuppliers);
                            //XpoHelper.CommitTransaction(uow);

                            //UserTypeAccess userTypeAccess = new UserTypeAccess(user.Session);
                            //userTypeAccess.IsActive = true;
                            //userTypeAccess.User = user;
                            //userTypeAccess.EntityOid = EffectiveOwner.Oid;
                            //userTypeAccess.EntityType = typeof(CompanyNew).ToString();
                            //userTypeAccess.Save();
                        }

                        if (!UserHelper.IsSystemAdmin(user))
                        {
                            if (Request["selectedStores"] != null)
                            {
                                string[] user_store_oids = Request["selectedStores"].ToString().Split(',');
                                ArrangeUserStores(user, user_store_oids);
                            }
                        }
                        XpoHelper.CommitTransaction(uow);
                        Session["Notice"] = Resources.SavedSuccesfully;
                    }
                    catch (Exception e)
                    {

                        uow.RollbackTransaction();

                        string errorMessage = Resources.AnErrorOccurred + ": " + (e.InnerException == null ? e.Message : e.InnerException.Message);
                        if (errorMessage.Contains("Cannot insert duplicate key row in object 'dbo.User'"))
                        {
                            Session["Error"] = Resources.DuplicateUser;
                            return Json(new { error = Session["Error"] });
                        }
                        else
                        {
                            Session["Error"] = errorMessage;
                            return Json(new { error = Session["Error"] });
                        }
                    }
                    finally
                    {
                        ((UnitOfWork)Session["uow"]).Dispose();
                        Session["uow"] = null;
                        Session["IsRefreshed"] = null;
                        Session["IsNewUser"] = null;
                        Session["UnsavedUser"] = null;
                        Session["UserStores"] = null;
                        Session["SelectedCustomer"] = null;
                        Session["SelectedSupplier"] = null;
                    }
                }
            }
            return Json(new { });
        }

        [Security(ReturnsPartial = false)]
        public ActionResult CancelEdit()
        {
            if (Session["IsRefreshed"] == null || !Boolean.Parse(Session["IsRefreshed"].ToString()))
            {
                if (Session["uow"] != null)
                {
                    ((UnitOfWork)Session["uow"]).ReloadChangedObjects();
                    ((UnitOfWork)Session["uow"]).RollbackTransaction();
                    ((UnitOfWork)Session["uow"]).Dispose();
                    Session["uow"] = null;
                }
                Session["IsRefreshed"] = null;
                Session["IsNewUser"] = null;
                Session["UnsavedUser"] = null;
                Session["UserStores"] = null;
            }
            return null;
        }

        private void ArrangeUserStores(User user, string[] store_oids)
        {
            List<UserTypeAccess> user_stores = new List<UserTypeAccess>();
            CriteriaOperator crop = CriteriaOperator.And(new BinaryOperator("User.Oid", user.Oid, BinaryOperatorType.Equal),
                                                         new BinaryOperator("EntityType", "ITS.Retail.Model.Store", BinaryOperatorType.Equal));
            user_stores = GetList<UserTypeAccess>((UnitOfWork)user.Session, crop).ToList();

            //If no stores are selected at all
            if (store_oids == null)
            {
                 user.Session.Delete(user_stores);
            }
            else
            {
                List<UserTypeAccess> stores_to_delete = new List<UserTypeAccess>();             
                List<Guid> new_store_guids = new List<Guid>();
                List<Guid> user_store_guids = new List<Guid>();

                //Create a Store Guid for every string
                foreach (String store_oid in store_oids)
                {
                    Guid store_guid;
                    if (Guid.TryParse(store_oid, out store_guid))
                    {
                        new_store_guids.Add(Guid.Parse(store_oid));
                    }
                }

                //Delete old stores of user that do not exist in new list
                if (user_stores.Count > 0)
                {
                    foreach (UserTypeAccess store in user_stores)
                    {
                        if (!new_store_guids.Contains(store.EntityOid))
                        {
                            stores_to_delete.Add(store);
                        }
                    }
                }
                user.Session.Delete(stores_to_delete);

                //Add stores from the new list that did not exist in user list before
                foreach (String store_oid in store_oids)
                {
                    Guid store_guid;
                    if (Guid.TryParse(store_oid, out store_guid))
                    {
                        if (user_stores.Where(store => store.EntityOid == store_guid).Count() == 0)
                        {
                            UserTypeAccess user_type_access = new UserTypeAccess(user.Session);
                            user_type_access.EntityType = "ITS.Retail.Model.Store";
                            user_type_access.EntityOid = store_guid;
                            user_type_access.User = user;
                            user_type_access.IsActive = true;
                            user_type_access.Save();
                        }
                    }
                }

            }
        }

        [HttpPost]
        public ActionResult InlineEditingDeletePartial([ModelBinder(typeof(RetailModelBinder))] User ct)
        {
            if (!TableCanDelete) return null;
            GenerateUnitOfWork();
            try
            {


                ct.IsActive = false;
                Delete(ct);


            }
            catch (Exception e)
            {
                Session["Error"] = e.Message;// +Environment.NewLine + e.StackTrace;
            }

            FillLookupComboBoxes();
            return PartialView("Grid", new XPCollection<User>(uow, (CriteriaOperator)Session["UserFilter"]));
        }

        protected override void FillLookupComboBoxes()
        {
            GenerateUnitOfWork();
            base.FillLookupComboBoxes();

        }

        protected override void UpdateLookupObjects(User ct)
        {
            base.UpdateLookupObjects(ct);
            ct.Role = GetObjectByArgument<Role>(ct.Session, "RoleId");
        }


        public ActionResult CustomersComboBoxPartial()
        {
            return PartialView();
        }

        public ActionResult StoresComboBoxPartial()
        {
            return PartialView();
        }


        public ActionResult UserStores()
        {
            if (Session["Error2"] != null)
            {
                Session["Error"] = Session["Error2"];
                Session["Error2"] = null;
            }
            if (Request["DXCallbackArgument"].Contains("CUSTOMCALLBACK"))
            {
                ViewData["ClearUserStores"] = true;
            }
            return PartialView("UserStores");
        }


        //public static object CustomerRequestedByValue(DevExpress.Web.ListEditItemRequestedByValueEventArgs e)
        //{
        //    if (e.Value != null)
        //    {
        //        Customer obj = XpoHelper.GetNewUnitOfWork().GetObjectByKey<Customer>(e.Value);
        //        return obj;
        //    }
        //    return null;

        //}

        //public static object GetCustomerByValue(object value)
        //{
        //    return GetObjectByValue<Customer>(value);
        //}

        public ActionResult UpdateRole()
        {
            User user = (Session["UnsavedUser"] as User);
            if (user != null)
            {
                GenerateUnitOfWork();
                BinaryOperator criteria = !UserHelper.IsSystemAdmin(CurrentUser) ?
                            new BinaryOperator("Type", eRoleType.SystemAdministrator, BinaryOperatorType.NotEqual) :
                            null;
                ViewBag.RoleComboBox = GetList<Role>(uow, criteria);
                Role role = uow.FindObject<Role>(new BinaryOperator("Oid", (Request["Role_VI"] == null || Request["Role_VI"] == "") ? Guid.Empty : Guid.Parse(Request["Role_VI"])));
                if (role != null && (role.Type == eRoleType.CompanyUser || role.Type == eRoleType.CompanyAdministrator))
                {
                    ViewData["IsPOS"] = true;
                }

                user.Role = role;
            }

            return PartialView("../User/UpdateRole", Session["UnsavedUser"]);
        }

        public static object CustomersRequestedByFilterCondition(DevExpress.Web.ListEditItemsRequestedByFilterConditionEventArgs e)
        {

            XPCollection<Customer> collection = GetList<Customer>(XpoHelper.GetNewUnitOfWork(),
                CriteriaOperator.Or(new BinaryOperator("CompanyName", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like),
                                                      new BinaryOperator("Trader.TaxCode", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like)), "CompanyName");
            collection.SkipReturnedObjects = e.BeginIndex;
            collection.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;
            CompanyNew selectedSupplier = System.Web.HttpContext.Current.Session["SelectedSupplier"] as CompanyNew;
            return collection;
        }

        public ActionResult GenerateUserKey(string UserID)
        {
            return PartialView();
        }

        public ActionResult ActiveUsers()
        {
            return PartialView();
        }
    
    }
}