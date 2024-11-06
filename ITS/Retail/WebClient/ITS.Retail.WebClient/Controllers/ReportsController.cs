using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.Model;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.ReportsLibrary;
using DevExpress.XtraReports.UI;
using System.Threading;
using System.Globalization;

namespace ITS.Retail.WebClient.Controllers
{
    public class ReportsController : BaseObjController<VatLevel>
    {

        [Security(ReturnsPartial = false)]
        public ActionResult Categories(Guid? category)
        {

            List<ReportCategory> reportCategories = ReportsHelper.GetVisibleReportCategories(CurrentUser);
            this.ToolbarOptions.ForceVisible = false;
            if (reportCategories.Count > 0)
            {
                ViewData["Categories"] = reportCategories;
                ViewData["Category"] = category;
            }
            this.CustomJSProperties.AddJSProperty("category", category.ToString());
            return View();
        }

        [Security(ReturnsPartial = false)]
        public ActionResult DocumentCustomReport(Guid? ObjectOid, Guid? Oid, int Duplicates = 0, bool IsDefault = false)
        {
            this.ToolbarOptions.ForceVisible = false;
            Guid objOid = ObjectOid.HasValue ? ObjectOid.Value : Guid.Empty;
            XtraReportBaseExtension report;
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                Guid dtCustomReportOid = Oid.HasValue ? Oid.Value: Guid.Empty;                
                if (IsDefault)
                {
                    CustomReport CustomReport = uow.GetObjectByKey<CustomReport>(dtCustomReportOid);
                    report = PrepareReport(CustomReport.Oid, objOid, (Duplicates > 0 ? Duplicates : 1));
                    ViewData["Title"] = CustomReport.Title;
                }
                else
                {
                    DocumentTypeCustomReport documentTypeCustomReport = uow.GetObjectByKey<DocumentTypeCustomReport>(dtCustomReportOid);
                    report = PrepareReport(documentTypeCustomReport.Report.Oid, objOid, (documentTypeCustomReport.Duplicates > 0 ? documentTypeCustomReport.Duplicates : 1));
                    ViewData["Title"] = documentTypeCustomReport.Description;
                }
                return View("WebDocumentViewer", report);
            }

        }
        [Security(ReturnsPartial = false)]
        public ActionResult CustomReport(Guid? Oid, Guid? ObjectOid, int Duplicates = 1)
        {
            this.ToolbarOptions.ForceVisible = false;
            XtraReportBaseExtension report = PrepareReport(Oid.HasValue ? Oid.Value : Guid.Empty,
                ObjectOid.HasValue ? ObjectOid.Value : Guid.Empty, Duplicates);
            if (Oid.HasValue)
            {
                CustomReport customReport = XpoSession.GetObjectByKey<CustomReport>(Oid.Value);
                if(customReport !=null)
                {
                    ViewData["Title"] = customReport.Title;
                }
            }
            return View("WebDocumentViewer", report);
        }

        private XtraReportBaseExtension PrepareReport(Guid reportID, Guid objOid, int duplicates, bool proccessParameters = false)
        {
            string title;
            string description;
            Guid repId = reportID;
            XtraReportBaseExtension report = ReportsHelper.GetXtraReport(repId, EffectiveOwner, CurrentUser, MvcApplication.OLAPConnectionString, out title, out description); //UserOwner ?? CurrentOwner
            if (report is SingleObjectXtraReport)
            {
                    (report as SingleObjectXtraReport).ObjectOid = objOid;
            }
            else
            {
                if (objOid != Guid.Empty && report.Parameters.Count==1)
                {
                    Guid Oid = Guid.Empty;
                    report.Parameters[0].Value = objOid;
                    report.Parameters[0].Visible = false;
                }
            }
            List<ReportCategory> reportCategories = ReportsHelper.GetVisibleReportCategories(CurrentUser);
            if (reportCategories.Count > 0)
            {
                ViewData["Categories"] = reportCategories;
            }
            if (report != null && proccessParameters)
            {
                ReportsHelper.ProccessReportParameters(report.Parameters, Request);
            }
            if (report != null)
            {
                ReportsHelper.DuplicateReport(report, duplicates);
            }

            return report;
        }

        [Security(ReturnsPartial = false)]
        public ActionResult DocumentReport()
        {

            User currentUser = CurrentUser;
            XtraReport report = null;
            if (bool.Parse(Session["IsAdministrator"].ToString()) || UserHelper.IsCompanyUser(currentUser))
            {
                report = new ITS.Retail.WebClient.Reports.SupplierDocumentReport(this.CurrentUser);
            }
            else
            {
                report = new ITS.Retail.WebClient.Reports.CustomerDocumentReport();
            }

            Guid documentOid;
            Guid.TryParse(Request["DOid"], out documentOid);
            report.DataSource = GetList<DocumentHeader>(XpoHelper.GetNewUnitOfWork(), new BinaryOperator("Oid", documentOid));
            return View("WebDocumentViewer", report);
        }

        [Security(ReturnsPartial = false)]
        public ActionResult PriceCatalogDetailReport(string PCOid)
        {
            Guid priceCatalogGuid = Guid.Empty;
            if (PCOid != null && PCOid != "")
            {
                Guid.TryParse(PCOid, out priceCatalogGuid);
            }

            ITS.Retail.ReportsLibrary.PriceCatalogDetailsReport report = new ITS.Retail.ReportsLibrary.PriceCatalogDetailsReport();
            report.DataSource = GetList<PriceCatalog>(XpoHelper.GetNewUnitOfWork(), new BinaryOperator("Oid", priceCatalogGuid));
            report.Filter.Value = Session["PriceCatalogDetailViewFilter"] == null ? "" : Session["PriceCatalogDetailViewFilter"].ToString();
            return View("WebDocumentViewer", report);
        }

        [Security(ReturnsPartial = false)]
        public ActionResult OffersReport()
        {
            XtraReport report = new OffersReport();
            report.DataSource = GetList<Offer>(XpoHelper.GetNewUnitOfWork(), (CriteriaOperator)Session["OfferFilter"]).AsEnumerable<Offer>();
            return View("WebDocumentViewer", report);
        }


        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            var cultureInfo = Session["cultureInfo"] as CultureInfo ;
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencySymbol = "€";
            Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyPositivePattern = 3;
            Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyNegativePattern = 8;
            Thread.CurrentThread.CurrentUICulture.NumberFormat.CurrencySymbol = "€";
            Thread.CurrentThread.CurrentUICulture.NumberFormat.CurrencyPositivePattern = 3;
            Thread.CurrentThread.CurrentUICulture.NumberFormat.CurrencyNegativePattern = 8;            
        }
    }
}
