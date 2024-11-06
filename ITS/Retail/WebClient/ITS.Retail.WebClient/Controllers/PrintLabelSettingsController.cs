using System;
using System.Web.Mvc;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Providers;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;


namespace ITS.Retail.WebClient.Controllers
{
    [StoreControllerEditable]
    public class PrintLabelSettingsController : BaseObjController<PrintLabelSettings>
    {

        private UnitOfWork uow;

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
            ruleset.PropertiesToIgnore.Add("CreatedOn");
            ruleset.PropertiesToIgnore.Add("UpdatedOnOn");
            ruleset.PropertiesToIgnore.Add("Port");
            ruleset.PropertiesToIgnore.Add("IsActive");
            ruleset.NumberOfColumns = 1;

            return ruleset;
        }

        [Display(ShowSettings = true)]
        public ActionResult Index()
        {
            ToolbarOptions.ForceVisible = false;          

            CustomJSProperties.AddJSProperty("editAction", "Edit");
            CustomJSProperties.AddJSProperty("editIDParameter", "StoreID");
            CustomJSProperties.AddJSProperty("gridName", "grdStore");

            return View(GetList<PrintLabelSettings>(XpoSession,new BinaryOperator("Store.Oid",StoreControllerAppiSettings.CurrentStore.Oid)));
        }

        protected override void FillLookupComboBoxes()
        {
            base.FillLookupComboBoxes();
            ViewBag.Labels = GetList<Label>(XpoSession);
        }

        public override ActionResult Grid()
        {
            GenerateUnitOfWork();
            FillLookupComboBoxes();
            Store store = StoreControllerAppiSettings.CurrentStore;
            XPCollection<PrintLabelSettings> printlabelsettings = GetList<PrintLabelSettings>(uow, new BinaryOperator("Store.Oid",store.Oid));
            return PartialView("Grid", printlabelsettings);
        }

        [HttpPost]
        public ActionResult PrintLabelSetInlineEditingAddNewPartial([ModelBinder(typeof(RetailModelBinder))] PrintLabelSettings ct)
        {
            Store store = ct.Session.GetObjectByKey<Store>(StoreControllerAppiSettings.CurrentStore.Oid);
            int count = (int)ct.Session.Evaluate(typeof(PrintLabelSettings), CriteriaOperator.Parse("Count"), CriteriaOperator.And(new BinaryOperator("Code", ct.Code), new BinaryOperator("Store.Oid", store.Oid)));

            if (count > 1)
            {
                ModelState.AddModelError("Print Settings", Resources.ItemAlreadyExists + ": (" + ct.Code + "-" + ct.Description + ")");
            }
            ViewData["EditMode"] = true;
            ct.Store = store;
            if (HasIsDefaultDuplicates(ct))
            {
                ModelState.AddModelError("IsDefault", Resources.DefaultAllreadyExists);
                Session["Error"] += Resources.DefaultAllreadyExists;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Guid labelOid;
                    if (Guid.TryParse(Request["Label!Key_VI"], out labelOid))
                    {
                        ct.Label = ct.Session.GetObjectByKey<Label>(labelOid);
                    }
                    AssignOwner(ct);
                    Save(ct);
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }
            else
            {
                Session["Error"] = Resources.AnErrorOccurred;
            }
            FillLookupComboBoxes();
            XPCollection<PrintLabelSettings> printlabelsettings = GetList<PrintLabelSettings>(ct.Session, new BinaryOperator("Store.Oid", store.Oid));
            return PartialView("Grid", printlabelsettings);
        }

        [HttpPost]
        public ActionResult PrintLabelSetInlineEditingUpdatePartial([ModelBinder(typeof(RetailModelBinder))] PrintLabelSettings ct)
        {
            ViewData["EditMode"] = true;
            if (HasIsDefaultDuplicates(ct))
            {
                ModelState.AddModelError("IsDefault", Resources.DefaultAllreadyExists);
                Session["Error"] += Resources.DefaultAllreadyExists;
            }
            if (ModelState.IsValid)
            {
                try
                {
                    Guid labelOid;
                    if (Guid.TryParse(Request["Label!Key_VI"], out labelOid))
                    {
                        ct.Label = ct.Session.GetObjectByKey<Label>(labelOid);
                    }
                    AssignOwner(ct);
                    Save(ct);
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }
            FillLookupComboBoxes();
            XPCollection<PrintLabelSettings> printlabelsettings = GetList<PrintLabelSettings>(ct.Session, new BinaryOperator("Store.Oid", StoreControllerAppiSettings.CurrentStore.Oid));
            return PartialView("Grid", printlabelsettings);
        }

        [HttpPost]
        public ActionResult PrintLabelSetInlineEditingDeletePartial([ModelBinder(typeof(RetailModelBinder))] PrintLabelSettings ct)
        {
            ViewData["EditMode"] = true;
            Store store = StoreControllerAppiSettings.CurrentStore;
            if (ModelState.IsValid)
            {
                try
                {
                    DeleteT(ct);
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message;
                }
            }
            FillLookupComboBoxes();
            XPCollection<PrintLabelSettings> printlabelsettings = GetList<PrintLabelSettings>(ct.Session, new BinaryOperator("Store.Oid", store.Oid));
            return PartialView("Grid", printlabelsettings);
        }
    }
}
