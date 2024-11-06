using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DevExpress.Xpo;
using ITS.Retail.Model;
using ITS.Retail.Common;
using DevExpress.Data.Filtering;
using System.Reflection;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Providers;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;
using ITS.Retail.WebClient.Helpers;

namespace ITS.Retail.WebClient.Controllers
{
    public class RoleController : BaseObjController<Role>
    {
        public override ActionResult LoadEditPopup()
        {
            base.LoadEditPopup();

            ViewBag.Title = Resources.EditRole;

            ActionResult rt = PartialView("LoadEditPopup");
            return rt;
        }

        [Security(ReturnsPartial = false), Display(ShowSettings = true)]
        public ActionResult Index()
        {

            FillLookupComboBoxes();
            ToolbarOptions.ExportToButton.Visible = false;
            ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            ToolbarOptions.NewButton.OnClick = "AddNewCustomV2";
            ToolbarOptions.EditButton.OnClick = "EditSelectedRowsCustomV2";
            ToolbarOptions.ViewButton.OnClick = "ShowGenericView";
            ToolbarOptions.ViewButton.Visible = true;
            ToolbarOptions.OptionsButton.Visible = false;

            CustomJSProperties.AddJSProperty("editAction", "Edit");
            CustomJSProperties.AddJSProperty("editIDParameter", "RoleID");
            CustomJSProperties.AddJSProperty("gridName", "grdRoles");

            return View("Index", GetList<Role>(XpoHelper.GetNewUnitOfWork()).AsEnumerable());
        }
                
        protected override GenericViewRuleset GenerateGenericViewRuleset()
        {
            GenericViewRuleset ruleset = base.GenerateGenericViewRuleset();
            ruleset.PropertiesToIgnore.AddRange(new List<string>() { "IsDefault", "IsActive" });
            ruleset.DetailsToIgnore.AddRange(new List<string>() { "Users", "DocumentTypeRoles", "CustomDataViews" });
            ruleset.DetailPropertiesToIgnore.Add(typeof(RoleEntityAccessPermision), new List<string>() { "IsActive" });
            ruleset.NumberOfColumns = 2;
            return ruleset;
        }


        public ActionResult Edit(String Oid)
        {            
            Guid RoleGuid;
            if (!Guid.TryParse(Oid, out RoleGuid))
                RoleGuid = Guid.Empty;
            Role role;

            if (RoleGuid != Guid.Empty)
            {
                role = XpoSession.FindObject<Role>(new BinaryOperator("Oid", RoleGuid));
                if (role.Type == eRoleType.SystemAdministrator)
                {
                    Session["Notice"] = "Administrator role is protected";
                }
            }
            else
            {
                UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
                Session["roleUow"] = uow;
                role = new Role(uow);
                Session["newRole"] = role;
                RoleGuid = role.Oid;
            }

            ViewData["Role_ID"] = RoleGuid;
            FillLookupComboBoxes();
            IDictionary<eRoleType, string> roleTypes = Enum<eRoleType>.GetLocalizedDictionary();
            if(!UserHelper.IsSystemAdmin(CurrentUser))
            {
                roleTypes.Remove(eRoleType.SystemAdministrator);
            }
            ViewBag.RoleTypes = roleTypes;
            return PartialView(role);
        }

        public JsonResult Save()
        {          
            Guid RoleGuid;
            if (Guid.TryParse(Request["Oid"], out RoleGuid))
            {
                eRoleType eRoleType;
                Role role = XpoSession.GetObjectByKey<Role>(RoleGuid) ?? Session["newRole"] as Role;
                role.Description = Request["Description"];
               if (Request["GDPREnabled"].ToString()=="C")
                {
                    role.GDPREnabled = true;
                }
                else
                {
                    role.GDPREnabled = false;
                }
                if (Request["GDPRActions"].ToString() == "C")
                {
                    role.GDPRActions = true;
                }
                else
                {
                    role.GDPRActions = false;
                }
                Enum.TryParse<eRoleType>(Request["Type_VI"], out eRoleType);
                role.Type = eRoleType;
                role.Save();
                Session["newRole"] = null;
                Session["roleUow"] = null;
                XpoHelper.CommitTransaction(role.Session);
            }
            return Json(new { });      
        }

        public ActionResult EntityAccessPermisionsGrid(String RoleID)
        {
            Guid RoleGuid;
            if (!Guid.TryParse(RoleID, out RoleGuid))
            {
                RoleGuid = Guid.Empty;
            }
            Role role = XpoSession.FindObject<Role>(new BinaryOperator("Oid", RoleGuid));
            if (role == null)
            {
                role = Session["newRole"] as Role;
            }
            ViewData["Role_ID"] = RoleGuid;
            FillLookupComboBoxes();
            IEnumerable<string> existingControllers = role.RoleEntityAccessPermisions.Select(x => x.EnityAccessPermision.EntityType);
            ViewBag.ControllerList = (ViewBag.ControllerList as IEnumerable<string>).Except(existingControllers);
            if (Request["DXCallbackArgument"]!=null && Request["DXCallbackArgument"].Contains("STARTEDIT"))
            {
                String oid = Request["DXCallbackArgument"].ToString().Split('|').Last().Split(';').First();
                var eap = role.RoleEntityAccessPermisions.FirstOrDefault(x => x.Oid == Guid.Parse(oid));
                if (eap != null)
                {
                    ViewBag.ControllerList = (ViewBag.ControllerList as IEnumerable<string>).ToList();
                    (ViewBag.ControllerList as List<string>).Add(eap.EnityAccessPermision.EntityType);
                    ViewBag.ControllerList = (ViewBag.ControllerList as List<string>).OrderBy(x => x);
                }
            }
            return PartialView(role.RoleEntityAccessPermisions);
        }

        public ActionResult CancelEdit()
        {
            Session["newRole"] = null;
            Session["roleUow"] = null;
            return null;
        }

        [HttpPost]
        public ActionResult InlineEntityAccessPermisionsGridUpdatePartial([ModelBinder(typeof(RetailModelBinder))] RoleEntityAccessPermision ct)
        {
            if (Session["newRole"] as Role == null)
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    if (Request["EntityType"] != null && Request["EntityType"].ToString() != null && Request["EntityType"].ToString().Length > 1)
                    {
                        Role role = uow.FindObject<Role>(new BinaryOperator("Oid", Request["RoleID"]));
                        RoleEntityAccessPermision newPermission = uow.FindObject<RoleEntityAccessPermision>(new BinaryOperator("Oid", Request["Oid"]));
                        newPermission.Role = role;
                        if (newPermission.EnityAccessPermision == null)
                        {
                            newPermission.EnityAccessPermision = new EntityAccessPermision(uow);
                        }
                        newPermission.EnityAccessPermision.EntityType = Request["EntityType"];
                        newPermission.EnityAccessPermision.Visible = (Request["View"].ToString().ToUpper() == "C");
                        newPermission.EnityAccessPermision.CanInsert = (Request["Insert"].ToString().ToUpper() == "C") ;
                        newPermission.EnityAccessPermision.CanUpdate = (Request["Update"].ToString().ToUpper() == "C") ;
                        newPermission.EnityAccessPermision.CanDelete = (Request["Delete"].ToString().ToUpper() == "C") ;
                        newPermission.EnityAccessPermision.CanExport = (Request["Export"].ToString().ToUpper() == "C");
                        newPermission.Save();
                        XpoHelper.CommitTransaction(uow);
                    }
                }
                UnitOfWork uow2 = XpoHelper.GetNewUnitOfWork();
                Role rol = uow2.FindObject<Role>(new BinaryOperator("Oid", Request["RoleID"]));
                ViewData["Role_ID"] = rol.Oid;
                return PartialView("EntityAccessPermisionsGrid", rol.RoleEntityAccessPermisions);
            }
            else
            {
                Guid reap;
                if(!Guid.TryParse(Request["Oid"].ToString(), out reap))
                {
                    //error
                    //todo
                }
                UnitOfWork uow = Session["roleUow"] as UnitOfWork;
                Role role = Session["newRole"] as Role;
                if (Request["EntityType"] != null && Request["EntityType"].ToString() != null && Request["EntityType"].ToString().Length > 1)
                {
                    foreach (RoleEntityAccessPermision newPermission in role.RoleEntityAccessPermisions)
                    {
                        if (newPermission.Oid == reap)
                        {
                            if (newPermission.EnityAccessPermision == null)
                            {
                                newPermission.EnityAccessPermision = new EntityAccessPermision(uow);
                            }
                            newPermission.EnityAccessPermision.EntityType = Request["EntityType"];
                            newPermission.EnityAccessPermision.Visible = (Request["View"].ToString().ToUpper() == "C");
                            newPermission.EnityAccessPermision.CanInsert = (Request["Insert"].ToString().ToUpper() == "C");
                            newPermission.EnityAccessPermision.CanUpdate = (Request["Update"].ToString().ToUpper() == "C");
                            newPermission.EnityAccessPermision.CanDelete = (Request["Delete"].ToString().ToUpper() == "C");
                            newPermission.EnityAccessPermision.CanExport = (Request["Export"].ToString().ToUpper() == "C");
                            newPermission.Save();
                        }
                    }
                }
                ViewData["Role_ID"] = role.Oid;
                return PartialView("EntityAccessPermisionsGrid", role.RoleEntityAccessPermisions);
            }
        }

        [HttpPost]
        public ActionResult InlineEntityAccessPermisionsGridAddNewPartial([ModelBinder(typeof(RetailModelBinder))] RoleEntityAccessPermision ct)
        {
            if (Session["newRole"] as Role == null)
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    if (Request["EntityType"] != null && Request["EntityType"].ToString() != null && Request["EntityType"].ToString().Length > 1)
                    {
                        Role role = uow.FindObject<Role>(new BinaryOperator("Oid", Request["RoleID"]));
                        RoleEntityAccessPermision newPermission = new RoleEntityAccessPermision(uow);
                        newPermission.Role = role;
                        newPermission.EnityAccessPermision = new EntityAccessPermision(uow);
                        newPermission.EnityAccessPermision.EntityType = Request["EntityType"];
                        newPermission.EnityAccessPermision.Visible = (Request["View"].ToString().ToUpper() == "C");
                        newPermission.EnityAccessPermision.CanInsert = (Request["Insert"].ToString().ToUpper() == "C") ;
                        newPermission.EnityAccessPermision.CanUpdate = (Request["Update"].ToString().ToUpper() == "C") ;
                        newPermission.EnityAccessPermision.CanDelete = (Request["Delete"].ToString().ToUpper() == "C") ;
                        newPermission.EnityAccessPermision.CanExport = (Request["Export"].ToString().ToUpper() == "C");
                        newPermission.Save();
                        XpoHelper.CommitTransaction(uow);
                    }
                }
                UnitOfWork uow2 = XpoHelper.GetNewUnitOfWork();
                Role rol = uow2.FindObject<Role>(new BinaryOperator("Oid", Request["RoleID"]));
                ViewData["Role_ID"] = rol.Oid;
                return PartialView("EntityAccessPermisionsGrid", rol.RoleEntityAccessPermisions);
            }
            else
            {
                UnitOfWork uow = Session["roleUow"] as UnitOfWork;
                Role role = Session["newRole"] as Role;
                if (Request["EntityType"] != null && Request["EntityType"].ToString() != null && Request["EntityType"].ToString().Length > 1)
                {
                    RoleEntityAccessPermision newPermission = new RoleEntityAccessPermision(uow);
                    newPermission.Role = role;
                    newPermission.EnityAccessPermision = new EntityAccessPermision(uow);
                    newPermission.EnityAccessPermision.EntityType = Request["EntityType"];
                    newPermission.EnityAccessPermision.Visible = (Request["View"].ToString().ToUpper() == "C");
                    newPermission.EnityAccessPermision.CanInsert = (Request["Insert"].ToString().ToUpper() == "C");
                    newPermission.EnityAccessPermision.CanUpdate = (Request["Update"].ToString().ToUpper() == "C");
                    newPermission.EnityAccessPermision.CanDelete = (Request["Delete"].ToString().ToUpper() == "C");
                    newPermission.EnityAccessPermision.CanExport = (Request["Export"].ToString().ToUpper() == "C");
                }
                ViewData["Role_ID"] = role.Oid;
                return PartialView("EntityAccessPermisionsGrid", role.RoleEntityAccessPermisions);
            }
        }

        [HttpPost]
        public ActionResult InlineEntityAccessPermisionsGridDeletePartial([ModelBinder(typeof(RetailModelBinder))] RoleEntityAccessPermision ct)
        {
            if (Session["newRole"] as Role == null)
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    RoleEntityAccessPermision newPermission = uow.FindObject<RoleEntityAccessPermision>(new BinaryOperator("Oid", Request["Oid"]));
                    ViewData["RoleID"] = newPermission.Role.Oid;
                    newPermission.Delete();
                    XpoHelper.CommitTransaction(uow);
                }
                UnitOfWork uow2 = XpoHelper.GetNewUnitOfWork();
                Role rol = uow2.FindObject<Role>(new BinaryOperator("Oid", ViewData["RoleID"]));
                return PartialView("EntityAccessPermisionsGrid", rol.RoleEntityAccessPermisions);
            }
            else
            {
                Guid reap;
                if (!Guid.TryParse(Request["Oid"].ToString(), out reap))
                {
                    //error
                    //todo
                }
                UnitOfWork uow = Session["roleUow"] as UnitOfWork;
                Role role = Session["newRole"] as Role;
                foreach (RoleEntityAccessPermision newPermission in role.RoleEntityAccessPermisions.ToList())
                {
                    if (newPermission.Oid == reap)
                    {
                        newPermission.EnityAccessPermision.Delete();
                        newPermission.Delete();
                    }
                }
                ViewData["Role_ID"] = role.Oid;
                return PartialView("EntityAccessPermisionsGrid", role.RoleEntityAccessPermisions);
            }
        }

        protected override void FillLookupComboBoxes()
        {

            base.FillLookupComboBoxes();
            List<String> theList = Assembly.GetExecutingAssembly().GetTypes().Where
                                        (t => t.Namespace == "ITS.Retail.WebClient.Controllers" 
                                        && t.IsClass == true && t.IsPublic == true 
                                        && t.IsAbstract == false 
                                        && t.BaseType.Name != "Controller" 
                                        && t.GetMember("Index").Length > 0)
                                        .OrderBy(x=> x.Name).Select(mi => mi.Name.Substring(0, mi.Name.IndexOf("Controller"))).ToList();           
            ViewBag.ControllerList = theList;
        }
    }
}
