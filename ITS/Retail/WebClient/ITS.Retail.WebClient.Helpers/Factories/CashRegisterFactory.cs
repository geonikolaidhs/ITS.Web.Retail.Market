using ITS.Hardware.RBSPOSEliot.CashRegister;
using ITS.POS.Hardware;
using ITS.POS.Hardware.Common;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using System;

namespace ITS.Retail.WebClient.Helpers.Factories
{
    public class CashRegisterFactory
    {
        public CashRegisterHardware GetCashRegisterHardware(DeviceType deviceType,DeviceSettings deviceSettings,string deviceName,int id,ConnectionType connectionType,int LineChars,int commandChars)
        {
            //if DeviceType not IsCashRegister throw exception
            switch (deviceType)
            {
                case DeviceType.RBSElioWebCashRegister:
                    Settings settings = SetupSettings(deviceSettings, connectionType);
                    RBSElioCashRegister cashRegister = new RBSElioCashRegister(deviceType, settings, deviceName,id, connectionType,LineChars,commandChars);
                    return cashRegister;
                default:
                    throw new Exception();
            }
        }

        private Settings SetupSettings(DeviceSettings deviceSettings, ConnectionType connectionType)
        {
            Settings settings = new Settings();
            switch ( connectionType )
            {
                case ConnectionType.COM:
                    ITS.Retail.Model.COMDeviceSettings comSettings = (ITS.Retail.Model.COMDeviceSettings)deviceSettings;
                    settings.COM = new POS.Hardware.Common.COMDeviceSettings()
                    {
                        BaudRate = comSettings.BaudRate,
                        DataBits = comSettings.DataBits,
                        Handshake = comSettings.Handshake,
                        Parity = comSettings.Parity,
                        PortName = comSettings.PortName,
                        /*ScaleSettings*/ //TOCHECK for later models
                        StopBits = comSettings.StopBits,
                        WriteTimeOut = comSettings.WriteTimeOut
                    };
                    break;
                case ConnectionType.ETHERNET:
                    ITS.Retail.Model.EthernetDeviceSettings ethernetSettings = (ITS.Retail.Model.EthernetDeviceSettings)deviceSettings;
                    settings.Ethernet = new POS.Hardware.Common.EthernetDeviceSettings()
                    {
                        IPAddress = ethernetSettings.IPAddress,
                        Port = ethernetSettings.Port
                    };
                    break;
                default:
                    throw new Exception();
            }
            
            return settings;
        }
    }
}
