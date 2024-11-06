using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using System;

namespace ITS.Retail.Model
{
    public class GeneratedCoupon : BaseObj, IRequiredOwner
    {
        public GeneratedCoupon()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public GeneratedCoupon(Session session)
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

        private string _Description;
        private CompanyNew _Owner;
        private string _Code;
        private CouponMask _CouponMask;
        private GeneratedCouponStatus _GeneratedCouponStatus;
        private Guid? _Device;

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

        public Guid? Device
        {
            get
            {
                return _Device;
            }
            set
            {
                SetPropertyValue("Device", ref _Device, value);
            }
        }

        public GeneratedCouponStatus Status
        {
            get
            {
                return _GeneratedCouponStatus;
            }
            set
            {
                SetPropertyValue("GeneratedCouponStatus", ref _GeneratedCouponStatus, value);
            }
        }
    }
}
