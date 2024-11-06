using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    public class DocumentPaymentCardlink : BaseObj
    {
        public DocumentPaymentCardlink()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public DocumentPaymentCardlink(Session session)
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

        private string _SessionId;
        private string _MsgType;
        private string _MsgCode;
        private string _RespCode;
        private string _RespMesg;
        private string _CardType;
        private string _AccNum;
        private string _RefNum;
        private string _AuthCode;
        private string _BatchNum;
        private int _Amount;
        private string _MsgOpt;
        private string _tipAmount;
        private string _ForeignAmount;
        private string _ForeignCurrencyCode;
        private string _ExchangeRateInclMarkup;
        private string _DccMarkupPercentage;
        private string _DccExchangeDateOfRate;
        private string _EftTid;
        private DocumentHeader _DocumentHeader;
        private Guid _DocumentPayment;

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string SessionId
        {
            get
            {
                return _SessionId;
            }
            set
            {
                SetPropertyValue("SessionId", ref _SessionId, value);
            }
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string MsgType
        {
            get
            {
                return _MsgType;
            }
            set
            {
                SetPropertyValue("MsgType", ref _MsgType, value);
            }
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string MsgCode
        {
            get
            {
                return _MsgCode;
            }
            set
            {
                SetPropertyValue("MsgCode", ref _MsgCode, value);
            }
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string RespCode
        {
            get
            {
                return _RespCode;
            }
            set
            {
                SetPropertyValue("RespCode", ref _RespCode, value);
            }
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string RespMesg
        {
            get
            {
                return _RespMesg;
            }
            set
            {
                SetPropertyValue("RespMesg", ref _RespMesg, value);
            }
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string CardType
        {
            get
            {
                return _CardType;
            }
            set
            {
                SetPropertyValue("CardType", ref _CardType, value);
            }
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string AccNum
        {
            get
            {
                return _AccNum;
            }
            set
            {
                SetPropertyValue("AccNum", ref _AccNum, value);
            }
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string RefNum
        {
            get
            {
                return _RefNum;
            }
            set
            {
                SetPropertyValue("RefNum", ref _RefNum, value);
            }
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string AuthCode
        {
            get
            {
                return _AuthCode;
            }
            set
            {
                SetPropertyValue("AuthCode", ref _AuthCode, value);
            }
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string BatchNum
        {
            get
            {
                return _BatchNum;
            }
            set
            {
                SetPropertyValue("BatchNum", ref _BatchNum, value);
            }
        }


        public int Amount
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

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string MsgOpt
        {
            get
            {
                return _MsgOpt;
            }
            set
            {
                SetPropertyValue("MsgOpt", ref _MsgOpt, value);
            }
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string tipAmount
        {
            get
            {
                return _tipAmount;
            }
            set
            {
                SetPropertyValue("tipAmount", ref _tipAmount, value);
            }
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string ForeignAmount
        {
            get
            {
                return _ForeignAmount;
            }
            set
            {
                SetPropertyValue("ForeignAmount", ref _ForeignAmount, value);
            }
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string ForeignCurrencyCode
        {
            get
            {
                return _ForeignCurrencyCode;
            }
            set
            {
                SetPropertyValue("ForeignCurrencyCode", ref _ForeignCurrencyCode, value);
            }
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string ExchangeRateInclMarkup
        {
            get
            {
                return _ExchangeRateInclMarkup;
            }
            set
            {
                SetPropertyValue("ExchangeRateInclMarkup", ref _ExchangeRateInclMarkup, value);
            }
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string DccMarkupPercentage
        {
            get
            {
                return _DccMarkupPercentage;
            }
            set
            {
                SetPropertyValue("DccMarkupPercentage", ref _DccMarkupPercentage, value);
            }
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string DccExchangeDateOfRate
        {
            get
            {
                return _DccExchangeDateOfRate;
            }
            set
            {
                SetPropertyValue("DccExchangeDateOfRate", ref _DccExchangeDateOfRate, value);
            }
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string EftTid
        {
            get
            {
                return _EftTid;
            }
            set
            {
                SetPropertyValue("EftTid", ref _EftTid, value);
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

        [Association("DocumentHeader-Cardlink")]
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
    }
}
