using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ITS.Retail.ResourcesLib;

namespace ITS.Retail.WebClient.Areas.B2C.Controllers
{
    public class WishListController : BaseController
    {
        //
        // GET: /B2C/WishList/

        public ActionResult Index()
        {
            ViewBag.Title = Resources.WishList; 
            ViewBag.MetaDescription = Resources.WishList;
            return View();
        }

        public ActionResult WishListPartial()
        {

            return PartialView("WishListPartial");

        }

    }
}
