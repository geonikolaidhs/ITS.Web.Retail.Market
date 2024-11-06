using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ITS.Licensing.LicenseModel;
using DevExpress.Data.Filtering;
using DevExpress.Web.Mvc;
using ITS.Licensing.Web.Helpers;

namespace ITS.Licensing.Web.Controllers
{
    public class ValidationRuleController : BaseObjController
    {
        public ActionResult Grid(String serialNumberGuidStr)
        {
            Guid serialNumberGuid;
            if (Guid.TryParse(serialNumberGuidStr, out serialNumberGuid))
            {
                SerialNumber sn = LicenseConnectionHelper.GetNewUnitOfWork().FindObject<SerialNumber>(new BinaryOperator("Oid", serialNumberGuid));
                ViewData["snOid"] = serialNumberGuidStr;
                return PartialView("Grid", sn.ValidationRules);
            }
            return PartialView("Grid", null);
        }

        [HttpPost]
        public ActionResult Update([ModelBinder(typeof(DevExpressEditorsBinder))] ValidationRule ct)
        {            
            ValidationRule validationRule = null;
            if (ModelState.IsValid)
            {
                try
                {
                    validationRule = ct.Session.FindObject<ValidationRule>(new BinaryOperator("Oid", ct.Oid));                    
                    validationRule.limit = ct.limit;
                    validationRule.Save();
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
            ViewData["snOid"] = validationRule.SerialNumber.Oid.ToString();
            return PartialView("Grid",validationRule.SerialNumber.ValidationRules);
        }
    }
}
