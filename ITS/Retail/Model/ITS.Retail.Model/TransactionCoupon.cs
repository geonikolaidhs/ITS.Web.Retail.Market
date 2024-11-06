using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    public class TransactionCoupon : BaseObj
    {
        private string _CouponCode;
        private CouponMask _CouponMask;
        private Coupon _Coupon;
        private DocumentHeader _DocumentHeader;
        private DocumentPayment _DocumentPayment;
        private DocumentDetailDiscount _DocumentDetailDiscount;
        private bool _IsCanceled;

        public TransactionCoupon()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public TransactionCoupon(Session session)
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

        [Association("Coupon-TransactionCoupons"), Indexed(Unique = false)]
        public Coupon Coupon
        {
            get
            {
                return _Coupon;
            }
            set
            {
                SetPropertyValue("Coupon", ref _Coupon, value);
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

        [Association("DocumentHeader-TransactionCoupons")]
        [Indexed(Unique = false)]
        public DocumentHeader DocumentHeader
        {
            get
            {
                return _DocumentHeader;
            }
            set
            {
                SetPropertyValue("DocumentHeader", ref _DocumentHeader, value);
            }
        }


        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string CouponCode
        {
            get
            {
                return _CouponCode;
            }
            set
            {
                SetPropertyValue("CouponCode", ref _CouponCode, value);
            }
        }

        public DocumentPayment DocumentPayment
        {
            get
            {
                return _DocumentPayment;
            }
            set
            {
                if (_DocumentPayment == value)
                {
                    return;
                }

                // Store a reference to the former value. 
                DocumentPayment documentPayment = _DocumentPayment;
                _DocumentPayment = value;

                if (IsLoading)
                {
                    return;
                }

                // Remove a reference if needed. 
                if (documentPayment != null && documentPayment.TransactionCoupon == this)
                {
                    documentPayment.TransactionCoupon = null;
                }

                // Specify the new owner value.
                if (_DocumentPayment != null)
                {
                    _DocumentPayment.TransactionCoupon = this;
                }

                OnChanged("DocumentPayment");
            }
        }

        public DocumentDetailDiscount DocumentDetailDiscount
        {
            get
            {
                return _DocumentDetailDiscount;
            }
            set
            {
                if (_DocumentDetailDiscount == value)
                {
                    return;
                }

                // Store a reference to the former value. 
                DocumentDetailDiscount documentDetailDiscount = _DocumentDetailDiscount;
                _DocumentDetailDiscount = value;

                if (IsLoading)
                {
                    return;
                }

                // Remove a reference if needed. 
                if (documentDetailDiscount != null && documentDetailDiscount.TransactionCoupon == this)
                {
                    documentDetailDiscount.TransactionCoupon = null;
                }

                // Specify the new owner value.
                if (_DocumentDetailDiscount != null)
                {
                    _DocumentDetailDiscount.TransactionCoupon = this;
                }

                OnChanged("DocumentDetailDiscount");
            }
        }

        public bool IsCanceled
        {
            get
            {
                return _IsCanceled;
            }
            set
            {
                SetPropertyValue("IsCanceled", ref _IsCanceled, value);
            }
        }
    }
}
