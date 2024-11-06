using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Client.Exceptions
{
    public class POSDeviceResultException : POSException
    {
         private DeviceResult _deviceResult;
        public DeviceResult DeviceResult
        {
            get
            {
                return _deviceResult;
            }
        }

        public string DeviceName{get; private set;}

        public POSDeviceResultException(DeviceResult deviceResult,string deviceName) : base(""+deviceResult)
        {
            _deviceResult = deviceResult;
            DeviceName = deviceName;

        }

        public POSDeviceResultException(DeviceResult deviceResult, string deviceName, string message)
            : base(message)
        {
            _deviceResult = deviceResult;
            DeviceName = deviceName;
        }

    }
}
