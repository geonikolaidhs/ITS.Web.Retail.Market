using DevExpress.Xpo;
using ITS.POS.Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Model.Transactions
{
    public class DocumentPaymentEdps: BaseObj
    {
        public DocumentPaymentEdps()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public DocumentPaymentEdps(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }


        // Fields...
        private decimal _Amount;
        private DocumentHeader _DocumentHeader;
        private Guid _DocumentPayment;
        private ulong _LoyaltyAdjustedAmount;
        private ulong _LoyaltyMasterMerchantPoints;
        private ulong _MerchantPoints;
        private ulong _LoyaltyBalance;
        private ulong _LoyaltyConsumedPoints;
        private ulong _LoyaltyAwardedPoints;
        private string _LoyaltyResponseCode;
        private byte _LoyaltySchemeID;
        private string _EMVCrypto;
        private string _EMVApplicationName;
        private string _EMVApplicationID;
        private string _TRM;
        private string _CardHolder;
        private string _CardProduct;
        private string _PAN;
        private int _OnTopAmount;
        private string _BankID;
        private string _AuthID;
        private string _RRN;
        private string _TimeStamp;
        private ulong _TransactionID;
        private int _BatchNumber;
        private string _ResponseCode;
        private int _ReceiptNumber;
        private string _ErrorCode;

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string ErrorCode
        {
            get
            {
                return _ErrorCode;
            }
            set
            {
                SetPropertyValue("ErrorCode", ref _ErrorCode, value);
            }
        }


        public int ReceiptNumber 
        {
            get
            {
                return _ReceiptNumber;
            }
            set
            {
                SetPropertyValue("ReceiptNumber", ref _ReceiptNumber, value);
            }
        }


        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string ResponseCode 
        {
            get
            {
                return _ResponseCode;
            }
            set
            {
                SetPropertyValue("ResponseCode", ref _ResponseCode, value);
            }
        }


        public int BatchNumber
        {
            get
            {
                return _BatchNumber;
            }
            set
            {
                SetPropertyValue("BatchNumber", ref _BatchNumber, value);
            }
        }


        public ulong TransactionID
        {
            get
            {
                return _TransactionID;
            }
            set
            {
                SetPropertyValue("TransactionID", ref _TransactionID, value);
            }
        }


        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string TimeStamp
        {
            get
            {
                return _TimeStamp;
            }
            set
            {
                SetPropertyValue("TimeStamp", ref _TimeStamp, value);
            }
        }


        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string RRN
        {
            get
            {
                return _RRN;
            }
            set
            {
                SetPropertyValue("RRN", ref _RRN, value);
            }
        }


        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string AuthID
        {
            get
            {
                return _AuthID;
            }
            set
            {
                SetPropertyValue("AuthID", ref _AuthID, value);
            }
        }


        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string BankID
        {
            get
            {
                return _BankID;
            }
            set
            {
                SetPropertyValue("BankID", ref _BankID, value);
            }
        }


        public int OnTopAmount
        {
            get
            {
                return _OnTopAmount;
            }
            set
            {
                SetPropertyValue("OnTopAmount", ref _OnTopAmount, value);
            }
        }


        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string PAN
        {
            get
            {
                return _PAN;
            }
            set
            {
                SetPropertyValue("PAN", ref _PAN, value);
            }
        }


        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string CardProduct
        {
            get
            {
                return _CardProduct;
            }
            set
            {
                SetPropertyValue("CardProduct", ref _CardProduct, value);
            }
        }


        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string CardHolder
        {
            get
            {
                return _CardHolder;
            }
            set
            {
                SetPropertyValue("CardHolder", ref _CardHolder, value);
            }
        }


        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string TRM
        {
            get
            {
                return _TRM;
            }
            set
            {
                SetPropertyValue("TRM", ref _TRM, value);
            }
        }


        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string EMVApplicationID
        {
            get
            {
                return _EMVApplicationID;
            }
            set
            {
                SetPropertyValue("EMVApplicationID", ref _EMVApplicationID, value);
            }
        }


        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string EMVApplicationName
        {
            get
            {
                return _EMVApplicationName;
            }
            set
            {
                SetPropertyValue("EMVApplicationName", ref _EMVApplicationName, value);
            }
        }


        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string EMVCrypto
        {
            get
            {
                return _EMVCrypto;
            }
            set
            {
                SetPropertyValue("EMVCrypto", ref _EMVCrypto, value);
            }
        }


        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public byte LoyaltySchemeID
        {
            get
            {
                return _LoyaltySchemeID;
            }
            set
            {
                SetPropertyValue("LoyaltySchemeID", ref _LoyaltySchemeID, value);
            }
        }


        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string LoyaltyResponseCode
        {
            get
            {
                return _LoyaltyResponseCode;
            }
            set
            {
                SetPropertyValue("LoyaltyResponseCode", ref _LoyaltyResponseCode, value);
            }
        }


        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public ulong LoyaltyAwardedPoints
        {
            get
            {
                return _LoyaltyAwardedPoints;
            }
            set
            {
                SetPropertyValue("LoyaltyAwardedPoints", ref _LoyaltyAwardedPoints, value);
            }
        }


        public ulong LoyaltyConsumedPoints
        {
            get
            {
                return _LoyaltyConsumedPoints;
            }
            set
            {
                SetPropertyValue("LoyaltyConsumedPoints", ref _LoyaltyConsumedPoints, value);
            }
        }



        public ulong LoyaltyBalance
        {
            get
            {
                return _LoyaltyBalance;
            }
            set
            {
                SetPropertyValue("LoyaltyBalance", ref _LoyaltyBalance, value);
            }
        }


        public ulong MerchantPoints
        {
            get
            {
                return _MerchantPoints;
            }
            set
            {
                SetPropertyValue("MerchantPoints", ref _MerchantPoints, value);
            }
        }


        public ulong LoyaltyMasterMerchantPoints
        {
            get
            {
                return _LoyaltyMasterMerchantPoints;
            }
            set
            {
                SetPropertyValue("LoyaltyMasterMerchantPoints", ref _LoyaltyMasterMerchantPoints, value);
            }
        }


        public ulong LoyaltyAdjustedAmount
        {
            get
            {
                return _LoyaltyAdjustedAmount;
            }
            set
            {
                SetPropertyValue("LoyaltyAdjustedAmount", ref _LoyaltyAdjustedAmount, value);
            }
        }
        public Guid DocumentPayment
        {
            get
            {
                return _DocumentPayment;
            }
            set
            {
                SetPropertyValue("DocumentPayment", ref _DocumentPayment, value);
            }
        }

        [Association("DocumentHeader-Edps")]
        public DocumentHeader DocumentHeader
        {
            get
            {
                return _DocumentHeader;
            }
            set
            {
                SetPropertyValue("DocumentHeader", ref _DocumentHeader, value);
            }
        }


        public decimal Amount
        {
            get
            {
                return _Amount;
            }
            set
            {
                SetPropertyValue("Amount", ref _Amount, value);
            }
        }

    }
}
