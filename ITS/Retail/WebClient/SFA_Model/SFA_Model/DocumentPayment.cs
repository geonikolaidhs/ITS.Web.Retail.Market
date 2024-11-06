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
   // [CreateOrUpdaterOrder(Order = 620, Permissions = eUpdateDirection.SFA_TO_MASTER)]
    public class DocumentPayment : CustomFieldStorage, IDocumentPayment
    {
        public DocumentPayment()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public DocumentPayment(Session session)
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
        public decimal Amount { get; set; }

        public Guid DocumentHeader { get; set; }
        
        public Guid PaymentMethod { get; set; }
        
        public string PaymentMethodCode { get; set; }

        public Guid TransactionCoupon { get; set; }
        [NonPersistent]
        IDocumentHeader IDocumentPayment.DocumentHeader
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
        IPaymentMethod IDocumentPayment.PaymentMethod
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

        public Guid DocumentHeaderOid { get; set; }

        [NonPersistent]
        ITransactionCoupon IDocumentPayment.TransactionCoupon
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