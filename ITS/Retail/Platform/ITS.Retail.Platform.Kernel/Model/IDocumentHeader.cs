using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Kernel.Model
{
    public interface IDocumentHeader : IPersistentObject
    {
        IEnumerable<IDocumentPromotion> DocumentPromotions { get; }
        decimal PromotionPoints { get; set; }
        IEnumerable<IDocumentDetail> DocumentDetails { get; }
        decimal GrossTotalBeforeDocumentDiscount { get; set; }
        Guid PromotionsDocumentDiscountTypeOid { get; set; }
        IEnumerable<IDocumentDetail> DiscountableDocumentDetails();
        decimal GrossTotal { get; set; }
        decimal DocumentDiscountAmount { get; set; }
        decimal PointsDiscountAmount { get; set; }
        decimal PromotionsDiscountAmount { get; set; }
        decimal CustomerDiscountAmount { get; set; }
        decimal DefaultDocumentDiscountAmount { get; set; }
        decimal PromotionsDiscountPercentage { get; set; }
        decimal PromotionsDiscountPercentagePerLine { get; set; }
        Guid CustomerOid { get; }
    }
}
