using ITS.Retail.Platform.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Kernel
{
    interface IDeviceChecker : IKernelModule
    {
        bool CheckDevices();
    }
}
