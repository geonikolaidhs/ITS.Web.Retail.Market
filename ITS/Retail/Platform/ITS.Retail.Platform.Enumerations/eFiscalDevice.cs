using System;
using System.Reflection;

namespace ITS.Retail.Platform.Enumerations
{
    public enum eFiscalDevice
    {
        [FiscalMethod(eFiscalMethod.EAFDSS)]
        DATASIGN,
        [FiscalMethod(eFiscalMethod.EAFDSS)]
        ALGOBOX_NET,
        [FiscalMethod(eFiscalMethod.EAFDSS)]
        DISIGN,
        [FiscalMethod(eFiscalMethod.ADHME)]
        MICRELEC_FISCAL_PRINTER,
        [FiscalMethod(eFiscalMethod.ADHME)]
        RBS_FISCAL_PRINTER,
        [FiscalMethod(eFiscalMethod.ADHME)]
        WINCOR_FISCAL_PRINTER,
        [FiscalMethod(eFiscalMethod.EAFDSS)]
        RBS_NET,
        [FiscalMethod(eFiscalMethod.EAFDSS)]
        ALGOBOX_NETV2
    }

    public static class eFiscalDeviceExtensions
    {
        public static eFiscalMethod GetFiscalMethod(this eFiscalDevice fiscalDevice)
        {
            try
            {
                Type type = typeof(eFiscalDevice);
                MemberInfo memInfo = type.GetMember(fiscalDevice.ToString())[0];
                object[] attributes = memInfo.GetCustomAttributes(typeof(FiscalMethodAttribute), false);
                return ((FiscalMethodAttribute)attributes[0]).FiscalMethod;
            }
            catch
            {
                return eFiscalMethod.UNKNOWN;
            }
        }

    }
}
