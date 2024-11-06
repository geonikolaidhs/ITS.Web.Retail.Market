using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Kernel.Model
{
    public interface IPromotionDocumentExecution : IPromotionExecution
    {
        bool KeepOnlyPoints { get; }
        decimal Points { get; }
    }
}
