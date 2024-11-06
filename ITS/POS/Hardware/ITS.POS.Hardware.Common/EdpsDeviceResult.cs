using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Hardware.Common
{

    public class EdpsEMVData
    {
        public EdpsEMVData()
        {

        }
        public EdpsEMVData(tagEMV_DATA loyaltyData)
        {
            if (loyaltyData.bEmvUsed)
            {
                this.ApplicationID = loyaltyData.szAppID;
                this.ApplicationName = loyaltyData.szAppName;
                this.Crypto = loyaltyData.szCrypto;
            }
        }
        public string ApplicationID { get; set; }
        public string ApplicationName { get; set; }
        public string Crypto { get; set; }
    }

    public class EdpsLoyaltyData
    {
        public EdpsLoyaltyData()
        {

        }
        public EdpsLoyaltyData(tagLOY_DATA loyaltyData)
        {
            if (loyaltyData.bLoyUsed)
            {
                this.LoyaltySchemeID = loyaltyData.schemeID;
                this.LoyaltyResponseCode = loyaltyData.szLoyRespCode;
                this.AwardedPoints = loyaltyData.awardedPoints;
                this.ConsumedPoints = loyaltyData.consumedPoints;
                this.LoyaltyBalance = loyaltyData.loyBalance;
                this.MerchantPoints = loyaltyData.merchantPoints;
                this.MasterMerchantPoints = loyaltyData.masterMerchantPoints;
                this.AdjustedAmount = loyaltyData.adjustedAmount;
            }
        }
        public byte LoyaltySchemeID { get; set; }
        public string LoyaltyResponseCode { get; set; }
        public ulong AwardedPoints { get; set; }
        public ulong ConsumedPoints { get; set; }
        public ulong LoyaltyBalance { get; set; }
        public ulong MerchantPoints { get; set; }
        public ulong MasterMerchantPoints { get; set; }
        public ulong AdjustedAmount { get; set; }
    }
    public class EdpsDeviceResult
    {
        public EdpsDeviceResult()
        {

        }

        public EdpsDeviceResult(tagTXN_RESPONSE response)
        {
            this.ResponseCode = response.szRespCode;
            this.ReceiptNumber = response.iReceiptNo;
            this.BatchNumber = response.iBatchNo;
            this.TransactionID = response.llTxnID;
            this.TimeStamp = response.szTimeStamp;
            this.RRN = response.szRRN;
            this.BankID = response.szBankID;
            this.OnTopAmount = response.iOnTopAmount;

            if (response.szRespCode == EdpsLink.RC_OK)
            {
                ErrorCode = EdpsLink.RC_OK;
                AuthID = response.szAuthID;
                PAN = response.szPAN;
                CardHolder = response.szCardHolder;
                CardProduct = response.szCardProduct;
                TRM = response.szTRM;
                if (response.strEMV_Data.bEmvUsed)
                {
                    EdpsEMVData = new EdpsEMVData(response.strEMV_Data);
                }
                if (response.strLOY_Data.bLoyUsed)
                {
                    EdpsLoyaltyData = new EdpsLoyaltyData(response.strLOY_Data);
                }
            }
            else
            {
                if (EdpsLink.ErrorCodes.ContainsKey(response.szRespCode))
                {
                    ErrorCode = EdpsLink.ErrorCodes[response.szRespCode];
                }
                else
                {
                    ErrorCode = Resources.POSClientResources.ERROR + ": " + response.szRespCode;
                }
            }
        }

        public string ErrorCode { get; set; }
        public int ReceiptNumber { get; set; }
        //public TXN_TYPE eTxnType { get; set; }        
        public string ResponseCode { get; set; }
        public int BatchNumber { get; set; }
        public ulong TransactionID { get; set; }
        public string TimeStamp { get; set; }
        public string RRN { get; set; }
        public string AuthID { get; set; }
        public string BankID { get; set; }
        public int OnTopAmount { get; set; }
        public string PAN { get; set; }
        public string CardProduct { get; set; }
        public string CardHolder { get; set; }

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
        public EdpsEMVData EdpsEMVData { get; set; }
        public EdpsLoyaltyData EdpsLoyaltyData { get; set; }
    }
}
