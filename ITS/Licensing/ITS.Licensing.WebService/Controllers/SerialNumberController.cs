using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ITS.Licensing.LicenseModel;
using DevExpress.Web.Mvc;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.Licensing.Web.Helpers;

namespace ITS.Licensing.Web.Controllers
{
    public class SerialNumberController : BaseObjController
    {
        //
        // GET: /SerialNumber/

        public ActionResult Index()
        {
            return View(new XPCollection<SerialNumber>(LicenseConnectionHelper.GetNewUnitOfWork()));
        }

        public ActionResult Grid()
        {
            return PartialView(new XPCollection<SerialNumber>(LicenseConnectionHelper.GetNewUnitOfWork()));
        }


        public ActionResult Add([ModelBinder(typeof(DevExpressEditorsBinder))] SerialNumber ct)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Guid appOid, custOid;
                    appOid = Guid.Parse(Request["Application!Key"].Substring(1, Request["Application!Key"].Length - 2));
                    custOid = Guid.Parse(Request["Customer!Key"].Substring(1, Request["Customer!Key"].Length - 2));
                    ct.Application = ct.Session.FindObject<ApplicationInfo>(new BinaryOperator("Oid", appOid));
                    ct.Customer = ct.Session.FindObject<Customer>(new BinaryOperator("Oid", custOid));
                    ct.Number = ITS.Licensing.Library.LicenseLib.GenerateKey(ct.Application.ApplicationOid);

                    foreach(Rule rule in ct.Application.Rules){
                        ValidationRule validationRule = new ValidationRule(ct.Session);
                        validationRule.Rule = rule;
                        validationRule.limit = 0;//TODO τι θα γίνει εδώ;
                        ct.ValidationRules.Add(validationRule);
                    }

                    foreach (ITS.Licensing.ClientLibrary.ITSLicense.UserAccessType val in Enum.GetValues(typeof(ITS.Licensing.ClientLibrary.ITSLicense.UserAccessType))  )
                    {
                        UserRule user_rule = new UserRule(ct.Session);
                        user_rule.UserType = val.ToString();
                        user_rule.Limit = 0;
                        ct.UserRules.Add(user_rule);
                    }

                    ct.Save();
                    ct.Session.CommitTransaction();
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                    ViewBag.CurrentItem = ct;
                }
            }
            else
            {
                Session["Error"] = "AnErrorOccurred";
                ViewBag.CurrentItem = ct;
            }
            return PartialView("Grid", new XPCollection<SerialNumber>(LicenseConnectionHelper.GetNewUnitOfWork()));
            
        }

        [HttpPost]
        public ActionResult Update([ModelBinder(typeof(DevExpressEditorsBinder))] SerialNumber ct)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    using (UnitOfWork uow = LicenseConnectionHelper.GetNewUnitOfWork())
                    {

                        SerialNumber appInfo = uow.FindObject<SerialNumber>(new BinaryOperator("Oid", ct.Oid));
                        Guid appOid, custOid;
                        appOid = Guid.Parse(Request["Application!Key"].Substring(1, Request["Application!Key"].Length - 2));
                        custOid = Guid.Parse(Request["Customer!Key"].Substring(1, Request["Customer!Key"].Length - 2));
                        ct.Application = ct.Session.FindObject<ApplicationInfo>(new BinaryOperator("Oid", appOid));
                        ct.Customer = ct.Session.FindObject<Customer>(new BinaryOperator("Oid", custOid));
                        LicenseConnectionHelper.CopyObject(ct, ref appInfo);
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
            return PartialView("Grid", new XPCollection<SerialNumber>(LicenseConnectionHelper.GetNewUnitOfWork()));
        }
        public ActionResult Delete([ModelBinder(typeof(DevExpressEditorsBinder))] SerialNumber ct)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (UnitOfWork uow = LicenseConnectionHelper.GetNewUnitOfWork())
                    {
                        SerialNumber appInfo = uow.FindObject<SerialNumber>(new BinaryOperator("Oid", ct.Oid));
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
            return PartialView("Grid", new XPCollection<SerialNumber>(LicenseConnectionHelper.GetNewUnitOfWork()));
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            ViewBag.Applications = new XPCollection<ApplicationInfo>(LicenseConnectionHelper.GetNewUnitOfWork());
            ViewBag.Customers = new XPCollection<Customer>(LicenseConnectionHelper.GetNewUnitOfWork());
            base.OnActionExecuted(filterContext);
        }
    }
}
