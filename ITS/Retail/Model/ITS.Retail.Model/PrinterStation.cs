using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    //κανονικά βρίσκεται στο namespace Microsoft.PointOfService
    //αλλά αν το μεταφέρουμε εδώ θα κλάψουνε μανούλες...
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
