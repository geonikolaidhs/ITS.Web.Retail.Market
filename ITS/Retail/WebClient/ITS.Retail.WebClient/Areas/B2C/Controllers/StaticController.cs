using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.WebClient.Extensions;
using ITS.Retail.WebClient.Helpers;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ITS.Retail.WebClient.Areas.B2C.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    [ActionLog(LogLevel = LogLevel.None)]
    public class StaticController : Controller
    {

        private CompanyNew _CurrentCompany;
        /// <summary>
        /// Use ONLY when there is Request. There are also things that need consideration. See Case http://www.its.net.gr/fb/default.asp?5473
        /// </summary>
        public CompanyNew CurrentCompany
        {
            get
            {
                if (_CurrentCompany == null)
                {
                    _CurrentCompany = B2CHelper.GetB2CCompany(Request["SERVER_NAME"], XpoHelper.GetNewUnitOfWork());                    
                }
                return _CurrentCompany;
            }
        }


        [OutputCache(Duration = 3600, VaryByParam = "Id;ticks;imageSize", Location = System.Web.UI.OutputCacheLocation.Server)]
        [ActionLog(LogLevel = LogLevel.None)]
        public FileContentResult ShowImageId(Guid? Id, long ticks, int imageSize = 0)
        {
            ImageConverter converter = new ImageConverter();
            byte[] imageBytes = null;
            string format = "jpg";
            if (Id.HasValue)
            {
                using (UnitOfWork XpoSession = XpoHelper.GetNewUnitOfWork())
                {
                    Item it = XpoSession.GetObjectByKey<Item>(Id.Value);
                    if (it != null)
                    {
                        Image img;
                        switch (imageSize)
                        {
                            case 1:
                                img = it.ImageMedium;
                                break;
                            case 2:
                                img = it.ImageLarge;
                                break;
                            default:
                                img = it.ImageSmall;
                                break;
                        }
                        if (img != null)
                        {
                            imageBytes = (byte[])converter.ConvertTo(img, typeof(byte[]));

                            if (img.RawFormat.Equals(ImageFormat.Jpeg))
                            {
                                format = "jpeg";
                            }
                            else if ((img.RawFormat.Equals(ImageFormat.Gif)))
                            {
                                format = "gif";
                            }
                            else if ((img.RawFormat.Equals(ImageFormat.Png)))
                            {
                                format = "png";
                            }
                        }
                    }
                }
                if (imageBytes == null)
                {
                    Image defaultImage = Image.FromFile(Server.MapPath("~/Content/B2C/img/noImg.jpg"));
                    imageBytes = (byte[])converter.ConvertTo(defaultImage, typeof(byte[]));
                }
                return new FileContentResult(imageBytes, "image/" + format);
            }
            return null;
        }

        [OutputCache(Duration = 3600, Location = System.Web.UI.OutputCacheLocation.Server)]
        [ActionLog(LogLevel = LogLevel.None)]
        public FileContentResult CompanyLogo()
        {
            FileContentResult result = null;
            using (UnitOfWork XpoSession = XpoHelper.GetNewUnitOfWork())
            {
                OwnerImage im = null;
                if (im == null && (CurrentCompany.OwnerApplicationSettings != null && !CurrentCompany.OwnerApplicationSettings.OwnerImageOid.Equals(Guid.Empty)))
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

                    result = new FileContentResult(imageBytes, "image/" + format);
                }
                else
                {
                    Image defaultImage = Image.FromFile(Server.MapPath("~/Content/B2C/img/icons/its-logo.jpg"));
                    ImageConverter converter = new ImageConverter();

                    byte[] imageBytes = (byte[])converter.ConvertTo(defaultImage, typeof(byte[]));
                    result = new FileContentResult(imageBytes, "image/gif");
                }
            }
            return result;
        }
    }
}
