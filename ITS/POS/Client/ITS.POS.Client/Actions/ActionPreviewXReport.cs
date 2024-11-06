using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Forms;
using ITS.POS.Client.Helpers;
using ITS.POS.Model.Settings;
using ITS.POS.Hardware;
using ITS.POS.Hardware.Common;
using ITS.POS.Client.Exceptions;
using ITS.POS.Resources;
using ITS.POS.Model.Master;


namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Displays a preview of the current X
    /// </summary>
    public class ActionPreviewXReport : Action
    {
        public ActionPreviewXReport(IPosKernel kernel)
            : base(kernel)
        {

        }

        public override eActions ActionCode
        {
            get { return eActions.PREVIEW_X_REPORT; }
        }

        public override bool RequiresParameters
        {
            get { return false; }
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.SALE | eMachineStatus.PAUSE; }
        }

        protected override void ExecuteCore(ActionParameters.ActionParams parameters, bool dontCheckPermissions = false)
        {
            IAppContext AppContext = Kernel.GetModule<IAppContext>();
            if (AppContext.GetMachineStatus() == eMachineStatus.SALE || AppContext.GetMachineStatus() == eMachineStatus.PAUSE)
            {
                IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
                IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();
                ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
                ITotalizersService totalizersService = Kernel.GetModule<ITotalizersService>();
                IReceiptBuilder receiptBuilder = Kernel.GetModule<IReceiptBuilder>();

                Device printer = null;
                if (config.FiscalMethod == eFiscalMethod.EAFDSS)
                {
                    printer = deviceManager.GetPrimaryDevice<Printer>();
                }
                else if (config.FiscalMethod == eFiscalMethod.ADHME)
                {
                    printer = deviceManager.GetPrimaryDevice<FiscalPrinter>();
                }

                if (printer == null)
                {
                    throw new POSException(POSClientResources.PRINTER_NULL);
                }
                
                Model.Settings.POS currentTerminal = sessionManager.GetObjectByKey<Model.Settings.POS>(config.CurrentTerminalOid);
                Customer defaultCustomer = sessionManager.GetObjectByKey<Customer>(config.DefaultCustomerOid);
                OwnerApplicationSettings appSettings = config.GetAppSettings();
                Report report = totalizersService.CreateXReport(appSettings,AppContext.CurrentDailyTotals, AppContext.CurrentUserDailyTotals, currentTerminal.Name, config.DefaultDocumentTypeOid,
                    config.ProFormaInvoiceDocumentTypeOid, defaultCustomer.VatLevel, config.DepositDocumentTypeOid, config.WithdrawalDocumentTypeOid, config.AsksForFinalAmount);

                List<string> allTheLines = new List<string>();
                Receipt.Receipt receipt = new Receipt.Receipt();
                receipt.Header = receiptBuilder.CreateReceiptHeader(config.ΧReportSchema, report, printer.Settings.LineChars, printer.ConType);
                receipt.Body = receiptBuilder.CreateReceiptBody( config.ΧReportSchema, report, printer.Settings.LineChars, printer.ConType);
                receipt.Footer = receiptBuilder.CreateReceiptFooter(config.ΧReportSchema, report, printer.Settings.LineChars, printer.ConType);
                allTheLines = receipt.GetReceiptLines();
                using (frmReportPreview form = new frmReportPreview("X", "X", allTheLines, printer, this.Kernel))
                {
                    form.ShowDialog();
                }

            }
        }

    }
}
