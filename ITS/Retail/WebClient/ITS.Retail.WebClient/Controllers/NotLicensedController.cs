using System.Web.Mvc;

namespace ITS.Retail.WebClient.Controllers
{
    public class NotLicensedController : Controller
    {
        public ActionResult Index()
        {
            if (Request.UserHostName == "localhost" || Request.UserHostName == "127.0.0.1" || Request.UserHostName == "::1")//Εάν θέλετε ορίστε κατάλληλα τα ποιες ip μπορούν να
            {																												//να κάνουν registration.By defaut localhost only.
                return Registration();
            }
            return View();
        }

        public ActionResult Registration()
        {
            /*
            if (Request.HttpMethod == "POST")
            {
                if (MvcApplication.license.HandleRegistration(Request.Params, ViewData))
                {
                    MvcApplication.lastLicCheck = DateTime.Now;
                    MvcApplication.lastLicCheck = MvcApplication.lastLicCheck.AddHours(-3);
                    return new RedirectResult("~");
                }
            }*/
            return View("Registration");
        }

        //protected override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    if (filterContext.ActionDescriptor.ControllerDescriptor.ControllerName != "Base" && filterContext.ActionDescriptor.ControllerDescriptor.ControllerName != "Notification")
        //    {
        //        Session["ControllerName"] = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
        //        Session["ActionName"] = filterContext.ActionDescriptor.ActionName;
        //    }

        //    ViewBag.ControllerName = Session["ControllerName"];
        //    ViewBag.ActionName = Session["ActionName"];
        //}

    }
}
