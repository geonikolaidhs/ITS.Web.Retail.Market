using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.POS.Hardware.Common
{
    public enum PrinterStation
    {
        None = 0,
        Journal = 1,
        Receipt = 2,
        Slip = 4,
        TwoReceiptJournal = 32771,
        TwoSlipJournal = 32773,
        TwoSlipReceipt = 32774,
    }
}
