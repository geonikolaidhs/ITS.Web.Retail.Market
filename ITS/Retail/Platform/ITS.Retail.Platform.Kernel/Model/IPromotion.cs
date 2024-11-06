using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Kernel.Model
{
    public interface IPromotion : ILookup2Fields
    {
        DateTime StartTime { get; }
        DateTime EndTime { get; }
        DaysOfWeek ActiveDaysOfWeek { get; }
        string PrintedDescription { get; }
        int MaxExecutionsPerReceipt { get; }
        Guid PromotionApplicationRuleGroupOid { get; }
    }
}
