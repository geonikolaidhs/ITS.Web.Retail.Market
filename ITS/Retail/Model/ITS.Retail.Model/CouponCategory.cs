using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using System;

namespace ITS.Retail.Model
{
    [Serializable]
    [Updater(Order = 268, Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER)]
    public class CouponCategory : Lookup2Fields
    {
        
         public CouponCategory()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

         public CouponCategory(Session session)
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

        [Aggregated, Association("CouponCategory-Coupons")]
        public XPCollection<Coupon> Coupons
        {
            get
            {
                return GetCollection<Coupon>("Coupons");
            }
        }

        [Association("CouponCategory-CouponMasks"), Indexed(Unique = false)]
        public XPCollection<CouponMask> CouponMasks
        {
            get
            {
                return GetCollection<CouponMask>("CouponMasks");
            }
        }
    }
}
