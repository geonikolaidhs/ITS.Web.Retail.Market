using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.POS.Hardware.Common
{
    public class LPTDeviceSettings
    {
        /// <summary>
        /// Gets or Sets the name of the port on the system. e.x. "LPT1"
        /// </summary>
        public string PortName { get; set; }

        public short FILE_ATTRIBUTE_NORMAL { get; set; } //*Used by the external system method for LPT communication. Default values in the constructor.
        public short INVALID_HANDLE_VALUE { get; set; } //*
        public uint GENERIC_READ { get; set; } //*
        public uint GENERIC_WRITE { get; set; } //*
        public uint CREATE_NEW { get; set; } //*
        public uint CREATE_ALWAYS { get; set; } //*
        public uint OPEN_EXISTING { get; set; } //*

        public LPTDeviceSettings()
        {
            FILE_ATTRIBUTE_NORMAL = 0x80;
            INVALID_HANDLE_VALUE = -1;
            GENERIC_READ = 0x80000000;
            GENERIC_WRITE = 0x40000000;
            CREATE_NEW = 1;
            CREATE_ALWAYS = 2;
            OPEN_EXISTING = 3;
        }
    }
}
