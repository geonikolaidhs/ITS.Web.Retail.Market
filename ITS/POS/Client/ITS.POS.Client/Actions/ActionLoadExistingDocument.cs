using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Forms;
using ITS.POS.Client.Kernel;
using ITS.POS.Hardware;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
using ITS.POS.Model.Transactions;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Linq;


namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Used at application startup. Finds and loads any existing open document. 
    /// </summary>
    public class ActionLoadExistingDocument : Action
    {

        public ActionLoadExistingDocument(IPosKernel kernel) : base(kernel)
        {

        }

        public override eActions ActionCode
        {
            get { return eActions.LOAD_EXISTING_DOCUMENT; }
        }

        public override bool RequiresParameters
        {
            get { return false; }
        }

        protected override eKeyStatus DefaultKeyStatusRequirement
        {
            get
            {
                return eKeyStatus.UNKNOWN;
            }
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.UNKNOWN | eMachineStatus.SALE | eMachineStatus.PAUSE | eMachineStatus.OPENDOCUMENT_PAYMENT | eMachineStatus.OPENDOCUMENT | eMachineStatus.DAYSTARTED | eMachineStatus.CLOSED; }
        }

        protected override void ExecuteCore(ActionParameters.ActionParams parameters = null, bool dontCheckPermissions = false)
        {
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();

            DocumentHeader header = sessionManager.FindObject<DocumentHeader>(
                CriteriaOperator.And(new BinaryOperator("IsOpen", true),
                                     new BinaryOperator("DocumentOnHold", false),
                                     new BinaryOperator("IsFiscalPrinterHandled", false),
                                     new BinaryOperator("IsCanceled", false)));

            if (header != null)
            {
                if (appContext.GetMachineStatus() == eMachineStatus.SALE)
                {

                    appContext.CurrentDocument = header;
                    appContext.CurrentDocumentLine = header.DocumentDetails.Where(x => x.IsCanceled == false).LastOrDefault();
                    appContext.CurrentCustomer = sessionManager.GetObjectByKey<Customer>(header.Customer);

                    if (appContext.CurrentDocument.DocumentPayments.Count > 0)
                    {
                        appContext.SetMachineStatus(eMachineStatus.OPENDOCUMENT_PAYMENT);
                    }
                    else
                    {
                        appContext.SetMachineStatus(eMachineStatus.OPENDOCUMENT);
                    }

                    DocumentType documentType = sessionManager.GetObjectByKey<DocumentType>(header.DocumentType);
                    if (config.FiscalMethod == eFiscalMethod.ADHME && header.DocumentType == config.DefaultDocumentTypeOid)
                    //check if printer has printed the receipt. This can happen if the fiscal printer gets the commands to print 
                    //but the application shuts down unexpectedly before it gets the response from the printer.
                    {
                        int fiscalPrinterReceipts = -1;


                        IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();

                        CriteriaOperator filter =
                        CriteriaOperator.And(new BinaryOperator("IsOpen", false),
                                 new BinaryOperator("DocumentType", config.DefaultDocumentTypeOid),
                                 new BinaryOperator("IsFiscalPrinterHandled", true),
                                 new BinaryOperator("UserDailyTotals.DailyTotals.Oid", appContext.CurrentDailyTotals.Oid));

                        int applicationReceipts = new XPCollection<DocumentHeader>(sessionManager.GetSession<DocumentHeader>(), filter).Count;

                        FiscalPrinter printer = deviceManager.GetPrimaryDevice<FiscalPrinter>();
                        if (printer == null)
                        {
                            throw new POSException(POSClientResources.NO_PRIMARY_PRINTER_FOUND);
                        }

                        printer.GetCurrentDayReceiptsCount(out fiscalPrinterReceipts);


                        if (fiscalPrinterReceipts >= 0)
                        {
                            int difference = fiscalPrinterReceipts - applicationReceipts;

                            if (difference == 1)  //Must close open document, fiscal printer already printed it
                            {
                                actionManager.GetAction(eActions.CLOSE_DOCUMENT).Execute(new ActionCloseDocumentParams(true, true), true);
                            }
                            else if (difference > 1 || difference < 0)
                            {
                                frmMessageBox form = formManager.CreateMessageBox(String.Format(POSClientResources.FISCAL_PRINTER_RECEIPTS_COUNT_MISSMATCH, applicationReceipts, fiscalPrinterReceipts));
                                form.btnCancel.Visible = false;
                                form.btnRetry.Visible = false;
                                form.Show();
                            }
                        }
                    }

                    actionManager.GetAction(eActions.PUBLISH_DOCUMENT_INFO).Execute(new ActionPublishDocumentInfoParams(header));
                    if (appContext.GetMachineStatus() == eMachineStatus.OPENDOCUMENT_PAYMENT)
                    {
                        DocumentPayment payment = header.DocumentPayments.LastOrDefault();
                        if (payment != null)
                        {
                            appContext.CurrentDocumentPayment = payment;
                            actionManager.GetAction(eActions.PUBLISH_DOCUMENT_PAYMENT_INFO).Execute(new ActionPublishDocumentPaymentInfoParams(payment, header, true, false));
                        }
                    }
                }
                else
                {
                    throw new Exception(POSClientResources.ERROR_LOADING_DOCUMENT);
                }
            }
        }

    }
}
