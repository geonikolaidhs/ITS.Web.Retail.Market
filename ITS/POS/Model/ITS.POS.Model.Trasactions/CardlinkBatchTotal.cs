using DevExpress.Xpo;
using ITS.POS.Model.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Model.Transactions
{
    public class CardlinkBatchTotal : BaseObj
    {
        public CardlinkBatchTotal() : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public CardlinkBatchTotal(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        // Fields...
        private Guid _UserDailyTotals;
        private DateTime _POSDateTime;
        private Guid _Store;
        private Guid _POS;
        private string _SessionId;
        private string _MsgType;
        private string _MsgCode;
        private string _RespCode;
        private string _RespMesg;
        private string _EftTid;
        private string _BatchNum;
        private string _BatchNetAmmount;
        private string _BatchOrigAmount;
        private string _BatchTotalCounter;
        private string _BatchTotal;
        private int _NumberOfSales;
        private decimal _AmountOfSales;
        private int _NumberOfVoidSales;
        private decimal _AmountOfVoidSales;



        public int NumberOfSales
        {
            get
            {
                return _NumberOfSales;
            }
            set
            {
                SetPropertyValue("NumberOfSales", ref _NumberOfSales, value);
            }
        }

        public int NumberOfVoidSales
        {
            get
            {
                return _NumberOfVoidSales;
            }
            set
            {
                SetPropertyValue("NumberOfVoidSales", ref _NumberOfVoidSales, value);
            }
        }



        public decimal AmountOfSales
        {
            get
            {
                return _AmountOfSales;
            }
            set
            {
                SetPropertyValue("AmountOfSales", ref _AmountOfSales, value);
            }

        }




        public decimal AmountOfVoidSales
        {
            get
            {
                return _AmountOfVoidSales;
            }
            set
            {
                SetPropertyValue("AmountOfVoidSales", ref _AmountOfVoidSales, value);
            }
        }


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

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string BatchNetAmmount
        {
            get
            {
                return _BatchNetAmmount;
            }
            set
            {
                SetPropertyValue("BatchNetAmmount", ref _BatchNetAmmount, value);
            }
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string BatchOrigAmount
        {
            get
            {
                return _BatchOrigAmount;
            }
            set
            {
                SetPropertyValue("BatchOrigAmount", ref _BatchOrigAmount, value);
            }
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string BatchTotalCounter
        {
            get
            {
                return _BatchTotalCounter;
            }
            set
            {
                SetPropertyValue("BatchTotalCounter", ref _BatchTotalCounter, value);
            }
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string BatchTotal
        {
            get
            {
                return _BatchTotal;
            }
            set
            {
                SetPropertyValue("BatchTotal", ref _BatchTotal, value);
            }
        }

        public Guid POS
        {
            get
            {
                return _POS;
            }
            set
            {
                SetPropertyValue("POS", ref _POS, value);
            }
        }


        public Guid Store
        {
            get
            {
                return _Store;
            }
            set
            {
                SetPropertyValue("Store", ref _Store, value);
            }
        }


        public DateTime POSDateTime
        {
            get
            {
                return _POSDateTime;
            }
            set
            {
                SetPropertyValue("POSDateTime", ref _POSDateTime, value);
            }
        }


        public Guid UserDailyTotals
        {
            get
            {
                return _UserDailyTotals;
            }
            set
            {
                SetPropertyValue("UserDailyTotals", ref _UserDailyTotals, value);
            }
        }

        [Association("CardlinkBatchTotal-CardlinkBatchTranasactions")]
        public XPCollection<CardlinkBatchTransaction> Transactions
        {
            get
            {
                return GetCollection<CardlinkBatchTransaction>("Transactions");
            }
        }

        public override Dictionary<string, object> GetDict(JsonSerializerSettings settings, bool includeType)
        {
            Dictionary<string, object> dictionary = base.GetDict(settings, includeType);
            dictionary.Add("Transactions", Transactions.Select(g => g.GetDict(settings, includeType)).ToList());
            return dictionary;
        }
    }
}
