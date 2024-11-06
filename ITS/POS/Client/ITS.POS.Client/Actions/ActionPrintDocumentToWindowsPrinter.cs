using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Helpers.POSReports;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.Reports;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Linq;

namespace ITS.POS.Client.Actions
{
    public class ActionPrintDocumentToWindowsPrinter : Action
    {
        public ActionPrintDocumentToWindowsPrinter(IPosKernel kernel) : base(kernel)
        {

        }

        public override eActions ActionCode
        {
            get
            {
                return eActions.PRINT_DOCUMENT_TO_WINDOWS_PRINTER;
            }
        }

        public override bool RequiresParameters
        {
            get
            {
                return true;
            }
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get
            {
                return eMachineStatus.OPENDOCUMENT_PAYMENT | eMachineStatus.OPENDOCUMENT | eMachineStatus.DAYSTARTED | eMachineStatus.CLOSED | eMachineStatus.SALE;
            }
        }

        protected override void ExecuteCore(ActionParams parameters, bool dontCheckPermissions)
        {
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            IConfigurationManager configurationManager = Kernel.GetModule<IConfigurationManager>();
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            IFormManager formManager = Kernel.GetModule<IFormManager>();

            ActionPrintDocumentToWindowsPrinterParams actionParameters = (ActionPrintDocumentToWindowsPrinterParams)parameters;

            try
            {
                POSDocumentReportSettings posDocumentReportSettings = configurationManager.DocumentReports.Where(documentReport => documentReport.DocumentTypeOid == actionParameters.DocumentHeader.DocumentType).FirstOrDefault();
                Guid customReportGuid = posDocumentReportSettings.CustomReportOid;
                CustomReport customReport = sessionManager.GetObjectByKey<CustomReport>(customReportGuid);
                Store store = sessionManager.GetObjectByKey<Store>(actionParameters.DocumentHeader.Store);


                DocumentHeaderPrinter documentHeaderPrinter = new DocumentHeaderPrinter();
                ReportPrintResult printResult = documentHeaderPrinter.PrintDocumentHeader(customReport.ReportFile, formManager, sessionManager, actionParameters.DocumentHeader, posDocumentReportSettings.Printer, store.Owner, appContext.CurrentUser.Oid, configurationManager);
                if (printResult.PrintResult == PrintResult.FAILURE && !String.IsNullOrEmpty(printResult.ErrorMessage))
                {
                    throw new POSException(printResult.ErrorMessage);
                }
            }
            catch (POSException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new POSException(exception.GetFullMessage());
            }


        }
    }
}
