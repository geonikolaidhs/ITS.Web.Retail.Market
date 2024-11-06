//-----------------------------------------------------------------------
// <copyright file="DocumentPayment.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.Retail.Model.Attributes;

namespace ITS.Retail.Model
{
    //[Updater(Order = 620,
    //    Permissions = eUpdateDirection.STORECONTROLLER_TO_MASTER | eUpdateDirection.POS_TO_STORECONTROLLER)]
    public class DocumentPayment : CustomFieldStorage
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


        private decimal _Amount;    
        private string _PaymentMethodCode;
        private DocumentHeader _DocumentHeader;
        private PaymentMethod _PaymentMethod;
        private Guid _DocumentHeaderOid;
        private TransactionCoupon _TransactionCoupon;
        [Association("DocumentHeader-DocumentPayments")]
        public DocumentHeader DocumentHeader
        {
            get
            {
                return _DocumentHeader;
            }
            set
            {
                DocumentHeader oldHeader = _DocumentHeader;
                SetPropertyValue("DocumentHeader", ref _DocumentHeader, value);
                if (_DocumentHeader != null)
                {
                    this.DocumentHeaderOid = _DocumentHeader.Oid;
                    this.DocumentHeader.RefreshNonPersistant();
                }
                else if(oldHeader !=null )
                {
                    oldHeader.RefreshNonPersistant();
                }
            }
        }


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


        [Association("PaymentMethod-DocumentPayments")]
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

        [DescriptionField]
        public string PaymentMethodCode
        {
            get
            {
                return _PaymentMethodCode;
            }
            set
            {
                SetPropertyValue("PaymentMethodCode", ref _PaymentMethodCode, value);
            }
        }

        [RequiredField]
        public decimal Amount
        {
        	get
        	{
        		return _Amount;
        	}
        	set
        	{                
        	    SetPropertyValue("Amount", ref _Amount, value);
                if (this.DocumentHeader != null)
                {
                    this.DocumentHeader.RefreshNonPersistant();
                }
        	}
        }

        [NonPersistent]
        public DocumentPaymentEdps DocumentPaymentsEdps
        {
            get
            {
                return this.Session.FindObject<DocumentPaymentEdps>(new BinaryOperator("DocumentPayment", this.Oid));
            }
        }

        public TransactionCoupon TransactionCoupon
        {
            get
            {
                return _TransactionCoupon;
            }
            set
            {
                if (_TransactionCoupon == value)
                {
                    return;
                }

                // Store a reference to the former value. 
                TransactionCoupon transactionCoupon = _TransactionCoupon;
                _TransactionCoupon = value;

                if (IsLoading)
                {
                    return;
                }

                // Remove a reference if needed. 
                if (transactionCoupon != null && transactionCoupon.DocumentPayment == this)
                {
                    transactionCoupon.DocumentPayment = null;
                }

                // Specify the new owner value.
                if (_TransactionCoupon != null)
                {
                    _TransactionCoupon.DocumentPayment = this;
                }

                OnChanged("TransactionCoupon");
            }
        }
    }
}