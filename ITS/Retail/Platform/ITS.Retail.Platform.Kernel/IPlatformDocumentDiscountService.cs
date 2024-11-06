using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Kernel
{
    public interface IPlatformDocumentDiscountService : IKernelModule
    {
        decimal GetDocumentDetailsSumOfGrossTotalBeforeDiscountBySource(IDocumentHeader header, eDiscountSource source);
        IDocumentDetailDiscount UpdateDiscount(IDocumentDetailDiscount discount, decimal percentage, decimal value, int priority, eDiscountSource source, eDiscountType eDiscountType, IDiscountType discountType, bool discardsOtherDiscounts, string description = null);
        decimal GetDocumentHeaderGrossTotalBeforeDiscountBySource(IDocumentHeader header, eDiscountSource source);
        decimal GetTotalNonDiscountableValue(IDocumentHeader document);
    }
}
