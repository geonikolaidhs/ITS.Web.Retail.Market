using ITS.Retail.Model;
using ITS.Retail.WebClient.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ITS.Retail.WebClient.Controllers
{
    [AllowAnonymous]
    [LicensedAuthorize]
    public class AboutController : BaseController
    {
        [OutputCache(Duration = 10, Location = System.Web.UI.OutputCacheLocation.Server)]
        public ActionResult Index()
        {
            //ViewBag.Owner = EffectiveOwner;

            if (!String.IsNullOrWhiteSpace(ViewBag.Owner.OwnerApplicationSettings.LocationGoogleID))
            {
                string location = ViewBag.Owner.OwnerApplicationSettings.LocationGoogleID.Trim();
                string[] locationArray = location.Split(',');
                this.CustomJSProperties.AddJSProperty("companyLatitute", locationArray[0]);
                this.CustomJSProperties.AddJSProperty("companyLongtitude", locationArray[1]);
            }
            else
            {

                this.CustomJSProperties.AddJSProperty("companyLatitute", null);
                this.CustomJSProperties.AddJSProperty("companyLongtitude", null);
            }

            System.Text.StringBuilder mapContent = new System.Text.StringBuilder();

            if (!String.IsNullOrWhiteSpace(ViewBag.Owner.CompanyName))
            {
                mapContent.Append("<p><b>").Append(ViewBag.Owner.CompanyName).Append("</b></p>");

                this.CustomJSProperties.AddJSProperty("companyName", ViewBag.Owner.CompanyName);
            }
            else
            {
                this.CustomJSProperties.AddJSProperty("companyName", "");
            }

            if (ViewBag.Owner.DefaultAddress != null)
            {

                mapContent.Append("<p>");
                if (!String.IsNullOrWhiteSpace(ViewBag.Owner.DefaultAddress.Street))
                {
                    mapContent.Append(ViewBag.Owner.DefaultAddress.Street);
                }

                if (!String.IsNullOrWhiteSpace(ViewBag.Owner.DefaultAddress.PostCode))
                {
                    mapContent.Append(", ").Append(ViewBag.Owner.DefaultAddress.PostCode);
                }
                mapContent.Append("</p>");
                if (!String.IsNullOrWhiteSpace(ViewBag.Owner.DefaultAddress.City))
                {
                    mapContent.Append("<p>").Append(ViewBag.Owner.DefaultAddress.City).Append("</p>");
                }
            }

            if (ViewBag.Owner.OwnerApplicationSettings != null)
            {
                mapContent.Append("<p>");
                if (!String.IsNullOrWhiteSpace(ViewBag.Owner.OwnerApplicationSettings.Phone))
                {
                    mapContent.Append("T: ").Append(ViewBag.Owner.OwnerApplicationSettings.Phone);
                }
                if (!String.IsNullOrWhiteSpace(ViewBag.Owner.OwnerApplicationSettings.FAX))
                {
                    mapContent.Append(", F: ").Append(ViewBag.Owner.OwnerApplicationSettings.FAX);
                }
                if (!String.IsNullOrWhiteSpace(ViewBag.Owner.OwnerApplicationSettings.eMail))
                {
                    mapContent.Append(", E: ").Append(ViewBag.Owner.OwnerApplicationSettings.eMail);
                }
                mapContent.Append("</p>");
            }
            this.CustomJSProperties.AddJSProperty("mapContent", mapContent);
            return View();
        }

        [OutputCache(Duration = 10, Location = System.Web.UI.OutputCacheLocation.Server)]
        public ActionResult ItsSA()
        {
            return View();
        }

    }
}
