using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Hardware.Common
{
    /// <summary>
    /// Settings regarding an Indirect connection.
    /// </summary>
    public class IndirectDeviceSettings
    {
        public string ParentDeviceName {get; set;}

        public string OpenCommandString { get; set; }

        public string KeyPosition0CommandString { get; set; }
        public string KeyPosition1CommandString { get; set; }
        public string KeyPosition2CommandString { get; set; }
        public string KeyPosition3CommandString { get; set; }
        public string KeyPosition4CommandString { get; set; }

        public IndirectDeviceSettings()
        {
            ParentDeviceName = "";
            OpenCommandString = "";
            KeyPosition0CommandString = "";
            KeyPosition1CommandString = "";
            KeyPosition2CommandString = "";
            KeyPosition3CommandString = "";
            KeyPosition4CommandString = "";
        }

    }
}
