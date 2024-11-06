using DevExpress.Xpo;
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Kernel.Model;
using System;

namespace ITS.POS.Model.Transactions
{
    [SyncInfoIgnoreAttribute]
    public class TransactionCoupon : BaseObj, ITransactionCoupon
    {
        private string _CouponCode;
        private Guid _Coupon;
        private DocumentHeader _DocumentHeader;
        private Guid _DocumentHeaderOid;
        private DocumentPayment _DocumentPayment;
        private DocumentDetailDiscount _DocumentDetailDiscount;
        private bool _IsCanceled;
        private Guid _CouponMask;


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

        [Indexed(Unique = false)]
        public Guid Coupon
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


        [Indexed(Unique = false)]
        public Guid CouponMask
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

        [NonPersistent]
        public Guid DocumentHeaderOid
        {
            get
            {
                return _DocumentHeaderOid;
            }
            set
            {
                SetPropertyValue("DocumentHeaderOid", ref _DocumentHeaderOid, value);
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
                if (_DocumentHeader != null)
                {
                    this.DocumentHeaderOid = _DocumentHeader.Oid;
                }
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
