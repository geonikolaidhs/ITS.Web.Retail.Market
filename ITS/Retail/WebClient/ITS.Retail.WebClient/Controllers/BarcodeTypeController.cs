using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.Model;
using ITS.Retail.Common;
using ITS.Retail.WebClient.Helpers;
using DevExpress.Data.Filtering;
using ITS.Retail.WebClient.Providers;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;
using DevExpress.Xpo;

namespace ITS.Retail.WebClient.Controllers
{
    public class BarcodeTypeController : BaseObjController<BarcodeType>
    {
        [Security(ReturnsPartial = false), Display(ShowSettings = true)]
        [OutputCache(Duration = 10, Location = System.Web.UI.OutputCacheLocation.Server)]
        public ActionResult Index()
        {
            ToolbarOptions.ExportToButton.Visible = false;
            ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            ToolbarOptions.EditButton.OnClick = "EditSelectedRowsFromGrid";
            ToolbarOptions.NewButton.OnClick = "AddNewFromGrid";
            ToolbarOptions.ViewButton.Visible = true;
            ToolbarOptions.ViewButton.OnClick = "ShowGenericView";
            CustomJSProperties.AddJSProperty("gridName", "grdBarcodeType");
            ToolbarOptions.OptionsButton.Visible = false;

            return View("Index", GetList<BarcodeType>(XpoSession).AsEnumerable<BarcodeType>());
        }

        protected override GenericViewRuleset GenerateGenericViewRuleset()
        {
            GenericViewRuleset ruleset = base.GenerateGenericViewRuleset();
            ruleset.PropertiesToIgnore.AddRange(new List<string>() { "IsDefault", "IsActive" });
            ruleset.ShowDetails = false;
            ruleset.NumberOfColumns = 2;
            return ruleset;
        }

        /*

        public override ActionResult Grid()
        {
            FillLookupComboBoxes();
            if (Request["DXCallbackArgument"] != null)
            {
                if (Request["DXCallbackArgument"].Contains("STARTEDIT"))
                {
                    Session["BarcodeType"] = XpoHelper.GetNewUnitOfWork().GetObjectByKey<BarcodeType>(RetailHelper.GetOidToEditFromDxCallbackArgument(Request["DXCallbackArgument"]));
                }
                else if(Request["DXCallbackArgument"].Contains("ADDNEWROW"))
                {
                    Session["BarcodeType"] = new BarcodeType(XpoHelper.GetNewUnitOfWork());
                }
            }
            return base.Grid();
        }


        [HttpPost]
        public ActionResult InlineEditingUpDatePartial([ModelBinder(typeof(RetailModelBinder))] BarcodeType ct)
        {
            BarcodeType barcodeType = (BarcodeType)Session["BarcodeType"];
            barcodeType.GetData(ct, new List<string>(){"Session","Stores"});
            barcodeType.Save();
            AssignOwner(barcodeType);
            barcodeType.Session.CommitTransaction();
            Session["BarcodeType"] = null;
            return base.Grid();
        }

        [HttpPost]
        public ActionResult InlineEditingAddNewStorePartial([ModelBinder(typeof(RetailModelBinder))] StoreBarcodeType ct)
        {
            BarcodeType barcodeType = (BarcodeType)Session["BarcodeType"];
            StoreBarcodeType storeBarcodeType = new StoreBarcodeType(barcodeType.Session);
            storeBarcodeType.GetData(ct, new List<String>() {"Session"});
            Guid storeOid;
            storeBarcodeType.Store = Guid.TryParse(Request["Store_VI"], out storeOid) ? storeBarcodeType.Session.GetObjectByKey<Store>(storeOid) : null;
            storeBarcodeType.BarcodeType = barcodeType;
            barcodeType.Stores.Add(storeBarcodeType);
            storeBarcodeType.Owner = barcodeType.Session.GetLoadedObjectByKey<CompanyNew>(StoreControllerAppiSettings.Owner.Oid);
            return PartialView("StoresPartialGrid", barcodeType.Stores);
        }

        [HttpPost]
        public ActionResult InlineEditingDeleteStorePartial([ModelBinder(typeof(RetailModelBinder))] StoreBarcodeType ct)
        {
            BarcodeType barcodeType = (BarcodeType)Session["BarcodeType"];
            StoreBarcodeType storeBarcodeType = barcodeType.Stores.Where(store => store.Oid == ct.Oid).FirstOrDefault();
            storeBarcodeType.Delete();
            return PartialView("StoresPartialGrid", barcodeType.Stores);
        }


        [HttpPost]
        public ActionResult StoresPartialGrid()
        {
            BarcodeType barcodeType = (BarcodeType)Session["BarcodeType"];
            ViewBag.StoresComboBox = GetList<Store>(barcodeType.Session,new NotOperator(
                                                                        new InOperator("Oid", barcodeType.Stores.Select(storeBarType => storeBarType.Store.Oid))),
                                                                        sortingField: "Code");
            return PartialView("StoresPartialGrid", barcodeType.Stores);
        }*/

        protected override void FillLookupComboBoxes()
        {
            base.FillLookupComboBoxes();
            ViewBag.EntityTypes = typeof(BarcodeType).Assembly.GetTypes()
                                                    .Where(typ => typeof(BasicObj).IsAssignableFrom(typ)
                                                           && typ.GetCustomAttributes(typeof(NonPersistentAttribute),false).Count() == 0
                                                        );
        }
    }
}
