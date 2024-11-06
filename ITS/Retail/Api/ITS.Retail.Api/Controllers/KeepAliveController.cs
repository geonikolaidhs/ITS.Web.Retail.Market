using ITS.Retail.Model;
using ITS.Retail.Platform.Kernel.Model;
using ITS.Retail.WRM.Kernel.Interface;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ITS.Retail.Api.Authentication;
using System.Net;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Api.Providers;
using System.Web.Http.ModelBinding;
using System.Web.Http;
using System.Collections.Generic;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using ITS.Retail.Common;
using DevExpress.Xpo;
using System.Web;

namespace ITS.Retail.Api.Controllers
{

    [RoutePrefix("api/KeepAlive")]
    [AllowAnonymous]
    public class KeepAliveController : ApiController
    {
        private static double LastRequest = TimeSpan.FromTicks(DateTime.Now.Ticks).TotalSeconds;
        private static int counter = 0;
        private static string _Secret = "9F7F0AC4-B9A1-45fd-A8CF-72F04E6BDE9F";

        [System.Web.Http.AcceptVerbs("GET")]
        [System.Web.Http.Route("KeepAlive/{secret}")]
        public async Task<IHttpActionResult> KeepAlive(String secret)
        {
            try
            {
                counter++;
                double now = TimeSpan.FromTicks(DateTime.Now.Ticks).TotalSeconds;
                if (now - LastRequest < 30)
                {
                    LastRequest = LastRequest = TimeSpan.FromTicks(DateTime.Now.Ticks).TotalSeconds;
                    WebApiConfig.ApiLogger.Log(counter.ToString() + " " + " TIME LIMIT Request From : " + HttpContext.Current.Request.UserHostAddress, KernelLogLevel.Error);
                    return Ok<long>(500);
                }
                if (secret != null && secret.ToUpper() != _Secret.ToUpper())
                {
                    LastRequest = TimeSpan.FromTicks(DateTime.Now.Ticks).TotalSeconds;
                    WebApiConfig.ApiLogger.Log("WRONG SECRET Request From: " + HttpContext.Current.Request.UserHostAddress, KernelLogLevel.Error);
                    return Ok<long>(500);
                }

                LastRequest = TimeSpan.FromTicks(DateTime.Now.Ticks).TotalSeconds;
                using (UnitOfWork uow = XpoHelper.GetReadOnlyUnitOfWork())
                {
                    List<Store> stores = new XPCollection<Store>(uow).ToList();
                }
            }
            catch (Exception ex)
            {
                WebApiConfig.ApiLogger.Log(ex, ex.Message + " ," + " ,KeepAlive ,KeepAlive ", KernelLogLevel.Error);
                return Ok<long>(500);
            }
            return Ok<long>(200);
        }
    }
}
