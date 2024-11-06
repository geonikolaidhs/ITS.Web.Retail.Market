using System;

namespace ITS.Retail.Platform.Kernel.Model
{
    public interface IPromotionApplicationRuleDetail : IPersistentObject
    {
        Guid PromotionApplicationRuleOid { get; }
    }
}
