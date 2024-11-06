using System;
using System.Collections.Generic;
using System.Linq;


namespace ITS.Retail.Platform.Enumerations
{
    [Flags]
    public enum ePermition
    {
        None = 0,
        Visible = 1,
        Insert = 2,
        Update = 4,
        Delete = 8
    }
}
