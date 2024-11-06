using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model.NonPersistant
{
    [NonPersistent]
    public class CouponBase : BaseObj, IRequiredOwner
    {
        public CouponBase()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public CouponBase(Session session)
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

        private long _IsActiveUntil;
        private long _IsActiveFrom;
        private bool _IsUnique;
        private CompanyNew _Owner;
        private string _Description;
        private CouponAppliesOn _CouponAppliesOn;
        private CouponAmountType _CouponAmountType;
        private CouponAmountIsAppliedAs _CouponAmountIsAppliedAs;
        private DiscountType _DiscountType;
        private PaymentMethod _PaymentMethod;


        public long IsActiveFrom
        {
            get
            {
                return _IsActiveFrom;
            }
            set
            {
                SetPropertyValue("IsActiveFrom", ref _IsActiveFrom, value);
            }
        }

        [NonPersistent]
        public DateTime IsActiveFromDate
        {
            get
            {
                return new DateTime(this.IsActiveFrom);
            }

            set
            {
                this.IsActiveFrom = value.Ticks;
            }
        }


        public long IsActiveUntil
        {
            get
            {
                return _IsActiveUntil;
            }
            set
            {
                SetPropertyValue("IsActiveUntil", ref _IsActiveUntil, value);
            }
        }

        [NonPersistent]
        public DateTime IsActiveUntilDate
        {
            get
            {
                return new DateTime(this.IsActiveUntil);
            }

            set
            {
                this.IsActiveUntil = value.Ticks;
            }
        }

        public bool IsUnique
        {
            get
            {
                return _IsUnique;
            }
            set
            {
                SetPropertyValue("IsUnique", ref _IsUnique, value);
            }
        }

        public CompanyNew Owner
        {
            get
            {
                return _Owner;
            }
            set
            {
                SetPropertyValue("Owner", ref _Owner, value);
            }
        }

        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                SetPropertyValue("Description", ref _Description, value);
            }
        }

        public CouponAppliesOn CouponAppliesOn
        {
            get
            {
                return _CouponAppliesOn;
            }
            set
            {
                SetPropertyValue("CouponAppliesOn", ref _CouponAppliesOn, value);
            }
        }

        public CouponAmountType CouponAmountType
        {
            get
            {
                return _CouponAmountType;
            }
            set
            {
                SetPropertyValue("CouponAmountType", ref _CouponAmountType, value);
            }
        }

        public CouponAmountIsAppliedAs CouponAmountIsAppliedAs
        {
            get
            {
                return _CouponAmountIsAppliedAs;
            }
            set
            {
                SetPropertyValue("CouponAmountIsAppliedAs", ref _CouponAmountIsAppliedAs, value);
            }
        }

        public DiscountType DiscountType
        {
            get
            {
                return _DiscountType;
            }
            set
            {
                SetPropertyValue("DiscountType", ref _DiscountType, value);
            }
        }

        public PaymentMethod PaymentMethod
        {
            get
            {
                return _PaymentMethod;
            }
            set
            {
                SetPropertyValue("PaymentMethod", ref _PaymentMethod, value);
            }
        }

    }
}
