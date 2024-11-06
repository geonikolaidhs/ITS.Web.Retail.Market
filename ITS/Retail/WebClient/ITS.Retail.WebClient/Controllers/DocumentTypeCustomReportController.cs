using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.WebClient.Providers;

namespace ITS.Retail.WebClient.Controllers
{
    public class DocumentTypeCustomReportController : BaseObjController<DocumentTypeCustomReport>
    {
        //
        // GET: /DocumentTypeCustomReport/

        [Security(ReturnsPartial=false)]
        public ActionResult Index()
        {
            //this.ToolbarOptions.Visible = true;
            //this.ToolbarOptions.FilterButton.Visible = true;
            this.ToolbarOptions.ExportToButton.Visible = false;
            this.ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            this.ToolbarOptions.EditButton.OnClick = "EditSelectedRowsFromGrid";
            this.ToolbarOptions.NewButton.OnClick = "AddNewFromGrid";

            this.CustomJSProperties.AddJSProperty("gridName", "grdDocumentTypeCustomReport");
            return View(GetList<DocumentTypeCustomReport>(XpoSession));
        }

        public override ActionResult Grid()
        {
            FillLookupComboBoxes();
            return PartialView( (Session["currentDocumentType"] as DocumentType)==null 
                                ? null 
                                : (Session["currentDocumentType"] as DocumentType).DocumentTypeCustomReports
                              );
        }

        protected override void UpdateLookupObjects(DocumentTypeCustomReport a)
        {
            base.UpdateLookupObjects(a);
            a.DocumentType = GetObjectByArgument<DocumentType>(a.Session, "DocumentTypeKey_VI");
            a.Report = GetObjectByArgument<CustomReport>(a.Session, "ReportKey_VI");
            a.UserType = String.IsNullOrWhiteSpace(Request["UserType_VI"]) ? UserType.NONE : (UserType)Enum.Parse(typeof(UserType), Request["UserType_VI"]);
        }

        protected override void FillLookupComboBoxes()
        {
            base.FillLookupComboBoxes();
            ViewBag.DocumentTypes = GetList<DocumentType>(XpoSession);
            ViewBag.Reports = GetList<CustomReport>(XpoSession).Where(
                                                         report => XtraReportBaseExtension.GetReportTypeFromFile(report.ReportFile) == typeof(SingleObjectXtraReport)   
                                                      && XtraReportBaseExtension.GetSingleObjectTypeFromFile(report.ReportFile) == typeof(DocumentHeader)
                                                           );
            ViewBag.UserTypes = new List<UserType>() { UserType.NONE, UserType.TRADER, UserType.COMPANYUSER, UserType.ALL }.ToDictionary(x => x, y => y.ToLocalizedString());
        }
       
        [HttpPost]
        public ActionResult Insert([ModelBinder(typeof(RetailModelBinder))] DocumentTypeCustomReport ct)
        {
            UpdateLookupObjects(ct);
            DocumentTypeCustomReport newObj = new DocumentTypeCustomReport((Session["currentDocumentType"] as DocumentType).Session);
            newObj.GetData(ct);
            newObj.DocumentType = (Session["currentDocumentType"] as DocumentType);
            (Session["currentDocumentType"] as DocumentType).DocumentTypeCustomReports.Add(newObj);
            FillLookupComboBoxes();
            return PartialView("Grid", (Session["currentDocumentType"] as DocumentType).DocumentTypeCustomReports);
        }

        [HttpPost]
        public ActionResult Update([ModelBinder(typeof(RetailModelBinder))] DocumentTypeCustomReport ct)
        {
            UpdateLookupObjects(ct);

            DocumentTypeCustomReport dtcr= (Session["currentDocumentType"] as DocumentType).DocumentTypeCustomReports.First(g => g.Oid == ct.Oid);
            dtcr.GetData(ct);
            (Session["currentDocumentType"] as DocumentType).DocumentTypeCustomReports.Add(dtcr);

            FillLookupComboBoxes();
            return PartialView("Grid", (Session["currentDocumentType"] as DocumentType).DocumentTypeCustomReports);
        }

        [HttpPost]
        public ActionResult Delete([ModelBinder(typeof(RetailModelBinder))] DocumentTypeCustomReport ct)
        {
            (Session["currentDocumentType"] as DocumentType).DocumentTypeCustomReports.First(g => g.Oid == ct.Oid).Delete();
            FillLookupComboBoxes();
            return PartialView("Grid", (Session["currentDocumentType"] as DocumentType).DocumentTypeCustomReports);
        }

    }
}
