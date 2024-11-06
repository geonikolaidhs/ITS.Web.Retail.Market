using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Kernel.Model
{
    public interface IPromotionItemApplicationRule : IPromotionApplicationRule
    {
        Guid ItemOid { get; }
        decimal Quantity { get; }
        decimal Value { get; }
    }
}
