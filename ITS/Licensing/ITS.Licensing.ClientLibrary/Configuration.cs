using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.Licensing.ClientLibrary
{
    public static class Configuration
    {
        public static readonly String webServiceUrl = "http://www.its.net.gr/ITS.Licensing/LicenseWebService.asmx";
        /* Use this if you want to debug the web service and make sure that the URL works dude!
#if DEBUG
        public static readonly String webServiceUrl = "http://localhost:4057/LicenseWebService.asmx";
#endif
        */
    }
}
