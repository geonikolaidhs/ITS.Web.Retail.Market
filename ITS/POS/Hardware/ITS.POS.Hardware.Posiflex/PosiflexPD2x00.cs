using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Hardware.Posiflex
{
    /// <summary>
    /// Specific pole display device implementation for the Posiflex PD2x00
    /// </summary>
    public class PosiflexPD2x00: PoleDisplay
    {
        public PosiflexPD2x00(ConnectionType conType,string deviceName)
            : base(conType,deviceName)
        {
            
        }

        override protected void SelectCodePageCOM(int codePage,SerialPort sp)
        {

            byte[] command;
            switch (codePage)
            {
                case 737:
                    command = new byte[]{0x1b, 0x74, 0xFD};
                    break;
                case 1253:
                    command = new byte[] { 0x1b, 0x74, 0xFF };
                break;
                    //ADD More here;
                default:
                    throw new NotSupportedException();
            }
            sp.Write(command, 0, command.Length);
        }


        override protected DeviceResult ClearDisplayCOM()
        {
            try
            {
                SerialPort sp = new SerialPort();

                sp.PortName = Settings.COM.PortName;
                sp.Parity = Settings.COM.Parity;
                sp.NewLine = Settings.NewLine;
                sp.BaudRate = Settings.COM.BaudRate;

                sp.Open();
                sp.Write(new byte[] { 0x0c }, 0, 1);
                sp.Close();
                return DeviceResult.SUCCESS;
            }
            catch (Exception )
            {
                return DeviceResult.FAILURE;
            }
        }
    }
}
