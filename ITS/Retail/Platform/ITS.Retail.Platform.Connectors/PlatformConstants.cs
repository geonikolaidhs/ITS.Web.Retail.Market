using ITS.Retail.Platform.Enumerations;
using Newtonsoft.Json;
using System.Globalization;

namespace ITS.Retail.Platform
{
    public static class PlatformConstants
    {

        public static readonly JsonSerializerSettings JSON_SERIALIZER_SETTINGS = new JsonSerializerSettings
        {
            Culture = DefaultCulture,
            DateFormatHandling = DateFormatHandling.IsoDateFormat
        };

        public static readonly CultureInfo DefaultCulture = CultureInfo.GetCultureInfo("en-US");

        
#if DEBUG
        public const KernelLogLevel DefaultKernelLogLevel = KernelLogLevel.Debug;
#else
        public const KernelLogLevel DefaultKernelLogLevel = KernelLogLevel.Info;
#endif
        public const string HEADQUARTERS = "Headquarters";
    }
}
