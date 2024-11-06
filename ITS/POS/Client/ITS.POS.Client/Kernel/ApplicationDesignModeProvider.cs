using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Kernel
{
    /// <summary>
    /// Provides info about the current mode that the code is executing in (Runtime or Designtime).
    /// </summary>
    public class ApplicationDesignModeProvider : IApplicationDesignModeProvider
    {
        public bool ApplicationIsInDesignMode()
        {
            return (LicenseManager.UsageMode == LicenseUsageMode.Designtime);
        }
    }
}
