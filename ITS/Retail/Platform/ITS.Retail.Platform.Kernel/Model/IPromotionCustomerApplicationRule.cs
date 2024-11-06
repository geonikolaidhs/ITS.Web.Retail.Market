using System;

namespace ITS.Retail.Platform.Kernel.Model
{
    public interface IPromotionCustomerApplicationRule : IPromotionApplicationRule
    {
        Guid CustomerOid { get; }
    }
}
