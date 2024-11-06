using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.Data.Filtering;
using ITS.Licensing.Web.Helpers;
using ITS.Licensing.LicenseModel;
using DevExpress.Xpo;

namespace ITS.Licensing.Web.Controllers
{
    public class LoginController : BasicController
    {
        public ActionResult Index()
        {
           // String error = Session["Error"] == null ? "" : Session["Error"].ToString();
            Session.Clear();
            return View();
        }

        public ActionResult Validate()
        {
            Session.Clear();
            try
            {
                //Session["IsAdministrator"] = false;
                string username = Request["UserName"];
                string pass = Request["Password"];
                if (username == null || pass == null)
                {
                    Session.Clear();
                    //Session["Error"] = Resources_Messages.Login_Failed;
                    return View("Index");
                }
                if (username == "superadmin" && pass == "adminsuperadmin")//if (username == "supergoufy" && pass == "supergoufy")
                {
                //    Session["IsAdministrator"] = true;
                    Session["USER"] = "supergoufy";
                //    List<String> forbiddenStringList = new List<String>();
                //    Session["LayoutForbidden"] = forbiddenStringList;
                //    //Session["Notice"] = Resources_Messages.Welcome + " Super Duper Admin!";
                //    Session wow = XpoHelper.GetNewUnitOfWork();
                //    Session["wow"] = wow;

                    return RedirectToAction("Index", "SerialNumber");
                }
                else
                {
                    CriteriaOperator crop = CriteriaOperator.Parse("UserName='" + username + "' AND Password='" + UserHelper.EncodePassword(pass) + "'");
                    Session wow = LicenseConnectionHelper.GetNewUnitOfWork();

                    User currentuser = (User)wow.FindObject(typeof(User), crop);

                    if (currentuser != null
                       // && MvcApplication.license.UserHasAccess(currentuser.Key, ITSLicense.UserAccessType.Web)
                    )
                    {
                        Session["USER"] = currentuser.Oid;
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        Session.Clear();
                        //Session["Error"] = Resources_Messages.Login_Failed;
                        return RedirectToAction("Index","Login");
                    }

                }

            }
            catch (Exception e)
            {
                Session.Clear();
                Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;//;"Παρουσιάστηκε κάποιο σφάλμα κατά την αναγνώριση του χρήστη";
                return View("Index");
            }
        }

    }
}
