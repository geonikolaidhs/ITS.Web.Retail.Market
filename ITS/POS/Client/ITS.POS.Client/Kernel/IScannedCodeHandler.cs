using ITS.POS.Client.Forms;
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Kernel
{
    interface IScannedCodeHandler : IKernelModule
    {
        void HandleScannedCode(IGetItemPriceForm form, OwnerApplicationSettings appSettings, bool includeInactive,
            string scannedCode, decimal qty, bool checkForWeightedItem, bool isReturn, bool fromScanner, bool readWeight); 
    }
}
