using ITS.Retail.Common;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.WebClient.AuxillaryClasses
{
    public static class ApplicationHelper
    {
        public static bool IsMasterInstance()
        {
            return MvcApplication.ApplicationInstance == (eApplicationInstance.RETAIL);
        }

        public static bool IsStoreControllerInstance()
        {
            return MvcApplication.ApplicationInstance == (eApplicationInstance.STORE_CONTROLER);
        }

        public static bool IsDualInstance()
        {
            return MvcApplication.ApplicationInstance == (eApplicationInstance.DUAL_MODE);
        }

    }
}