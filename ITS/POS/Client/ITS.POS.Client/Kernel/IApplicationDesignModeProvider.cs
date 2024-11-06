using ITS.Retail.Platform.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Kernel
{
    public interface IApplicationDesignModeProvider : IKernelModule
    {
        bool ApplicationIsInDesignMode();
    }
}
