using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Hardware.Common
{
    public interface IDrawerPrinter
    {
        DeviceResult OpenDrawer(string openDrawerCustomString);
    }
}
