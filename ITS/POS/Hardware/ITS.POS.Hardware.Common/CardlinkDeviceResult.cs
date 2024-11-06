using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Hardware.Common
{

    public class CardlinkDeviceResult
    {

        public CardlinkDeviceResult(StringBuilder sessionId, StringBuilder msgType, StringBuilder msgCode,
                                 StringBuilder uniqueTxnId, StringBuilder amount, int amountLen, StringBuilder msgOpt, StringBuilder roomNumber,
                                 StringBuilder checkinDate, StringBuilder checkoutDate, StringBuilder agreementNum, StringBuilder agreementDate,
                                 StringBuilder cashierNum, StringBuilder tillNum, StringBuilder invoiceNum, StringBuilder invoiceNumCum,
                                 StringBuilder sessionIdResp, StringBuilder msgTypeResp, StringBuilder msgCodeResp, StringBuilder respCodeResp,
                                 StringBuilder respMessageResp, StringBuilder cardTypeResp, StringBuilder accNumResp, StringBuilder NumResp,
                                 StringBuilder authCodeResp, StringBuilder batchNumResp, StringBuilder amountResp, StringBuilder msgOptResp,
                                 StringBuilder tipAmountResp, StringBuilder foreignAmountResp, StringBuilder foreignCurrencyCodeResp, StringBuilder exchangeRateInclMarkupResp,
                                 StringBuilder dccMarkupPercentageResp, StringBuilder dccExchangeDateOfRatetResp, StringBuilder bankId,
                                 StringBuilder aquirerName, StringBuilder numOfInstallmentsResp, StringBuilder numOfPostdateInstallments,
                                 StringBuilder earned, StringBuilder balance, StringBuilder discount, StringBuilder redemption,
                                 StringBuilder bonusPoolBlock, StringBuilder eftTidResp, StringBuilder mid, StringBuilder sn, StringBuilder merchantName,
                                 StringBuilder address, StringBuilder city, StringBuilder phone, StringBuilder applicationName, StringBuilder cardExpiryDate, StringBuilder posApplicationVersion,
                                 StringBuilder paymentSpecs, StringBuilder AID, StringBuilder TC, StringBuilder transactionType)
        {

            SessionId = sessionIdResp.ToString();
            MsgType = msgTypeResp.ToString();
            MsgCode = msgCodeResp.ToString();
            RespCode = respCodeResp.ToString();
            RespMesg = respMessageResp.ToString();
            CardType = cardTypeResp.ToString();
            AccNum = accNumResp.ToString();
            RefNum = NumResp.ToString();
            AuthCode = authCodeResp.ToString();
            BatchNum = batchNumResp.ToString();
            Amount = amountResp.ToString();
            MsgOpt = msgOptResp.ToString();
            tipAmount = tipAmountResp.ToString();
            ForeignAmount = foreignAmountResp.ToString();
            ForeignCurrencyCode = foreignCurrencyCodeResp.ToString();
            ExchangeRateInclMarkup = exchangeRateInclMarkupResp.ToString();
            DccMarkupPercentage = dccMarkupPercentageResp.ToString();
            DccExchangeDateOfRate = dccExchangeDateOfRatetResp.ToString();
            EftTid = eftTidResp.ToString();
            BankId = bankId.ToString();
            SetPosMessage(RespCode);
            int receipt;
            Int32.TryParse(invoiceNum.ToString(), out receipt);
            ReceiptNumber = receipt;

        }


        private void SetPosMessage(string RespCode)
        {
            foreach (KeyValuePair<string, string> error in ErrorCodes)
            {
                if (RespCode.ToString() == "UN" && (string.IsNullOrEmpty(BankId) || string.IsNullOrWhiteSpace(BankId)))
                {
                    PosMessage = "User Cancelled";
                    return;
                }
                if (RespCode == error.Key)
                {
                    PosMessage = error.Value;
                    return;
                }
            }
        }

        public int ReceiptNumber { get; set; }
        public string SessionId { get; set; }
        public string MsgType { get; set; }
        public string MsgCode { get; set; }
        public string RespCode { get; set; }
        public string RespMesg { get; set; }
        public string CardType { get; set; }
        public string AccNum { get; set; }
        public string RefNum { get; set; }
        public string AuthCode { get; set; }
        public string BatchNum { get; set; }
        public string Amount { get; set; }
        public string MsgOpt { get; set; }
        public string tipAmount { get; set; }
        public string ForeignAmount { get; set; }
        public string ForeignCurrencyCode { get; set; }
        public string ExchangeRateInclMarkup { get; set; }
        public string DccMarkupPercentage { get; set; }
        public string DccExchangeDateOfRate { get; set; }
        public string EftTid { get; set; }
        public string PosMessage { get; set; }
        public string BankId { get; set; }



        public const string OK = "00";
        public static Dictionary<string, string> ErrorCodes = new Dictionary<string, string>()
        {
            { "00","Approved" },
            { "51","Declined By Host" },
            { "DC","Declined by Card (EMV)" },
            { "UD","Unsupported Card" },
            { "UC","User Cancelled" },
            { "LC","Lost Carrier (Communication Error)" },
            { "TO","Time Out (Communication Error)" },
            { "CE","Communication Error (Other)" },
            { "ND","Not Delivered" },
            { "NA","Host Not Available" },
            { "IM","Invalid MAC received from host" },
            { "JF","Journal Full – batch is need to be settled" },
            { "UN","Error – Wrong Transaction" },
            { "IS","Invalid Sequence Number" },
            { "ID","Invalid Transaction Data (Void)" },
            { "IB","Invalid Batch Number" },
            { "EC","Expired Card" },
            { "WC","Wrong Card (Invalid card used for Void)" },
            { "VT","Transaction already voided (Void)" },
            { "XC","Transaction is approved however EMV Fail appears on 2nd GenAC, auto-reversal is generated (EMV)" },
            { "RC","Error occurred during chip card reading or card was removed before whole chip checking performed." },
            { "CL","Connection Lost. Sends when DLL received at least one hold on from POS but after timeout no other message has been received from POS" },
            { "Y1","Offline approved. Special response code for the interaction with the chip card." },
            { "Y2","Approval (after card-initiated referral). Special response code for the interaction with the chip card." },
            { "Y3","Unable to go online, offline approved. Special response code for the interaction with the chip card." },
            { "Z1","Offline declined. Special response code for the interaction with the chip card." },
            { "Z2","Decline (after card-initiated referral). Special response code for the interaction with the chip card." },
            { "Z3","Unable to go online, offline declined. Special response code for the interaction with the chip card." },
            { "EA","Unknown authorization code" },
            { "NT","No Transaction found in journal to provide transaction details code" }
            };

        /// <summary>
        /// Contains extra info about the transaction
        /// The first character describes of the following:
        ///'Τ' -> Typed
        ///'C' -> Chip
        ///'D' -> Magnetic Card Reader
        ///'P' -> Contactless - EMV
        ///'p' -> Contactless - MSR
        ///
        /// The second character describes the Customer Verification Method - CVM)
        ///'@' -> Customer Signature
        ///'P' -> Online PIN
        ///'p' -> Offline PIN
        ///' ' -> Failure (not verified)
        ///'-' -> No CVM
        ///
        /// The third character describes the transaction approval method
        ///'1' -> online approval
        ///'3' -> Approval from the terminal
        ///'5' -> Approval by phone
        /// </summary>
        public string TRM { get; set; }
        //public CardlinkLoyaltyData CardlinkLoyaltyData { get; set; }
    }
}
