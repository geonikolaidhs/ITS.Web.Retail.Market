﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations
{
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
    public class FiscalDeviceAttribute : Attribute
    {
        public eFiscalDevice FiscalDevice { get; set; }

        public FiscalDeviceAttribute(eFiscalDevice fiscalDevice)
        {
            this.FiscalDevice = fiscalDevice;
        }
    }
}
