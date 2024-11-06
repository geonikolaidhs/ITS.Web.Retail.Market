using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface IDailyTotalsDetail
    {
        //IDailyTotals DailyTotals { get; set; }
        IVatFactor VatFactor { get; set; }
        IPaymentMethod Payment { get; set; }
    }
}
