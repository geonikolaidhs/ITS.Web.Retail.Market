using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using ITS.POS.Resources;

namespace ITS.Retail.Platform.Enumerations
{
    public enum DeviceResult
    {
        [Display(Name="SUCCESS",ResourceType=typeof(POSClientResources))]
        SUCCESS,
        [Display(Name = "TIMEOUT", ResourceType = typeof(POSClientResources))]
        TIMEOUT,
        [Display(Name = "DEVICE_NOT_READY", ResourceType = typeof(POSClientResources))]
        DEVICENOTREADY,
        [Display(Name = "FAILURE", ResourceType = typeof(POSClientResources))]
        FAILURE,
        [Display(Name = "CHARACTER_SET_NOT_SUPPORTED", ResourceType = typeof(POSClientResources))]
        CHARACTERSETNOTSUPPORTED,
        [Display(Name = "COVER_OPEN", ResourceType = typeof(POSClientResources))]
        COVEROPEN,
        [Display(Name = "OUT_OF_PAPER", ResourceType = typeof(POSClientResources))]
        OUTOFPAPER,
        [Display(Name = "ACTION_NOT_SUPPORTED", ResourceType = typeof(POSClientResources))]
        ACTIONNOTSUPPORTED,
        [Display(Name = "STATION_NOT_SUPPORTED", ResourceType = typeof(POSClientResources))]
        STATIONNOTSUPPORTED,
        [Display(Name = "UNAUTHORIZED_ACCESS", ResourceType = typeof(POSClientResources))]
        UNAUTHORIZEDACCESS,
        [Display(Name = "INVALID_PROPERTY", ResourceType = typeof(POSClientResources))]
        INVALIDPROPERTY,
        [Display(Name = "NO_EXIST", ResourceType = typeof(POSClientResources))]
        NOEXIST,
        [Display(Name = "SHOULD_OPEN_FROM_PRINTER", ResourceType = typeof(POSClientResources))]
        SHOULDOPENFROMPRINTER,
        [Display(Name = "CONNECTION_NOT_SUPPORTED", ResourceType = typeof(POSClientResources))]
        CONNECTIONNOTSUPPORTED
    }

    public static class DeviceResultExtensions
    {

    }
}
