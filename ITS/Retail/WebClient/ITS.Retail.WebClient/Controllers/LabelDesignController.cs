using System;
using System.Collections.Generic;
using System.Web.Mvc;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.WebClient.Providers;
using System.IO;
using System.Text;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.PrintServer.Common;
using ITS.Retail.WebClient.Helpers;

namespace ITS.Retail.WebClient.Controllers
{    
    public class LabelDesignController : BaseObjController<Label>
    {
        public override ActionResult LoadEditPopup()
        {
            base.LoadEditPopup();

            ViewBag.Title = Resources.Label;

            ActionResult rt = PartialView("LoadEditPopup");
            return rt;
        }

        [Display(ShowSettings = true)]
        public ActionResult Index()
        {
            this.ToolbarOptions.ViewButton.Visible = true;
            this.ToolbarOptions.ViewButton.OnClick = "ShowGenericView";

            this.ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            this.ToolbarOptions.NewButton.OnClick = "AddNewCustomV2";
            this.ToolbarOptions.EditButton.OnClick = "EditSelectedRowsCustomV2";
            this.ToolbarOptions.ExportButton.OnClick = "LabelDesign.ExportEJF";
            this.ToolbarOptions.ExportButton.Visible = true;
            this.ToolbarOptions.ExportButton.OnClick = "LabelDesign.ExportEJF";
            this.ToolbarOptions.ExportButton.Visible = true;

            this.CustomJSProperties.AddJSProperty("editAction", "Edit");
            this.CustomJSProperties.AddJSProperty("editIDParameter", "Oid");

            this.CustomJSProperties.AddJSProperty("gridName", "grdLabelDesign");
            this.ToolbarOptions.ExportToButton.Visible = false;
            return View(GetList<Label>(XpoSession));
        }

        protected override GenericViewRuleset GenerateGenericViewRuleset()
        {
            GenericViewRuleset ruleset = base.GenerateGenericViewRuleset();
            ruleset.PropertiesToIgnore.AddRange(new List<string>() { "IsActive"});
            ruleset.NumberOfColumns = 2;
            return ruleset;
        }

        public ActionResult Edit(Guid Oid)
        {
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            Label model;
            if (Oid != Guid.Empty)
            {
                model = uow.GetObjectByKey<Label>(Oid);
            }
            else
            {
                model = new Label(uow);
            }
            Session["EditingItem"] = model;
            FillLookupComboBoxes();
            return PartialView(model);
        }

        public static readonly UploadControlValidationSettings UploadControlValidationSettings = new UploadControlValidationSettings
        {
            AllowedFileExtensions = new string[] { ".ejf" },
            MaxFileSize = 200971520
        };

        public JsonResult Save([ModelBinder(typeof(RetailModelBinder))]Label ct)
        {            
            Label label = Session["EditingItem"] as Label;
            label.Code = ct.Code;
            label.Description = ct.Description;
            label.IsDefault = ct.IsDefault;
            label.UseDirectSQL = ct.UseDirectSQL;
            label.DirectSQL = ct.DirectSQL;
            label.PrinterEncoding = ct.PrinterEncoding;

            //if ( label.PrintServiceSettings == null )
            //{
            //    label.PrintServiceSettings = new LabelPrintServiceSettings(label.Session);
            //}

            //label.PrintServiceSettings.RemotePrinterService = String.IsNullOrWhiteSpace(Request["RemotePrinterServices_VI"])
            //                              ? null
            //                              : label.Session.GetObjectByKey<POSDevice>(Guid.Parse(Request["RemotePrinterServices_VI"].ToString()));
            //label.PrintServiceSettings.PrinterNickName = ct.PrintServiceSettings.PrinterNickName;

            label.Save();
            XpoHelper.CommitTransaction(label.Session as UnitOfWork);
            return Json(new { });      
        }

        public ActionResult UploadControl()
        {
            UploadControlValidationSettings validationSettings = new UploadControlValidationSettings() { ShowErrors = true };
            UploadControlExtension.GetUploadedFiles("UploadControl", validationSettings, UploadLabelCompleted);
            return null;
        }
        public static void UploadLabelCompleted(object sender, FileUploadCompleteEventArgs e)
        {
            if (e.UploadedFile.IsValid)
            {
                Label currentLabel = System.Web.HttpContext.Current.Session["EditingItem"] as Label;
                if (currentLabel != null)
                {
                    using (StreamReader sr = new StreamReader(e.UploadedFile.FileContent))
                    {
                        string allLines = sr.ReadToEnd();
                        currentLabel.LabelFile = Encoding.UTF8.GetBytes(allLines);
                    }
                    currentLabel.LabelFileName = e.UploadedFile.FileName;
                    e.CallbackData = e.UploadedFile.FileName;
                }
            }
        }

        public FileContentResult DownloadEJF(Guid? LabelOid)
        {
            string message = string.Empty;
            if(LabelOid.HasValue)
            {
                using (UnitOfWork unitOfWork = XpoHelper.GetNewUnitOfWork())
                {
                    Label label = unitOfWork.GetObjectByKey<Label>(LabelOid);
                    if(label != null )
                    {
                        return File(label.LabelFile, "text/plain", label.LabelFileName);
                    }
                    else
                    {
                        message = Resources.InvalidValue;
                    }
                }
            }
            else
            {
                message = Resources.InvalidValue;
            }
            return null;
        }

        protected override void FillLookupComboBoxes()
        {
            base.FillLookupComboBoxes();
        }

        public ActionResult UpdateLabelRemotePrinters(Guid? printerServer, string selectedPrinter)
        {
            ViewBag.selectedPrinter = selectedPrinter;
            if (printerServer.HasValue == false)
            {
                ViewBag.PrinterNickNames = new List<string>();
            }
            else
            {
                POSDevice printerService = XpoSession.GetObjectByKey<POSDevice>(printerServer.Value);
                PrintServerGetPrintersResponse result = PrinterServiceHelper.TestRemotePrinterServerConnection(printerService);
                if (result == null)
                {
                    Session["Error"] = Resources.CouldNotEstablishConnection + " Remote Print Service :" + printerService.Name;
                    List<string> printers = new List<string>();
                    if (!String.IsNullOrEmpty(selectedPrinter))
                    {
                        printers.Add(selectedPrinter);
                    }
                    ViewBag.PrinterNickNames = printers;
                }
                else if (result == null || result.Result == ePrintServerResponseType.FAILURE)
                {
                    Session["Error"] = result.Explanation;
                    List<string> printers = new List<string>();
                    if (!String.IsNullOrEmpty(selectedPrinter))
                    {
                        printers.Add(selectedPrinter);
                    }
                    ViewBag.PrinterNickNames = printers;
                }
                else
                {
                    ViewBag.PrinterNickNames = result.Printers;
                }
            }
            return PartialView();
        }
    }
}
