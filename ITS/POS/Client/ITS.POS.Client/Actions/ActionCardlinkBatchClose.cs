using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Hardware.Common;
using ITS.POS.Model.Transactions;
using DevExpress.Xpo;
using ITS.POS.Client.Exceptions;
using ITS.POS.Resources;

namespace ITS.POS.Client.Actions
{
    public class ActionCardlinkBatchClose : Action
    {
        public ActionCardlinkBatchClose(IPosKernel kernel) : base(kernel)
        {
        }

        public override eActions ActionCode
        {
            get { return eActions.CARDLINK_BATCH_CLOSE; }
        }

        public override bool RequiresParameters
        {
            get { return false; }
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get
            {
                return eMachineStatus.DAYSTARTED | eMachineStatus.SALE;
            }
        }

        protected override void ExecuteCore(ActionParams parameters, bool dontCheckPermissions)
        {
            IDeviceManager deviceManager = this.Kernel.GetModule<IDeviceManager>();
            ISessionManager sessionManager = this.Kernel.GetModule<ISessionManager>();
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            ITotalizersService totalizerService = Kernel.GetModule<ITotalizersService>();
            IAppContext appContext = Kernel.GetModule<IAppContext>();

            CardlinkPaymentCreditDevice cardlinkDevice = deviceManager.Devices.FirstOrDefault(x => x is CardlinkPaymentCreditDevice) as CardlinkPaymentCreditDevice;
            XPCollection<DocumentPaymentCardlink> cardlinkPayments = new XPCollection<DocumentPaymentCardlink>(sessionManager.GetSession<DocumentPaymentCardlink>());
            string lastBatchPaymentNumber = new XPCollection<DocumentPaymentCardlink>(sessionManager.GetSession<DocumentPaymentCardlink>())?.ToList()?.OrderByDescending(x => x.BatchNum).FirstOrDefault()?.BatchNum ?? null;
            if (cardlinkPayments.Count > 0 && !string.IsNullOrEmpty(lastBatchPaymentNumber))
            {
                string lastBatch = new XPCollection<CardlinkBatchTotal>(sessionManager.GetSession<CardlinkBatchTotal>())?.ToList()?.OrderByDescending(x => x.BatchNum).FirstOrDefault()?.BatchNum ?? null;
                int lastBatchInt;
                Int32.TryParse(lastBatch, out lastBatchInt);

                int lastBatchPaymentNumberInt;
                Int32.TryParse(lastBatchPaymentNumber, out lastBatchPaymentNumberInt);

                if (lastBatchPaymentNumberInt > lastBatchInt || string.IsNullOrEmpty(lastBatch))
                {
                    ActionCardlinkBatchCloseParams param = parameters as ActionCardlinkBatchCloseParams;
                    if (cardlinkDevice == null)
                    {
                        throw new POSException(POSClientResources.NO_PRIMARY_CARDLINK_FOUND);
                    }

                    CardlinkBatchCloseResult result = CardlinkLink.BatchClose(cardlinkDevice.Settings.Ethernet, lastBatchPaymentNumber);
                    if (result != null)
                    {
                        int salesCounter;
                        Int32.TryParse(result.BatchTotalCounter, out salesCounter);
                        decimal salesamount;
                        decimal.TryParse(result.BatchOrigAmountResp, out salesamount);

                        decimal netamount;
                        decimal.TryParse(result.BatchNetAmountResp, out netamount);
                        CardlinkBatchTotal total = new CardlinkBatchTotal(sessionManager.GetSession<CardlinkBatchTotal>());
                        total.UserDailyTotals = param?.UserDailyTotals ?? appContext.CurrentUserDailyTotals.Oid;
                        total.POS = config.CurrentTerminalOid;
                        total.POSDateTime = DateTime.Now;
                        total.BatchNum = result?.BatchNum;
                        try
                        {
                            total.NumberOfSales = salesCounter;
                            total.BatchNetAmmount = (netamount / 100).ToString();
                            total.BatchOrigAmount = result?.BatchOrigAmountResp;
                            total.BatchTotalCounter = result?.BatchTotalCounter;
                            total.AmountOfSales = salesamount / 100;
                            total.BatchTotal = result?.BatchTotal;
                        }
                        catch (Exception ex) { }

                        result?.Transactions?.ForEach(x => new ITS.POS.Model.Transactions.CardlinkBatchTransaction(total.Session)
                        {
                            TransactionAmount = x.TransactionAmount,
                            TransactionQty = x.TransactionQty
                        });
                        total.Save();
                        total.Session.CommitTransaction();
                    }
                }
            }
        }
    }
}
