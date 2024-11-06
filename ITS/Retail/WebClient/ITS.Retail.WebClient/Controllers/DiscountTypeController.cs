using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.WebClient.Providers;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;

namespace ITS.Retail.WebClient.Controllers
{
    public class DiscountTypeController : BaseObjController<DiscountType>
    {
        public override ActionResult LoadEditPopup()
        {
            base.LoadEditPopup();

            ViewBag.Title = Resources.DiscountType;

            ActionResult rt = PartialView("LoadEditPopup");
            return rt;
        }

        [Security(ReturnsPartial = false), Display(ShowSettings = true)]
        public ActionResult Index()
        {
            ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            ToolbarOptions.EditButton.OnClick = "EditSelectedRowsCustomV2";// "EditSelectedRowsFromGrid";//          
            ToolbarOptions.NewButton.OnClick = "AddNewCustomV2";//"AddNewFromGrid";//
            ToolbarOptions.ViewButton.Visible = true;
            ToolbarOptions.ViewButton.OnClick = "ShowGenericView";
            ToolbarOptions.OptionsButton.Visible = false;

            this.CustomJSProperties.AddJSProperty("editAction", "Edit");
            this.CustomJSProperties.AddJSProperty("editIDParameter", "objectSid");
            this.CustomJSProperties.AddJSProperty("gridName", "grdDiscountType");
            return View(GetList<DiscountType>(XpoSession));
        }

        protected override GenericViewRuleset GenerateGenericViewRuleset()
        {
            GenericViewRuleset ruleset = base.GenerateGenericViewRuleset();
            ruleset.DetailPropertiesToIgnore.Add(typeof(DiscountTypeField), new List<string>() { "CreatedOn", "UpdatedOn", "IsActive" });
            ruleset.PropertiesToIgnore.Add("IsDefault");
            ruleset.PropertiesToIgnore.Add("IsActive");
            ruleset.NumberOfColumns = 2;
            return ruleset;
        }


        
        public ActionResult Edit(String Oid)
        {
            Guid objectId;
            if (Guid.TryParse(Oid, out objectId) == false)
            {
                objectId = Guid.Empty;
            }
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();            

            DiscountType obj;
            if (objectId == Guid.Empty)
            {
                obj = new DiscountType(uow);
            }
            else
            {
                obj = uow.GetObjectByKey<DiscountType>(objectId);
            }
            
            Session["DiscountType"] = obj;
            return PartialView(obj);
        }



        [Security(ReturnsPartial = false)]
        public ActionResult CancelEdit()
        {
            if ((Session["DiscountType"] as UnitOfWork) != null)
            {
                (Session["DiscountType"] as DiscountType).Session.Dispose();
            }
            Session["DiscountType"] = null;
            return null;
        }

        public ActionResult ValueGrid()
        {
            XPCollection<DiscountTypeField> modelValues = (Session["DiscountType"] as DiscountType) == null ? null : (Session["DiscountType"] as DiscountType).DiscountTypeFields;
            ViewBag.DiscountFields = DiscountHelper.OptionalDiscountFields().Select(propertyInfo => propertyInfo.Name);
            ViewBag.CustomEnumerationDefinitions = GetList<CustomEnumerationDefinition>(XpoHelper.GetNewUnitOfWork());
            return PartialView(modelValues);
        }

        public ActionResult InsertValue([ModelBinder(typeof(RetailModelBinder))] DiscountTypeField ct)
        {

            DiscountType editingObject = (Session["DiscountType"] as DiscountType);
            if (ModelState.IsValid)
            {
                DiscountTypeField newValue = new DiscountTypeField(editingObject.Session);
                newValue.FieldName = ct.FieldName;
                newValue.Label = ct.Label;
                Guid customEnumGui;
                if (Guid.TryParse(Request["CustomEnumerationComboBox_VI"], out customEnumGui))
                {
                    newValue.CustomEnumeration = newValue.Session.GetObjectByKey<CustomEnumerationDefinition>(customEnumGui);
                }
                editingObject.DiscountTypeFields.Add(newValue);
            }
            return PartialView("ValueGrid", editingObject.DiscountTypeFields);
        }

        public ActionResult UpdateValue([ModelBinder(typeof(RetailModelBinder))] DiscountTypeField ct)
        {         

            DiscountType editingObject = (Session["DiscountType"] as DiscountType);
            if (ModelState.IsValid)
            {
                DiscountTypeField updateValue = editingObject.DiscountTypeFields.FirstOrDefault(g => g.Oid == ct.Oid);
                updateValue.FieldName = ct.FieldName;
                updateValue.Label = ct.Label;
                Guid customEnumGui;
                if (Guid.TryParse(Request["CustomEnumerationComboBox_VI"], out customEnumGui))
                {
                     updateValue.CustomEnumeration = updateValue.Session.GetObjectByKey<CustomEnumerationDefinition>(customEnumGui);
                }
                editingObject.DiscountTypeFields.Add(updateValue);
            }
            return PartialView("ValueGrid", editingObject.DiscountTypeFields);
        }

        public ActionResult DeleteValue([ModelBinder(typeof(RetailModelBinder))] DiscountTypeField ct)
        {

            DiscountType editingObject = (Session["DiscountType"] as DiscountType);
            if (ModelState.IsValid)
            {
                DiscountTypeField deleteValue = editingObject.DiscountTypeFields.FirstOrDefault(g => g.Oid == ct.Oid);
                if (deleteValue != null)
                {
                    deleteValue.Delete();
                }
            }
            return PartialView("ValueGrid", editingObject.DiscountTypeFields);
        }
                
        public JsonResult Save()
        {
            DiscountType editingObject = (Session["DiscountType"] as DiscountType);
            try
            {
                editingObject.Code = Request["Code"].ToString();
                editingObject.Description = Request["Description"].ToString();
                editingObject.IsUnique = Request["IsUnique"] == "C" ? true : false;
                editingObject.DiscardsOtherDiscounts = Request["DiscardsOtherDiscounts"] == "C";
                editingObject.IsHeaderDiscount = Request["IsHeaderDiscount"] == "C";
                editingObject.Priority = int.Parse(Request["Priority"]);
                editingObject.eDiscountType = (eDiscountType)Enum.Parse(typeof(eDiscountType), Request["eDiscountType_VI"]);
                AssignOwner(editingObject);

                if (HasDuplicate(editingObject))
                {
                    ModelState.AddModelError("Code", Resources.CodeAlreadyExists);
                    Session["Error"] += Resources.CodeAlreadyExists;
                    throw new Exception(Session["Error"].ToString());
                }

                editingObject.Save();
                XpoHelper.CommitChanges(editingObject.Session as UnitOfWork);
                
                return Json(new { });
            }
            catch(Exception exc)
            {
                if (Session["Error"] == null)
                {
                    Session["Error"] = exc.GetFullMessage();
                }
                return Json(new { error = Session["Error"] });
            }
        }
    }
}
