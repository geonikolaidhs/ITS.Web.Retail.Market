using ITS.Retail.Platform.Kernel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Kernel
{
    /// <summary>
    /// Platform-wide rounding handler.
    /// </summary>
    public class PlatformRoundingHandler : IPlatformRoundingHandler
    {
        IOwnerApplicationSettings OwnerApplicationSettings { get; set; }

        public void SetOwnerApplicationSettings(IOwnerApplicationSettings ownerApplicationSettings)
        {
            OwnerApplicationSettings = ownerApplicationSettings;
        }

        /// <summary>
        /// Rounds a value, using the application setings' compute digits.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="midpointRounding"></param>
        /// <returns></returns>
        public decimal RoundValue(decimal value, MidpointRounding midpointRounding = MidpointRounding.AwayFromZero)
        {
            if (OwnerApplicationSettings != null)
            {
                return Math.Round(value, (int)OwnerApplicationSettings.ComputeDigits, midpointRounding);
            }
            else
            {
                throw new Exception("OwnerApplicationSettings are not defined");
            }
        }

        /// <summary>
        /// Rounds a value, using the application setings' display digits.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="midpointRounding"></param>
        /// <returns></returns>
        public decimal RoundDisplayValue(decimal value, MidpointRounding midpointRounding = MidpointRounding.AwayFromZero)
        {
            if (OwnerApplicationSettings != null)
            {
                return Math.Round(value, (int)OwnerApplicationSettings.DisplayDigits, midpointRounding);
            }
            else
            {
                throw new Exception("OwnerApplicationSettings are not defined");
            }
        }
    }
}
