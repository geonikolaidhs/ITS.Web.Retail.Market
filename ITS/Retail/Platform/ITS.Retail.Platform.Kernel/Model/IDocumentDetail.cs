using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Kernel.Model
{
    public interface IDocumentDetail : IPersistentObject
    {
        IEnumerable<IDocumentDetailDiscount> DocumentDetailDiscounts { get; }
        decimal GrossTotal { get; set; }
        decimal GrossTotalBeforeDocumentDiscount { get; set; }
        decimal Qty { get; set; }
        decimal CurrentPromotionDiscountValue { get; set; }
        decimal DocumentDiscountAmount { get; }
        decimal PointsDiscountAmount { get; }
        decimal DefaultDocumentDiscountAmount { get; }
        decimal CustomerDiscountAmount { get; }
        decimal PromotionsDocumentDiscountAmount { get; }
        decimal PromotionsLineDiscountsAmount { get; }
        Guid ItemOid { get; }
        decimal FinalUnitPrice { get; }
        bool IsCanceled { get; }
        Guid PriceCatalog { get; }
        bool DoesNotAllowDiscount { get; set; }
        bool IsTax { get; set; }
        bool IsReturn { get; set; }

    }
}
