using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.Xpo;
using DevExpress.Web.Mvc;
using ITS.Licensing.LicenseModel;
using DevExpress.Data.Filtering;
using ITS.Licensing.Web.Helpers;

namespace ITS.Licensing.Web.Controllers
{
    public class CustomerController : BaseObjController
    {
        //
        // GET: /Application/

        public ActionResult Index()
        {
            return View(new XPCollection<Customer>(LicenseConnectionHelper.GetNewUnitOfWork()));
        }

        public ActionResult Grid()
        {
            return PartialView(new XPCollection<Customer>(LicenseConnectionHelper.GetNewUnitOfWork()));
        }

        public ActionResult AddNewPartial([ModelBinder(typeof(DevExpressEditorsBinder))] Customer ct)
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
            return PartialView("Grid", new XPCollection<Customer>(LicenseConnectionHelper.GetNewUnitOfWork()));
        }

        [HttpPost]
        public ActionResult UpdatePartial([ModelBinder(typeof(DevExpressEditorsBinder))] Customer ct)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    using (UnitOfWork uow = LicenseConnectionHelper.GetNewUnitOfWork())
                    {
                        Customer customer = uow.FindObject<Customer>(new BinaryOperator("Oid", ct.Oid));
                        customer.CompanyName = ct.CompanyName;
                        customer.Save();
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
            return PartialView("Grid", new XPCollection<Customer>(LicenseConnectionHelper.GetNewUnitOfWork()));
        }
        public ActionResult DeletePartial([ModelBinder(typeof(DevExpressEditorsBinder))] Customer ct)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (UnitOfWork uow = LicenseConnectionHelper.GetNewUnitOfWork())
                    {
                        Customer customer = uow.FindObject<Customer>(new BinaryOperator("Oid", ct.Oid));
                        customer.Delete();
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
            return PartialView("Grid", new XPCollection<Customer>(LicenseConnectionHelper.GetNewUnitOfWork()));
        }
    }
}
