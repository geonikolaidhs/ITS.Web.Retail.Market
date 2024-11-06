using System;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.POS.Model.Settings;
using ITS.POS.Model.Master;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace ITS.POS.Model.Transactions
{
    [SyncInfoIgnoreAttribute]
    public class UserDailyTotals : BaseObj
    {
        public UserDailyTotals()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public UserDailyTotals(Session session)
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

        private string _StoreCode;
        private int _POSID;
        private string _UserName;
        private DailyTotals _DailyTotals;
        [Association("DailyTotals-UserDailyTotalss")]
        public DailyTotals DailyTotals
        {
            get
            {
                return _DailyTotals;
            }
            set
            {
                SetPropertyValue("DailyTotals", ref _DailyTotals, value);
            }
        }

        private DateTime _FiscalDate;
        public DateTime FiscalDate
        {
            get
            {
                return _FiscalDate;
            }
            set
            {
                SetPropertyValue("FiscalDate", ref _FiscalDate, value);
            }
        }

        private DateTime _PrintedDate;
        public DateTime PrintedDate
        {
            get
            {
                return _PrintedDate;
            }
            set
            {
                SetPropertyValue("PrintedDate", ref _PrintedDate, value);
            }
        }

        private Guid _User;
        [DenormalizedField("UserName", typeof(User), "ITS.Retail.User", "UserName")]
        public Guid User
        {
            get
            {
                return _User;
            }
            set
            {
                SetPropertyValue("User", ref _User, value);
            }
        }


        public string UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                SetPropertyValue("UserName", ref _UserName, value);
            }
        }

        public bool InEmulationMode
        {
            get
            {
                return _InEmulationMode;
            }
            set
            {
                SetPropertyValue("InEmulationMode", ref _InEmulationMode, value);
            }
        }

        private Guid _POS;
        [DenormalizedField("POSID", typeof(POS.Model.Settings.POS), "ITS.Retail.Model.POS", "ID")]
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


        public int POSID
        {
            get
            {
                return _POSID;
            }
            set
            {
                SetPropertyValue("POSID", ref _POSID, value);
            }
        }

        private Guid _Store;
        [DenormalizedField("StoreCode", typeof(Store), "ITS.Retail.Model.Store", "Code")]
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

        public string StoreCode
        {
            get
            {
                return _StoreCode;
            }
            set
            {
                SetPropertyValue("StoreCode", ref _StoreCode, value);
            }
        }

        private bool _IsOpen;
        public bool IsOpen
        {
            get
            {
                return _IsOpen;
            }
            set
            {
                SetPropertyValue("IsOpen", ref _IsOpen, value);
            }
        }

        private decimal _UserCashFinalAmount;
        private bool _InEmulationMode;
        private decimal _CashDifference;

        public decimal UserCashFinalAmount
        {
            get
            {
                return _UserCashFinalAmount;
            }
            set
            {
                SetPropertyValue("UserCashFinalAmount", ref _UserCashFinalAmount, value);
            }
        }

        [Association("UserDailyTotals-UserDailyTotalsDetails")]
        public XPCollection<UserDailyTotalsDetail> UserDailyTotalsDetails
        {
            get
            {
                return GetCollection<UserDailyTotalsDetail>("UserDailyTotalsDetails");
            }
        }

        [Association("UserDailyTotals-UserDailyTotalsCashCountDetail")]
        public XPCollection<UserDailyTotalsCashCountDetail> UserDailyTotalsCashCountDetails
        {
            get
            {
                return GetCollection<UserDailyTotalsCashCountDetail>("UserDailyTotalsCashCountDetails");
            }
        }

        public override Dictionary<string, object> GetDict(JsonSerializerSettings settings, bool includeType)
        {
            Dictionary<string, object> dictionary = base.GetDict(settings, includeType);
            dictionary.Add("UserDailyTotalsDetails", UserDailyTotalsDetails.Select(g => g.GetDict(settings, includeType)).ToList());
            return dictionary;
        }

        protected override void OnSaving()
        {
            if (DailyTotals != null)
            {
                DailyTotals.UpdatedOnTicks = DateTime.Now.Ticks;
            }
            base.OnSaving();
        }

        public decimal CashDifference
        {
            get
            {
                return _CashDifference;
            }
            set
            {
                SetPropertyValue("CashDifference", ref _CashDifference, value);
            }
        }
    }
}
