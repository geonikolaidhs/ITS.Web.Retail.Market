using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ITS.Retail.WebClient.Helpers
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
