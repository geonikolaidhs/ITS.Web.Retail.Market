using System.Web.Mvc;

namespace ITS.Retail.WebClient.Areas.B2C
{
    public class B2CAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "B2C";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            //context.Routes.Add("b2cSubdomain",new DomainRoute(
            //    "www.mydomain.gr",
            //    "{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional },
            //    new string[] { "ITS.Retail.WebClient.Areas.B2C.Controllers" }
            //));

            //context.MapRoute(
            //    name: null,
            //    url: "B2C/Products/Page/{page}",
            //    defaults: new { Controller = "Products", action = "Index" }
            //);


            context.MapRoute(
               "Pages",
               "B2C/Pages/{page}",
               defaults: new
               {
                   controller = "Pages",
                   action = "Show",
                   page = UrlParameter.Optional
               },
               namespaces: new[] { "ITS.Retail.WebClient.Areas.B2C.Controllers" }
           );

            context.MapRoute(
                "B2C_default",
                "B2C/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "ITS.Retail.WebClient.Areas.B2C.Controllers" }
            );


            context.MapRoute("Home", "B2C", new { controller = "Home", action = "Index" });
        }
    }
}
