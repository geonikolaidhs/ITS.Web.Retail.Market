using DevExpress.Xpo;
using ITS.POS.Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Model.Transactions
{
    public class EdpsBatchTransaction : BaseObj
    {
        public EdpsBatchTransaction()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

         public EdpsBatchTransaction(Session session)
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
        private string _Cashier;
        private string _TRM;
        private string _CardProduct;
        private string _PAN;
        private string _TimeStamp;
        private string _BankID;
        private string _AuthID;
        private int _STAN;
        private int _OnTopAmount;
        private int _InstPart;
        private int _PostDating;
        private int _Installments;
        private decimal _TipAmount;
        private decimal _Amount;
        private int _TransactionType;
        private int _ReceiptNumber;
        private EdpsBatchTotal _EdpsBatchTotal;

        [Association("EdpsBatchTotal-EdpsBatchTranasactions")]
        public EdpsBatchTotal EdpsBatchTotal
        {
            get
            {
                return _EdpsBatchTotal;
            }
            set
            {
                SetPropertyValue("EdpsBatchTotal", ref _EdpsBatchTotal, value);
            }
        }


        public int ReceiptNumber
        {
            get
            {
                return _ReceiptNumber;
            }
            set
            {
                SetPropertyValue("ReceiptNumber", ref _ReceiptNumber, value);
            }
        }


        public int TransactionType
        {
            get
            {
                return _TransactionType;
            }
            set
            {
                SetPropertyValue("TransactionType", ref _TransactionType, value);
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


        public decimal TipAmount
        {
            get
            {
                return _TipAmount;
            }
            set
            {
                SetPropertyValue("TipAmount", ref _TipAmount, value);
            }
        }


        public int Installments
        {
            get
            {
                return _Installments;
            }
            set
            {
                SetPropertyValue("Installments", ref _Installments, value);
            }
        }


        public int PostDating
        {
            get
            {
                return _PostDating;
            }
            set
            {
                SetPropertyValue("PostDating", ref _PostDating, value);
            }
        }


        public int InstPart
        {
            get
            {
                return _InstPart;
            }
            set
            {
                SetPropertyValue("InstPart", ref _InstPart, value);
            }
        }


        public int OnTopAmount
        {
            get
            {
                return _OnTopAmount;
            }
            set
            {
                SetPropertyValue("OnTopAmount", ref _OnTopAmount, value);
            }
        }


        public int STAN
        {
            get
            {
                return _STAN;
            }
            set
            {
                SetPropertyValue("STAN", ref _STAN, value);
            }
        }


        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string AuthID
        {
            get
            {
                return _AuthID;
            }
            set
            {
                SetPropertyValue("AuthID", ref _AuthID, value);
            }
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string BankID
        {
            get
            {
                return _BankID;
            }
            set
            {
                SetPropertyValue("BankID", ref _BankID, value);
            }
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string TimeStamp
        {
            get
            {
                return _TimeStamp;
            }
            set
            {
                SetPropertyValue("TimeStamp", ref _TimeStamp, value);
            }
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string PAN
        {
            get
            {
                return _PAN;
            }
            set
            {
                SetPropertyValue("PAN", ref _PAN, value);
            }
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string CardProduct
        {
            get
            {
                return _CardProduct;
            }
            set
            {
                SetPropertyValue("CardProduct", ref _CardProduct, value);
            }
        }


        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string TRM
        {
            get
            {
                return _TRM;
            }
            set
            {
                SetPropertyValue("TRM", ref _TRM, value);
            }
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string Cashier
        {
            get
            {
                return _Cashier;
            }
            set
            {
                SetPropertyValue("Cashier", ref _Cashier, value);
            }
        }
    }
}
