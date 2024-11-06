using System;

namespace ITS.Retail.Platform.Kernel.Model
{
    public interface IPromotionApplicationRule : IPersistentObject
    {
        Guid PromotionApplicationRuleGroupOid { get; }
    }
}
