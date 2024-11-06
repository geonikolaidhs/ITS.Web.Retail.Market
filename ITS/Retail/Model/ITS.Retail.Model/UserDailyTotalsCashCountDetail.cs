using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{
    [Updater(Order = 1040,
    Permissions = eUpdateDirection.STORECONTROLLER_TO_MASTER | eUpdateDirection.POS_TO_STORECONTROLLER)]
    public class UserDailyTotalsCashCountDetail : BaseObj
    {
        public UserDailyTotalsCashCountDetail()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public UserDailyTotalsCashCountDetail(Session session)
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
        [Association("UserDailyTotals-UserDailyTotalsCashCountDetail")]
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
            if (this.SkipOnSavingProcess)
            {
                base.OnSaving();
                return;
            }

            if (UserDailyTotals != null)
            {
                UserDailyTotals.UpdatedOnTicks = DateTime.Now.Ticks;
            }
            base.OnSaving();
        }
        private decimal _CountedAmount;
        private eCashCountRecordTypes _DetailType;
        public eCashCountRecordTypes DetailType
        {
            get
            {
                return _DetailType;
            }
            set
            {
                SetPropertyValue("DetailType", ref _DetailType, value);
            }
        }
        private PaymentMethod _Payment;
        [Association("PaymentMethod-UserDailyTotalsCashCountDetails")]
        public PaymentMethod Payment
        {
            get
            {
                return _Payment;
            }
            set
            {
                SetPropertyValue("Payment", ref _Payment, value);
            }
        }
        private DocumentType _DocumentType;
        [Association("DocumentType-UserDailyTotalsCashCountDetails")]
        public DocumentType DocumentType
        {
            get
            {
                return _DocumentType;
            }
            set
            {
                SetPropertyValue("DocumentType", ref _DocumentType, value);
            }
        }
        private decimal _QtyValue;
        public decimal QtyValue
        {
            get
            {
                return _QtyValue;
            }
            set
            {
                SetPropertyValue("QtyValue", ref _QtyValue, value);
            }
        }

        private decimal _Amount;
        public decimal Amount
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
        public decimal CountedAmount
        {
            get
            {
                return _CountedAmount;
            }
            set
            {
                SetPropertyValue("CountedAmount", ref _CountedAmount, value);
            }
        }
    }
}
