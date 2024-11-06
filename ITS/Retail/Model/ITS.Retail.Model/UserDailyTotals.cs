//-----------------------------------------------------------------------
// <copyright file="UserDailyTotals.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ITS.Retail.Model
{

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
        private string _POSCode;
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

        private User _User;
        [Association("User-UserDailyTotalss")]
        public User User
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

        private POS _POS;
        [Association("POS-UserDailyTotalss")]
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

        private Store _Store;
        [Association("Store-UserDailyTotalss")]
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
        public override Dictionary<string, object> GetDict(JsonSerializerSettings settings, bool includeType, bool includeDetails, eUpdateDirection direction = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.POS_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_MASTER | eUpdateDirection.STORECONTROLLER_TO_POS)
        {
            Dictionary<string, object> dictionary = base.GetDict(settings, includeType, includeDetails);

            if (includeDetails)
            {
                dictionary.Add("UserDailyTotalsDetails", UserDailyTotalsDetails.Select(g => g.GetDict(settings, includeType, includeDetails)).ToList());
            }
            return dictionary;
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
