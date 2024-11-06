using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.POS.Model.Settings;

namespace ITS.POS.Model.Transactions
{
    [SyncInfoIgnoreAttribute]
    public class UserDailyTotalsDetail : TotalDetail
    {
        public UserDailyTotalsDetail()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public UserDailyTotalsDetail(Session session)
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

        private UserDailyTotals _UserDailyTotals;
        [Association("UserDailyTotals-UserDailyTotalsDetails")]
        public UserDailyTotals UserDailyTotals
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

        protected override void OnSaving()
        {
            if (UserDailyTotals != null)
            {
                UserDailyTotals.UpdatedOnTicks = DateTime.Now.Ticks;
            }
            base.OnSaving();
        }
    }
}
