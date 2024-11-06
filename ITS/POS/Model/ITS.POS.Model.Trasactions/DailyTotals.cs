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
    public class DailyTotals : BaseObj
    {
        public DailyTotals()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public DailyTotals(Session session)
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


        private string _FiscalDeviceSerialNumber;
        private string _StoreCode;
        private int _POSID;
        private Guid _POS;
        [DenormalizedField("POSID",typeof(POS.Model.Settings.POS),"ITS.Retail.Model.POS","ID")]
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

        private DateTime _FiscalDate;
        [Indexed("POS;GCRecord", Unique = true)]
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

        private Guid _Store;
       // [Association("Store-UserDailyTotalss")]
        [DenormalizedField("StoreCode",typeof(Store),"ITS.Retail.Model.Store","Code")]
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

        private bool _FiscalDateOpen;
        public bool FiscalDateOpen
        {
            get
            {
                return _FiscalDateOpen;
            }
            set
            {
                SetPropertyValue("FiscalDateOpen", ref _FiscalDateOpen, value);
            }
        }
        private int _ZReportNumber;
        private bool _InEmulationMode;

        public int ZReportNumber
        {
            get
            {
                return _ZReportNumber;
            }
            set
            {
                SetPropertyValue("ZReportNumber", ref _ZReportNumber, value);
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

        public string FiscalDeviceSerialNumber
        {
            get
            {
                return _FiscalDeviceSerialNumber;
            }
            set
            {
                SetPropertyValue("FiscalDeviceSerialNumber", ref _FiscalDeviceSerialNumber, value);
            }
        }

        [Association("DailyTotals-DailyTotalsDetails")]
        public XPCollection<DailyTotalsDetail> DailyTotalsDetails
        {
            get
            {
                return GetCollection<DailyTotalsDetail>("DailyTotalsDetails");
            }
        }

        [Association("DailyTotals-UserDailyTotalss")]
        public XPCollection<UserDailyTotals> UserDailyTotalss
        {
            get
            {
                return GetCollection<UserDailyTotals>("UserDailyTotalss");
            }
        }

        public override Dictionary<string, object> GetDict(JsonSerializerSettings settings, bool includeType)
        {
            Dictionary<string, object> dictionary = base.GetDict(settings, includeType);
            dictionary.Add("DailyTotalsDetails", DailyTotalsDetails.Select(g => g.GetDict(settings, includeType)).ToList());
            dictionary.Add("UserDailyTotalss", UserDailyTotalss.Select(g => g.GetDict(settings, includeType)).ToList());
            return dictionary;
        }
    }
}
