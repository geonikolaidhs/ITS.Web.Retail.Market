using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Kernel.Model
{
    public interface IDiscountType : ILookup2Fields
    {
        eDiscountType eDiscountType { get; }
        bool DiscardsOtherDiscounts { get; }
    }
}
