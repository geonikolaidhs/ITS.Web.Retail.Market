using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.Xpo;
using ITS.Licensing.LicenseModel;
using DevExpress.Web.Mvc;
using DevExpress.Data.Filtering;
using ITS.Licensing.Web.Helpers;

namespace ITS.Licensing.Web.Controllers
{
    public class ApplicationController : BaseObjController
    {
        //
        // GET: /Application/

        public ActionResult Index()
        {
            return View(new XPCollection<ApplicationInfo>(LicenseConnectionHelper.GetNewUnitOfWork()));
        }

        public ActionResult Grid()
        {
            return PartialView(new XPCollection<ApplicationInfo>(LicenseConnectionHelper.GetNewUnitOfWork()));
        }

        public ActionResult AddNewPartial([ModelBinder(typeof(DevExpressEditorsBinder))] ApplicationInfo ct)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ct.Save();
                    ct.Session.CommitTransaction();
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
            return PartialView("Grid", new XPCollection<ApplicationInfo>(LicenseConnectionHelper.GetNewUnitOfWork()));
        }

        [HttpPost]
        public ActionResult UpdatePartial([ModelBinder(typeof(DevExpressEditorsBinder))] ApplicationInfo ct){

            if (ModelState.IsValid)
            {
                try
                {
                    using (UnitOfWork uow = LicenseConnectionHelper.GetNewUnitOfWork())
                    {
                        ApplicationInfo appInfo = uow.FindObject<ApplicationInfo>(new BinaryOperator("Oid", ct.Oid));
                        appInfo.Name = ct.Name;
                        appInfo.ApplicationOid = ct.ApplicationOid;
                        appInfo.Save();
                        uow.CommitChanges();
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
            return PartialView("Grid", new XPCollection<ApplicationInfo>(LicenseConnectionHelper.GetNewUnitOfWork()));
        }
        public ActionResult DeletePartial([ModelBinder(typeof(DevExpressEditorsBinder))] ApplicationInfo ct){
            if (ModelState.IsValid)
            {
                try
                {
                    using (UnitOfWork uow = LicenseConnectionHelper.GetNewUnitOfWork())
                    {
                        ApplicationInfo appInfo = uow.FindObject<ApplicationInfo>(new BinaryOperator("Oid", ct.Oid));
                        appInfo.Delete();
                        uow.CommitTransaction();
                        uow.PurgeDeletedObjects();
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
            return PartialView("Grid", new XPCollection<ApplicationInfo>(LicenseConnectionHelper.GetNewUnitOfWork()));
        }
    }
}
