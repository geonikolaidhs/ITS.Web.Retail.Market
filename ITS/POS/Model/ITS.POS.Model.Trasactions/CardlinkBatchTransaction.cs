using DevExpress.Xpo;
using ITS.POS.Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Model.Transactions
{
    public class CardlinkBatchTransaction : BaseObj
    {
        public CardlinkBatchTransaction()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public CardlinkBatchTransaction(Session session)
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
        private string _AcqId;
        private string _Cashier;
        private string _IssId;
        private string _IssName;
        private string _TransactionQty;
        private string _TransactionAmount;
        private bool _IsRefund;
        private CardlinkBatchTotal _CardlinkBatchTotal;

        [Association("CardlinkBatchTotal-CardlinkBatchTranasactions")]
        public CardlinkBatchTotal CardlinkBatchTotal
        {
            get
            {
                return _CardlinkBatchTotal;
            }
            set
            {
                SetPropertyValue("CardlinkBatchTotal", ref _CardlinkBatchTotal, value);
            }
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string AcqId
        {
            get
            {
                return _AcqId;
            }
            set
            {
                SetPropertyValue("AcqId", ref _AcqId, value);
            }
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string IssId
        {
            get
            {
                return _IssId;
            }
            set
            {
                SetPropertyValue("IssId", ref _IssId, value);
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

        public bool IsRefund
        {
            get
            {
                return _IsRefund;
            }
            set
            {
                SetPropertyValue("IsRefund", ref _IsRefund, value);
            }
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string IssName
        {
            get
            {
                return _IssName;
            }
            set
            {
                SetPropertyValue("IssName", ref _IssName, value);
            }
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string TransactionQty
        {
            get
            {
                return _TransactionQty;
            }
            set
            {
                SetPropertyValue("TransactionQty", ref _TransactionQty, value);
            }
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string TransactionAmount
        {
            get
            {
                return _TransactionAmount;
            }
            set
            {
                SetPropertyValue("TransactionAmount", ref _TransactionAmount, value);
            }
        }
    }
}
