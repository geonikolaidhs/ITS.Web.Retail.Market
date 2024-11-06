using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Kernel.Model
{
    public interface IPromotionItemCategoryApplicationRule : IPromotionApplicationRule
    {
        Guid ItemCategoryOid { get; }
        decimal Quantity { get; }
        decimal Value { get; }
    }
}
