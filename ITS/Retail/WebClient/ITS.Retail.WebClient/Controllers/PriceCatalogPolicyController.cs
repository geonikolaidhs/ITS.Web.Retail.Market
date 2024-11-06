using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.WebClient.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;

namespace ITS.Retail.WebClient.Controllers
{
    public class PriceCatalogPolicyController : BaseObjController<PriceCatalogPolicy>
    {
        protected override Dictionary<PropertyInfo, string> PropertyMapping
        {
            get
            {
                return new Dictionary<PropertyInfo, string>()
                {
                    { typeof(PriceCatalogPolicyDetail).GetProperty("PriceCatalog"), "PriceCatalogCb_VI" }
                };
            }
        }

        [Security(ReturnsPartial = false), Display(ShowSettings = true)]
        public ActionResult Index()
        {
            ToolbarOptions.ExportToButton.Visible = false;
            ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            ToolbarOptions.NewButton.OnClick = "AddNewCustomV2";
            ToolbarOptions.EditButton.OnClick = "EditSelectedRowsCustomV2";
            ToolbarOptions.ViewButton.Visible = true;
            ToolbarOptions.ViewButton.OnClick = "ShowGenericView";
            ToolbarOptions.OptionsButton.Visible = false;

            CustomJSProperties.AddJSProperty("editAction", "Edit");
            CustomJSProperties.AddJSProperty("gridName","grdPriceCatalogPolicy");

            return View(GetList<PriceCatalogPolicy>(XpoSession));
        }
        public ActionResult Edit(string Oid)
        {
            PriceCatalogPolicy priceCatalogPolicy = null;
            Guid priceCatalogPolicyGuid;

            if (Guid.TryParse(Oid, out priceCatalogPolicyGuid))
            {
                priceCatalogPolicy = XpoHelper.GetNewUnitOfWork().GetObjectByKey<PriceCatalogPolicy>(priceCatalogPolicyGuid) ??
                                                        new PriceCatalogPolicy(XpoHelper.GetNewUnitOfWork());
            }

            Session["PriceCatalogPolicy"] = priceCatalogPolicy;
            return PartialView(priceCatalogPolicy);
        }

        public JsonResult Save()
        {
            PriceCatalogPolicy priceCatalogPolicy = (PriceCatalogPolicy)Session["PriceCatalogPolicy"];
            try
            {
                TryUpdateModel<PriceCatalogPolicy>(priceCatalogPolicy);
                AddModelErrors(priceCatalogPolicy);
                
                if (ModelState.IsValid)
                {
                    AssignOwner(priceCatalogPolicy);
                    XpoHelper.CommitChanges((UnitOfWork)priceCatalogPolicy.Session);
                    return Json(new { });
                }
                else
                {
                    ViewBag.CurrentItem = priceCatalogPolicy;
                    Session["Error"] = ModelState.Values.First().Errors.First().ErrorMessage;
                    return Json(new { error = Session["Error"] });
                }
            }
            catch (Exception ex)
            {
                Session["Error"] = Resources.AnErrorOccurred + ":" + ex.Message;
                return Json(new { error = Session["Error"] });
            }
        }

        private void FillPriceCatalogPolicyDetailComboboxes()
        {
            ViewBag.PriceCatalogs = GetList<PriceCatalog>(XpoSession);
        }

        public ActionResult PriceCatalogPolicyDetailsGrid()
        {
            PriceCatalogPolicy priceCatalogPolicy = (PriceCatalogPolicy)Session["PriceCatalogPolicy"];
            FillPriceCatalogPolicyDetailComboboxes();
            return PartialView("PriceCatalogPolicyDetailsGrid", priceCatalogPolicy.PriceCatalogPolicyDetails);
        }

        public ActionResult PriceCatalogPolicyDetailUpdate([ModelBinder(typeof(RetailModelBinder))] PriceCatalogPolicyDetail model)
        {
            PriceCatalogPolicy priceCatalogPolicy = (PriceCatalogPolicy)Session["PriceCatalogPolicy"];
            TryUpdateModel<PriceCatalogPolicyDetail>(model);
            UpdateLookupObjects(model);

            if (priceCatalogPolicy != null)
            {
                string modelKey;
                string modelError = PriceCatalogHelper.CheckedPriceCatalogPolicyDetails(priceCatalogPolicy, model, out modelKey);
                if (String.IsNullOrWhiteSpace(modelError))
                {
                    PriceCatalogPolicyDetail priceCatalogPolicyDetail = priceCatalogPolicy.PriceCatalogPolicyDetails.FirstOrDefault(detail => detail.Oid == model.Oid) ?? new PriceCatalogPolicyDetail(priceCatalogPolicy.Session);
                    priceCatalogPolicyDetail.GetData(model, new List<string>() { "Session" });
                    priceCatalogPolicyDetail.PriceCatalogPolicy = priceCatalogPolicy;
                }
                else
                {
                    ViewBag.CurrentItem = model;
                    ModelState.AddModelError(modelKey, modelError);
                    FillPriceCatalogPolicyDetailComboboxes();
                }
            }
            return PartialView("PriceCatalogPolicyDetailsGrid", priceCatalogPolicy.PriceCatalogPolicyDetails);
        }

        public ActionResult PriceCatalogPolicyDetailDelete([ModelBinder(typeof(RetailModelBinder))] PriceCatalogPolicyDetail model)
        {
            PriceCatalogPolicy priceCatalogPolicy = (PriceCatalogPolicy)Session["PriceCatalogPolicy"];
            if (priceCatalogPolicy != null)
            {
                PriceCatalogPolicyDetail priceCatalogPolicyDetail = priceCatalogPolicy.PriceCatalogPolicyDetails.FirstOrDefault(detail => detail.Oid == model.Oid);
                if (priceCatalogPolicyDetail != null)
                {
                    priceCatalogPolicyDetail.Delete();
                }          
            }
            return PartialView("PriceCatalogPolicyDetailsGrid", priceCatalogPolicy.PriceCatalogPolicyDetails);
        }

        public ActionResult CancelEdit()
        {
            Session["PriceCatalogPolicy"] = null;
            return null;
        }

        protected override GenericViewRuleset GenerateGenericViewRuleset()
        {
            GenericViewRuleset ruleset = base.GenerateGenericViewRuleset();
            ruleset.DetailsToIgnore.Add("DocumentHeaders");
            ruleset.DetailsToIgnore.Add("PriceCatalogPolicyPromotions");
            return ruleset;
        }
    }
}
