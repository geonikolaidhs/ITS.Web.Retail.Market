using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ITS.Licensing.LicenseModel;
using DevExpress.Web.Mvc;
using DevExpress.Xpo;
using ITS.Licensing.Web.Helpers;

namespace ITS.Licensing.Web.Controllers
{
    public class UserRuleController : BaseObjController
    {
        public ActionResult Grid(String serialNumberGuidStr)
        {
            Guid serialNumberGuid;
            if (Guid.TryParse(serialNumberGuidStr, out serialNumberGuid))
            {
                SerialNumber sn = LicenseConnectionHelper.GetNewUnitOfWork().GetObjectByKey<SerialNumber>( serialNumberGuid );
                ViewData["snOid"] = serialNumberGuidStr;
                return PartialView("Grid", sn.UserRules);
            }
            return PartialView("Grid", null);
        }

        [HttpPost]
        public ActionResult Update([ModelBinder(typeof(DevExpressEditorsBinder))] UserRule ct)
        {
            UserRule userRule = null;
            if (ModelState.IsValid)
            {
                try
                {
                    userRule = ct.Session.GetObjectByKey<UserRule>(ct.Oid);
                    userRule.UserType = ct.UserType;
                    userRule.Limit= ct.Limit;
                    userRule.Save();
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }
            else
            {
                Session["Error"] = "AnErrorOccurred";
                ViewBag.CurrentItem = ct;
            }
            ViewData["snOid"] = userRule.SerialNumber.Oid.ToString();
            return PartialView("Grid", userRule.SerialNumber.UserRules);
        }

        
        /*
        public ActionResult Add([ModelBinder(typeof(DevExpressEditorsBinder))] UserRule ct, String serialNumberGuidStr)
        {[HttpPost]
            if (ModelState.IsValid)
            {
                try
                {
                    Guid serialNumberGuid;
                    if (Guid.TryParse(serialNumberGuidStr, out serialNumberGuid))
                    {
                        SerialNumber sn = ct.Session.GetObjectByKey<SerialNumber>(serialNumberGuid);
                        ct.SerialNumber = sn;
                        ct.Save();
                    }
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }
            else
            {
                Session["Error"] = "AnErrorOccurred";
                ViewBag.CurrentItem = ct;
            }
            ViewData["snOid"] = serialNumberGuidStr;
            return PartialView("Grid", ct.SerialNumber.UserRules);
        }*/
    }
}
