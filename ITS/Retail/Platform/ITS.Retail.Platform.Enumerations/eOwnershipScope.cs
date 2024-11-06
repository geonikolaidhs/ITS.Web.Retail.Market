using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ITS.Retail.Platform.Enumerations
{
    public enum eOwnershipScope
    {
        [Description("Global")]
        Global,
        [Description("Owner")]
        Owner
    }
}
