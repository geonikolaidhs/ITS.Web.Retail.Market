using System;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Enumerations;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ITS.POS.Model.Transactions
{
    [SyncInfoIgnoreAttribute]
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


        private bool _IncreasesDrawerAmount;
        private bool _CanExceedTotal;
        private Guid _DocumentHeaderOid;
        private TransactionCoupon _TransactionCoupon;
        private decimal _Amount;
        private ePaymentMethodType _PaymentMethodType;
        private Guid _PaymentMethod;
        private string _PaymentMethodCode;
        private string _PaymentMethodDescription;
        private DocumentHeader _DocumentHeader;
        [Association("DocumentHeader-Payments")]
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

        [DenormalizedField("PaymentMethodCode",typeof(PaymentMethod),"ITS.Retail.Model.PaymentMethod","Code")]
        public Guid PaymentMethod
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

        public string PaymentMethodDescription
        {
            get
            {
                return _PaymentMethodDescription;
            }
            set
            {
                SetPropertyValue("PaymentMethodDescription", ref _PaymentMethodDescription, value);
            }
        }

        public ePaymentMethodType PaymentMethodType
        {
            get
            {
                return _PaymentMethodType;
            }
            set
            {
                SetPropertyValue("PaymentMethodType", ref _PaymentMethodType, value);
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



        public bool IncreasesDrawerAmount
        {
            get
            {
                return _IncreasesDrawerAmount;
            }
            set
            {
                SetPropertyValue("IncreasesDrawerAmount", ref _IncreasesDrawerAmount, value);
            }
        }

        public bool CanExceedTotal
        {
            get
            {
                return _CanExceedTotal;
            }
            set
            {
                SetPropertyValue("CanExceedTotal", ref _CanExceedTotal, value);
            }
        }

        public DocumentPaymentEdps DocumentPaymentEdps
        {
            get
            {
                return this.DocumentHeader.DocumentPaymentsEdps.FirstOrDefault(x => x.DocumentPayment == this.Oid);
            }
        }

        public DocumentPaymentCardlink DocumentPaymentCardlink
        {
            get
            {
                return this.DocumentHeader.DocumentPaymentsCardlink.FirstOrDefault(x => x.DocumentPayment == this.Oid);
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
