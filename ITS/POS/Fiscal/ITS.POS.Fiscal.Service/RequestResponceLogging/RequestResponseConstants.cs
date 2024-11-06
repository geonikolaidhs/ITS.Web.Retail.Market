using Newtonsoft.Json;
using System.Globalization;

namespace ITS.POS.Fiscal.Service.RequestResponceLogging
{
    public static class RequestResponseConstants
    {
        public static readonly JsonSerializerSettings JSON_SERIALIZER_SETTINGS = new JsonSerializerSettings
        {
            Culture = DefaultCulture,
            DateFormatHandling = DateFormatHandling.IsoDateFormat
        };

        public static readonly CultureInfo DefaultCulture = CultureInfo.GetCultureInfo("en-US");

        public static readonly string DiSignService = "DiSign Service";
    }
}
