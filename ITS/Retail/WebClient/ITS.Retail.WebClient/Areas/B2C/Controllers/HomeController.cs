using ITS.Retail.Model;
using ITS.Retail.WebClient.Areas.B2C.ViewModel;
using ITS.Retail.WebClient.Providers;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web.Mvc;

namespace ITS.Retail.WebClient.Areas.B2C.Controllers
{
    public class HomeController : BaseProductController
    {
        public override ActionResult Index([ModelBinder(typeof(RetailModelBinder))]ProductSearchCriteria criteria)
        {
            ViewBag.searchResultTitle = ResourcesLib.Resources.LatestProducts;
            ViewBag.Title = ResourcesLib.Resources.LatestProducts;
            return base.Index(criteria);
        }

        
        [AllowAnonymous]
        public FileContentResult CompanyLogo()
        {
            if (Session["CompanyLogo"] == null)
            {
                OwnerImage im = null;
                if (im == null && (CurrentCompany.OwnerApplicationSettings != null && CurrentCompany.OwnerApplicationSettings.OwnerImageOid != Guid.Empty))
                {
                    im = XpoSession.GetObjectByKey<OwnerImage>(CurrentCompany.OwnerApplicationSettings.OwnerImageOid);
                }

                if (im != null)
                {
                    ImageConverter converter = new ImageConverter();
                    byte[] imageBytes = (byte[])converter.ConvertTo(im.Image, typeof(byte[]));
                    string format = "";

                    if (im.Image.RawFormat.Equals(ImageFormat.Jpeg))
                    {
                        format = "jpeg";
                    }
                    else if ((im.Image.RawFormat.Equals(ImageFormat.Gif)))
                    {
                        format = "gif";
                    }
                    else if ((im.Image.RawFormat.Equals(ImageFormat.Png)))
                    {
                        format = "png";
                    }

                    Session["CompanyLogo"] = new FileContentResult(imageBytes, "image/" + format);
                }
                else
                {
                    Image defaultImage = Image.FromFile(Server.MapPath("~/Content/B2C/img/icons/its-logo.jpg"));
                    ImageConverter converter = new ImageConverter();

                    byte[] imageBytes = (byte[])converter.ConvertTo(defaultImage, typeof(byte[]));
                    Session["CompanyLogo"] = new FileContentResult(imageBytes, "image/gif");
                }
            }
            return (FileContentResult)Session["CompanyLogo"];
        }

        public ActionResult Pages(string page)
        {
            return View();
        }

    }
}
