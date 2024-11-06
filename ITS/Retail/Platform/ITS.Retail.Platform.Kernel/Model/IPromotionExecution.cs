using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Kernel.Model
{
    public interface IPromotionExecution : IPersistentObject
    {
        Guid DiscountTypeOid { get;}
        decimal Percentage { get; }
        decimal Value { get; }
    }
}
