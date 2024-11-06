using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Receipt
{
    public enum eCondition
    {
        NONE,
        SINGLEQUANTITY,
        MULTIQUANTITY,
        RECEIPT,
        PROFORMA,
        NONZEROLINEDISCOUNT,
        NONZERODOCUMENTDISCOUNT,
        HASCHANGE,
        NONDEFAULTCUSTOMER
    }
}
