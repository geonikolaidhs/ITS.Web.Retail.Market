using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ITS.Licensing.LicenseModel;
using ITS.Licensing.Web.Helpers;

namespace ITS.Licensing.Web.Controllers
{
    public class ApplicationUserController : BaseObjController
    {
        public ActionResult Grid(string serialNumberGuidStr)
        {
            ViewData["snOid"] = serialNumberGuidStr;
            Guid snGuid = Guid.Empty;
            ViewData["ApplicationUsers"] = null;
            if(Guid.TryParse(serialNumberGuidStr,out snGuid)){
                SerialNumber sn = LicenseConnectionHelper.GetNewUnitOfWork().GetObjectByKey<SerialNumber>(snGuid);
                ViewData["ApplicationUsers"] = sn.ApplicationUsers;
            }
            return PartialView("Grid",ViewData["ApplicationUsers"]);
        }

    }
}
