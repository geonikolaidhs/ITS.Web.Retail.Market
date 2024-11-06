using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{

    public class PromotionExecution : BaseObj, IPromotionExecution
    {
        private DiscountType _DiscountType;
        private decimal _Percentage;
        private decimal _Value;


                
         public PromotionExecution()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

         public PromotionExecution(Session session)
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

        private Promotion _Promotion;
        [Association("Promotion-PromotionExecutions")]
        public Promotion Promotion
        {
            get
            {
                return _Promotion;
            }
            set
            {
                SetPropertyValue("Promotion", ref _Promotion, value);
            }
        }

        public DiscountType DiscountType
        {
            get
            {
                return _DiscountType;
            }
            set
            {
                SetPropertyValue("DiscountType", ref _DiscountType, value);
            }
        }



        public decimal Percentage
        {
            get
            {
                return _Percentage;
            }
            set
            {
                SetPropertyValue("Percentage", ref _Percentage, value);
            }
        }

        public decimal Value
        {
            get
            {
                return _Value;
            }
            set
            {
                SetPropertyValue("Value", ref _Value, value);
            }
        }

        protected override void OnSaving()
        {
            if(this.DiscountType != null)
            {
                if(this.DiscountType.eDiscountType == eDiscountType.PERCENTAGE)
                {
                    this.Value = 0;
                }
                else
                {
                    this.Percentage = 0;
                }
            }
            base.OnSaving();
        }


        public virtual string Description
        {
            get
            {
                return "";
            }
     
        }

        public Guid DiscountTypeOid
        {
            get
            {
                if (this.DiscountType == null)
                {
                    return Guid.Empty;
                }
                return this.DiscountType.Oid;
            }
        }
    }
}
