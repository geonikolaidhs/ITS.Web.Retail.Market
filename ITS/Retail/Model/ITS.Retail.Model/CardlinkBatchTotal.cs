using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    [Updater(Order = 993,
    Permissions = eUpdateDirection.STORECONTROLLER_TO_MASTER | eUpdateDirection.POS_TO_STORECONTROLLER)]
    public class CardlinkBatchTotal : BaseObj
    {
        public CardlinkBatchTotal()
           : base()
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

        public override Dictionary<string, object> GetDict(JsonSerializerSettings settings, bool includeType, bool includeDetails,
            eUpdateDirection direction = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.POS_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_MASTER | eUpdateDirection.STORECONTROLLER_TO_POS)
        {
            Dictionary<string, object> dictionary = base.GetDict(settings, includeType, includeDetails, direction);
            dictionary.Add("Transactions", Transactions.Select(g => g.GetDict(settings, includeType, includeDetails, direction)).ToList());
            return dictionary;
        }

    }

}
