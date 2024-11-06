using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.POS.Hardware.Common
{
    /// <summary>
    /// Settings regarding an Ethernet connection.
    /// </summary>
    public class EthernetDeviceSettings
    {
        public string IPAddress { get; set; }
        public int Port { get; set; }
    }
}
