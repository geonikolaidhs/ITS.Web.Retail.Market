using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations
{
    public enum eMinistryVatCategoryCode
    {
        NONE = 0,
        [Description("A")]
        A = 1,
        [Description("Β")]
        B = 2,
        [Description("Γ")]
        C = 3,
        [Description("Δ")]
        D = 4,
        [Description("Ε")]
        E = 5
    }
}
