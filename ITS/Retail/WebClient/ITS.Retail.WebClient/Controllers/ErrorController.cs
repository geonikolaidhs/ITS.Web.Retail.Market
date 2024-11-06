using System;
using System.Web.Mvc;

namespace ITS.Retail.WebClient.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/

        public ActionResult E404()
        {
            return View();
        }

        public ActionResult E500()
        {
            return View();
        }

        //public ActionResult Error()
        //{
        //    return View();
        //}

        //public ActionResult test500()
        //{
        //    throw new NotImplementedException();

        //}

    }
}
