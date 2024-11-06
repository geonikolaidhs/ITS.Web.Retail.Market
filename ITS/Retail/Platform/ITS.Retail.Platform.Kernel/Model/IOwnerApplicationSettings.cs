using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Platform.Kernel.Model
{
    public interface IOwnerApplicationSettings : IPersistentObject
    {
        ePromotionExecutionPriority PromotionExecutionPriority { get; }
        double ComputeDigits { get; }
        double DisplayDigits { get; }
        decimal DiscountAmount { get; }
        decimal RefundPoints { get; }
    }
}
