using ITS.Retail.WebClient.Extensions;
using StackExchange.Profiling.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ITS.Retail.WebClient
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ActionLogAttribute());
            filters.Add(new HandleErrorAttribute());

            
            filters.Add(new ProfilingActionFilter());

            filters.Add(new HandleErrorAttribute());
        }
    }
}