#if _RETAIL_STORECONTROLLER || _RETAIL_DUAL
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.PrintServer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ITS.Retail.ResourcesLib;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.WebClient.Providers;

namespace ITS.Retail.WebClient.Controllers
{
    [StoreControllerEditable]
    public class LabelPrintServiceSettingsController : BaseObjController<LabelPrintServiceSettings>
    {
        [Security(ReturnsPartial = false), Display(ShowSettings = true)]
        public ActionResult Index()
        {
            this.ToolbarOptions.OptionsButton.Visible = false;
            this.ToolbarOptions.NewButton.OnClick = "AddNewFromGrid";
            this.ToolbarOptions.EditButton.OnClick = "EditSelectedRowsFromGrid";
            this.ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            this.ToolbarOptions.DeleteButton.Visible = false;
            this.CustomJSProperties.AddJSProperty("gridName", "grdLabelPrintServiceSettings");

            return View(GetList<LabelPrintServiceSettings>(XpoSession));
        }

        public override ActionResult Grid()
        {
            if (Request["DXCallbackArgument"] != null)
            {
                if (Request["DXCallbackArgument"].Contains("ADDNEWROW"))
                {
                    Session["PrintServiceSettings"] = new LabelPrintServiceSettings(XpoHelper.GetNewUnitOfWork());
                }
                else if (Request["DXCallbackArgument"].Contains("STARTEDIT"))
                {
                    UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
                    Session["PrintServiceSettings"] = uow.GetObjectByKey<LabelPrintServiceSettings>(RetailHelper.GetOidToEditFromDxCallbackArgument(Request["DXCallbackArgument"]));
                    ViewBag.Labels = GetList<Label>(uow);
                }
            }

            FillLookupComboBoxes();
            return base.Grid();
        }

        protected override void FillLookupComboBoxes()
        {
            base.FillLookupComboBoxes();
            CriteriaOperator remotePrintServiceCriteria = new BinaryOperator("DeviceSettings.DeviceType", DeviceType.RemotePrint);
            ViewBag.RemotePrinterServicesComboBox = GetList<POSDevice>(XpoSession, remotePrintServiceCriteria);

            ViewBag.PrinterNickNames = new List<string>();
            LabelPrintServiceSettings labelTypePrintServiceSettings = Session["PrintServiceSettings"] as LabelPrintServiceSettings;
            Label label = labelTypePrintServiceSettings == null ? null : labelTypePrintServiceSettings.Label;
            if (label != null && label.PrintServiceSettings != null && label.PrintServiceSettings.RemotePrinterService != null)
            {
                PrintServerGetPrintersResponse result = PrinterServiceHelper.TestRemotePrinterServerConnection(label.PrintServiceSettings.RemotePrinterService);
                if (result != null && result.Result == ePrintServerResponseType.SUCCESS)
                {
                    ViewBag.PrinterNickNames = result.Printers;
                }
                else
                {
                    ViewBag.PrinterNickNames = label.PrintServiceSettings.PrinterNickName;
                }
            }
        }

        public ActionResult UpdateRemotePrinters(Guid? printerServer, string selectedPrinter)
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

        public ActionResult Labels()
        {
            ViewBag.Labels = GetList<Label>(XpoSession);
            return PartialView();
        }

        [HttpPost]
        public ActionResult Insert([ModelBinder(typeof(RetailModelBinder))] LabelPrintServiceSettings printerSettings)
        {
            try
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    LabelPrintServiceSettings newPrinterSettings = new LabelPrintServiceSettings(uow);
                    newPrinterSettings.Label = uow.GetObjectByKey<Label>(Guid.Parse(Request["Labels_VI"]));
                    newPrinterSettings.RemotePrinterService = uow.GetObjectByKey<POSDevice>(Guid.Parse(Request["RemotePrinterServices_VI"]));
                    newPrinterSettings.PrinterNickName = printerSettings.PrinterNickName;
                    newPrinterSettings.IsActive = Request["IsActive"] == "C";

                    newPrinterSettings.Save();
                    XpoHelper.CommitChanges(uow);
                }
            }
            catch (Exception exception)
            {
                Session["Error"] = exception.GetFullMessage();
            }
            return base.Grid();
        }

        [HttpPost]
        public ActionResult Update([ModelBinder(typeof(RetailModelBinder))] LabelPrintServiceSettings printerSettings)
        {
            try
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    LabelPrintServiceSettings newPrinterSettings = uow.GetObjectByKey<LabelPrintServiceSettings>(printerSettings.Oid);
                    newPrinterSettings.Label = uow.GetObjectByKey<Label>(Guid.Parse(Request["Labels_VI"]));
                    newPrinterSettings.RemotePrinterService = uow.GetObjectByKey<POSDevice>(Guid.Parse(Request["RemotePrinterServices_VI"]));
                    newPrinterSettings.PrinterNickName = printerSettings.PrinterNickName;
                    newPrinterSettings.IsActive = Request["IsActive"] == "C";

                    newPrinterSettings.Save();
                    XpoHelper.CommitChanges(uow);
                }
            }
            catch (Exception exception)
            {
                Session["Error"] = exception.GetFullMessage();
            }
            return base.Grid();
        }
    }
}
#endif