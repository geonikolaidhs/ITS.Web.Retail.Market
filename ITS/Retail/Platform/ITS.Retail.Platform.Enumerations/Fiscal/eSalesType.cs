using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations.Fiscal
{
    public enum eSalesType
    {
        SALE = 1,
        VOID = 2,
        REFUND = 3
    }

    public enum eAdjustmentTypes
    {
        DIDSCOUNT = 1,
        MARKUP = 2,
        COUPON = 3,
        TICKET = 4
    }

    public enum eDrawerTypes
    {
        CashIN = 1,
        CashOut = 2
    }

    public enum eLCDLines{
        First = 0,
        Second = 1,
    }

    public enum ePadding
    {
        Left = 0,
        Right = 1
    }

    public enum ePrinterFontType
    {
        NORMAL = 1,
        DOUBLE_WIDTH = 2,
        DOUBLE_HEIGHT = 3,
        DOUBLE_WIDTH_HEIGHT = 4,
    }

    public enum PrinterStation
    {
        RECEIPT = 1,
        JOURNAL = 2
    }


}
