using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Model.NonPersistant;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.ResourcesLib;
using System;

namespace ITS.Retail.Model
{
    [Serializable]
    [Updater(Order = 269, Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER)]
    [EntityDisplayName("CouponMask", typeof(Resources))]
    public class CouponMask : CouponBase
    {
        
        public CouponMask()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public CouponMask(Session session)
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
        private string _Prefix;
        private string _Mask;
        private string _PropertyName;
        private CouponCategory _CouponCategory;

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        [Indexed("GCRecord;Owner", Unique = true)]
        public string Mask
        {
            get
            {
                return _Mask;
            }
            set
            {
                SetPropertyValue("Mask", ref _Mask, value);
            }
        }



        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string Prefix
        {
            get
            {
                return _Prefix;
            }
            set
            {
                SetPropertyValue("Prefix", ref _Prefix, value);
            }
        }        
        
        public string EntityName
        {
            get
            {
                switch (CouponAmountIsAppliedAs)
                {
                    case Platform.Enumerations.CouponAmountIsAppliedAs.DISCOUNT:
                        return typeof(DocumentDetailDiscount).FullName;
                    case Platform.Enumerations.CouponAmountIsAppliedAs.PAYMENT_METHOD:
                        return typeof(PaymentMethod).FullName;
                    case Platform.Enumerations.CouponAmountIsAppliedAs.POINTS:
                        return string.Empty;
                    default:
                        return string.Empty;
                }
            }
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string PropertyName
        {
            get
            {
                return _PropertyName;
            }
            set
            {
                SetPropertyValue("PropertyName", ref _PropertyName, value);
            }
        }

        [Association("CouponCategory-CouponMasks"), Indexed(Unique = false)]
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
        public XPCollection<Coupon> Coupons
        {
            get
            {
                return GetCollection<Coupon>("Coupons");
            }
        }

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew owner, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    Type thisType = typeof(CouponMask);
                    if (owner == null)
                    {
                        throw new Exception(thisType.Name + ".GetUpdaterCriteria(); Owner: Owner is null");
                    }

                    crop = new BinaryOperator("Owner.Oid", owner.Oid);
                    break;
            }
            return crop;
        }
    }
}
