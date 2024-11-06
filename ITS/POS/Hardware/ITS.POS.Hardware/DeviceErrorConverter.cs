using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Exceptions;

namespace ITS.POS.Hardware
{
    /// <summary>
    /// Handles the convertion of the exceptions to DeviceResult
    /// </summary>
    public static class DeviceErrorConverter
    {
        private static DeviceResult ToDeviceResult(OPOSErrorCode errorCode)
        {
            switch (errorCode)
            {
                case OPOSErrorCode.Busy:
                    return DeviceResult.DEVICENOTREADY;
                case OPOSErrorCode.NoExist:
                    return DeviceResult.NOEXIST;
                case OPOSErrorCode.Failure:
                    return DeviceResult.FAILURE;
                case OPOSErrorCode.Claimed:
                    return DeviceResult.DEVICENOTREADY;
                case OPOSErrorCode.Closed:
                    return DeviceResult.DEVICENOTREADY;
                case OPOSErrorCode.Illegal:
                    return DeviceResult.DEVICENOTREADY; 
                case OPOSErrorCode.Timeout:
                    return DeviceResult.TIMEOUT;
                default:
                    return DeviceResult.FAILURE;
            }
        }

        /// <summary>
        /// Converts an exception to a device result
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static DeviceResult ToDeviceResult(Exception ex)
        {
            if (ex is OPOSDriverException)
            {
                return ToDeviceResult((ex as OPOSDriverException).ErrorCode);
            }
            else if (ex is NotSupportedException)
            {
                return DeviceResult.ACTIONNOTSUPPORTED;
            }
            else if (ex is UnauthorizedAccessException)
            {
                return DeviceResult.UNAUTHORIZEDACCESS;
            }
            else
            {
                return DeviceResult.FAILURE;
            }
        }
    }
}