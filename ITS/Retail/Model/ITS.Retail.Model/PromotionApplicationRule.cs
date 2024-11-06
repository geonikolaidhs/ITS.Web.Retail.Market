using DevExpress.Xpo;
using ITS.Retail.Platform.Kernel.Model;
using System;

namespace ITS.Retail.Model
{
    public class PromotionApplicationRule : BaseObj , IPromotionApplicationRule
    {

        public PromotionApplicationRule()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public PromotionApplicationRule(Session session)
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

        private PromotionApplicationRuleGroup _PromotionApplicationRuleGroup;
        [Association("PromotionApplicationRuleGroup-PromotionApplicationRules")]
        public PromotionApplicationRuleGroup PromotionApplicationRuleGroup
        {
            get
            {
                return _PromotionApplicationRuleGroup;
            }
            set
            {
                SetPropertyValue("PromotionApplicationRuleGroup", ref _PromotionApplicationRuleGroup, value);
            }
        }

        public virtual string Description
        {
            get
            {
                return "";
            }
        }


        public Guid PromotionApplicationRuleGroupOid
        {
            get
            {
                if (this.PromotionApplicationRuleGroup == null)
                {
                    return Guid.Empty;
                }
                return this.PromotionApplicationRuleGroup.Oid;
            }
        }
    }
}
