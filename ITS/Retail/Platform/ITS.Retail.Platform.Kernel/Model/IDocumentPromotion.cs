using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Kernel.Model
{
    public interface IDocumentPromotion : IPersistentObject
    {
        IDocumentHeader DocumentHeader { get; set; }
        decimal TotalGain { get; set; }
        Guid PromotionOid { get; set; }
        string PromotionCode { get; set; }
        string PromotionDescription { get; set; }
    }
}
