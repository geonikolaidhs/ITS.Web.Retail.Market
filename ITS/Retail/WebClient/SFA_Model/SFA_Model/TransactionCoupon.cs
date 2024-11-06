using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using SFA_Model.NonPersistant;
using ITS.WRM.Model.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 1053, Permissions = eUpdateDirection.NONE)]
    public class TransactionCoupon : BaseObj, ITransactionCoupon
    {
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

        public Guid Coupon { get; set; }
        
        public string CouponCode { get; set; }

        public Guid CouponMask { get; set; }

        public Guid DocumentDetailDiscount { get; set; }
        
        public Guid DocumentHeader { get; set; }
        
        public Guid DocumentPayment { get; set; }
        
        public bool IsCanceled { get; set; }
        [NonPersistent]
        ICouponMask ITransactionCoupon.CouponMask
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        ICoupon ITransactionCoupon.Coupon
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        IDocumentHeader ITransactionCoupon.DocumentHeader
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        IDocumentPayment ITransactionCoupon.DocumentPayment
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        IDocumentDetailDiscount ITransactionCoupon.DocumentDetailDiscount
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
    }
}