using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ITS.Retail.WebClient.Controllers
{
    public class ToolsController : BaseController
    {
        //
        // GET: /Tools/

        [Security(ReturnsPartial=false)]
        public ActionResult Index()
        {

            return View("Index");
        }

        public ActionResult UpgradeDB()
        {
            ViewBag.DatabaseMessage = "Database Scheme Upgraded";
            return View("Index");
        }
    }
     
}
