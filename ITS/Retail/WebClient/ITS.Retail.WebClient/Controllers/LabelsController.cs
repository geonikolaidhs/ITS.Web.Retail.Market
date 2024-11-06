//#if _RETAIL_STORECONTROLLER || _RETAIL_DUAL

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.Model;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.WebClient.Helpers;
using DevExpress.Data.Filtering;
using System.Text;
using ITS.Retail.ResourcesLib;
using System.IO;
using ITS.Retail.Mobile.AuxilliaryClasses;
using ITS.Retail.WebClient.Providers;
using DevExpress.Xpo.DB;
using ITS.Retail.Common.ViewModel;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;
using ITS.Retail.Model.NonPersistant;
using ITS.Retail.PrintServer.Common;

namespace ITS.Retail.WebClient.Controllers
{
    public class LabelsController : BaseObjController<PriceCatalogDetail>
    {
        [Security(ReturnsPartial = false), Display(ShowSettings = false)]
        public ActionResult Index()
        {
            GenerateUnitOfWork();
            ToolbarOptions.ForceVisible = false;
            ToolbarOptions.ViewButton.Visible = false;
            ToolbarOptions.ExportToButton.Visible = false;
            ToolbarOptions.CustomButton.Visible = false;
            ToolbarOptions.OptionsButton.Visible = false;
            ToolbarOptions.ShowHideMenu.Visible = false;
            ToolbarOptions.DeleteButton.Visible = false;
            ToolbarOptions.NewButton.Visible = false;
            ToolbarOptions.EditButton.Visible = false;
            this.CustomJSProperties.AddJSProperty("gridName", "grdLabels");
            List<PrintLabelSettings> printsettings = GetList<PrintLabelSettings>(uow, new BinaryOperator("Store.Oid", StoreControllerAppiSettings.CurrentStore.Oid)).ToList();
            ViewBag.Labels = printsettings.OrderByDescending(criteria => criteria.IsDefault);
            return View((object)null);
        }

        protected override GenericViewRuleset GenerateGenericViewRuleset()
        {
            GenericViewRuleset ruleset = base.GenerateGenericViewRuleset();
            ruleset.PropertiesToIgnore.Add("IsActive");
            ruleset.NumberOfColumns = 2;
            return ruleset;
        }

        public override ActionResult Grid()
        {
            if (Request["DXCallbackArgument"].Contains("CLEAR"))
            {
                return PartialView(new List<PriceCatalogDetail>());
            }
            GenerateUnitOfWork();
            if (StoreControllerAppiSettings.CurrentStore.DefaultPriceCatalogPolicy == null)
            {
                Session["Error"] = String.Format(Resources.DefaultPriceCatalogPolicyIsNotDefinedForStore, StoreControllerAppiSettings.CurrentStore.Description);
                return PartialView();
            }
            CriteriaOperator LabelsWithValueChangeFilter = new BinaryOperator("Oid", Guid.Empty);
            if (Request["DXCallbackArgument"].Contains("SEARCH"))
            {
                LabelItemChangeCriteria filter = new LabelItemChangeCriteria();
                TryUpdateModel<LabelItemChangeCriteria>(filter);
                LabelsWithValueChangeFilter = filter.BuildCriteria();
                Session["UserFilter"] = LabelsWithValueChangeFilter;
            }
            else if(Session["UserFilter"] is CriteriaOperator)
            {
                LabelsWithValueChangeFilter = (CriteriaOperator)Session["UserFilter"];
            }

            IEnumerable<PriceCatalogDetail> priceCatalogDetails = PriceCatalogHelper.GetAllSortedPriceCatalogDetails(StoreControllerAppiSettings.CurrentStore, LabelsWithValueChangeFilter);    
            IEnumerable<Item> items = priceCatalogDetails.Select(x => x.Item).Distinct();
            if (items.Count() > 0)
            {
                Store store = XpoSession.GetObjectByKey<Store>(StoreControllerAppiSettings.CurrentStoreOid);
                EffectivePriceCatalogPolicy effectivePriceCatalogPolicy = new EffectivePriceCatalogPolicy(store);
                IEnumerable<PriceCatalogDetail> pricesFromPolicy = items.Select(item => PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(store.Session as UnitOfWork,
                                                                                                                                           effectivePriceCatalogPolicy,
                                                                                                                                           item))
                                                                         .Where( priceCatalogPolicyResult => priceCatalogPolicyResult != null 
                                                                                                          && priceCatalogPolicyResult.PriceCatalogDetail != null
                                                                                                          && priceCatalogPolicyResult.PriceCatalogDetail.Value > 0
                                                                               )
                                                                         .Select( priceCatalogPolicyResult => priceCatalogPolicyResult.PriceCatalogDetail );
                //priceCatalogDetails = pricesFromPolicy.Where(price => price != null && price.Value > 0);
            }

            if (Request.Params.AllKeys.Contains("DXCallbackArgument"))
            {
                if (Request["DXCallbackArgument"].Contains("SELECTROWS") && Request["DXCallbackArgument"].Contains("|all"))
                {
                    //Select All
                    ViewBag.ValuesToSelect = priceCatalogDetails.Select(x => x.Oid).ToList();
                }
                else if (Request["DXCallbackArgument"].Contains("SELECTROWS") && Request["DXCallbackArgument"].Contains("|unall"))
                {
                    //Deselect All
                    ViewBag.ValuesToDeselect = priceCatalogDetails.Select(x => x.Oid).ToList();
                }
            }
            return PartialView(priceCatalogDetails);
        }



        UnitOfWork uow;

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


        [Security(OverrideSecurity = true, ReturnsPartial = false)]
        public FileContentResult ExportLabels()
        {
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                string allOids = Request["PriceCatalogDetailGuids"];
                string type = Request["Type"];
                string mode = Request["Mode"];

                if (!String.IsNullOrWhiteSpace(allOids) && !String.IsNullOrWhiteSpace(type))
                {
                    string[] strOids = allOids.Split(',');
                    List<Guid> oids = new List<Guid>();


                    if (mode == "pcd")
                    {
                        foreach (string strOid in strOids)
                        {
                            Guid oid;
                            if (Guid.TryParse(strOid, out oid))
                            {
                                oids.Add(oid);
                            }
                        }
                    }
                    else if (mode == "document")
                    {
                        Store store = XpoSession.GetObjectByKey<Store>(StoreControllerAppiSettings.CurrentStoreOid);
                        EffectivePriceCatalogPolicy effectivePriceCatalogPolicy = new EffectivePriceCatalogPolicy(store);
                        foreach (string strOid in strOids)
                        {
                            Guid oid;
                            if (Guid.TryParse(strOid, out oid))
                            {
                                DocumentDetail detail = uow.GetObjectByKey<DocumentDetail>(oid);
                                if (StoreControllerAppiSettings.CurrentStore.DefaultPriceCatalogPolicy != null)
                                {
                                    PriceCatalogPolicyPriceResult priceCatalogPolicyPriceResult = PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(store.Session as UnitOfWork,
                                                                                                                               effectivePriceCatalogPolicy,
                                                                                                                               detail.Item);
                                    PriceCatalogDetail priceCatalogDetail = priceCatalogPolicyPriceResult == null ? null : priceCatalogPolicyPriceResult.PriceCatalogDetail;

                                    if (priceCatalogDetail != null)
                                    {
                                        oids.Add(priceCatalogDetail.Oid);
                                    }
                                }
                            }
                        }
                    }

                    XPCollection<PriceCatalogDetail> priceCatalogDetails = GetList<PriceCatalogDetail>(uow, new InOperator("Oid", oids));


                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (StreamWriter sr = new StreamWriter(ms))
                        {
                            List<string> lines = GridExportHelper.GetLabelExportLines(priceCatalogDetails, type, EffectiveOwner);
                            if (lines == null)
                            {
                                Session["Error"] = Resources.PleaseDefineVatLevelForAddress + " " + StoreControllerAppiSettings.CurrentStore.Address;
                                return null;
                            }
                            foreach (string line in lines)
                            {
                                sr.WriteLine(line);
                            }
                            sr.Flush();
                            return File(ms.ToArray(), "text/plain", "labels_export.txt");
                        }
                    }
                }
                else
                {
                    return null;
                }
            }

        }

        public JsonResult jsonPrint()
        {
            try
            {
                PrintLabelSettings printsettings = null;
                string printsettingsOid = Request["label"];

                if (StoreControllerAppiSettings.CurrentStore.DefaultPriceCatalogPolicy == null)
                {
                    return Json(new { error = String.Format(Resources.DefaultPriceCatalogPolicyIsNotDefinedForStore, StoreControllerAppiSettings.CurrentStore.Description) });
                }

                if (!String.IsNullOrEmpty(printsettingsOid))
                {
                    Guid printsettingsGuid;
                    if (Guid.TryParse(printsettingsOid, out printsettingsGuid))
                    {
                        printsettings = XpoHelper.GetNewUnitOfWork().GetObjectByKey<PrintLabelSettings>(printsettingsGuid);
                        if (printsettings == null || printsettings.IsActive == false )
                        {
                            return Json(new { error = Resources.PrintLabelSettingsNotDefined });
                        }
                    }
                    else
                    {
                        return Json(new { error = Resources.PrintLabelSettingsNotDefined });
                    }
                }
                else
                {
                    return Json(new { error = Resources.PrintLabelSettingsNotDefined });
                }

                string type = Request["Type"] ?? "item"; //This variable shows us if label printing is called from item grid or label settings
                string[] selectedItems = Request["SelectedItems[]"] == null ? null : Request["SelectedItems[]"].Split(',');
                string selectedItemsWithCommas = Request["SelectedItems[]"];
                string mode = Request["mode"];
                string[] dets = new string[] { };

                int parcedCopies;
                parcedCopies = printsettings.Copies;

                string output = null;
                string ejf = null;
                Label lbl = null;
                lbl = printsettings.Label;
                if (lbl != null)
                {
                    ejf = Encoding.UTF8.GetString(lbl.LabelFile);
                }
                int requestEncoding = printsettings.PrinterEncoding.HasValue && printsettings.PrinterEncoding.Value != 0 ? printsettings.PrinterEncoding.Value : printsettings.Label.PrinterEncoding;
                List<PriceCatalogPolicyDetail> policyDetails = StoreControllerAppiSettings.CurrentStore.DefaultPriceCatalogPolicy.PriceCatalogPolicyDetails.ToList();
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    Store store = uow.GetObjectByKey<Store>(StoreControllerAppiSettings.CurrentStoreOid);
                    EffectivePriceCatalogPolicy effectivePriceCatalogPolicy = new EffectivePriceCatalogPolicy(store);
                    if (mode == "document")
                    {
                        try
                        {
                            List<Guid> selectedDocumentDetailOids = selectedItems.Select(x => Guid.Parse(x)).ToList();
                            XPCollection<DocumentDetail> lines = GetList<DocumentDetail>(uow, new InOperator("Oid", selectedDocumentDetailOids));
                            lines.Sorting.Add(new SortProperty("DocumentHeader.UpdatedOnTicks", SortingDirection.Ascending));
                            lines.Sorting.Add(new SortProperty("LineNumber", SortingDirection.Ascending));
                            
                            selectedItems = lines.Select(documentDetail => PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(uow,
                                                                                                                              effectivePriceCatalogPolicy,
                                                                                                                              documentDetail.Item))
                                                                                              .Where(priceCatalogPolicyPriceResult => priceCatalogPolicyPriceResult != null
                                                                                                  && priceCatalogPolicyPriceResult.PriceCatalogDetail != null)
                                                                                              .Select(priceCatalogPolicyPriceResult =>priceCatalogPolicyPriceResult.PriceCatalogDetail.Oid.ToString()).ToArray();
                            dets = lines.Select(det => det.Oid.ToString()).ToArray();
                        }
                        catch (Exception ex)
                        {
                            string exceptionMessage = ex.GetFullMessage();
                            return Json(new { error = Resources.PriceCatalogDetailNotExists });
                        }
                    }


                    if (selectedItems != null && ejf != null)
                    {
                        string message;
                        byte[] outBytes;
                        IEnumerable<PriceCatalogDetail> pcds;
                        //CALL NEW FUNCTION
                        if (type == "item")
                        {
                            XPCollection<Item> selectedItemCollection = new XPCollection<Item>(uow, new InOperator("Oid", selectedItems));
                            pcds = selectedItemCollection.Select(priceCatalogPolicyResult=> PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(uow,
                                                                                                                                              effectivePriceCatalogPolicy, 
                                                                                                                                              priceCatalogPolicyResult))
                                                         .Where(priceCatalogPolicyResult => priceCatalogPolicyResult != null && priceCatalogPolicyResult.PriceCatalogDetail != null)
                                                         .Select(priceCatalogPolicyResult => priceCatalogPolicyResult.PriceCatalogDetail);
                        }
                        else
                        {
                            Guid[] selectedPriceCatalogDetailsOids = selectedItems.Select(x => Guid.Parse(x)).ToArray();
                            pcds = uow.Query<PriceCatalogDetail>().Where(x => selectedPriceCatalogDetailsOids.Contains(x.Oid));                            
                        }
                        output = ToolsHelper.GetLabelsStringToPrint(StoreControllerAppiSettings.CurrentStore, type, pcds /*selectedItems.Select(x => Guid.Parse(x)).ToArray()*/, dets, parcedCopies, output, ejf, lbl, uow, /*pricecatalog,*/ requestEncoding, new List<LeafletDetail>(), out message, out outBytes);

                        if (lbl.PrintServiceSettings != null && lbl.PrintServiceSettings.RemotePrinterService != null) // print remotely
                        {
                            PrintServerPrintLabelResponse response = PrinterServiceHelper.PrintLabel(lbl.PrintServiceSettings.RemotePrinterService,
                                                                                                    output,
                                                                                                    requestEncoding,
                                                                                                    lbl.PrintServiceSettings.PrinterNickName);
                            if ( response == null )
                            {
                                return Json(new { error = Resources.CouldNotEstablishConnection + " Remote Print Service :" + lbl.PrintServiceSettings.RemotePrinterService.Name });
                            }
                            switch(response.Result)
                            {
                                case ePrintServerResponseType.FAILURE:
                                    return Json(new { error = response.Explanation });
                                case ePrintServerResponseType.SUCCESS:
                                    return Json(new { success = Resources.SuccefullyCompleted });
                                default:
                                    throw new NotImplementedException();
                            }
                        }
                        else // print localy
                        {
                            if (message.Equals(String.Empty))
                            {
                                return Json(new { output = outBytes.Select(x => (int)x).ToArray(), printingType = printsettings.PrintingType, portName = printsettings.PortName });
                            }
                            else
                            {
                                return Json(new { output = outBytes.Select(x => (int)x).ToArray(), printingType = printsettings.PrintingType, portName = printsettings.PortName, error = message });
                            }
                        }
                    }
                    else
                    {
                        return Json(new { error = Resources.PleaseSelectAtLeastOneRecordToPrint });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }

        }



        public JsonResult jsonMarkPrintedPriceCatalogDetails()
        {
            GenerateUnitOfWork();
            string[] selectedOids = Request["SelectedOids[]"] == null ? null : Request["SelectedOids[]"].Split(',');
            string type = Request["type"] ?? "item";
            string mode = Request["mode"] ?? "";
            List<Guid> selectedDetailOids = selectedOids.Select(x => Guid.Parse(x)).ToList();

            Store store = uow.GetObjectByKey<Store>(StoreControllerAppiSettings.CurrentStoreOid);
            EffectivePriceCatalogPolicy effectivePriceCatalogPolicy = new EffectivePriceCatalogPolicy(store);

            foreach (Guid oid in selectedDetailOids)
            {
                PriceCatalogDetail pcd;
                if (mode == "document")
                {
                    DocumentDetail detail = uow.GetObjectByKey<DocumentDetail>(oid);
                    PriceCatalogPolicyPriceResult priceCatalogPolicyPriceResult = PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(uow, effectivePriceCatalogPolicy , detail.Item);
                    pcd = priceCatalogPolicyPriceResult == null ? null : priceCatalogPolicyPriceResult.PriceCatalogDetail;
                }
                else
                {
                    if (type == "item")
                    {
                        Item item = (uow.GetObjectByKey<Item>(oid));
                        PriceCatalogPolicyPriceResult priceCatalogPolicyPriceResult = PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(uow, effectivePriceCatalogPolicy, item);
                        pcd = priceCatalogPolicyPriceResult == null ? null : priceCatalogPolicyPriceResult.PriceCatalogDetail;
                    }
                    else
                    {
                        pcd = uow.GetObjectByKey<PriceCatalogDetail>(oid);
                    }
                }
                if (pcd != null)
                {
                    pcd.LabelPrintedOn = DateTime.Now.Ticks;
                    pcd.Save();
                }
            }
            uow.CommitTransaction();
            return Json(new { });
        }

        public ActionResult LabelsWithChangesFilters()
        {
            return PartialView();
        }

        public ActionResult LabelsFromDocumentTagsFilters()
        {
            return PartialView();
        }

        public ActionResult LabelsDocumentGrid([ModelBinder(typeof(RetailModelBinder))] LabelSearchCriteria criteria)
        {
            if (Request["DXCallbackArgument"].Contains("CLEAR"))
            {
                return PartialView(new List<DocumentHeader>());
            }
            GenerateUnitOfWork();
            CriteriaOperator filter = Session["grdLabelsDocumentGridFilter"] as CriteriaOperator;
            if (ReferenceEquals(filter,null))
            {
                filter = new BinaryOperator("Oid", Guid.Empty);
            }
            if (Request["DXCallbackArgument"].Contains("SEARCH"))
            {
                TryUpdateModel<LabelSearchCriteria>(criteria);
                Store store = uow.GetObjectByKey<Store>(StoreControllerAppiSettings.CurrentStoreOid);
                if (store == null)
                {
                    Session["Error"] = Resources.PleaseSelectAStore;
                    return PartialView("LabelsDocumentGrid", null);
                }
                if (StoreControllerAppiSettings.CurrentStore.DefaultPriceCatalogPolicy == null)
                {
                    Session["Error"] = String.Format(Resources.DefaultPriceCatalogPolicyIsNotDefinedForStore, StoreControllerAppiSettings.CurrentStore.Description);
                    return PartialView("LabelsDocumentGrid", null);
                }

                CriteriaOperator StoreCriteria = new BinaryOperator("Store.Oid", store.Oid);
                filter = StoreCriteria;
                bool crop_is_null = Object.ReferenceEquals(filter, null);

                CriteriaOperator labelDocumentCriteria = CriteriaOperator.And(new BinaryOperator("Owner", StoreControllerAppiSettings.Owner.Oid),
                    new BinaryOperator("Type", eDocumentType.TAG));
                DocumentType documentType = uow.FindObject<DocumentType>(labelDocumentCriteria);
                if (documentType == null)
                {
                    Session["Error"] = Resources.PleaseSelectADocumentTypeForMobile;
                    return PartialView("LabelsDocumentGrid", null);
                }
                filter = crop_is_null ? new BinaryOperator("DocumentType.Oid", documentType, BinaryOperatorType.Equal)
                                            : CriteriaOperator.And(filter, new BinaryOperator("DocumentType.Oid", documentType.Oid, BinaryOperatorType.Equal));
                crop_is_null = false;
                filter = CriteriaOperator.And(filter, criteria.BuildCriteria());
                Session["grdLabelsDocumentGridFilter"] = filter;
            }

            return PartialView(GetList<DocumentHeader>(XpoSession, filter));
        }

        public ActionResult LabelDocumentDetaisGrid(Guid DocumentOid)
        {            
            DocumentHeader header = XpoSession.GetObjectByKey<DocumentHeader>(DocumentOid);
            Store store = XpoSession.GetObjectByKey<Store>(StoreControllerAppiSettings.CurrentStoreOid);
            EffectivePriceCatalogPolicy effectivePriceCatalogPolicy = new EffectivePriceCatalogPolicy(store);

            List<LabelDocument> priceCatalogDetails = new List<LabelDocument>();
            if (header != null)
            {
                //TODO Use one query to fetch all priceCatalogDetails instead of foreach
                foreach (DocumentDetail detail in header.DocumentDetails)
                {
                    LabelDocument labelDocument = new LabelDocument();
                    labelDocument.Oid = detail.Oid;
                    PriceCatalogPolicyPriceResult priceCatalogPolicyPriceResult = PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(XpoSession, effectivePriceCatalogPolicy, detail.Item);
                    labelDocument.PriceCatalogDetail = priceCatalogPolicyPriceResult == null ? null : priceCatalogPolicyPriceResult.PriceCatalogDetail;
                    labelDocument.DocumentDetail = detail;
                    if (labelDocument.PriceCatalogDetail != null)
                    {
                        priceCatalogDetails.Add(labelDocument);
                    }
                }
            }
            ViewBag.DocumentOid = DocumentOid;
            return PartialView(priceCatalogDetails);
        }
    }
}
//#endif