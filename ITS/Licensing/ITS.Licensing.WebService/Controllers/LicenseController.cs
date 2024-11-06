using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DevExpress.Xpo;
using ITS.Licensing.LicenseModel;
using DevExpress.Data.Filtering;
using DevExpress.Web.Mvc;
using ITS.Licensing.Library;
using ITS.Licensing.Web.Helpers;

namespace ITS.Licensing.Web.Controllers
{
    public class LicenseController : BaseObjController
    {
        //
        // GET: /License/

        //public ActionResult Index()
        //{
        //    return View();
        //}

        public ActionResult Grid(String serialNumberGuidStr)
        {
            Guid serialNumberGuid;
            if (Guid.TryParse(serialNumberGuidStr, out serialNumberGuid))
            {
                SerialNumber sn = LicenseConnectionHelper.GetNewUnitOfWork().FindObject<SerialNumber>(new BinaryOperator("Oid", serialNumberGuid));
                ViewData["snOid"] = serialNumberGuidStr;                
                return PartialView("Grid", sn.Licences);
            }
            return PartialView("Grid", null);
        }

        public ActionResult Add([ModelBinder(typeof(DevExpressEditorsBinder))] License ct)
        {
            SerialNumber sn = null;
            using (UnitOfWork uow = LicenseConnectionHelper.GetNewUnitOfWork())
            {
                sn = uow.FindObject<SerialNumber>(new BinaryOperator("Oid", Request["DXCallbackName"].Replace("grdLicense", "")));
                if (sn != null)
                {
                    License newLic = new License(uow);
                    newLic.SerialNumber = sn;
                    newLic.MachineID = ct.MachineID;
                    newLic.InstalledVersionDateTime = ct.InstalledVersionDateTime;
                    LicenseExtended licExtended = new LicenseExtended(newLic);
                    newLic.ActivationKey = licExtended.ActivationKey;
                    newLic.Save();
                    uow.CommitChanges();
                    ViewData["snOid"] = sn.Oid.ToString();
                }
            }
            return PartialView("Grid", sn == null ? null : LicenseConnectionHelper.GetNewUnitOfWork().FindObject<SerialNumber>(new BinaryOperator("Oid", sn.Oid)).Licences);
        }
    }
}
