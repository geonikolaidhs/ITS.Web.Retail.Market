using ITS.POS.Client.Actions.ActionParameters;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Client.Kernel;
using ITS.POS.Hardware.Common;
using DevExpress.Xpo;
using ITS.POS.Model.Transactions;
using ITS.POS.Client.Exceptions;
using ITS.POS.Resources;
namespace ITS.POS.Client.Actions
{
    /// <summary>
    ///  Performs an EDPS batch close. For internal use (not directly invoked by the user)
    /// </summary>
    public class ActionEdpsBatchClose : Action
    {
        public ActionEdpsBatchClose(IPosKernel kernel)
            : base(kernel)
        {
            
        }
        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.DAYSTARTED | eMachineStatus.SALE; }
        }

        public override eActions ActionCode
        {
            get { return eActions.EDPS_BATCH_CLOSE; }
        }

        protected override void ExecuteCore(ActionParams parameters, bool dontCheckPermissions)
        {
            IDeviceManager deviceManager = this.Kernel.GetModule<IDeviceManager>();
            ISessionManager sessionManager = this.Kernel.GetModule<ISessionManager>();
             IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();

            EdpsPaymentCreditDevice edpsDevice = deviceManager.Devices.FirstOrDefault(x => x is EdpsPaymentCreditDevice) as EdpsPaymentCreditDevice;
            XPCollection<DocumentPaymentEdps> edpsPayments = new XPCollection<DocumentPaymentEdps>(sessionManager.GetSession<DocumentPaymentEdps>());
            if (edpsPayments.Count > 0)
            {
                ActionEdpsBatchCloseParams param = parameters as ActionEdpsBatchCloseParams;
                if(edpsDevice == null)
                {
                    throw new POSException(POSClientResources.NO_PRIMARY_EDPS_FOUND);
                }                
                EdpsBatchCloseResult result = edpsDevice.BatchClose();
                EdpsBatchTotal total = new EdpsBatchTotal(sessionManager.GetSession<EdpsBatchTotal>())
                {
                    BatchNumber = result.BatchNumber,
                    NumberOfSales = result.NumberOfSales,
                    AmountOfSales = result.AmountSales,
                    NumberOfVoidSales = result.NumberOfVoidSales,
                    AmountOfVoidSales = result.AmountOfVoidSales,
                    NumberOfRefunds = result.NumberOfRefunds,
                    AmountOfRefunds = result.AmountOfRefunds,
                    NumberOfVoidRefunds = result.NumberOfVoidRefunds,
                    AmountOfVoidRefunds = result.AmountOfVoidRefunds,
                    Mismatch = result.bMismatch,
                    UserDailyTotals = param.UserDailyTotals,
                    POS = config.CurrentTerminalOid,
                    Store = config.CurrentStoreOid

                };

                result.Transactions.ForEach(x => new ITS.POS.Model.Transactions.EdpsBatchTransaction(total.Session)
                {
                    EdpsBatchTotal = total,
                    ReceiptNumber = x.ReceiptNumber,
                    TransactionType = x.TransactionType,
                    Amount = x.Amount,
                    TipAmount = x.TipAmount,
                    Installments = x.Installments,
                    PostDating = x.PostDating,
                    InstPart = x.eInstPart,
                    OnTopAmount = x.OnTopAmount,
                    STAN = x.STAN,
                    AuthID = x.AuthID,
                    BankID = x.BankID,
                    TimeStamp = x.TimeStamp,
                    PAN = x.PAN,
                    CardProduct = x.CardProduct,
                    TRM = x.TRM,
                    Cashier = x.Cashier
                });
                total.Save();
                total.Session.CommitTransaction();
            }
        }

        public override bool RequiresParameters
        {
            get { return true; }
        }
        
    }
}
