//-----------------------------------------------------------------------
// <copyright file="DailyTotals.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using System.Linq;
using ITS.Retail.Platform.Enumerations;
using System.Collections.Generic;
using Newtonsoft.Json;
using ITS.Retail.Model.Attributes;

namespace ITS.Retail.Model
{
    [Updater(Order = 930,
        Permissions = eUpdateDirection.STORECONTROLLER_TO_MASTER | eUpdateDirection.POS_TO_STORECONTROLLER)]
    public class DailyTotals : BaseObj, IOwner
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

        public CompanyNew Owner
        {
            get
            {
                if (POS == null || POS.Store == null)
                {
                    return null;
                }
                return POS.Store.Owner;
            }
        }

        [DescriptionField]
        public String Description
        {
            get
            {
                return POS.Store.Name + " " + POS.Name;
            }
        }

        private string _FiscalDeviceSerialNumber;
        private int _ZReportNumber;
        private string _StoreCode;
        private string _POSCode;
        private POS _POS;
        [Association("POS-DailyTotalss")]
        public POS POS
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

        public string POSCode
        {
            get
            {
                return _POSCode;
            }
            set
            {
                SetPropertyValue("POSCode", ref _POSCode, value);
            }
        }

        private DateTime _FiscalDate;
        [Indexed("POS;GCRecord", Unique = false)]
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
        [Indexed("POS;GCRecord", Unique = false)]
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

        private Store _Store;
        [Association("Store-DailyTotalss")]
        public Store Store
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
        private int _POSID;
        private bool _InEmulationMode;

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

        public decimal Sum
        {
            get
            {
                decimal sum = 0;
                if (DailyTotalsDetails != null && DailyTotalsDetails.Count > 0)
                {
                    sum = DailyTotalsDetails.Where(dailyTotDet => (dailyTotDet.DetailType == eDailyRecordTypes.TAXRECORD)).Sum(dailyTotDet => dailyTotDet.Amount);
                }
                return sum;
            }
        }

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

        public override Dictionary<string, object> GetDict(JsonSerializerSettings settings, bool includeType, bool includeDetails, eUpdateDirection direction = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.POS_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_MASTER | eUpdateDirection.STORECONTROLLER_TO_POS)
        {
            Dictionary<string, object> dictionary = base.GetDict(settings, includeType, includeDetails);

            if (includeDetails)
            {
                dictionary.Add("DailyTotalsDetails", DailyTotalsDetails.Select(g => g.GetDict(settings, includeType, includeDetails)).ToList());
                dictionary.Add("UserDailyTotalss", UserDailyTotalss.Select(g => g.GetDict(settings, includeType, includeDetails)).ToList());
            }
            return dictionary;
        }
    }
}
