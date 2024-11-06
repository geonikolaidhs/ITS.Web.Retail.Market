using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace ITS.Retail.Api.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "WRM ODATA API";
            ViewBag.Version = string.Empty;

            try
            {
                ViewBag.Version = System.Reflection.Assembly.GetAssembly(typeof(ITS.Retail.Api.Api)).GetName().Version.ToString() + Environment.NewLine;
                string patchFile = Server.MapPath("~/patch.txt");
                if (System.IO.File.Exists(patchFile))
                {
                    using (StreamReader reader = new StreamReader(patchFile))
                    {
                        ViewBag.Version += reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return View();
        }

        public ActionResult Login()
        {
            ApplicationUserManager userManager = Request.GetOwinContext().Get<ApplicationUserManager>();
            ApplicationUserManager userManager2 = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            return null;
        }

        public ActionResult Token()
        {

            return null;
        }
    }
}
