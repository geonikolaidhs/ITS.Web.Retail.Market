using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Kernel.Model
{
    public interface IPaymentMethod : IPersistentObject
    {
        bool UsesInstallments { get; set; }
        bool IsNegative { get; set; }
        bool UseEDPS { get; set; }
        bool GiveChange { get; set; }
        bool NeedsValidation { get; set; }
        bool NeedsRatification { get; set; }
        bool CanExceedTotal { get; set; }
        bool OpensDrawer { get; set; }
        bool IncreasesDrawerAmount { get; set; }
        bool UseCardlink { get; set; }
    }
}
