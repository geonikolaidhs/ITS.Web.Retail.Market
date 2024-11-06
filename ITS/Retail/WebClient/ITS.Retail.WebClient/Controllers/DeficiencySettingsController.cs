using DevExpress.Data.Filtering;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Providers;
using System;
using System.Linq;
using System.Web.Mvc;

namespace ITS.Retail.WebClient.Controllers
{
    public class DeficiencySettingsController : BaseObjController<DeficiencySettings>
    {
        public ActionResult Index()
        {
            this.ToolbarOptions.CustomButton.Visible =
            this.ToolbarOptions.ExportButton.Visible =
            this.ToolbarOptions.ExportToButton.Visible = false;
            this.ToolbarOptions.ViewButton.Visible = true;
            this.ToolbarOptions.ViewButton.OnClick = "ShowGenericView";
            this.ToolbarOptions.NewButton.OnClick = "AddNewCustom";
            this.ToolbarOptions.EditButton.OnClick = "EditSelectedRowsCustom";
            this.ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            

            this.CustomJSProperties.AddJSProperty("editAction", "Edit");
            this.CustomJSProperties.AddJSProperty("editIDParameter", "DeficiencyID");
            this.CustomJSProperties.AddJSProperty("gridName", "grdDeficiencySettings");
            
            ViewBag.ShowSettings = true;

            return View();
        }

        public ActionResult Edit(string DeficiencyID)
        {
            Session["Error"] = Session["DeficiencySettings"] = null;
            this.ToolbarOptions.ForceVisible = false;
            Guid deficiencyGuid;

            if (!Guid.TryParse(DeficiencyID, out deficiencyGuid))
            {
                Session["Error"] = Resources.AnErrorOccurred;
                return View("../Home/CloseWindow");
            }

            DeficiencySettings deficiencySettings = deficiencyGuid == Guid.Empty ? new DeficiencySettings(XpoHelper.GetNewUnitOfWork()) : XpoHelper.GetNewUnitOfWork().GetObjectByKey<DeficiencySettings>(deficiencyGuid);

            if (deficiencySettings == null)
            {
                Session["Error"] = Resources.AnErrorOccurred;
                return View("../Home/CloseWindow");
            }

            Session["DeficiencySettings"] = deficiencySettings;
            FillLookupComboBoxes();
            return View(deficiencySettings);
        }

        public ActionResult DeficiencySettingsDetails()
        {
            DeficiencySettings deficiencySettings = (DeficiencySettings)Session["DeficiencySettings"];
            return PartialView(deficiencySettings.DeficiencySettingsDetails);
        }

        public ActionResult AddDeficiencySettingsDetail([ModelBinder(typeof(RetailModelBinder))] DeficiencySettingsDetail deficiencySettingsDetail)
        {
            DeficiencySettings deficiencySettings = (DeficiencySettings)Session["DeficiencySettings"];
            if (ModelState.IsValid)
            {
                DeficiencySettingsDetail newValue = new DeficiencySettingsDetail(deficiencySettings.Session);
                newValue.GetData(deficiencySettingsDetail);
                deficiencySettings.DeficiencySettingsDetails.Add(newValue);
            }
            return PartialView("DeficiencySettingsDetails",deficiencySettings.DeficiencySettingsDetails);
        }


        public ActionResult UpdateDeficiencySettingsDetail([ModelBinder(typeof(RetailModelBinder))] DeficiencySettingsDetail deficiencySettingsDetail)
        {
            DeficiencySettings deficiencySettings = (DeficiencySettings)Session["DeficiencySettings"];
            if (ModelState.IsValid)
            {
                DeficiencySettingsDetail defDet = deficiencySettings.DeficiencySettingsDetails.First(deficDetail => deficDetail.Oid == deficiencySettingsDetail.Oid);
                defDet.GetData(deficiencySettingsDetail);
                defDet.DeficiencySettings = deficiencySettings;
            }
            return PartialView("DeficiencySettingsDetails", deficiencySettings.DeficiencySettingsDetails);
        }

        public ActionResult DeleteDeficiencySettingsDetail([ModelBinder(typeof(RetailModelBinder))] DeficiencySettingsDetail deficiencySettingsDetail)
        {
            DeficiencySettings deficiencySettings = (DeficiencySettings)Session["DeficiencySettings"];
            if (ModelState.IsValid)
            {
                DeficiencySettingsDetail defDet = deficiencySettings.DeficiencySettingsDetails.First(deficDetail => deficDetail.Oid == deficiencySettingsDetail.Oid);
                deficiencySettings.DeficiencySettingsDetails.Remove(defDet);
                defDet.Delete();
            }
            return PartialView("DeficiencySettingsDetails", deficiencySettings.DeficiencySettingsDetails);
        }

        protected override void FillLookupComboBoxes()
        {
            DeficiencySettings deficiencySettings = (DeficiencySettings)Session["DeficiencySettings"];
            CriteriaOperator crop = deficiencySettings==null || deficiencySettings.DeficiencyDocumentType==null ? null : new BinaryOperator("Oid",deficiencySettings.DeficiencyDocumentType.Oid,BinaryOperatorType.NotEqual);
            ViewBag.DocumentTypesComboBox = GetList<DocumentType>(XpoHelper.GetNewUnitOfWork(),crop);
            base.FillLookupComboBoxes();
        }
    }
}
