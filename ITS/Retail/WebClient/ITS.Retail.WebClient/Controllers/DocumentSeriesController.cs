using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.Model;
using ITS.Retail.Common;
using ITS.Retail.WebClient.Helpers;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.Retail.WebClient.Providers;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;
using ITS.Retail.ResourcesLib;

namespace ITS.Retail.WebClient.Controllers
{
    [StoreControllerEditable]
    public class DocumentSeriesController : BaseObjController<DocumentSeries>
    {
        [Security(ReturnsPartial = false), Display(ShowSettings = true)]
        public ActionResult Index()
        {
            ToolbarOptions.ExportToButton.Visible = false;
            ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            ToolbarOptions.EditButton.Visible = true;
            ToolbarOptions.EditButton.OnClick = "EditSelectedRowsFromGrid";
            ToolbarOptions.NewButton.OnClick = "AddNewFromGrid";
            ToolbarOptions.ViewButton.Visible = true;
            ToolbarOptions.ViewButton.OnClick = "ShowGenericView";
            CustomJSProperties.AddJSProperty("gridName", "grdDocumentSeries");
            ToolbarOptions.OptionsButton.Visible = false;
            return View("Index", GetList<DocumentSeries>(XpoHelper.GetNewUnitOfWork()).AsEnumerable());
        }

        protected override GenericViewRuleset GenerateGenericViewRuleset()
        {
            GenericViewRuleset ruleset = base.GenerateGenericViewRuleset();
            ruleset.PropertiesToIgnore.AddRange(new List<string>() { "FullName", "IsActive", "IsCanceledByOid", "ShouldResetMenu","DocumentSequence"});
            ruleset.DetailsToIgnore.Add("StoreDocumentSeriesTypes");
            ruleset.NumberOfColumns = 2;
            return ruleset;
        }

        public ActionResult StoresComboBoxPartial()
        {
            return PartialView();
        }

        public ActionResult POSComboBoxPartial(string storeOid)
        {
            ViewBag.POS = GetList<Model.POS>(XpoSession, new BinaryOperator("Store.Oid", storeOid));
            return PartialView();
        }

        public static object StoresRequestedByFilterCondition(DevExpress.Web.ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            XPCollection<Store> collection = GetList<Store>(XpoHelper.GetNewUnitOfWork(),
                                                            CriteriaOperator.Or(new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Name"), e.Filter),
                                                                                new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Address.City"), e.Filter),
                                                                                new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Address.Description"), e.Filter)
                                                                                //new BinaryOperator("Name", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like),
                                                                                //new BinaryOperator("Address.City", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like),
                                                                                //new BinaryOperator("Address.Description", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like))
                                                                                )
                                                           );
            collection.SkipReturnedObjects = e.BeginIndex;
            collection.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;
            return collection;
        }

        protected override void UpdateLookupObjects(DocumentSeries obj)
        {
            base.UpdateLookupObjects(obj);
            obj.Store = GetObjectByArgument<Store>(obj.Session, "StoresComboBox_VI");
            obj.POS = GetObjectByArgument<Model.POS>(obj.Session, "POSComboBox_VI");
            obj.IsCanceledBy = GetObjectByArgument<DocumentSeries>(obj.Session, "DocumentSeriesComboBox_VI");
        }

        public ActionResult SeriesComboBoxPartial()
        {
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            Guid storeGuid;
            if (Guid.TryParse(Request["Store"], out storeGuid))
            {
                Store store = uow.GetObjectByKey<Store>(storeGuid);
                IEnumerable<DocumentSeries> result = store.DocumentSeries.Where(series => series.IsCancelingSeries == true);
                ViewBag.DocumentSeries = result;
            }
            return PartialView();
        }

        public override ActionResult UpdatePartial([ModelBinder(typeof(RetailModelBinder))] DocumentSeries ct)
        {            
            if (!TableCanUpdate)
            {
                return null;
            }
            UpdateLookupObjects(ct);

            if (MvcApplication.ApplicationInstance == eApplicationInstance.STORE_CONTROLER)
            {
                if (ct.eModule == eModule.STORECONTROLLER)
                {
                    try
                    {
                        using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                        {
                            DocumentSequence documentSequence = uow.GetObjectByKey<DocumentSequence>(ct.DocumentSequence.Oid);
                            if ( documentSequence == null )
                            {
                                documentSequence = new DocumentSequence(uow);
                                DocumentSeries documentSeries = uow.GetObjectByKey<DocumentSeries>(ct.Oid);
                                documentSequence.DocumentSeries = documentSeries;
                            }
                            documentSequence.DocumentNumber = ct.DocumentSequence.DocumentNumber;
                            documentSequence.Save();
                            XpoHelper.CommitTransaction(uow);
                            ModelState.Keys.Where(key => ModelState.ContainsKey(key)).ToList().ForEach(key =>
                            {
                                ModelState[key].Errors.Clear();
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        ViewBag.CurrentItem = ct;
                        if (Session["Error"] == null || String.IsNullOrWhiteSpace(Session["Error"].ToString()))
                        {
                            Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                        }
                    }
                }
            }
            else
            {

                if (ct.eModule == eModule.POS && ct.POS == null)
                {
                    ModelState.AddModelError("POSComboBox", Resources.POSNotDefined);
                }

                if (typeof(Lookup2Fields).IsAssignableFrom(typeof(DocumentSeries)))
                {
                    AddModelErrors(ct as Lookup2Fields);
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        ViewBag.DocSeqNumCanChange = DocumentSeriesHelper.DocumentSeriesNumberCanBeEdited(ct, CurrentUser, MvcApplication.ApplicationInstance);
                        if (ct.ShouldResetMenu)
                        {
                            Session["Menu"] = null;
                        }
                        ct.Owner = ct.Session.GetObjectByKey<CompanyNew>(EffectiveOwner.Oid);
                        Save(ct);
                        if (MvcApplication.ApplicationInstance == eApplicationInstance.DUAL_MODE
                            || (MvcApplication.ApplicationInstance == eApplicationInstance.RETAIL && ct.eModule == eModule.HEADQUARTERS))
                        {
                            SaveT(ct.DocumentSequence);
                        }
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
            }
            //FillLookupComboBoxes();
            return PartialView("Grid", GetList<DocumentSeries>(XpoSession).AsEnumerable());
        }

        public override ActionResult Grid()
        {
            ViewBag.DocSeqNumCanChange = UserHelper.IsAdmin(CurrentUser);
            if (Request["DXCallbackArgument"].Contains("DELETESELECTED"))
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    List<Guid> seriesOids = RetailHelper.GetOidsToDeleteFromDxCallbackArgument(Request["DXCallbackArgument"]);
                    List<DocumentType> docTypes = new List<DocumentType>();
                    foreach (Guid docSeriesOid in seriesOids)
                    {
                        docTypes.AddRange(GetList<DocumentType>(uow, new ContainsOperator("StoreDocumentSeriesTypes", new BinaryOperator("DocumentSeries.Oid", docSeriesOid))));
                    }
                    if (docTypes.Count > 0)
                    {
                        foreach (DocumentType docType in docTypes.Distinct())
                        {
                            IEnumerable<Guid> docTypeSeriesOids = docType.StoreDocumentSeriesTypes.Select(storeDocType => storeDocType.DocumentSeries.Oid);
                            if (seriesOids.OrderBy(oid => oid).ContainsSequence(docTypeSeriesOids.OrderBy(oid => oid)))
                            {
                                Session["Menu"] = null;
                                break;
                            }
                        }
                    }

                    foreach(Guid seriesOid in seriesOids)
                    {
                        int documentsOfSeries = Convert.ToInt32(uow.Evaluate<DocumentHeader>(CriteriaOperator.Parse("Count()"),
                                                   new BinaryOperator("DocumentSeries.Oid", seriesOid)));

                        if (documentsOfSeries > 0)
                        {
                            Session["Error"] = Resources.CannotDeleteObject;
                            return PartialView("Grid", GetList<DocumentSeries>(XpoHelper.GetNewUnitOfWork()).AsEnumerable());
                        }
                    }                 
                }
            }

            CriteriaOperator joinDocSeriesCriteria = (new OperandProperty("^.Store.Oid") == new OperandProperty("Store.Oid") &
                                                     (new OperandProperty("Oid") == RetailHelper.GetOidToEditFromDxCallbackArgument(Request["DXCallbackArgument"])));
            JoinOperand joinDocSeriesOperand = new JoinOperand("DocumentSeries", joinDocSeriesCriteria);
            ViewBag.POS = GetList<Model.POS>(XpoSession, joinDocSeriesOperand);
            return base.Grid();
        }

        public ActionResult EditDocumentSeries()
        {
            return PartialView();
        }

        public ActionResult EditDocumentSeriesSequence()
        {
            return PartialView();
        }
    }
}
