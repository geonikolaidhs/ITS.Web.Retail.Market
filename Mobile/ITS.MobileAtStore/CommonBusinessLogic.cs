using System;

using System.Collections.Generic;
using System.Text;
using ITS.MobileAtStore.ObjectModel;
using ITS.Common.Utilities.EAN128BarcodeNS;
using System.Windows.Forms;

namespace ITS.MobileAtStore
{
    public static class CommonBusinessLogic
    {
        [ObsoleteAttribute] 
        public static decimal GetEAN128Quantity(DOC_TYPES type, EAN128Barcode bar, decimal defaultReturn)
        {
            if (bar == null || !bar.IsEAN128Barcode)
                return defaultReturn;
            //Keep it 0 in Orders ( by Theos )
            if (type == DOC_TYPES.ORDER)
                return defaultReturn;
            else
                return bar.EAN128BarcodeType == EAN128BarcodeTypes.VARIABLE_WEIGHT ? bar.QuantityWeight : bar.QuantityUnits;
        }

        public static string GetEAN128ProductCode(DOC_TYPES type, EAN128Barcode bar, string givenCode, Control c)
        {
            if (!bar.IsEAN128Barcode)
                return givenCode;
            if (c != null)
                c.Text = bar.ProductCode;
            return bar.ProductCode;
        }
    }
}
