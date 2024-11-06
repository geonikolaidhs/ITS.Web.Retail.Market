using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;
using ITS.Retail.WebClient.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web.Mvc;

namespace ITS.Retail.WebClient.Controllers
{
    public class FormMessageController : BaseObjController<FormMessage>
    {
        public override ActionResult LoadEditPopup()
        {
            base.LoadEditPopup();

            ViewBag.Title = Resources.EditFormMessages;
            
            ActionResult rt = PartialView("LoadEditPopup");
            return rt;
        }

        [Security(ReturnsPartial = false), Display(ShowSettings=true)]
        public ActionResult Index()
        {
            FillLookupComboBoxes();
            ToolbarOptions.ExportToButton.Visible = false;
            ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            ToolbarOptions.NewButton.OnClick = "AddNewCustomV2";
            ToolbarOptions.EditButton.OnClick = "EditSelectedRowsCustomV2"; 
            ToolbarOptions.ViewButton.Visible = true;
            ToolbarOptions.ViewButton.OnClick = "ShowGenericView";
            ToolbarOptions.OptionsButton.Visible = false;

            CustomJSProperties.AddJSProperty("editAction", "Edit");
            CustomJSProperties.AddJSProperty("editIDParameter", "FormMessageID");
            CustomJSProperties.AddJSProperty("gridName", "grdFormMessages");

            return View("Index", GetList<FormMessage>(XpoHelper.GetNewUnitOfWork()).AsEnumerable<FormMessage>());
        }

        protected override GenericViewRuleset GenerateGenericViewRuleset()
        {
            GenericViewRuleset ruleset = base.GenerateGenericViewRuleset();
            ruleset.PropertiesToIgnore.AddRange(new List<string>() { "Description","OwnershipScope", "IsActive", "FieldName","ObjectName" });

            ruleset.DetailPropertiesToIgnore.Add(typeof(ControllerMessage), new List<string>() { "IsActive", "IsDefault"});
            ruleset.DetailPropertiesToIgnore.Add(typeof(FormMessageDetail), new List<string>() { "Parent", "IsActive" });
            ruleset.NumberOfColumns = 2;
            return ruleset;
        }


        public ActionResult Edit(String Oid)
        { 
            Guid FormMessageGuid;
            if (!Guid.TryParse(Oid, out FormMessageGuid))
                FormMessageGuid = Guid.Empty;
            FormMessage formMessage;

            if (FormMessageGuid != Guid.Empty)
            {
                formMessage = XpoSession.FindObject<FormMessage>(new BinaryOperator("Oid", FormMessageGuid));
                if (this.UserCanEdit(formMessage) == false)
                {
                    Session["Error"] = Resources.YouCannotEditThisElement;                    
                }
            }
            else
            {
                UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
                Session["formMessageUow"] = uow;
                formMessage = new FormMessage(uow);
                Session["newFormMessage"] = formMessage;
                FormMessageGuid = formMessage.Oid;
                AssignOwner(formMessage);
            }

            ViewData["FormMessage_ID"] = FormMessageGuid;
            FillLookupComboBoxes();
            return PartialView(formMessage);
            
        }

        public JsonResult Save(String FormMessageID)
        {
            Guid FormMessageGuid;
            if (!Guid.TryParse(FormMessageID, out FormMessageGuid))
            {
                FormMessageGuid = Guid.Empty;
            }

            FormMessage formMessage;
            formMessage = XpoSession.FindObject<FormMessage>(new BinaryOperator("Oid", FormMessageGuid));
            if (formMessage == null)
            {
                formMessage = Session["newFormMessage"] as FormMessage;
            }
            formMessage.ObjectName = "Controller";
            formMessage.FieldName = "None";
            formMessage.MessagePlace = (eMessageType)Enum.Parse(typeof(eMessageType), Request["MessagePlace"]);

            formMessage.Save();
            XpoHelper.CommitTransaction(formMessage.Session);
            Session["newFormMessage"] = null;
            Session["formMessageUow"] = null;            
            
            return Json(new { });                    
        }

        public ActionResult FormMessageDetailGrid(String FormMessageID)
        {
            Guid FormMessageGuid;
            if (!Guid.TryParse(FormMessageID, out FormMessageGuid))
                FormMessageGuid = Guid.Empty;
            FormMessage formMessage = XpoSession.FindObject<FormMessage>(new BinaryOperator("Oid", FormMessageGuid));
            if (formMessage == null)
            {
                formMessage = Session["newFormMessage"] as FormMessage;
            }
			ViewData["FormMessage_ID"] = FormMessageGuid;
            FillLookupComboBoxes();
            if (Request["DXCallbackArgument"].Contains("ADDNEWROW"))
            {
                ViewBag.Cultures = CultureHelper.Cultures.Where(x => formMessage.FormMessageDetails.Select(f => f.Locale).Contains(x) == false);
            }
            else if (Request["DXCallbackArgument"].Contains("STARTEDIT"))
            {
                String oid = Request["DXCallbackArgument"].ToString().Split('|').Last().Split(';').First();
                Guid guid;
                Guid.TryParse(oid, out guid);
                ViewBag.Cultures = CultureHelper.Cultures.Where(x => formMessage.FormMessageDetails.Select(f => f.Locale).Contains(x) == false || formMessage.FormMessageDetails.FirstOrDefault(f=>f.Oid==guid).Locale==x);
            }
            return PartialView(formMessage.FormMessageDetails);
        }


        public ActionResult CancelEdit()
        {
            Session["newFormMessage"] = null;
            Session["formMessageUow"] = null;
            return null;
        }


		[HttpPost]
        public ActionResult FormMessageDetailInlineEditingUpdatePartial([ModelBinder(typeof(RetailModelBinder))] FormMessageDetail ct)
        {
            FillLookupComboBoxes();
            
            ct.Locale = Request["Locale"].ToString();
            ct.Description = WebUtility.HtmlDecode(Request["Description"].ToString());
			if (Session["newFormMessage"] as FormMessage == null)
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    if (Request["Description"] != null && Request["Description"].ToString() != null && Request["Description"].ToString().Length > 1)
                    {
                        FormMessage fm = uow.FindObject<FormMessage>(new BinaryOperator("Oid", Request["FormMessageID"]));
                        FormMessageDetail newfmd = uow.FindObject<FormMessageDetail>(new BinaryOperator("Oid", Request["Oid"]));
                        newfmd.FormMessage = fm;
                        newfmd.Locale = Request["Locale"].ToString();
                        newfmd.Description = WebUtility.HtmlDecode(Request["Description"].ToString());
						newfmd.IsDefault = Request["IsDefaultCheckbox"] == "C";
                        newfmd.Save();
                        if (FormMessageHelper.FormMessageIsValid(fm) == false)
                        {
                            newfmd.FormMessage = null;
                            throw new Exception(Resources.DefaultAllreadyExists);
                        }
                        XpoHelper.CommitTransaction(uow);
                    }
                }
                UnitOfWork uow2 = XpoHelper.GetNewUnitOfWork();
				FormMessage formMessage = uow2.FindObject<FormMessage>(new BinaryOperator("Oid", Request["FormMessageID"]));
				ViewData["FormMessage_ID"] = formMessage.Oid;
				return PartialView("FormMessageDetailGrid", formMessage.FormMessageDetails);
            }
            else
            {
                Guid reap;
                if (!Guid.TryParse(Request["Oid"].ToString(), out reap))
                {
                    //error
                    //todo
                }
                UnitOfWork uow = Session["formMessageUow"] as UnitOfWork;
                FormMessage fm = Session["newFormMessage"] as FormMessage;
                if (Request["Description"] != null && Request["Description"].ToString() != null && Request["Description"].ToString().Length > 1)
                {
                    if (fm.FormMessageDetails.Where(g => g.Locale == Request["Locale"].ToString() && g.Oid != ct.Oid).Count() > 0)
                    {
                        ModelState.AddModelError("Locale", Resources.KeyCodeAlreadyExists);

                        Session["Error"] = Resources.KeyCodeAlreadyExists;
                    }
                    else
                    {
                        FormMessageDetail fmd = fm.FormMessageDetails.FirstOrDefault(g => g.Oid == reap);
                        if (fmd != null)
                        {
                            fmd.Locale = Request["Locale"].ToString();
                            fmd.IsDefault = Request["IsDefaultCheckbox"] == "C";
                            fmd.Description = Request["Description"].ToString();
                            if (FormMessageHelper.FormMessageIsValid(fm) == false)
                            {
                                throw new Exception(Resources.DefaultAllreadyExists);
                            }
                            fmd.Save();
                        }
                    }
                }
                ViewData["FormMessage_ID"] = fm.Oid;
                return PartialView("FormMessageDetailGrid", fm.FormMessageDetails);
            }
        }        

		[HttpPost]
        public ActionResult FormMessageDetailInlineEditingAddNewPartial([ModelBinder(typeof(RetailModelBinder))] FormMessageDetail ct)
        {
            FillLookupComboBoxes();
            ct.Locale = Request["Locale"].ToString();
            ct.Description = WebUtility.HtmlDecode(Request["Description"].ToString());
            if (Session["newFormMessage"] as FormMessage == null)
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    if (Request["Description"] != null && Request["Description"].ToString() != null && Request["Description"].ToString().Length > 1)
                    {
                        FormMessage fm = uow.FindObject<FormMessage>(new BinaryOperator("Oid", Request["FormMessageID"]));
                        FormMessageDetail newfmd = new FormMessageDetail(uow);
                        newfmd.FormMessage = fm;
                        newfmd.Locale = Request["Locale"].ToString();
                        newfmd.Description = WebUtility.HtmlDecode(Request["Description"].ToString());
                        newfmd.IsDefault = Request["IsDefaultCheckbox"] == "C";
                        if (FormMessageHelper.FormMessageIsValid(fm) == false)
                        {
                            newfmd.FormMessage = null;
                            throw new Exception(Resources.DefaultAllreadyExists);
                        }
                        newfmd.Save();
                        XpoHelper.CommitTransaction(uow);
                    }
                }
                UnitOfWork uow2 = XpoHelper.GetNewUnitOfWork();
                FormMessage formMessage = uow2.FindObject<FormMessage>(new BinaryOperator("Oid", Request["FormMessageID"]));
                ViewData["FormMessage_ID"] = formMessage.Oid;
                return PartialView("FormMessageDetailGrid", formMessage.FormMessageDetails);
            }
            else
            {
                UnitOfWork uow = Session["formMessageUow"] as UnitOfWork;
                FormMessage fm = Session["newFormMessage"] as FormMessage;
                if (Request["Description"] != null && Request["Description"].ToString() != null && Request["Description"].ToString().Length > 1)
                {
                    if (fm.FormMessageDetails.Where(g => g.Locale == Request["Locale"].ToString() && g.Oid != ct.Oid).Count() > 0)
                    {
                        ModelState.AddModelError("Locale", Resources.KeyCodeAlreadyExists);
                        Session["Error"] = Resources.KeyCodeAlreadyExists;
                    }
                    else
                    {
                        FormMessageDetail newfmd = new FormMessageDetail(uow);
                        newfmd.FormMessage = fm;
                        newfmd.Locale = Request["Locale"].ToString();
                        newfmd.Description = Request["Description"].ToString();
                        newfmd.IsDefault = Request["IsDefaultCheckbox"] == "C";
                        if (FormMessageHelper.FormMessageIsValid(fm) == false)
                        {
                            newfmd.FormMessage = null;
                            ModelState.AddModelError("IsDefaultCheckbox", Resources.DefaultAllreadyExists);
                            Session["Error"] = Resources.KeyCodeAlreadyExists;
                        }
                    }
                }
                ViewData["FormMessage_ID"] = fm.Oid;
                return PartialView("FormMessageDetailGrid", fm.FormMessageDetails);
            }
        }

        [HttpPost]
        public ActionResult FormMessageDetailInlineEditingDeletePartial([ModelBinder(typeof(RetailModelBinder))] FormMessageDetail ct)
        {
            FillLookupComboBoxes();
			if (Session["newFormMessage"] as FormMessage == null)
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    
                    {
                        FormMessageDetail newPermission = uow.FindObject<FormMessageDetail>(new BinaryOperator("Oid", Request["Oid"]));
                        ViewData["FormMessage_ID"] = newPermission.FormMessage.Oid;
                        newPermission.Delete();
                        //uow.CommitTransaction();
                        XpoHelper.CommitTransaction(uow);
                    }
                }
                UnitOfWork uow2 = XpoHelper.GetNewUnitOfWork();
                FormMessage rol = uow2.FindObject<FormMessage>(new BinaryOperator("Oid", ViewData["FormMessage_ID"]));

                return PartialView("FormMessageDetailGrid", rol.FormMessageDetails);
            }
            else
            {
                Guid reap;
                if (!Guid.TryParse(Request["Oid"].ToString(), out reap))
                {
                    //error
                    //todo
                }
                UnitOfWork uow = Session["formMessageUow"] as UnitOfWork;
				FormMessage formMessage = Session["newFormMessage"] as FormMessage;
				foreach (FormMessageDetail newPermission in formMessage.FormMessageDetails)
                {
                    if (newPermission.Oid == reap)
                    {
                        newPermission.Delete();
                    }
                }
				ViewData["FormMessage_ID"] = formMessage.Oid;
				return PartialView("FormMessageDetailGrid", formMessage.FormMessageDetails);
            }
        }
       
        [HttpPost]
        public ActionResult InlineEditingDeletePartial([ModelBinder(typeof(RetailModelBinder))] FormMessage ct)
        {
            try
            {
                Delete(ct);
            }
            catch (Exception e)
            {
                Session["Error"] = e.Message;// +Environment.NewLine + e.StackTrace;
            }
            return PartialView("Grid", GetList<FormMessage>(XpoHelper.GetNewUnitOfWork()).AsEnumerable<FormMessage>());
        }

        public ActionResult ControllerGrid(String FormMessageID)
        {
            Guid FormMessageGuid;
            if (!Guid.TryParse(FormMessageID, out FormMessageGuid))
                FormMessageGuid = Guid.Empty;
            FormMessage formMessage = XpoSession.FindObject<FormMessage>(new BinaryOperator("Oid", FormMessageGuid));
            if (formMessage == null)
            {
                formMessage = Session["newFormMessage"] as FormMessage;
            }
            ViewData["FormMessage_ID"] = FormMessageGuid;
            FillLookupComboBoxes();
            return PartialView(formMessage.ControllerMessages);
        }

        public ActionResult ControllerInlineEditingAddNewPartial([ModelBinder(typeof(RetailModelBinder))] ControllerMessage ct)
        {
            if (Session["newFormMessage"] as FormMessage == null)
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    if (Request["Description"] != null && Request["Description"].ToString() != null && Request["Description"].ToString().Length > 1)
                    {
                        FormMessage fm = uow.FindObject<FormMessage>(new BinaryOperator("Oid", Request["FormMessageID"]));
                        ControllerMessage newfmd = new ControllerMessage(uow);
                        newfmd.FormMessage = fm;
                        newfmd.Description = Request["Description"].ToString();
                        newfmd.Save();
                        XpoHelper.CommitChanges(uow);
                    }
                }
                UnitOfWork uow2 = XpoHelper.GetNewUnitOfWork();
                FormMessage formMessage = uow2.FindObject<FormMessage>(new BinaryOperator("Oid", Request["FormMessageID"]));
                ViewData["FormMessage_ID"] = formMessage.Oid;
                return PartialView("ControllerGrid", formMessage.ControllerMessages);
            }
            else
            {
                UnitOfWork uow = Session["formMessageUow"] as UnitOfWork;
                FormMessage fm = Session["newFormMessage"] as FormMessage;
                if (Request["Description"] != null && Request["Description"].ToString() != null && Request["Description"].ToString().Length > 1)
                {
                    ControllerMessage newfmd = new ControllerMessage(uow);
                    newfmd.FormMessage = fm;
                    newfmd.Description = Request["Description"].ToString();
                }
                ViewData["FormMessage_ID"] = fm.Oid;
                return PartialView("ControllerGrid", fm.ControllerMessages);
            }            
        }

        [HttpPost]
        public ActionResult ControllerInlineEditingUpdatePartial([ModelBinder(typeof(RetailModelBinder))] ControllerMessage ct)
        {
            if (Session["newFormMessage"] as FormMessage == null)
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    if (Request["Description"] != null && Request["Description"].ToString() != null && Request["Description"].ToString().Length > 1)
                    {
                        FormMessage fm = uow.FindObject<FormMessage>(new BinaryOperator("Oid", Request["FormMessageID"]));                        
                        ControllerMessage newfmd = uow.FindObject<ControllerMessage>(new BinaryOperator("Oid", Request["Oid"]));
                        newfmd.FormMessage = fm;
                        newfmd.Description = Request["Description"].ToString();
                        newfmd.Save();
                        XpoHelper.CommitChanges(uow);
                    }
                }
                UnitOfWork uow2 = XpoHelper.GetNewUnitOfWork();
                FormMessage formMessage = uow2.FindObject<FormMessage>(new BinaryOperator("Oid", Request["FormMessageID"]));
                ViewData["FormMessage_ID"] = formMessage.Oid;
				return PartialView("ControllerGrid", formMessage.ControllerMessages);
            }
            else
            {
                Guid reap;
                if (!Guid.TryParse(Request["Oid"].ToString(), out reap))
                {
                    //error
                    throw new NotImplementedException();
                }
                UnitOfWork uow = Session["formMessageUow"] as UnitOfWork;
                FormMessage fm = Session["newFormMessage"] as FormMessage;
                if (Request["Description"] != null && Request["Description"].ToString() != null && Request["Description"].ToString().Length > 1)
                {
                    foreach (ControllerMessage fmd in fm.ControllerMessages)
                    {
                        if (fmd.Oid == reap)
                        {
                            //fmd.Code = Request["Code"].ToString();
                            fmd.Description = Request["Description"].ToString();
                            fmd.Save();
                        }
                    }
                }
                ViewData["FormMessage_ID"] = fm.Oid;
				return PartialView("ControllerGrid", fm.ControllerMessages);
            }
        }

        [HttpPost]
        public ActionResult ControllerInlineEditingDeletePartial([ModelBinder(typeof(RetailModelBinder))] ControllerMessage ct)
        {
            if (Session["newFormMessage"] as FormMessage == null)
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {

                    {
                        ControllerMessage newPermission = uow.FindObject<ControllerMessage>(new BinaryOperator("Oid", Request["Oid"]));
                        ViewData["FormMessage_ID"] = newPermission.FormMessage.Oid;
                        newPermission.Delete();
                        //uow.CommitTransaction();
                        XpoHelper.CommitTransaction(uow);
                    }
                }
                UnitOfWork uow2 = XpoHelper.GetNewUnitOfWork();
                FormMessage rol = uow2.FindObject<FormMessage>(new BinaryOperator("Oid", ViewData["FormMessage_ID"]));

				return PartialView("ControllerGrid", rol.ControllerMessages);
            }
            else
            {
                Guid reap;
                if (!Guid.TryParse(Request["Oid"].ToString(), out reap))
                {
                    //error
                    //todo
                }
                UnitOfWork uow = Session["formMessageUow"] as UnitOfWork;
                FormMessage formMessage = Session["newFormMessage"] as FormMessage;
                foreach (ControllerMessage newPermission in formMessage.ControllerMessages)
                {
                    if (newPermission.Oid == reap)
                    {
                        newPermission.Delete();
                    }
                }
                ViewData["FormMessage_ID"] = formMessage.Oid;
				return PartialView("ControllerGrid", formMessage.ControllerMessages);
            }
        }

        protected override void FillLookupComboBoxes()
        {
            base.FillLookupComboBoxes();
            IEnumerable<Type> types = Assembly.GetExecutingAssembly().GetTypes().ToList()
                .Where(t => t.Namespace == "ITS.Retail.WebClient.Controllers" && t.IsClass == true && t.IsPublic == true &&
                    t.IsAbstract == false && t.BaseType.Name != "Controller" && t.BaseType.Name != "BaseController" &&
                    t.GetMember("Index").Length > 0
                    ).ToList();
            ViewBag.ControllerList = types.Select(p => new KeyValuePair<String, String>(p.Name, p.ToLocalizedString())).OrderBy(p => p.Value).ToDictionary(p => p.Key, p => p.Value);

			ViewBag.MessagePlaces = Enum.GetNames(typeof(eMessageType));
        }

    }
}
