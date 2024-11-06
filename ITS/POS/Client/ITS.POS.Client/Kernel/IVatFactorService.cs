using ITS.POS.Hardware;
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Kernel
{
    public interface IVatFactorService : IKernelModule
    {
        VatFactor GetApplicationVatFactor(eMinistryVatCategoryCode ministryCode, Guid vatLevel);
        void CheckIfVatFactorsAreValid(FiscalPrinter fiscalPrinter, out bool? isVatAValid, out bool? isVatBValid, out bool? isVatCValid, out bool? isVatDValid, out bool? isVatEValid);
    }
}
