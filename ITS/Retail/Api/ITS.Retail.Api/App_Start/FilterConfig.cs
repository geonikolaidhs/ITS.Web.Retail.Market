using System.Web;
using System.Web.Mvc;

namespace ITS.Retail.Api
{
    /// <summary>
    /// Static class, utilized to initiate Global filters of the webapp
    /// </summary>
    public class FilterConfig
    {
        /// <summary>
        /// Registering the Global filters of the webapp
        /// </summary>
        /// <param name="filters">The Global filter collection of the webapp</param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
