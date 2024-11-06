using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Kernel.Model
{
    public interface IPromotionItemExecution : IPromotionExecution
    {
        Guid ItemOid { get; }
        eItemExecutionMode ExecutionMode { get; }
        bool OncePerItem { get; }
        decimal Quantity { get;}
        decimal FinalUnitPrice { get; }
    }
}
