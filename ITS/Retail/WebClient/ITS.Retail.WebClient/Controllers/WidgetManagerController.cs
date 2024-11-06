using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.Model;

namespace ITS.Retail.WebClient.Controllers
{
    public class WidgetManagerController : BaseObjController<WidgetManager>
    {
      public JsonResult SaveDockPanel()
      {
          //Session["currentUser"]
          return Json(new { });
      }


    }
}
