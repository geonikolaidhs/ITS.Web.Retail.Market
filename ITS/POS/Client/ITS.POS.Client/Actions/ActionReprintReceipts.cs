using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Forms;
using ITS.POS.Client.Helpers;
using ITS.POS.Hardware;

using ITS.POS.Model.Transactions;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Reprints previous receipts. Promts the user for the receipts' numbers range.
    /// </summary>
    public class ActionReprintReceipts : Action
    {
        public ActionReprintReceipts(IPosKernel kernel) : base(kernel)
        {
        }

        protected override eKeyStatus DefaultKeyStatusRequirement
        {
            get
            {
                return eKeyStatus.POSITION3;
            }
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.CLOSED | eMachineStatus.DAYSTARTED | eMachineStatus.PAUSE | eMachineStatus.SALE; }
        }

        public override eActions ActionCode
        {
            get { return eActions.REPRINT_RECEIPTS; }
        }

        public override bool RequiresParameters
        {
            get { return false; }
        }

        protected override void ExecuteCore(ActionParameters.ActionParams parameters = null, bool dontCheckPermissions = false)
        {
            IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();

            if (config.FiscalMethod == eFiscalMethod.ADHME)
            {
                FiscalPrinter printer = deviceManager.GetPrimaryDevice<FiscalPrinter>();

                if (printer != null)
                {
                    int fromReceiptNumber, toReceiptNumber;
                    DialogResult result = ShowFormAndGetFilter(out fromReceiptNumber, out toReceiptNumber);

                    if (result == DialogResult.OK)
                    {
                        if ((toReceiptNumber - fromReceiptNumber) >= 0)
                        {
                            printer.ReprintReceiptsOfCurrentZ(fromReceiptNumber, toReceiptNumber);
                        }
                        else
                        {
                            throw new POSException(POSClientResources.INVALID_FILTER);
                        }
                    }
                }
                else
                {
                    throw new POSException(POSClientResources.NO_PRIMARY_PRINTER_FOUND);
                }
            }
            else if (config.FiscalMethod == eFiscalMethod.EAFDSS)
            {
                 Printer printer = deviceManager.GetPrimaryDevice<Printer>();

                 if (printer != null)
                 {
                     int fromReceiptNumber, toReceiptNumber;
                     DialogResult result = ShowFormAndGetFilter(out fromReceiptNumber, out toReceiptNumber);

                     if (result == DialogResult.OK)
                     {
                         if ((toReceiptNumber - fromReceiptNumber) >= 0)
                         {
                             XPCollection<DocumentHeader> headers = new XPCollection<DocumentHeader>(sessionManager.GetSession<DocumentHeader>(),
                                                  CriteriaOperator.And(new BinaryOperator("IsOpen", false),
                                                  new BinaryOperator("IsCanceled", false),
                                                  new BinaryOperator("DocumentType", config.DefaultDocumentTypeOid),
                                                  new BinaryOperator("UserDailyTotals.DailyTotals.Oid", appContext.CurrentDailyTotals.Oid),
                                                  new BetweenOperator("DocumentNumber",fromReceiptNumber,toReceiptNumber)));
                             foreach (DocumentHeader header in headers)
                             {
                                 actionManager.GetAction(eActions.PRINT_RECEIPT).Execute(new ActionPrintReceiptParams(printer,true,header),true);
                             }
                         }
                         else
                         {
                             throw new POSException(POSClientResources.INVALID_FILTER);
                         }
                     }
                 }
                 else
                 {
                     throw new POSException(POSClientResources.NO_PRIMARY_PRINTER_FOUND);
                 }
            }
            else
            {
                throw new POSException("UNKNOWN FISCAL METHOD: " + config.FiscalMethod);
            }
        }

        private DialogResult ShowFormAndGetFilter(out int fromReceiptNumber, out int toReceiptNumber)
        {
            using (frmReprintReceipts form = new frmReprintReceipts(this.Kernel))
            {
                DialogResult result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    fromReceiptNumber = form.FromReceiptNumber;
                    toReceiptNumber = form.ToReceiptNumber;
                }
                else
                {
                    fromReceiptNumber = -1;
                    toReceiptNumber = -1;
                }


                return result;
            }
        }
    }
}
