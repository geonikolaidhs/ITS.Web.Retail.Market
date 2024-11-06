using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Kernel.Model
{
    public interface IDocumentDetailDiscount : IPersistentObject
    {
        eDiscountSource DiscountSource { get; set; }
        Guid PromotionOid { get; set;  }
        IDocumentDetail DocumentDetail { get; set; }

        decimal Percentage {get; set;}
        decimal Value {get; set;}
        Guid TypeOid {get; set;}
        string Description {get; set;}
        string TypeDescription {get; set;}
        eDiscountType DiscountType { get; set; }
        bool DiscardsOtherDiscounts {get; set;}
        int Priority { get; set; }
    }
}
