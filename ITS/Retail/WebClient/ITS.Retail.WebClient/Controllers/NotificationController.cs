using System;
using System.Web.Mvc;
using ITS.Retail.WebClient.Extensions;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.WebClient.Controllers
{
    public class NotificationController : BaseController
    {
        [ActionLog(LogLevel = LogLevel.None)]
        public JsonResult jsonNotifyUser()
        {
            String Notification = Session["Notice"] == null || String.IsNullOrWhiteSpace(Session["Notice"].ToString()) ? null : Session["Notice"].ToString();
            String Error = Session["Error"] == null || String.IsNullOrWhiteSpace(Session["Error"].ToString()) ? null : Session["Error"].ToString();
            Session["Notice"] = null;
            Session["Error"] = null;
            return Json(new { error=Error,notification=Notification });
        }

    }
}
