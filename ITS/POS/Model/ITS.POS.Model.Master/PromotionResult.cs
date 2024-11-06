using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Model.Master
{
    [SyncInfoIgnore]
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

        private Guid _Promotion;
        private ePromotionResultExecutionPlan _ExecutionPlan;
        public Guid Promotion
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

    }
}
