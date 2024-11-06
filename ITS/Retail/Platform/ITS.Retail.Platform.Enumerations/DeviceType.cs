using ITS.Retail.Platform.Enumerations.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ITS.Retail.Platform.Enumerations
{
    public enum DeviceType
    {
        Undefined,

        [SupportedConnections(ConnectionType.OPOS, ConnectionType.COM)]
        Scanner,

        [SupportedConnections(ConnectionType.NONE, ConnectionType.OPOS, ConnectionType.COM, ConnectionType.LPT, ConnectionType.EMULATED, ConnectionType.OPERATING_SYSTEM_DRIVER)]
        Printer,

        [SupportedConnections(ConnectionType.OPOS, ConnectionType.COM, ConnectionType.EMULATED)]
        PoleDisplay,

        [SupportedConnections(ConnectionType.NONE)] //Not yet Implemented
        FiscalPrinter,

        [SupportedConnections(ConnectionType.ETHERNET, ConnectionType.EMULATED)]
        [FiscalDevice(eFiscalDevice.DATASIGN)]
        DataSignESD,

        [SupportedConnections(ConnectionType.INDIRECT, ConnectionType.OPOS)]
        Drawer,

        [SupportedConnections(ConnectionType.INDIRECT, ConnectionType.OPOS)]
        KeyLock,

        [SupportedConnections(ConnectionType.OPOS)]
        MagneticStripReader,

        [SupportedConnections(ConnectionType.ETHERNET, ConnectionType.EMULATED, ConnectionType.COM)]
        [FiscalDevice(eFiscalDevice.ALGOBOX_NET)]
        AlgoboxNetESD,

        [SupportedConnections(ConnectionType.ETHERNET, ConnectionType.EMULATED)]
        [FiscalDevice(eFiscalDevice.DISIGN)]
        DiSign,

        [SupportedConnections(ConnectionType.COM)]
        [FiscalDevice(eFiscalDevice.MICRELEC_FISCAL_PRINTER)]
        MicrelecFiscalPrinter,

        [SupportedConnections(ConnectionType.COM)]
        [FiscalDevice(eFiscalDevice.WINCOR_FISCAL_PRINTER)]
        WincorFiscalPrinter,

        [SupportedConnections(ConnectionType.COM, ConnectionType.OPOS)]
        Scale,

        [SupportedConnections(ConnectionType.COM)]
        Edps,

        [SupportedConnections(ConnectionType.COM)]
        [FiscalDevice(eFiscalDevice.RBS_FISCAL_PRINTER)]
        RBSElioFiscalPrinter,

        [SupportedConnections(ConnectionType.ETHERNET)]
        RemotePrint,

        [SupportedConnections(ConnectionType.COM, ConnectionType.ETHERNET)]
        [FiscalDevice(eFiscalDevice.RBS_FISCAL_PRINTER)]
        [IsCashRegister]
        RBSElioWebCashRegister,

        [SupportedConnections(ConnectionType.COM, ConnectionType.ETHERNET)]
        Cardlink
    }

    public static class DeviceTypeExtensions
    {
        public static ConnectionType[] GetSupportedConnections(this DeviceType deviceType)
        {
            try
            {
                Type type = typeof(DeviceType);
                MemberInfo memInfo = type.GetMember(deviceType.ToString())[0];
                object[] attributes = memInfo.GetCustomAttributes(typeof(SupportedConnectionsAttribute), false);
                return ((SupportedConnectionsAttribute)attributes[0]).SupportedConnections;
            }
            catch
            {
                return new ConnectionType[] { ConnectionType.NONE };
            }
        }

        public static eFiscalDevice? GetFiscalDevice(this DeviceType deviceType)
        {
            try
            {
                Type type = typeof(DeviceType);
                MemberInfo memInfo = type.GetMember(deviceType.ToString())[0];
                object[] attributes = memInfo.GetCustomAttributes(typeof(FiscalDeviceAttribute), false);
                return ((FiscalDeviceAttribute)attributes[0]).FiscalDevice;
            }
            catch
            {
                return null;
            }
        }
    }
}
