using ITS.POS.Model.Master;
using System;
using ITS.POS.Hardware;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using DevExpress.Data.Filtering;
using ITS.POS.Client.Exceptions;
using ITS.POS.Hardware.Common;


namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Starts a new document. For internal use (not directly invoked by the user)
    /// </summary>
    public class ActionStartNewDocument : Action
    {

        public ActionStartNewDocument(IPosKernel kernel) : base(kernel)
        {

        }

        public override eActions ActionCode
        {
            get { return eActions.START_NEW_DOCUMENT; }
        }

        public override bool RequiresParameters
        {
            get { return true; }
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.SALE; }
        }

        public override bool NeedsDrawerClosed
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Creates a new DocumentHeader assigning it to Globals.CurrentDocument and sets the Globals.Status to eMachineStatus.OPENDOCUMENT.
        /// Afterwards the Observers of this action are notified with the Globals.CurrentDocument.
        /// </summary>
        protected override void ExecuteCore(ActionParams parameters = null, bool dontCheckPermissions = false)
        {
            IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            ITotalizersService totalizersService = Kernel.GetModule<ITotalizersService>();
            ICustomerService customerService = Kernel.GetModule<ICustomerService>();
            IDocumentService documentService = Kernel.GetModule<IDocumentService>();
            IVatFactorService vatFactorService = Kernel.GetModule<IVatFactorService>();
            IFormManager formManager = Kernel.GetModule<IFormManager>();

            if (!deviceManager.HasDemoModeBeenSetupCorrectly(config.DemoMode, config.FiscalMethod, config.FiscalDevice))
            {
                throw new Exception(POSClientResources.DEMO_MODE_ERROR);
            }

            ActionStartNewDocumentParams castedParams = parameters as ActionStartNewDocumentParams;
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            sessionManager.ClearMasterSession();
            sessionManager.ClearSettingsSession();
            string message;
            if (!config.CheckIfSettingsAreValid(out message))
            {
                throw new POSException(POSClientResources.INCORRECT_SETTINGS + " " + message);
            }
            if (appContext.GetMachineStatus() != eMachineStatus.SALE)
            {
                throw new POSException(POSClientResources.NOT_IN_SALE_MODE);
            }

            if (totalizersService.CheckIfMustIssueZ(appContext.CurrentDailyTotals))
            {
                string errorMessage = String.Format(POSClientResources.YOU_MUST_ISSUE_Z, appContext.CurrentDailyTotals.FiscalDate);
                formManager.ShowCancelOnlyMessageBox(errorMessage);
                throw new POSException(errorMessage);
            }

            Customer documentCustomer = null;
            if (appContext.CurrentCustomer != null)
            {
                documentCustomer = appContext.CurrentCustomer;
            }
            else
            {
                documentCustomer = sessionManager.GetObjectByKey<Customer>(config.DefaultCustomerOid);
            }

            //PriceCatalog pc = customerService.GetPriceCatalog(documentCustomer.Oid, config.CurrentStoreOid);
            PriceCatalogPolicy priceCatalogPolicy = customerService.GetPriceCatalogPolicy(documentCustomer.Oid, config.CurrentStoreOid);

            if (priceCatalogPolicy == null)
            {
                throw new Exception(POSClientResources.STORE_HAS_NO_DEFAULT_PRICECATALOG);
            }

            Device primaryPrinter = null;
            if (config.FiscalMethod == eFiscalMethod.EAFDSS)
            {
                primaryPrinter = deviceManager.GetPrimaryDevice<Printer>();
            }
            else if (config.FiscalMethod == eFiscalMethod.ADHME)
            {
                primaryPrinter = deviceManager.GetPrimaryDevice<FiscalPrinter>();
            }

            if (primaryPrinter == null)
            {
                throw new Exception(POSClientResources.NO_PRIMARY_PRINTER_FOUND);
            }

            if (config.FiscalMethod == eFiscalMethod.ADHME)
            {
                bool? isVatAValid, isVatBValid, isVatCValid, isVatDValid, isVatEValid;
                vatFactorService.CheckIfVatFactorsAreValid((FiscalPrinter)primaryPrinter, out isVatAValid, out  isVatBValid, out isVatCValid, out isVatDValid, out isVatEValid);
                if (isVatAValid == false ||
                   isVatBValid == false ||
                   isVatCValid == false ||
                   isVatDValid == false ||
                   isVatEValid == false)
                {
                    double fiscalVatRateA, fiscalVatRateB, fiscalVatRateC, fiscalVatRateD, fiscalVatRateE;
                    ((FiscalPrinter)primaryPrinter).ReadVatRates(out fiscalVatRateA, out fiscalVatRateB, out fiscalVatRateC, out fiscalVatRateD, out  fiscalVatRateE);
                    message = (POSClientResources.FISCAL_VAT_FACTORS_ARE_INVALID + Environment.NewLine +
                                            "(" + POSClientResources.FISCAL_PRINTER_VAT_FACTORS + ") " +
                                            "A:" + fiscalVatRateA + " " +
                                            "B:" + fiscalVatRateB + " " +
                                            "C:" + fiscalVatRateC + " " +
                                            "D:" + fiscalVatRateD + " " +
                                            "E:" + fiscalVatRateE);
                    formManager.ShowCancelOnlyMessageBox(message);
                    throw new POSException(message);
                }

            }

            if (appContext.CurrentDocument == null)
            {

                appContext.CurrentDocument = documentService.CreateDocumentHeader(eDivision.Sales, config.CurrentTerminalOid,config.DefaultDocumentTypeOid, config.DefaultDocumentSeriesOid, 
                                                                                config.CurrentStoreOid, documentCustomer.Oid, documentCustomer.Code, documentCustomer.CompanyName, priceCatalogPolicy.Oid,
                                                                                config.DefaultDocumentStatusOid, appContext.CurrentUserDailyTotals.Oid);

                appContext.CurrentDocument.DocumentDetails.Filter = new BinaryOperator("IsCanceled", false);
                appContext.CurrentDocument.IsOpen = true;
                appContext.CurrentDocument.CreatedBy = appContext.CurrentUser == null ? Guid.Empty : appContext.CurrentUser.Oid;
                appContext.CurrentDocument.Save();

                ////case 5801 Open receipt at start document
                if (primaryPrinter is FiscalPrinter && castedParams.OpenFiscalPrinterReceipt)
                {
                    FiscalPrinter printer = primaryPrinter as FiscalPrinter;
                    printer.ReadDeviceStatus();

                    try
                    {
                        printer.CancelLegalReceipt(); //if a receipt is already open, then some error has occured
                    }
                    catch
                    {

                    }

                    printer.OpenLegalReceipt();

                }
            }
            appContext.SetMachineStatus(eMachineStatus.OPENDOCUMENT);
            //GlobalContext.GetAction(eActions.PUBLISH_DOCUMENT_INFO).Execute(new ActionPublishDocumentInfoParams(GlobalContext.CurrentDocument));
        }
    }
}
