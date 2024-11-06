using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Extensions;
using ITS.Retail.WebClient.Helpers;

namespace ITS.Retail.WebClient.Controllers
{
    [RoleAuthorize]
    public class TemporaryObjectController : BaseController
    {
        public ActionResult Index()
        {
            ToolbarOptions ToolbarOptions = new ToolbarOptions();
            ToolbarOptions.DeleteButton.Visible = true;
            ToolbarOptions.EditButton.Visible = true;
            ToolbarOptions.NewButton.Visible = false;
            ToolbarOptions.ExportToButton.Visible = false;
            ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            

            this.CustomJSProperties.AddJSProperty("gridName", "grdTempObject");


            ToolbarOptions.EditButton.OnClick = "EditSelectedRowsCustomV2";

            this.CustomJSProperties.AddJSProperty("editAction", "StartEdit");
            this.CustomJSProperties.AddJSProperty("editIDParameter", "Oid");

            ViewData["ToolbarOptions"] = ToolbarOptions;    
            return View(GetList<TemporaryObject>(this.XpoSession));
        }

        public JsonResult JsonUserHasPermission(Guid StringOid)
        {
            if(this.CurrentUser.Role.Type == Platform.Enumerations.eRoleType.CompanyAdministrator || this.CurrentUser.Role.Type == Platform.Enumerations.eRoleType.SystemAdministrator)
            {
                return Json(new { Permitted = true });
            }
            return Json(new { Permitted = false });
        }

        public JsonResult StartEdit(Guid Oid)
        {
            
            TemporaryObject obj = XpoSession.GetObjectByKey<TemporaryObject>(Oid);
            if (obj == null)
            {
                Session["Error"] = Resources.ItemNotFound;
            }
            else if (obj.EntityType == typeof(DocumentHeader).FullName)
            {                
                DocumentHeader dh = new DocumentHeader(XpoSession);
                string error;
                dh.FromJson(obj.SerializedData, Platform.PlatformConstants.JSON_SERIALIZER_SETTINGS, true,false,out error);
                if (String.IsNullOrWhiteSpace(error))
                {
                    return Json(new { Success = true });
                }
                else
                {
                    Session["Error"] = error;
                }
            }
            
            return Json(new { Error = Session["Error"] });
        }

        public  ActionResult Grid()
        {
            if (Request["DXCallbackArgument"] != null)
            {
                if (Request["DXCallbackArgument"].Contains("DELETESELECTED"))
                {
                    ViewData["CallbackMode"] = "DELETESELECTED";                    
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
                                    XPCollection<TemporaryObject> objects = GetList<TemporaryObject>(uow, CriteriaOperator.Or(oids.Select(x => new BinaryOperator("Oid", x))));
                                    uow.Delete(objects);
                                    XpoHelper.CommitChanges(uow);
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
            return PartialView(GetList<TemporaryObject>(this.XpoSession));
        }
    }
}
