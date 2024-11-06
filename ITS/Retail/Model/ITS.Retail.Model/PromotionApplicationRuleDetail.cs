using System;
using ITS.Retail.Platform.Kernel.Model;

namespace ITS.Retail.Model
{
    public class PromotionApplicationRuleDetail : BaseObj, IPromotionApplicationRuleDetail
    {
        private PromotionApplicationRule _PromotionApplicationRule;
        public PromotionApplicationRule PromotionApplicationRule
        {
            get
            {
                return _PromotionApplicationRule;
            }
            set
            {
                SetPropertyValue("PromotionApplicationRule", ref _PromotionApplicationRule, value);
            }
        }

        public Guid PromotionApplicationRuleOid
        {
            get
            {
                return this.PromotionApplicationRule == null ? Guid.Empty : this.PromotionApplicationRule.Oid;
            }
        }
    }
}
