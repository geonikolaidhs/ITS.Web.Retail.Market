using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraWaitForm;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Forms;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.UserControls;
using ITS.POS.Hardware;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
using ITS.POS.Resources;
using ITS.Retail.Platform.Common.AuxilliaryClasses;
using ITS.Retail.Platform.Common.OposReportSchema;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace ITS.POS.Client.Actions
{
    public class ActionPrintReportToOposPrinterInernal : Action
    {
        public ActionPrintReportToOposPrinterInernal(IPosKernel kernel) : base(kernel)
        {

        }


        protected override void ExecuteCore(ActionParams parameters = null, bool dontCheckPermissions = false)
        {
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            IConfigurationManager configurationManager = Kernel.GetModule<IConfigurationManager>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            ISessionManager sessionManager = this.Kernel.GetModule<ISessionManager>();
            IDeviceManager deviceManager = this.Kernel.GetModule<IDeviceManager>();
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            IOposReportService oposReportService = Kernel.GetModule<IOposReportService>();


            ActionPrintReportToOposPrinterInernalParams castedParams = parameters as ActionPrintReportToOposPrinterInernalParams;
            Printer printer = deviceManager.GetDeviceByName<Printer>(castedParams.PosReportSettings.PrinterName) as Printer;
            PosReport posReport = castedParams.PosReport;
            List<string> printerlines = new List<string>() { };
            object reportClassInstace = oposReportService.GetReportInstance(posReport.Oid.ToString() + ".dll", "PosOposReport");
            if (reportClassInstace == null)
            {
                throw new Exception("report Instance not found");
            }
            List<UserParameter> reportParams = oposReportService.GetReportParameters(reportClassInstace);


            if (reportParams.Count > 0)
            {
                using (frmReportParameters frm = new frmReportParameters(reportParams, this.Kernel, posReport, oposReportService, reportParams))
                {

                    DialogResult result = frm.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        try
                        {

                            printerlines = oposReportService.GetStringLinesResult(frm, reportClassInstace, reportParams, appContext, appContext.MainForm);
                        }
                        catch (Exception ex)
                        {
                            Kernel.LogFile.Error(DateTime.Now.ToString() + "CreateReportParameters " + ex.GetFullMessage());
                        }
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        return;
                    }
                    else { return; }
                }





                if (printer == null)
                {
                    throw new POSException(POSClientResources.NO_PRIMARY_PRINTER_FOUND);
                }

                CheckPrinter(printer);


                #region
                /// <summary>
                /// Prints the Report Lines
                /// </summary>

                DialogResult dialogResult = DialogResult.None;
                bool feedAndPaperCut = false;
                do
                {
                    DeviceResult printResult = DeviceResult.SUCCESS;

                    if (printer.ConType == ConnectionType.OPOS && printer.SupportsCutter)
                    {
                        if (feedAndPaperCut == false)
                        {
                            printerlines.Add(Hardware.OPOSDriverCommands.PrinterCommands.OneShotCommands.FeedAndPaperCut());
                            feedAndPaperCut = true;
                        }
                    }
                    try
                    {
                        Kernel.LogFile.Debug(DateTime.Now + " Print Receipt: Start Send Print");
                        printResult = printer.PrintLines(printerlines.ToArray());
                        Kernel.LogFile.Debug(DateTime.Now + " Print Receipt: End Send Print");
                    }
                    catch (Exception ex)
                    {
                        printer.EndTransaction();
                        Kernel.LogFile.Error("ActionCloseDocument: Fail to print" + ex.GetFullMessage());
                    }

                    if (printResult != DeviceResult.SUCCESS)
                    {
                        printer.EndTransaction();
                        string dialogMessage = string.Empty;
                        string dialogMessageType = string.Empty;

                        dialogMessage = dialogMessageType
                             + Environment.NewLine +
                               POSClientResources.PRESS_ENTER_FOR_REPRINT
                             + Environment.NewLine +
                               POSClientResources.PRESS_C_TO_CONTINUE;

                        dialogResult = formManager.ShowFailToPrintMessageBox(dialogMessage);
                    }
                    else
                    {
                        break;
                    }
                } while (dialogResult == DialogResult.OK);

                Kernel.LogFile.Debug(DateTime.Now + " Print Receipt: Start EndTransaction");
                printer.EndTransaction();
                Kernel.LogFile.Debug(DateTime.Now + " Print Receipt: End EndTransaction");

                #endregion
            }


        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.SALE | eMachineStatus.OPENDOCUMENT | eMachineStatus.UNKNOWN; }
        }


        public override eActions ActionCode
        {
            get { return eActions.PRINT_REPORT_TO_OPOS_PRINTER; }
        }

        public override bool RequiresParameters
        {
            get { return true; }
        }


        private void CheckPrinter(Printer printer)
        {
            DeviceResult beginTranResult = printer.BeginTransaction();

            if (beginTranResult != DeviceResult.SUCCESS)
            {
                string message = Resources.POSClientResources.PRINTER_FAILURE + " : " + beginTranResult.ToLocalizedString();
                printer.EndTransaction();
                throw new Exception(message);
            }
        }





    }
}
