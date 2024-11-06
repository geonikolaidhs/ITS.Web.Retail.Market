using ITS.Retail.Platform.Enumerations;
using ITS.WRM.Model.Interface.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface IDiscountType: ILookUp2Fields
    {
        bool DiscardsOtherDiscounts  { get; set; }
        bool IsUnique { get; set; }
        eDiscountType EDiscountType { get; set; }
        int Priority { get; set; }
        bool IsHeaderDiscount { get; set; }
    }
}
