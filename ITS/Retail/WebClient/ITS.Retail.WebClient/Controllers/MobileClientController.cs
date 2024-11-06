using System;
using System.Web.Mvc;
using ITS.Retail.Model;

namespace ITS.Retail.WebClient.Controllers
{
    public class MobileClientController : BaseObjController<BaseObj>
    {
        //
        // GET: /MobileClient/

        [Security(ReturnsPartial=false)]
        public ActionResult Index()
        {
            return View("Download");
        }

        [Security(ReturnsPartial = false)]
        public ActionResult Download()
        {
            return View();
        }

        [Security(ReturnsPartial = false)]
        public FileContentResult GetSettings()
        {
            String ip = Request.Url.ToString().Replace("/MobileClient/GetSettings", "");
            String config = String.Format("<?xml version=\"1.0\"?><request  errorcode=\"\" errordescr=\"\"><settings><item id=\"ip\">{0}/retailservice.asmx </item><item id=\"PDA_ID\"></item><item id=\"LocalDBPath\"></item><item id=\"Username\"></item><item id=\"Password\"></item></settings></request>",ip);
            FileContentResult fcr = new FileContentResult(System.Text.Encoding.UTF8.GetBytes(config), "application/xml");
            fcr.FileDownloadName = "config.xml";
            return fcr;

        }
    }
}
