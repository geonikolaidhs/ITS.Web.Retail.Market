using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using DevExpress.Data.Filtering;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Helpers;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.Common.ViewModel;
using System.Threading.Tasks;

using System.Net.Http;

namespace ITS.Retail.WebClient.Controllers
{
    [AllowAnonymous]
    public class KeepAliveController : Controller
    {
        private static double LastRequest = TimeSpan.FromTicks(DateTime.Now.Ticks).TotalSeconds;
        private static int counter = 0;
        private static string _Secret = "9F7F0AC4-B9A1-45fd-A8CF-72F04E6BDE9F";


        private System.Web.Http.IHttpActionResult Get(System.Net.HttpStatusCode code)
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage(code);
            return new System.Web.Http.Results.ResponseMessageResult(responseMessage);
        }



        public async Task<System.Web.Http.IHttpActionResult> KeepAlive(String secret)
        {
            try
            {
                counter++;
                double now = TimeSpan.FromTicks(DateTime.Now.Ticks).TotalSeconds;
                if (now - LastRequest < 30)
                {
                    LastRequest = LastRequest = TimeSpan.FromTicks(DateTime.Now.Ticks).TotalSeconds;
                    MvcApplication.WRMLogModule.Log(" TIME LIMIT Request From : " + Request.UserHostAddress, KernelLogLevel.Error);
                    return Get(System.Net.HttpStatusCode.BadRequest);
                }
                if (secret != null && secret.ToUpper() != _Secret.ToUpper())
                {
                    LastRequest = TimeSpan.FromTicks(DateTime.Now.Ticks).TotalSeconds;
                    MvcApplication.WRMLogModule.Log("WRONG SECRET Request From: " + Request.UserHostAddress, KernelLogLevel.Error);
                    return Get(System.Net.HttpStatusCode.BadRequest);
                }

                LastRequest = TimeSpan.FromTicks(DateTime.Now.Ticks).TotalSeconds;
                using (UnitOfWork uow = XpoHelper.GetReadOnlyUnitOfWork())
                {
                    List<Store> stores = new XPCollection<Store>(uow).ToList();
                }
            }
            catch (Exception ex)
            {
                MvcApplication.WRMLogModule.Log(ex, ex.Message + " ," + " ,KeepAlive ,KeepAlive ", KernelLogLevel.Error);
                return Get(System.Net.HttpStatusCode.BadRequest);
            }

            return Get(System.Net.HttpStatusCode.OK);

        }

    }
}
