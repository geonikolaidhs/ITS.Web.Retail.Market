using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    [Updater(Order = 990,
    Permissions = eUpdateDirection.STORECONTROLLER_TO_MASTER | eUpdateDirection.POS_TO_STORECONTROLLER)]
    public class EdpsBatchTotal : BaseObj
    {
        public EdpsBatchTotal()
           : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public EdpsBatchTotal(Session session)
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
        private DateTime _POSDateTime;
        private Guid _Store;
        private Guid _POS;
        private bool _Mismatch;
        private decimal _AmountOfVoidRefunds;
        private int _NumberOfVoidRefunds;
        private decimal _AmountOfRefunds;
        private int _NumberOfRefunds;
        private decimal _AmountOfVoidSales;
        private int _NumberOfVoidSales;
        private decimal _AmountOfSales;
        private int _NumberOfSales;
        private int _BatchNumber;
        private Guid _UserDailyTotals;

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


        public int NumberOfRefunds
        {
            get
            {
                return _NumberOfRefunds;
            }
            set
            {
                SetPropertyValue("NumberOfRefunds", ref _NumberOfRefunds, value);
            }
        }


        public decimal AmountOfRefunds
        {
            get
            {
                return _AmountOfRefunds;
            }
            set
            {
                SetPropertyValue("AmountOfRefunds", ref _AmountOfRefunds, value);
            }
        }


        public int NumberOfVoidRefunds
        {
            get
            {
                return _NumberOfVoidRefunds;
            }
            set
            {
                SetPropertyValue("NumberOfVoidRefunds", ref _NumberOfVoidRefunds, value);
            }
        }


        public decimal AmountOfVoidRefunds
        {
            get
            {
                return _AmountOfVoidRefunds;
            }
            set
            {
                SetPropertyValue("AmountOfVoidRefunds", ref _AmountOfVoidRefunds, value);
            }
        }


        public bool Mismatch
        {
            get
            {
                return _Mismatch;
            }
            set
            {
                SetPropertyValue("Mismatch", ref _Mismatch, value);
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

        [Association("EdpsBatchTotal-EdpsBatchTranasactions")]
        public XPCollection<EdpsBatchTransaction> Transactions
        {
            get
            {
                return GetCollection<EdpsBatchTransaction>("Transactions");
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
