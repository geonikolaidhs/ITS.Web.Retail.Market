﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations
{
    public enum eCondition
    {
        NONE,//
        SINGLEQUANTITY,//
        MULTIQUANTITY, //
        RECEIPT,//
        PROFORMA, //
        NONZEROLINEDISCOUNT, //
        NONZERODOCUMENTDISCOUNT,//
        HASCHANGE,//
        NONDEFAULTCUSTOMER,
        DOESNOTINCREASEDRAWERAMOUNT,
        INCREASESDRAWERAMOUNT

    }
}