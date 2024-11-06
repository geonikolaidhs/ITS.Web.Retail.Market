using ITS.POS.Model.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Hardware.Common
{
    public class CardlinkBatchCloseResult
    {
        public CardlinkBatchCloseResult()
        {
            Transactions = new List<CardlinkBatchTransaction>();
        }

        public CardlinkBatchCloseResult(StringBuilder sessionId, StringBuilder msgType, StringBuilder msgCode, StringBuilder respCode, StringBuilder respMsg, StringBuilder eftId
                                        , StringBuilder batchNum, StringBuilder batchNetAmount, StringBuilder batchOrigAmount, StringBuilder batchTotalCounter, StringBuilder batchTotal,
                                        StringBuilder batchNumResp, StringBuilder batchNetAmountResp, StringBuilder batchTotalsResp, StringBuilder batchTotalCounterResp, StringBuilder batchOrigAmountResp)
        {
            SessionId = sessionId.ToString();
            MsgType = msgType.ToString();
            MsgCode = msgCode.ToString();
            RespCode = respCode.ToString();
            RespMesg = respMsg.ToString();
            EftTid = eftId.ToString();
            BatchNum = batchNum.ToString();
            BatchNetAmmount = batchNetAmount.ToString();
            BatchOrigAmount = batchOrigAmount.ToString();
            BatchTotalCounter = batchTotalCounter.ToString();
            BatchTotal = batchTotal.ToString();
            BatchNumResp = batchNumResp.ToString();
            BatchNetAmountResp = batchNetAmountResp.ToString();
            BatchTotalsResp = batchTotalsResp.ToString();
            BatchTotalCounterResp = batchTotalCounterResp.ToString();
            BatchOrigAmountResp = batchOrigAmountResp.ToString();
        }
        public List<CardlinkBatchTransaction> Transactions { get; private set; }
        public string SessionId { get; private set; }
        public string MsgType { get; private set; }
        public string MsgCode { get; private set; }
        public string RespCode { get; private set; }
        public string RespMesg { get; private set; }
        public string EftTid { get; private set; }
        public string BatchNum { get; private set; }
        public string BatchNetAmmount { get; private set; }
        public string BatchOrigAmount { get; private set; }
        public string BatchTotalCounter { get; private set; }
        public string BatchTotal { get; private set; }

        public string BatchNumResp { get; private set; }
        public string BatchNetAmountResp { get; private set; }
        public string BatchTotalsResp { get; private set; }
        public string BatchTotalCounterResp { get; private set; }
        public string BatchOrigAmountResp { get; private set; }


    }

    public class CardlinkBatchTransaction
    {
        public CardlinkBatchTransaction()
        {

        }

        public CardlinkBatchTransaction(StringBuilder acqId, StringBuilder issId, StringBuilder issName, StringBuilder transactionQty, StringBuilder transactionAmount)
        {
            AcqId = acqId.ToString();
            IssId = issId.ToString();
            IssName = issName.ToString();
            TransactionAmount = transactionAmount.ToString();
            TransactionQty = transactionQty.ToString();
        }
        public string AcqId { get; private set; }
        public string IssId { get; private set; }
        public string IssName { get; private set; }
        public string TransactionQty { get; private set; }
        public string TransactionAmount { get; private set; }


    }

}

