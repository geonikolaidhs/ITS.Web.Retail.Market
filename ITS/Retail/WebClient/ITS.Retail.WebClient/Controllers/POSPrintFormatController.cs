//#if _RETAIL_STORECONTROLLER || _RETAIL_DUAL
using System;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.Model;
using DevExpress.Xpo;
using ITS.Retail.Common;
using DevExpress.Web.Mvc;
using ITS.Retail.WebClient.Providers;
using DevExpress.Web;
using System.IO;
using ITS.Retail.WebClient.AuxillaryClasses;
using DevExpress.Data.Filtering;
using System.Text;
using Ionic.Zip;
using ITS.Retail.Platform.Enumerations;
using System.Collections.Generic;
using ITS.Retail.ResourcesLib;

namespace ITS.Retail.WebClient.Controllers
{
    [StoreControllerEditable]
    public class POSPrintFormatController : BaseObjController<POSPrintFormat>
    {

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

        public ActionResult FormatEditor()
        {

            return PartialView();
        }

        public static readonly UploadControlValidationSettings UploadFormatFileControlValidationSettings = new UploadControlValidationSettings
        {
            AllowedFileExtensions = new string[] { ".xml", },
            //MaxFileSize = 200971520
        };

        [Security(ReturnsPartial = false), Display(ShowSettings = true)]
        public ActionResult Index()
        {
            this.ToolbarOptions.ViewButton.Visible = false;
            this.ToolbarOptions.ExportToButton.Visible = false;
            this.ToolbarOptions.ExportButton.Visible = true;
            this.ToolbarOptions.ExportButton.OnClick = "ExportButtonOnClick";
            this.ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            this.ToolbarOptions.EditButton.OnClick = "EditSelectedRowsFromGrid";
            this.ToolbarOptions.NewButton.OnClick = "AddNewFromGrid";
            this.CustomJSProperties.AddJSProperty("gridName", "grdPOSPrintFormats");
            Session["uow"] = null;
            GenerateUnitOfWork();
            return View(GetList<POSPrintFormat>(uow));
        }

        public ActionResult ExportDevices([ModelBinder(typeof(ValueModelBinder<Guid>))] Guid[] POSDeviceGuids)
        {
            XPCollection<POSPrintFormat> formats = GetList<POSPrintFormat>(XpoSession, new InOperator("Oid", POSDeviceGuids));
            if (formats.Count == 1)
            {
                return File(Encoding.UTF8.GetBytes(formats[0].Format), "application/xml", formats[0].Description + ".xml");
            }
            else if (formats.Count > 1)
            {
                using (ZipFile zip = new ZipFile())
                {
                    formats.ToList().ForEach(x => zip.AddEntry(x.Description + ".xml", Encoding.UTF8.GetBytes(x.Format)));
                    using (MemoryStream mem = new MemoryStream())
                    {
                        zip.Save(mem);
                        return File(mem.ToArray(), "application/zip", "Pos_Print_Formats.zip");
                    }
                }
            }
            return null;
        }

        [HttpPost]
        public override ActionResult Grid()
        {
            Session["UnsavedFormatString"] = null;
            ViewData["ViewMode"] = true;
            FillLookupComboBoxes();
            if (Request["DXCallbackArgument"].Contains("STARTEDIT") || Request["DXCallbackArgument"].Contains("ADDNEWROW"))
            {
                ViewData["ViewMode"] = false;
            }
            if (uow == null)
            {
                GenerateUnitOfWork();
            }
            return PartialView("Grid", GetList<POSPrintFormat>(uow, new OperandProperty("GCRecord").IsNull()));
        }

        [HttpPost]
        public override ActionResult InsertPartial([ModelBinder(typeof(RetailModelBinder))]POSPrintFormat ct)
        {
            try
            {
                ViewData["ViewMode"] = false;
                if (uow == null)
                {
                    GenerateUnitOfWork();
                }
                ct.Format = String.IsNullOrEmpty(Session["UnsavedFormatString"] as string) ? String.Empty : Session["UnsavedFormatString"].ToString();
                Guid docTypeOid;
                if (Guid.TryParse(Request["DocumentType!Key_VI"], out docTypeOid))
                {
                    ct.DocumentType = ct.Session.GetObjectByKey<DocumentType>(docTypeOid);
                }
                if (CheckDoubleFormatOnDocumentType(ct.Oid, docTypeOid, ct.FormatType))
                {
                    Save(ct);
                    Session["Notice"] = Resources.SavedSuccesfully;
                }
                else
                {
                    Session["Error"] = Resources.KeyCodeAlreadyExists;
                }
            }
            catch (Exception e)
            {
                Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
            }
            FillLookupComboBoxes();
            return PartialView("Grid", GetList<POSPrintFormat>(uow, new OperandProperty("GCRecord").IsNull()));
        }

        [HttpPost]
        public override ActionResult UpdatePartial([ModelBinder(typeof(RetailModelBinder))]POSPrintFormat ct)
        {
            try
            {
                ViewData["ViewMode"] = false;
                if (uow == null)
                {
                    GenerateUnitOfWork();
                }
                ct.Format = String.IsNullOrEmpty(Session["UnsavedFormatString"] as string) ? String.Empty : Session["UnsavedFormatString"].ToString();
                Guid docTypeOid;
                if (Guid.TryParse(Request["DocumentType!Key_VI"], out docTypeOid))
                {
                    ct.DocumentType = ct.Session.GetObjectByKey<DocumentType>(docTypeOid);
                }
                if (CheckDoubleFormatOnDocumentType(ct.Oid, docTypeOid, ct.FormatType))
                {
                    Save(ct);
                    Session["Notice"] = Resources.SavedSuccesfully;
                }
                else
                {
                    Session["Error"] = Resources.KeyCodeAlreadyExists;
                }
            }
            catch (Exception e)
            {
                Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
            }
            FillLookupComboBoxes();
            return PartialView("Grid", GetList<POSPrintFormat>(uow, new OperandProperty("GCRecord").IsNull()));
        }

        [HttpPost]
        public override ActionResult DeletePartial([ModelBinder(typeof(RetailModelBinder))]POSPrintFormat ct)
        {
            FillLookupComboBoxes();
            Delete(ct);
            return PartialView("Grid", GetList<POSPrintFormat>(uow, new OperandProperty("GCRecord").IsNull()));
        }

        [Security(ReturnsPartial = false)]
        public ActionResult UploadFormatFileControl()
        {
            UploadControlExtension.GetUploadedFiles("UploadFormatFileControl", UploadFormatFileControlValidationSettings, FormatFileUpload_FileUploadComplete);
            FillLookupComboBoxes();
            return null;
        }

        public void FormatFileUpload_FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
        {
            if (e.UploadedFile.IsValid)
            {
                string xmlString = System.Text.UTF8Encoding.UTF8.GetString(e.UploadedFile.FileBytes);
                Session["UnsavedFormatString"] = xmlString;
            }
        }

        protected override void FillLookupComboBoxes()
        {
            base.FillLookupComboBoxes();
            CriteriaOperator criteria = CriteriaOperator.And(new BinaryOperator("DocumentSeries.eModule", eModule.SFA),
                                                                          new OperandProperty("DocumentType").IsNotNull());
            if (this.uow == null)
            {
                GenerateUnitOfWork();
            }
            List<DocumentType> docTypes = new XPCollection<StoreDocumentSeriesType>(this.uow, criteria).Select(sdst => sdst.DocumentType).ToList();
            ViewBag.DocumentTypes = docTypes.Distinct().ToList();
        }

        private bool CheckDoubleFormatOnDocumentType(Guid Oid, Guid docTypeOid, eFormatType formatType)
        {
            POSPrintFormat pf = null;
            if (Oid != null && Oid != Guid.Empty && docTypeOid != null && docTypeOid != Guid.Empty)
            {
                if (uow == null)
                {
                    GenerateUnitOfWork();
                }
                CriteriaOperator crit = CriteriaOperator.And(new BinaryOperator("DocumentType.Oid", docTypeOid, BinaryOperatorType.Equal),
                                                                   new BinaryOperator("FormatType", formatType, BinaryOperatorType.Equal),
                                                                             new BinaryOperator("Oid", Oid, BinaryOperatorType.NotEqual));
                pf = new XPCollection<POSPrintFormat>(uow, crit).FirstOrDefault();
            }
            return (pf == null);
        }
    }
}
//#endif