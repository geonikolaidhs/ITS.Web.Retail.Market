﻿using ITS.Retail.Platform.Enumerations;
using ITS.WRM.Model.Interface.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface IPaymentMethod: ILookUp2Fields
    {
        bool IsNegative { get; set; }
        bool UseEDPS { get; set; }
        bool GiveChange { get; set; }
        ePaymentMethodType PaymentMethodType { get; set; }
        bool OpensDrawer { get; set; }
        bool IncreasesDrawerAmount { get; set; }
        bool UsesInstallments { get; set; }
        bool ForceEdpsOffline { get; set; }
        bool NeedsValidation { get; set; }
        bool NeedsRatification { get; set; }
        bool CanExceedTotal { get; set; }
    }
}
