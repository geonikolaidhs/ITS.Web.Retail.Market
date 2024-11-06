using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.Model;
using ITS.Retail.Common;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.Retail.WebClient.Providers;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.PrintServer.Common;

namespace ITS.Retail.WebClient.Controllers
{
    public class StoreDocumentSeriesTypeController : BaseObjController<StoreDocumentSeriesType>
    {

        public override ActionResult Grid()
        {
            if (Request["DXCallbackArgument"] != null &&
                (Request["DXCallbackArgument"].Contains("STARTEDIT") ||
                Request["DXCallbackArgument"].Contains("ADDNEWROW")))
            {
                FillLookupComboBoxes(RetailHelper.GetOidToEditFromDxCallbackArgument(Request["DXCallbackArgument"]).ToString());
            }
            return PartialView((Session["currentDocumentType"] as DocumentType).StoreDocumentSeriesTypes);
        }


        [HttpPost]
        public ActionResult Insert([ModelBinder(typeof(RetailModelBinder))] StoreDocumentSeriesType ct)
        {
            UpdateLookupObjects(ct);

            StoreDocumentSeriesType newStoreDocumentSeriesType = new StoreDocumentSeriesType((Session["currentDocumentType"] as DocumentType).Session);
            newStoreDocumentSeriesType.GetData(ct);
            newStoreDocumentSeriesType.DocumentType = (Session["currentDocumentType"] as DocumentType);
            AssignOwner(newStoreDocumentSeriesType);


            (Session["currentDocumentType"] as DocumentType).StoreDocumentSeriesTypes.Add(newStoreDocumentSeriesType);
            FillLookupComboBoxes();
            if (String.IsNullOrEmpty(ct.DefaultDiscountPercentage.ToString())) 
            {
                ct.DefaultDiscountPercentage = 0;
                ct.DefaultDiscountType = eDiscountType.PERCENTAGE;
            }
            if (ct.DefaultDiscountPercentage > 0)
            {
                ct.DefaultDiscountPercentage = ct.DefaultDiscountPercentage / 100;
                ct.DefaultDiscountType = eDiscountType.PERCENTAGE;
            }
            else
            {
                ct.DefaultDiscountPercentage = 0;
                ct.DefaultDiscountType = eDiscountType.PERCENTAGE;
            }
            return PartialView("Grid", (Session["currentDocumentType"] as DocumentType).StoreDocumentSeriesTypes);
        }


        [HttpPost]
        public ActionResult Update([ModelBinder(typeof(RetailModelBinder))] StoreDocumentSeriesType ct)
        {

            if (String.IsNullOrEmpty(ct.DefaultDiscountPercentage.ToString()) || String.IsNullOrEmpty(Request["DefaultDiscountPercentage"]))
            {
                ct.DefaultDiscountPercentage = 0;
                ct.DefaultDiscountType = eDiscountType.PERCENTAGE;
            }
            if (ct.DefaultDiscountPercentage > 100)
            {
                ct.DefaultDiscountPercentage = 100;
            }
            if (ct.DefaultDiscountPercentage > 0)
            {
                ct.DefaultDiscountPercentage = ct.DefaultDiscountPercentage / 100;
                ct.DefaultDiscountType = eDiscountType.PERCENTAGE;
            }
            else
            {
                ct.DefaultDiscountPercentage = 0;
                ct.DefaultDiscountType = eDiscountType.PERCENTAGE;
            }

            UpdateLookupObjects(ct);
            if (ct.DocumentSeries.eModule == eModule.POS && ct.StoreDocumentType == eStoreDocumentType.ORDER)
            {
                ModelState.AddModelError("DocumentSeriess", Resources.POSDocumentSeriesForOrder);
            }
            if (ct.DocumentSeries.eModule != eModule.POS || ct.StoreDocumentType != eStoreDocumentType.ORDER)
            {
                DocumentType docType = (DocumentType)Session["currentDocumentType"];
                StoreDocumentSeriesType type = docType.StoreDocumentSeriesTypes.FirstOrDefault(g => g.Oid == ct.Oid) ?? new StoreDocumentSeriesType(docType.Session);
                type.GetData(ct);

                docType.StoreDocumentSeriesTypes.Add(type);
            }
            else
            {
                FillLookupComboBoxes(ct.Oid.ToString());
            }
            return PartialView("Grid", (Session["currentDocumentType"] as DocumentType).StoreDocumentSeriesTypes);
        }

        [HttpPost]
        public new ActionResult Delete([ModelBinder(typeof(RetailModelBinder))] StoreDocumentSeriesType ct)
        {
            (Session["currentDocumentType"] as DocumentType).StoreDocumentSeriesTypes.First(g => g.Oid == ct.Oid).Delete();
            //FillLookupComboBoxes();
            return PartialView("Grid", (Session["currentDocumentType"] as DocumentType).StoreDocumentSeriesTypes);
        }

        protected void FillLookupComboBoxes(string storeDocSeriesTypeOid = "")
        {
            //base.FillLookupComboBoxes();
            DocumentType doctype = Session["currentDocumentType"] as DocumentType;
            CriteriaOperator docSeriescriteria = new NotOperator(new ContainsOperator("StoreDocumentSeriesTypes", new BinaryOperator("DocumentType", doctype.Oid)));
            //List <Guid> existingdocseriesOids =  doctype.StoreDocumentSeriesTypes.Select<StoreDocumentSeriesType,Guid>(storedoctype => storedoctype.DocumentSeries.Oid).ToList();
            if (storeDocSeriesTypeOid != "")
            {
                Guid editGuid;
                if (Guid.TryParse(storeDocSeriesTypeOid, out editGuid))
                {
                    StoreDocumentSeriesType storeDocSeriesType = doctype.StoreDocumentSeriesTypes.FirstOrDefault(sdst => sdst.Oid == editGuid);
                    if (storeDocSeriesType != null && storeDocSeriesType.DocumentSeries != null)
                    {
                        docSeriescriteria = CriteriaOperator.Or(docSeriescriteria, new BinaryOperator("Oid", storeDocSeriesType.DocumentSeries.Oid));
                    }
                    //ViewBag.PrinterNickNames = new List<string>();
                    //if (storeDocSeriesType != null && storeDocSeriesType.PrintServiceSettings != null && storeDocSeriesType.PrintServiceSettings.RemotePrinterService != null)
                    //{
                    //    PrintServerGetPrintersResponse result = PrinterServiceHelper.TestRemotePrinterServerConnection(storeDocSeriesType.PrintServiceSettings.RemotePrinterService);
                    //    if (result != null && result.Result == ePrintServerResponseType.SUCCESS)
                    //    {
                    //        ViewBag.PrinterNickNames = result.Printers;
                    //    }
                    //    else
                    //    {
                    //        ViewBag.PrinterNickNames = storeDocSeriesType.PrintServiceSettings.PrinterNickName;
                    //    }
                    //}
                }
            }
            ViewBag.StoreDocumentSeriesComboBox = GetList<DocumentSeries>(XpoSession, docSeriescriteria).OrderBy(docseries => docseries.Description);
            ViewBag.Reports = GetList<CustomReport>(XpoSession).Where(report => XtraReportBaseExtension.GetReportTypeFromFile(report.ReportFile) == typeof(SingleObjectXtraReport)
                                                                               && XtraReportBaseExtension.GetSingleObjectTypeFromFile(report.ReportFile) == typeof(DocumentHeader));

            //CriteriaOperator remotePrintServiceCriteria = new BinaryOperator("DeviceSettings.DeviceType", DeviceType.RemotePrint);
            //ViewBag.RemotePrinterServicesComboBox = GetList<POSDevice>(XpoSession, remotePrintServiceCriteria);

            ViewBag.UserTypes = new List<UserType>() { UserType.NONE, UserType.TRADER, UserType.COMPANYUSER, UserType.ALL }.ToDictionary(x => x, y => y.ToLocalizedString());

            if (StoresThatCurrentUserOwns == null)
            {
                ViewBag.CancelStoreDocumentSeriesTypeComboBox = GetList<StoreDocumentSeriesType>(XpoSession);
            }
            else
            {
                ViewBag.CancelStoreDocumentSeriesTypeComboBox = (Session["currentDocumentType"] as DocumentType).Stores;
            }
        }

        protected override void UpdateLookupObjects(StoreDocumentSeriesType storeDocumentSeriesType)
        {
            base.UpdateLookupObjects(storeDocumentSeriesType);
            string customerOid = Request["DefaultCustomerr_VI"];
            if (!String.IsNullOrWhiteSpace(customerOid))
            {
                Guid customerGuid;
                if (Guid.TryParse(customerOid, out customerGuid))
                {
                    storeDocumentSeriesType.DefaultCustomer = storeDocumentSeriesType.Session.GetObjectByKey<Customer>(customerGuid);
                }
            }
            else
            {
                storeDocumentSeriesType.DefaultCustomer = null;
            }
            string supplierOid = Request["DefaultSupplierr_VI"];
            if (!String.IsNullOrEmpty(supplierOid))
            {
                Guid supplierGuid;
                if (Guid.TryParse(supplierOid, out supplierGuid))
                {
                    storeDocumentSeriesType.DefaultSupplier = storeDocumentSeriesType.Session.GetObjectByKey<SupplierNew>(supplierGuid);
                }
            }
            else
            {
                storeDocumentSeriesType.DefaultSupplier = null;
            }
            string CustomReportOid = Request["DefaultCustomReportt_VI"];
            if (!String.IsNullOrEmpty(CustomReportOid))
            {
                Guid CustomReportGuid;
                if (Guid.TryParse(CustomReportOid, out CustomReportGuid))
                {
                    storeDocumentSeriesType.DefaultCustomReport = storeDocumentSeriesType.Session.GetObjectByKey<CustomReport>(CustomReportGuid);
                }
            }
            else
            {
                storeDocumentSeriesType.DefaultCustomReport = null;
            }
            storeDocumentSeriesType.Duplicates = String.IsNullOrWhiteSpace(Request["Duplicates"]) ? 0 : int.Parse(Request["Duplicates"]);
            storeDocumentSeriesType.UserType = String.IsNullOrWhiteSpace(Request["UserType_VI"]) ? UserType.NONE : (UserType)Enum.Parse(typeof(UserType), Request["UserType_VI"]);
            storeDocumentSeriesType.DocumentSeries = GetObjectByArgument<DocumentSeries>(storeDocumentSeriesType.Session, "DocumentSeriess_VI");//"DocumentSeries_VI");//DevExpress 15.1 stupid bug https://community.devexpress.com/blogs/aspnet/archive/2015/06/09/asp-net-mvc-other-enhancements-in-v15-1.aspx
            //storeDocumentSeriesType.RemotePrinterService = GetObjectByArgument<POSDevice>(storeDocumentSeriesType.Session, "RemotePrinterServices_VI");//DevExpress 15.1 stupid bug https://community.devexpress.com/blogs/aspnet/archive/2015/06/09/asp-net-mvc-other-enhancements-in-v15-1.aspx
        }

        protected override void ExecuteCore()
        {
            base.ExecuteCore();
        }

        public static object GetTraderByValue<W>(object value) where W : BaseObj
        {
            return GetObjectByValue<W>(value);
        }

        public static object GetCustomReportByValue(object value)
        {
            return GetObjectByValue<CustomReport>(value);
        }

        public static object TraderRequestedByFilterCondition<W>(DevExpress.Web.ListEditItemsRequestedByFilterConditionEventArgs e) where W : BaseObj
        {
            if (e.Filter == "") { return null; }
            string proccessed_filter = e.Filter.Replace("*", "%");
            if (!proccessed_filter.Contains("%"))
            {
                proccessed_filter = String.Format("%{0}%", proccessed_filter);
            }
            UnitOfWork uow = (System.Web.HttpContext.Current.Session["currentDocumentType"] as DocumentType).Session as UnitOfWork;

            CriteriaOperator crop;

            crop = CriteriaOperator.And(new BinaryOperator("IsActive", true), CriteriaOperator.Or(new BinaryOperator("CompanyName", proccessed_filter, BinaryOperatorType.Like),
                                                                                                  new BinaryOperator("Code", proccessed_filter, BinaryOperatorType.Like)));


            XPCollection<W> searched_items = GetList<W>(uow, crop, "CompanyName");

            searched_items.SkipReturnedObjects = e.BeginIndex;
            searched_items.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;

            return searched_items;
        }



        public ActionResult SelectDefaultCustomer()
        {
            return PartialView();
        }

        public ActionResult SelectDefaultSupplier()
        {
            return PartialView();
        }

        public ActionResult SelectDefaultCustomReport()
        {
            return PartialView();
        }
    }
}
