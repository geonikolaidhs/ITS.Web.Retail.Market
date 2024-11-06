using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Model.Transactions
{
    [SyncInfoIgnoreAttribute]
    public class DailyTotalsDetail : TotalDetail
    {
        public DailyTotalsDetail()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public DailyTotalsDetail(Session session)
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


        private DailyTotals _DailyTotals;
        [Association("DailyTotals-DailyTotalsDetails")]
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

        protected override void OnSaving()
        {
            if (DailyTotals != null)
            {
                DailyTotals.UpdatedOnTicks = DateTime.Now.Ticks;
            }

            base.OnSaving();
        }
    }
}
