using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{
    [NonPersistent]
    public class TotalDetail : BaseObj
    {
        public TotalDetail()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public TotalDetail(Session session)
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

        private decimal _VatAmount;
        private eDailyRecordTypes _DetailType;
        public eDailyRecordTypes DetailType
        {
            get
            {
                return _DetailType;
            }
            set
            {
                SetPropertyValue("DetailType", ref _DetailType, value);
            }
        }

        private Guid _DocumentType;
        public Guid DocumentType
        {
            get
            {
                return _DocumentType;
            }
            set
            {
                SetPropertyValue("DocumentType", ref _DocumentType, value);
            }
        }


        private decimal _QtyValue;
        public decimal QtyValue
        {
            get
            {
                return _QtyValue;
            }
            set
            {
                SetPropertyValue("QtyValue", ref _QtyValue, value);
            }
        }

        private decimal _Amount;
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


        public decimal VatAmount
        {
            get
            {
                return _VatAmount;
            }
            set
            {
                SetPropertyValue("VatAmount", ref _VatAmount, value);
            }
        }
    }
}
