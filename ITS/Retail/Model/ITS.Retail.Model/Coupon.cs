//-----------------------------------------------------------------------
// <copyright file="Coupon.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Model.NonPersistant;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.ResourcesLib;
using System;

namespace ITS.Retail.Model
{
    [Serializable]
    [Updater(Order = 270, Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER)]
    [EntityDisplayName("Coupon", typeof(Resources))]
    public class Coupon : CouponBase
    {
        public Coupon()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Coupon(Session session)
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

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew owner, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    Type thisType = typeof(Coupon);
                    if (owner == null)
                    {
                        throw new Exception(thisType.Name + ".GetUpdaterCriteria(); Owner: Owner is null");
                    }

                    crop = new BinaryOperator("Owner.Oid", owner.Oid);
                    break;
            }
            return crop;
        }

        // Fields...
        private string _Code;
        private decimal _Amount;
        private CouponCategory _CouponCategory;
        private int _NumberOfTimesUsed;
        private CouponMask _CouponMask;

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        [Indexed("GCRecord;Owner", Unique = true)]
        public string Code
        {
            get
            {
                return _Code;
            }
            set
            {
                SetPropertyValue("Code", ref _Code, value);
            }
        }

        public int NumberOfTimesUsed
        {
            get
            {
                return _NumberOfTimesUsed;
            }
            set
            {
                SetPropertyValue("NumberOfTimesUsed", ref _NumberOfTimesUsed, value);
            }
        }

        public bool HasBeenUsed
        {
            get
            {
                return NumberOfTimesUsed > 0;
            }
        }

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

        [Association("CouponCategory-Coupons"), Indexed(Unique = false)]
        public CouponCategory CouponCategory
        {
            get
            {
                return _CouponCategory;
            }
            set
            {
                SetPropertyValue("CouponCategory", ref _CouponCategory, value);
            }
        }

        [Association("CouponMask-Coupons"), Indexed(Unique = false)]
        public CouponMask CouponMask
        {
            get
            {
                return _CouponMask;
            }
            set
            {
                SetPropertyValue("CouponMask", ref _CouponMask, value);
            }
        }

        protected override void OnDeleting()
        {
            if (this.NumberOfTimesUsed > 0)
            {
                throw new Exception(string.Format(Resources.COUPON_HAS_BEEN_USED_TIMES, NumberOfTimesUsed));
            }
            base.OnDeleting();
        }

        [Aggregated, Association("Coupon-TransactionCoupons")]
        public XPCollection<TransactionCoupon> TransactionCoupons
        {
            get
            {
                return GetCollection<TransactionCoupon>("TransactionCoupons");
            }
        }
    }
}
