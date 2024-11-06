#if _RETAIL_STORECONTROLLER || _RETAIL_DUAL
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.PrintServer.Common;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.WebClient.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ITS.Retail.WebClient.Controllers
{
    [StoreControllerEditable]
    public class StoreDocumentSeriesTypePrintServiceSettingsController : BaseObjController<StoreDocumentSeriesTypePrintServiceSettings>
    {
        [Security(ReturnsPartial = false), Display(ShowSettings = true)]
        public ActionResult Index()
        {
            this.ToolbarOptions.OptionsButton.Visible = false;
            this.ToolbarOptions.NewButton.OnClick = "AddNewFromGrid";
            this.ToolbarOptions.EditButton.OnClick = "EditSelectedRowsFromGrid";
            this.ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            this.ToolbarOptions.DeleteButton.Visible = false;
            this.CustomJSProperties.AddJSProperty("gridName", "grdStoreDocumentSeriesTypePrintServiceSettings");

            return View(GetList<StoreDocumentSeriesTypePrintServiceSettings>(XpoSession));
        }

        public override ActionResult Grid()
        {
            if (Request["DXCallbackArgument"] != null)
            {   
                if (Request["DXCallbackArgument"].Contains("ADDNEWROW"))
                {
                    Session["PrintServiceSettings"] = new StoreDocumentSeriesTypePrintServiceSettings(XpoHelper.GetNewUnitOfWork());
                }
                else if (Request["DXCallbackArgument"].Contains("STARTEDIT"))
                {
                    UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
                    Session["PrintServiceSettings"] = uow.GetObjectByKey<StoreDocumentSeriesTypePrintServiceSettings>(RetailHelper.GetOidToEditFromDxCallbackArgument(Request["DXCallbackArgument"]));
                    ViewBag.StoreDocumentSeriesTypes = GetList<StoreDocumentSeriesType>(uow);                    
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
            StoreDocumentSeriesTypePrintServiceSettings storeDocumentSeriesTypePrintServiceSettings = Session["PrintServiceSettings"] as StoreDocumentSeriesTypePrintServiceSettings;
            StoreDocumentSeriesType storeDocumentSeriesType = storeDocumentSeriesTypePrintServiceSettings == null ? null : storeDocumentSeriesTypePrintServiceSettings.StoreDocumentSeriesType;
            if (storeDocumentSeriesType != null && storeDocumentSeriesType.PrintServiceSettings != null && storeDocumentSeriesType.PrintServiceSettings.RemotePrinterService != null)
            {
                PrintServerGetPrintersResponse result = PrinterServiceHelper.TestRemotePrinterServerConnection(storeDocumentSeriesType.PrintServiceSettings.RemotePrinterService);
                if (result != null && result.Result == ePrintServerResponseType.SUCCESS)
                {
                    ViewBag.PrinterNickNames = result.Printers;
                }
                else
                {
                    ViewBag.PrinterNickNames = storeDocumentSeriesType.PrintServiceSettings.PrinterNickName;
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

        [HttpPost]
        public ActionResult Insert([ModelBinder(typeof(RetailModelBinder))] StoreDocumentSeriesTypePrintServiceSettings printerSettings)
        {
            try
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    StoreDocumentSeriesTypePrintServiceSettings newPrinterSettings = new StoreDocumentSeriesTypePrintServiceSettings(uow);
                    newPrinterSettings.StoreDocumentSeriesType = uow.GetObjectByKey<StoreDocumentSeriesType>(Guid.Parse(Request["StoreDocumentSeriesTypess_VI"]));
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
        public ActionResult Update([ModelBinder(typeof(RetailModelBinder))] StoreDocumentSeriesTypePrintServiceSettings printerSettings)
        {
            try
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    StoreDocumentSeriesTypePrintServiceSettings newPrinterSettings = uow.GetObjectByKey<StoreDocumentSeriesTypePrintServiceSettings>(printerSettings.Oid);
                    newPrinterSettings.StoreDocumentSeriesType = uow.GetObjectByKey<StoreDocumentSeriesType>(Guid.Parse(Request["StoreDocumentSeriesTypess_VI"]));
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

        public ActionResult StoreDocumentSeriesTypes()
        {
            ViewBag.StoreDocumentSeriesTypes = GetList<StoreDocumentSeriesType>(XpoSession);
            return PartialView();
        }
    }
}
#endif