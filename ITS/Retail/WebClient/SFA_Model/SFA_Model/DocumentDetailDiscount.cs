using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.WRM.Model.Interface;

using SFA_Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA_Model
{
    //[CreateOrUpdaterOrder(Order = 1051, Permissions = eUpdateDirection.NONE)]
    public class DocumentDetailDiscount : CustomFieldStorage, IDocumentDetailDiscount
    {
        public DocumentDetailDiscount()
        {

        }
        public DocumentDetailDiscount(Session session)
            : base(session)
        {

        }
        public string Description { get; set; }
        
        public bool DiscardsOtherDiscounts { get; set; }

        public eDiscountSource DiscountSource { get; set; }

        public eDiscountType DiscountType { get; set; }

        public decimal DiscountWithoutVAT { get; set; }

        public decimal DiscountWithVAT { get; set; }

        public Guid DocumentDetail { get; set; }
        
        public decimal Percentage { get; set; }

        public int Priority { get; set; }

        public Guid Promotion { get; set; }

        public Guid TransactionCoupon { get; set; }
        
        public Guid Type { get; set; }
        
        public string TypeDescription { get; set; }
        public decimal Value { get; set; }
        [NonPersistent]
        IDocumentDetail IDocumentDetailDiscount.DocumentDetail
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
        ITransactionCoupon IDocumentDetailDiscount.TransactionCoupon
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
        IDiscountType IDocumentDetailDiscount.Type
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