using ITS.Retail.Platform.Kernel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Kernel
{
    public interface IPlatformRoundingHandler : IKernelModule
    {
        void SetOwnerApplicationSettings(IOwnerApplicationSettings ownerApplicationSettings);

        decimal RoundValue(decimal value, MidpointRounding midpointRounding = MidpointRounding.AwayFromZero);

        decimal RoundDisplayValue(decimal value, MidpointRounding midpointRounding = MidpointRounding.AwayFromZero);
    }
}
