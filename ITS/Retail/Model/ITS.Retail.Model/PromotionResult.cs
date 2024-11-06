using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    public class PromotionResult : BaseObj
    {
         public PromotionResult()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

         public PromotionResult(Session session)
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
        private ePromotionResultExecutionPlan _ExecutionPlan;
        [Association("Promotion-PromotionResults")]
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

        public ePromotionResultExecutionPlan ExecutionPlan
        {
            get
            {
                return _ExecutionPlan;
            }
            set
            {
                SetPropertyValue("ExecutionPlan", ref _ExecutionPlan, value);
            }
        }

        public virtual string Description
        {
            get
            {
                return "";
            }
        }

    }
}
