using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ITS.Retail.WebClient.Extensions;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.WebClient.Areas.B2C.Controllers
{
    public class NotificationController : BaseController
    {
        [ActionLog(LogLevel = LogLevel.None)]
        public JsonResult jsonNotifyUser()
        {
            String Success = Session["Success"] == null || String.IsNullOrWhiteSpace(Session["Success"].ToString()) ? null : Session["Success"].ToString();
            String Info = Session["Info"] == null || String.IsNullOrWhiteSpace(Session["Info"].ToString()) ? null : Session["Info"].ToString();
            String Warning = Session["Warning"] == null || String.IsNullOrWhiteSpace(Session["Warning"].ToString()) ? null : Session["Warning"].ToString();
            String Danger = Session["Danger"] == null || String.IsNullOrWhiteSpace(Session["Danger"].ToString()) ? null : Session["Danger"].ToString();
            Session["Success"] = null;
            Session["Info"] = null;
            Session["Warning"] = null;
            Session["Danger"] = null;
            return Json(new { info = Info, success = Success, warning = Warning, danger = Danger });
        }

    }
}
