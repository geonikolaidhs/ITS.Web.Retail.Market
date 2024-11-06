using System.Web.Mvc;
using System.Collections.Generic;
using DevExpress.Xpo;
using DevExpress.Xpo.DB.Exceptions;
using ITS.Retail.Model;
using System;
using ITS.Retail.Common;
using DevExpress.Data.Filtering;
using System.Reflection;
using ITS.Retail.WebClient.Helpers;
using System.Linq;
using ITS.Retail.ResourcesLib;
using DevExpress.Web.Mvc;
using ITS.Retail.WebClient.Extensions;
using ITS.Retail.WebClient.Providers;
using ITS.Retail.Model.Attributes;
using StackExchange.Profiling;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Common.ViewModel;

namespace ITS.Retail.WebClient.Controllers
{
    [RoleAuthorize]
    [LicensedAuthorize]
    public abstract class BaseObjController<T> : BaseController where T : BasicObj
    {

        //private static readonly Dictionary<PropertyInfo, string> propMapping = new Dictionary<PropertyInfo, string>();
        protected virtual Dictionary<PropertyInfo, string> PropertyMapping
        {
            get
            {
                return new Dictionary<PropertyInfo, string>(); ;
            }
        }


        protected ToolbarOptions ToolbarOptions { get; set; }


        public static readonly int GuidLength = Guid.Empty.ToString().Length;
        public BaseObjController()
            : base()
        {
            if (typeof(T).GetProperty("Code") != null)
            {
                GridSortingField = "Code";
            }
            else
            {
                GridSortingField = "Oid";
            }
        }

        protected bool TableVisible, TableCanDelete, TableCanInsert, TableCanUpdate, TableCanExport;

        public const string PluginVersion = "1.0.0.1";

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (this.ControllerContext.IsChildAction == false && this.Request.IsAjaxRequest() == false)
            {
                using (MiniProfiler.Current.Step("BaseObjController.OnActionExecutingBefore"))
                {
                    if (CurrentOwner == null && (typeof(IRequiredOwner).IsAssignableFrom(typeof(T)) || typeof(T) == typeof(User))
                        && filterContext.ActionDescriptor.ActionName.ToUpper() != "TERMSANDCONDITIONS")
                    {
                        Session["Error"] = Resources.SelectCompany;
                        if (filterContext.HttpContext.Request.UrlReferrer != null && filterContext.HttpContext.Request.UrlReferrer.OriginalString.ToLower().Contains("/login") == false)
                        {
                            filterContext.Result = new RedirectResult(filterContext.HttpContext.Request.UrlReferrer.AbsoluteUri);
                        }
                        else if (filterContext.HttpContext.Request.Url != null)
                        {
                            filterContext.Result = new RedirectResult(filterContext.HttpContext.Request.Url.AbsoluteUri);
                        }
                        else
                        {
                            //TODO serious fraking problem
                        }
                    }
                    if (Session["PasswordMustChange"] != null && (bool)Session["PasswordMustChange"] == true)
                    {
                        if (filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToUpper() != "USER"
                            && filterContext.ActionDescriptor.ActionName.ToUpper() != "PROFILE"
                            && filterContext.ActionDescriptor.ActionName.ToUpper() != "TERMSANDCONDITIONS"
                            && filterContext.ActionDescriptor.ActionName.ToUpper() != "TERMSFORMSUBMIT"
                            && filterContext.ActionDescriptor.ActionName.ToUpper() != "LOGIN"
                            && filterContext.ActionDescriptor.ActionName.ToUpper() != "USERCONNECTED" && !ViewBag.IsMobileDevice)
                        {
                            filterContext.Result = new RedirectResult("~/User/Profile?ID=changepassword");
                        }
                    }
                }
            }
            base.OnActionExecuting(filterContext);
            using (MiniProfiler.Current.Step("BaseObjController.OnActionExecutingAfter"))
            {
                ViewData["TableVisible"] = TableVisible = true;
                if (filterContext != null && filterContext.ActionDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Length != 0)
                {
                    ViewBag.TableCanDelete = ViewBag.TableCanInsert = ViewBag.TableCanUpdate = ViewBag.TableCanExport = false;
                }
                PrepareTablePermissions(filterContext);
                if (this.ControllerContext.IsChildAction == false && this.Request.IsAjaxRequest() == false)
                {
                    this.CustomJSProperties.AddJSProperty("customjs_entityName", typeof(T).Name);
                    this.CustomJSProperties.AddJSProperty("emptyGuid", Guid.Empty.ToString());
                    this.CustomJSProperties.AddJSProperty("pluginCurrentVersion", PluginVersion);

                    ToolbarOptions = new ToolbarOptions();
                    ToolbarOptions.DeleteButton.Visible = TableCanDelete;
                    ToolbarOptions.EditButton.Visible = TableCanUpdate;
                    ToolbarOptions.NewButton.Visible = TableCanInsert;
                    ToolbarOptions.ExportToButton.Visible = TableCanExport;

                    ViewData["ToolbarOptions"] = ToolbarOptions;
                }
            }
            if (typeof(T) == typeof(Customer) || typeof(T) == typeof(SupplierNew) || typeof(T) == typeof(Trader) || typeof(T) == typeof(Address) || typeof(T) == typeof(DocumentHeader))
            {
                ViewData["GDPREnabled"] = CurrentUser == null || CurrentUser.Role == null ? false : CurrentUser.Role.GDPREnabled;
            }
        }

        /// <summary>
        /// Contains a map for Entitities that their Authorisation Rules MUST be consistent with another Entity.
        /// They Discitonary key is the Entity that has different  Authorisation rules, and the value is the relative entity name that we should retrieve the Authorisation rules from instead.
        /// </summary>
        private Dictionary<string, string> AuthorisationRuleModelNameMismatches = new Dictionary<string, string>(){
            {typeof(ItemBarcode).FullName.Split('.').Last(), typeof(Item).FullName.Split('.').Last()}
        };

        private void PrepareTablePermissions(ActionExecutingContext filterContext)
        {
            if (CurrentUser == null)
            {
                ViewData["TableCanDelete"] = ViewBag.TableCanDelete = TableCanDelete = false;
                ViewData["TableCanInsert"] = ViewBag.TableCanInsert = TableCanInsert = false;
                ViewData["TableCanUpdate"] = ViewBag.TableCanUpdate = TableCanUpdate = false;
                ViewData["TableCanExport"] = ViewBag.TableCanExport = TableCanExport = false;
            }
            else if (CurrentUser.UserName == "superadmin")
            {
                ViewData["TableCanDelete"] = ViewBag.TableCanDelete = TableCanDelete = true;
                ViewData["TableCanInsert"] = ViewBag.TableCanInsert = TableCanInsert = true;
                ViewData["TableCanUpdate"] = ViewBag.TableCanUpdate = TableCanUpdate = true;
                ViewData["TableCanExport"] = ViewBag.TableCanExport = TableCanExport = true;
            }
            else
            {
                User currentUser = CurrentUser;
                Role rl = currentUser.Role;

                TableVisible = TableCanDelete = TableCanInsert = TableCanUpdate = TableCanExport = true;
                object[] attributes = filterContext.ActionDescriptor.GetCustomAttributes(typeof(SecurityAttribute), true);
                if ((attributes.Count() == 0 || attributes.Cast<SecurityAttribute>().First().OverrideSecurity == false))
                {
                    string entityType = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                    if (AuthorisationRuleModelNameMismatches.Keys.Contains(entityType))
                    {
                        entityType = AuthorisationRuleModelNameMismatches[entityType];
                    }
                    RoleEntityAccessPermision rolePermissions = XpoSession.FindObject<RoleEntityAccessPermision>(CriteriaOperator.And(new BinaryOperator("Role.Oid", rl.Oid),
                        new BinaryOperator("EnityAccessPermision.EntityType", entityType)
                        ));

                    if (rolePermissions != null && rolePermissions.EnityAccessPermision.Visible == true)
                    {
                        TableCanDelete = rolePermissions.EnityAccessPermision.CanDelete;
                        TableCanInsert = rolePermissions.EnityAccessPermision.CanInsert;
                        TableCanUpdate = rolePermissions.EnityAccessPermision.CanUpdate;
                        TableCanExport = rolePermissions.EnityAccessPermision.CanExport;
                    }
                }

                if (MvcApplication.ApplicationInstance == eApplicationInstance.STORE_CONTROLER)
                {
                    StoreControllerEditableAttribute attribute = filterContext.Controller.GetType().
                        GetCustomAttributes(typeof(StoreControllerEditableAttribute), true).FirstOrDefault() as StoreControllerEditableAttribute;
                    if (attribute == null)
                    {
                        TableCanDelete = false;
                        TableCanInsert = false;
                        TableCanUpdate = false;
                        TableCanExport = false;
                    }
                }

                ViewData["TableCanDelete"] = ViewBag.TableCanDelete = TableCanDelete;
                ViewData["TableCanInsert"] = ViewBag.TableCanInsert = TableCanInsert;
                ViewData["TableCanUpdate"] = ViewBag.TableCanUpdate = TableCanUpdate;
                ViewData["TableCanExport"] = ViewBag.TableCanExport = TableCanExport;
            }
        }

        protected bool SaveT<OB>(OB viewModel) where OB : BasicObj
        {
            if (!this.TableCanInsert && !this.TableCanUpdate)
                RedirectToAction("E404", "Error");
            return SaveT(viewModel, false);
        }

        protected bool DeleteT<OB>(OB viewModel) where OB : BasicObj
        {
            if (!this.TableCanDelete)
                RedirectToAction("E404", "Error");
            return SaveT(viewModel, true);
        }

        protected bool SaveT<OB>(OB item, bool delete) where OB : BasicObj
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                OB foundItem = uow.GetObjectByKey<OB>(item.Oid);

                if (typeof(IOwner).IsAssignableFrom(typeof(OB)))
                {
                    if (foundItem != null && UserCanPerformAction(foundItem) == false)
                    {
                        Session["Error"] = Resources.PermissionDenied;
                        return false;
                    }
                }

                if (foundItem == null && !delete) // insert object
                {
                    if (!this.TableCanInsert)
                    {
                        RedirectToAction("E404", "Error");
                    }

                    AssignOwner(item);
                    foundItem = (OB)uow.GetClassInfo<OB>().CreateNewObject(uow);
                }
                else if (foundItem != null && !delete) // update object object
                {
                    if (!this.TableCanUpdate)
                        RedirectToAction("E404", "Error");
                    if (typeof(IOwner).IsAssignableFrom(typeof(OB)))
                    {
                        AssignOwner(item, ((IOwner)foundItem).Owner);
                    }
                }

                if (!delete)  // edit object
                {
                    var ignoreProperties = new List<string>();
                    ignoreProperties.Add("UpdatedOnTicks");
                    ignoreProperties.Add("UpdatedOn");
                    foundItem.GetData(item, ignoreProperties);
                }
                else if (foundItem != null)   // delete Object 
                {
                    if (!this.TableCanDelete)
                        RedirectToAction("E404", "Error");
                    foundItem.Delete();
                    //uow.Delete(foundItem);

                }
                try
                {
                    XpoHelper.CommitChanges(uow);
                    return true;
                }
                catch (Exception e)
                {
                    string errorMessage = e.Message;

                    if (e.InnerException != null && String.IsNullOrEmpty(e.InnerException.Message))
                    {
                        errorMessage += e.InnerException.Message;
                    }

                    errorMessage += Environment.NewLine + e.StackTrace;

                    Session["Error"] = errorMessage;
                    return false;
                }
            }
        }

        bool Save(T item, bool delete)
        {
            return SaveT(item, delete);
        }

        protected bool Save(T viewModel)
        {
            if (!this.TableCanInsert && !this.TableCanUpdate)
            {
                RedirectToAction("E404", "Error");
            }
            return Save(viewModel, false);
        }

        protected bool Delete(T viewModel)
        {
            if (!this.TableCanDelete)
                RedirectToAction("E404", "Error");
            return Save(viewModel, true);
        }



        protected W GetObjectByArgument<W>(Session session, string argument, IEnumerable<W> listToSearch = null) where W : BasicObj
        {
            if (Request.Params[argument] != null && Request.Params[argument] != "null")
            {
                Guid key;
                if (Guid.TryParse(Request.Params[argument], out key))
                {
                    if (listToSearch == null)
                    {
                        return session.GetObjectByKey<W>(key);
                    }
                    return listToSearch.First(g => g.Oid == key);
                }
            }
            return null;
        }

        protected virtual void FillLookupComboBoxes()
        {
        }

        protected virtual void UpdateLookupObjects(BasicObj basicObj)
        {
            foreach (KeyValuePair<PropertyInfo, string> key in this.PropertyMapping)
            {
                PropertyInfo property = key.Key;
                string Value = Request.Params[key.Value];
                if (property.PropertyType.IsSubclassOf(typeof(BasicObj)))
                {
                    Guid guid;
                    if (Guid.TryParse(Value, out guid))
                    {
                        object fk = basicObj.Session.GetObjectByKey(property.PropertyType, guid);
                        if (fk != null)
                        {
                            property.SetValue(basicObj, fk, null);
                        }
                    }
                }
                else
                {
                    property.SetValue(basicObj, Value, null);
                }
            }
        }

        protected virtual void UpdateLookupObjects(T obj) //where W : BasicObj
        {
            this.UpdateLookupObjects(basicObj: obj);
        }

        protected CriteriaOperator CreateCriteria(string value, string fieldname)
        {
            string fil = "";
            value = value.Replace("'", "").Replace("\"", "");
            if (string.IsNullOrEmpty(value) == false)
            {
                if (value.Replace('%', '*').Contains("*"))
                {
                    fil += String.Format("{0} like '{1}'", fieldname, value.Replace('*', '%'));

                }
                else
                {
                    fil += String.Format("{0}='{1}'", fieldname, value);
                }
            }
            return CriteriaOperator.Parse(fil, "");
        }

        private bool UserCanPerformAction(object obj, Type type)
        {
            if (typeof(IOwner).IsAssignableFrom(type))
            {
                if (UserOwner == null)
                {
                    return true;
                }
                IOwner ownObj = (IOwner)obj;
                if (ownObj.Owner == null)
                {
                    return false;
                }
                return ownObj.Owner.Oid == UserOwner.Oid;
            }
            return true;
        }

        protected bool UserCanPerformAction<OB>(OB obj)
        {
            if (typeof(IOwner).IsAssignableFrom(typeof(OB)))
            {
                if (UserOwner == null && CurrentOwner == null)
                {
                    return true;
                }
                CompanyNew owner = EffectiveOwner;

                if (owner == null)
                {
                    return false;
                }
                IOwner ownObj = (IOwner)obj;
                if (ownObj.Owner == null)
                {
                    if (UserOwner == null)
                    {
                        return true;  //admin can edit nullable owners
                    }
                    else
                    {
                        return false;
                    }
                }

                return ownObj.Owner.Oid == owner.Oid;
            }
            return true;
        }

        protected bool UserCanEdit(object obj, Type type)
        {
            if (TableCanUpdate == false)
            {
                return false;
            }
            return UserCanPerformAction(obj, type);
        }

        protected bool UserCanEdit(T obj)
        {
            if (TableCanUpdate == false)
                return false;
            return UserCanPerformAction(obj);
        }

        protected bool UserCanEditRequest(Type type = null)
        {
            if (type == null)
            {
                type = typeof(T);
            }
            if (typeof(IOwner).IsAssignableFrom(type) == false)
            {
                return true;
            }

            string stringOid = Request["DXCallbackArgument"].ToString().Substring(Request["DXCallbackArgument"].ToString().Length - (GuidLength + 1), GuidLength);
            Guid Oid;
            if (Guid.TryParse(stringOid, out Oid))
            {
                object obj = XpoSession.GetObjectByKey(type, Oid);
                return UserCanEdit(obj, type);
            }
            else
            {
                return false;
            }
        }

        protected bool UserCanDelete<W>(W obj) where W : BasicObj
        {
            if (TableCanDelete == false)
                return false;
            return UserCanPerformAction(obj);
        }

        protected void DeleteAll(UnitOfWork uow, List<Guid> oids)
        {
            DeleteAllT<T>(uow, oids);
        }

        protected void DeleteAllT<W>(UnitOfWork uow, List<Guid> oids) where W : BasicObj //BaseObj
        {
            Session["Notice"] = "";
            bool commit = false;
            foreach (Guid oid in oids)
            {
                W obj = uow.GetObjectByKey<W>(oid);
                if (obj != null && UserCanDelete(obj))
                {
                    if (obj is Role && (obj as Role).Type == eRoleType.SystemAdministrator)
                    {
                        Session["Error"] += Resources.CannotDeleteObject + " " + ((IOwner)obj).Description + Environment.NewLine;
                        continue;
                    }
                    else if (obj is User && (obj as User).Oid == CurrentUser.Oid)
                    {
                        Session["Error"] += Resources.CannotDeleteObject + " " + (obj as User).FullName + Environment.NewLine;
                        continue;
                    }
                    obj.Delete();
                    commit = true;
                }
                else
                {
                    if (typeof(IOwner).IsAssignableFrom(typeof(T)) && obj != null)
                    {
                        Session["Error"] += Resources.CannotDeleteObject + " " + ((IOwner)obj).Description;
                    }
                }
            }
            if (String.IsNullOrWhiteSpace(Session["Notice"].ToString()))
            {
                Session["Notice"] = null;
            }
            if (commit == true)
            {
                XpoHelper.CommitTransaction(uow);
            }
        }

        protected void AssignOwner<W>(W obj, CompanyNew Owner) where W : BasicObj
        {
            if (typeof(IOwner).IsAssignableFrom(typeof(W)) && Owner != null)
            {
                CompanyNew sameSessionOwner = (Owner.Session != obj.Session) ? obj.Session.GetObjectByKey<CompanyNew>(Owner.Oid) : Owner;
                PropertyInfo propertyOwner = typeof(W).GetProperty("Owner");
                if (propertyOwner.CanWrite)
                {
                    propertyOwner.SetValue(obj, sameSessionOwner, null);
                }
            }
        }

        protected void AssignOwner<W>(W obj) where W : BasicObj
        {
            if (typeof(IOwner).IsAssignableFrom(typeof(W)))
            {
                IOwner ownItem = (IOwner)obj;
                if (obj.Session.IsNewObject(obj) == false)
                {
                    return;
                }

                CompanyNew owner = null;
                if (ownItem.Owner != UserOwner && UserOwner != null)
                {
                    owner = UserOwner;
                }
                else if (ownItem.Owner != CurrentOwner && CurrentOwner != null)
                {
                    owner = CurrentOwner;
                }

                if (owner != null)
                {
                    AssignOwner(obj, owner);
                }
            }
        }

        protected bool HasDuplicate<W>(string code) where W : Lookup2Fields
        {
            CriteriaOperator crop = new BinaryOperator("Code", code);
            return XpoSession.FindObject<W>(ApplyOwnerCriteria(crop, typeof(W))) != null;
        }

        protected bool HasDuplicate<W>(W ct) where W : Lookup2Fields
        {

            CriteriaOperator crop = CriteriaOperator.And(new NotOperator(new BinaryOperator("Oid", ct.Oid)), new BinaryOperator("Code", ct.Code));
            object result = ct.Session.FindObject(ct.GetType(), ApplyOwnerCriteria(crop, ct.GetType()));
            return result != null;

        }

        protected void AddModelErrors<W>(W ct) where W : Lookup2Fields
        {
            if (String.IsNullOrWhiteSpace(ct.Code))
            {
                Session["Error"] = Resources.CodeIsEmpty;
                ModelState.AddModelError(PropertyMapping.ContainsKey(ct.GetType().GetProperty("Code")) ? PropertyMapping[ct.GetType().GetProperty("Code")] : "Code", Resources.CodeIsEmpty);
            }
            if (String.IsNullOrWhiteSpace(ct.Description))
            {
                ModelState.AddModelError(PropertyMapping.ContainsKey(ct.GetType().GetProperty("Description")) ? PropertyMapping[ct.GetType().GetProperty("Description")] : "Description", Resources.DescriptionIsEmpty);
            }
            if (HasDuplicate(ct))
            {
                ModelState.AddModelError(PropertyMapping.ContainsKey(ct.GetType().GetProperty("Code")) ? PropertyMapping[ct.GetType().GetProperty("Code")] : "Code", Resources.CodeAlreadyExists);
            }
            if (HasIsDefaultDuplicates(ct))
            {
                ModelState.AddModelError(PropertyMapping.ContainsKey(ct.GetType().GetProperty("IsDefault")) ? PropertyMapping[ct.GetType().GetProperty("IsDefault")] : "IsDefault", Resources.DefaultAllreadyExists);
            }
        }

        protected CriteriaOperator GridFilter { get; set; }
        protected string GridSortingField { get; set; }

        public virtual ActionResult Grid()
        {
            if (Request["DXCallbackArgument"] != null)
            {
                if (Request["DXCallbackArgument"].Contains("STARTEDIT") && UserCanEditRequest() == false)
                {
                    Session["Error"] = Resources.YouCannotEditThisElement;
                    return null;
                }
                if (Request["DXCallbackArgument"].Contains("DELETE") && UserCanEditRequest() == false)
                {
                    Session["Error"] = Resources.CannotDeleteObject;
                    return null;
                }

                if (Request["DXCallbackArgument"].Contains("DELETESELECTED"))
                {
                    ViewData["CallbackMode"] = "DELETESELECTED";
                    if (TableCanDelete)
                    {
                        using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                        {
                            List<Guid> oids = new List<Guid>();
                            string allOids = Request["DXCallbackArgument"].Split(new string[] { "DELETESELECTED|" }, new StringSplitOptions())[1].Trim(';');
                            string[] unparsed = allOids.Split(',');
                            foreach (string unparsedOid in unparsed)
                            {
                                oids.Add(Guid.Parse(unparsedOid));
                            }
                            if (oids.Count > 0)
                            {
                                try
                                {
                                    DeleteAll(uow, oids);
                                }
                                catch (ConstraintViolationException)
                                {
                                    Session["Error"] = Resources.CannotDeleteObject;
                                }
                                catch (Exception e)
                                {
                                    Session["Error"] = e.Message;
                                }
                            }
                        }
                    }
                }
                else if (Request["DXCallbackArgument"].Contains("APPLYCOLUMNFILTER"))
                {
                    ViewData["CallbackMode"] = "APPLYCOLUMNFILTER";
                }
            }
            FillLookupComboBoxes();
            return PartialView("Grid", GetList<T>(XpoSession, GridFilter, GridSortingField).AsEnumerable());
        }

        protected bool HasIsDefaultDuplicates(BasicObj objekt)
        {
            Type type = objekt.GetType();
            PropertyInfo propertyIsDefault = type.GetProperty("IsDefault");
            bool acceptsDefaultValues = propertyIsDefault != null;
            if (acceptsDefaultValues)
            {
                bool ctIsDefaultValue = (bool)propertyIsDefault.GetValue(objekt, null);
                if (ctIsDefaultValue)
                {
                    IEnumerable<string> uniqueFields = objekt.GetType().GetCustomAttributes(typeof(IsDefaultUniqueFieldsAttribute), true).SelectMany(attribute => ((IsDefaultUniqueFieldsAttribute)attribute).UniqueFields);
                    List<BinaryOperator> binaryOperators = new List<BinaryOperator>() { new BinaryOperator("IsDefault", true),
                                                                                            new BinaryOperator("Oid", objekt.Oid, BinaryOperatorType.NotEqual)
                                                                                          };
                    if (uniqueFields.Count() > 0)
                    {
                        foreach (string uniqueField in uniqueFields)
                        {
                            PropertyInfo uniqueFieldProperty = objekt.GetType().GetProperty(uniqueField);
                            if (uniqueField == null)
                            {
                                throw new Exception(Resources.IsDefaultUniqueFieldsAreNotCorrectlyDefined);
                            }
                            object Value;
                            if (typeof(BasicObj).IsAssignableFrom(uniqueFieldProperty.PropertyType))
                            {
                                Value = ((BasicObj)uniqueFieldProperty.GetValue(objekt, null)).Oid;
                            }
                            else
                            {
                                Value = uniqueFieldProperty.GetValue(objekt, null);
                            }
                            binaryOperators.Add(new BinaryOperator(uniqueField, Value));
                        }
                    }

                    CriteriaOperator crop = CriteriaOperator.And(binaryOperators);
                    crop = ApplyOwnerCriteria(crop, objekt.GetType(), EffectiveOwner);

                    int countedIsDeafultValues = (int)objekt.Session.Evaluate(type, CriteriaOperator.Parse("Count()"), crop);
                    return countedIsDeafultValues > 0;
                }
            }
            return false;
        }

        [HttpPost]
        public virtual ActionResult InsertPartial([ModelBinder(typeof(RetailModelBinder))] T ct)
        {
            if (!TableCanInsert)
            {
                return null;
            }
            UpdateLookupObjects(ct);
            if (typeof(Lookup2Fields).IsAssignableFrom(typeof(T)))
            {
                AddModelErrors(ct as Lookup2Fields);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (Save(ct))
                    {
                        Session["Notice"] = Resources.SavedSuccesfully;
                    }
                    else
                    {
                        Session["Error"] = Resources.CodeAlreadyExists;
                    }
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }
            else
            {
                ViewBag.CurrentItem = ct;
            }
            FillLookupComboBoxes();
            return PartialView("Grid", GetList<T>(XpoSession).AsEnumerable());
        }
        [HttpPost]
        public virtual ActionResult UpdatePartial([ModelBinder(typeof(RetailModelBinder))] T ct)
        {
            if (!TableCanUpdate)
            {
                return null;
            }

            UpdateLookupObjects(ct);
            if (typeof(Lookup2Fields).IsAssignableFrom(typeof(T)))
            {
                AddModelErrors(ct as Lookup2Fields);
            }


            if (ModelState.IsValid)
            {
                try
                {
                    Save(ct);
                    Session["Notice"] = Resources.SavedSuccesfully;
                }
                catch (Exception e)
                {

                    if (Session["Error"] == null || String.IsNullOrWhiteSpace(Session["Error"].ToString()))
                    {
                        Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                    }
                }
            }
            else
            {
                ViewBag.CurrentItem = ct;
            }
            FillLookupComboBoxes();
            return PartialView("Grid", GetList<T>(XpoSession).AsEnumerable());
        }

        [HttpPost]
        public virtual ActionResult DeletePartial([ModelBinder(typeof(RetailModelBinder))] T ct)
        {
            if (!TableCanDelete)
            {
                return null;
            }

            try
            {
                Delete(ct);
            }
            catch (Exception e)
            {
                Session["Error"] = e.Message;
            }
            FillLookupComboBoxes();
            return PartialView("Grid", GetList<T>(XpoSession).AsEnumerable());
        }

        public virtual JsonResult JsonUserHasPermission(string StringOid)
        {
            bool result = false;
            if (!typeof(IOwner).IsAssignableFrom(typeof(T)))
            {
                result = true;
            }
            else
            {
                Guid Oid;
                if (Guid.TryParse(StringOid, out Oid))
                {
                    T Object = XpoSession.GetObjectByKey<T>(Oid);
                    result = UserCanPerformAction(Object);
                }
            }
            return Json(new { Permitted = result });
        }

        public virtual JsonResult JsonOwnerIsSelected(string StringOid)
        {
            bool result = false;
            if (UserHelper.IsSystemAdmin(CurrentUser) == false)
            {
                result = true;
            }
            else if (typeof(IOwner).IsAssignableFrom(typeof(T)))
            {
                result = (this.CurrentOwner != null);
            }
            else
            {
                result = true;
            }
            return Json(new { Result = result });
        }

        protected void SaveObjectTemp(T objekt, User user)
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                TemporaryObject tempObject = objekt.GetTemporaryObject(uow);
                if (tempObject == null)
                {
                    tempObject = new TemporaryObject(uow);
                    tempObject.CreatedBy = uow.GetObjectByKey<User>(user.Oid);
                    tempObject.EntityType = typeof(DocumentHeader).FullName;
                    tempObject.EntityOid = objekt.Oid;
                }
                tempObject.UpdatedBy = uow.GetObjectByKey<User>(user.Oid);
                tempObject.UpdateOnTicks = DateTime.Now.Ticks;
                tempObject.SerializedData = objekt.ToJson(Platform.PlatformConstants.JSON_SERIALIZER_SETTINGS);
                tempObject.Save();
                XpoHelper.CommitTransaction(uow);
            }
        }



        [Security(ReturnsPartial = false)]
        protected ActionResult ExportToFile<W>(GridViewSettings gridViewSettings, CriteriaOperator criteria) where W : BasicObj
        {
            if (gridViewSettings != null)
            {
                string allOids = Request["Oids"];
                if (!String.IsNullOrWhiteSpace(allOids))
                {
                    string[] strOids = allOids.Split(',');
                    List<Guid> oids = new List<Guid>();
                    foreach (string strOid in strOids)
                    {
                        Guid oid;
                        if (Guid.TryParse(strOid, out oid))
                        {
                            oids.Add(oid);
                        }
                    }
                    foreach (string typeName in GridExportHelper.ExportTypes.Keys)
                    {
                        if (Request.Params[typeName] != null)
                        {
                            return GridExportHelper.ExportTypes[typeName].Method(gridViewSettings, GetList<W>(XpoHelper.GetNewUnitOfWork(),
                                                                                                                        CriteriaOperator.And(new InOperator("Oid", oids), criteria)));
                        }
                    }
                }
            }
            return RedirectToAction("Index");
        }


        [Security(ReturnsPartial = false)]
        protected ActionResult ExportDetailsToFile(GridViewSettings gridViewSettings, List<MergedDocumentDetail> details)
        {
            if (gridViewSettings != null)
            {
                //GridViewExtension.ExportToPdf(gridViewSettings, details, true);
                //GridViewExtension.ExportToXls(gridViewSettings, details, true);
                return GridViewExtension.ExportToXlsx(gridViewSettings, details);

            }
            return RedirectToAction("Index");
        }


    }
}
