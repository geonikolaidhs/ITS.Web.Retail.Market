using ITS.Retail.Model;
using ITS.Retail.WebClient.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData;
using System.Web.Http.Filters;

namespace ITS.Retail.WebClient
{
    public class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            /*config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }                
            );        */
        }
    }
}
