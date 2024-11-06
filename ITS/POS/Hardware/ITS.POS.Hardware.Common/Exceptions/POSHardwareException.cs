using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Hardware.Common.Exceptions
{
    public class POSHardwareException : Exception
    {
        public string DeviceName { get; protected set; }

        public POSHardwareException(string deviceName,string message)
            : base(message)
        {
            DeviceName = deviceName;
        }
    }
}
