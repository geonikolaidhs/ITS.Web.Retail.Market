using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;


namespace ITS.Retail.Platform.Enumerations
{
    [Flags]
    public enum ePOSDbType
    {

        POS_SETTINGS = 1,
        POS_MASTER = 32,
        POS_TRANSCATION = 35,
        POS_VERSION = 39
    }
}