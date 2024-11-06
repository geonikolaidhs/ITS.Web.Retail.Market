using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Web;
using System.Web.Mvc;

namespace ITS.Retail.WebClient.Areas.B2C.Controllers
{
    public class PagesController : BaseController
    {
        public ActionResult Show(string page)
        {
            List<string> validPages = new List<string>()
            {
                "ProductsShipping",
                "TransactionsSafety",
                "Company",
                "UsefullInfo",
                "FAQ",
                "Terms"
            };

            string redirectTo = "~/B2C/";

            if (CurrentCompany == null || CurrentCompany.OwnerApplicationSettings == null || string.IsNullOrEmpty(page) || validPages.Contains(page) == false)
            {
                return new RedirectResult(redirectTo);
            }

            string propertyName = "B2C" + page;
            PropertyInfo property = CurrentCompany.OwnerApplicationSettings.GetType().GetProperty(propertyName);

            if (property == null)
            {
                propertyName = "Application" + page;
                property = CurrentCompany.OwnerApplicationSettings.GetType().GetProperty(propertyName);
                if (property == null)
                {
                    return new RedirectResult(redirectTo);
                }
            }

            string htmlContent = (string)property.GetValue(CurrentCompany.OwnerApplicationSettings, null);
            if (string.IsNullOrEmpty(htmlContent))
            {
                return new RedirectResult(redirectTo);
            }

            ViewBag.HtmlContent = htmlContent;

            ResourceManager rm = Resources.ResourceManager;
            string title = rm.GetString(page);
            ViewBag.Title = title;
            ViewBag.ActivePage = page;

            return View("Show");
        }

    }
}
