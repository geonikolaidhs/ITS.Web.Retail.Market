using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Hardware.Common
{
    /// <summary>
    /// Represents the extra settings required for a COM scale
    /// </summary>
    public class COMScaleSettings
    {
        public ScaleCommunicationType CommunicationType { get; set; }

        public string ScaleReadPattern { get; set; }

        public COMScaleSettings()
        {
            this.CommunicationType = ScaleCommunicationType.CONTINUOUS;
            this.ScaleReadPattern = @"AB\r(\d{3}.\d{3})\r\d{3}.\d{3}\r";
        }
    }
}
