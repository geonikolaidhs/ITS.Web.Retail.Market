using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using System.Web;
using ITS.Retail.Model;
using ITS.Retail.WebClient.Controllers;
using ITS.Retail.Common.ViewModel;
using ITS.Retail.Common.Helpers;

namespace ITS.Retail.WebClient.Helpers
{
    public class SessionHelper
    {

        public static void ReloadSessionCommonItems()
        {
            User currentuser = BaseController.CurrentUserStatic;
            Session session = currentuser.Session;
            HttpContext.Current.Session["UserName"] = currentuser.UserName;
            XPCollection<Customer> getUserCustomers = BOApplicationHelper.GetUserEntities<Customer>(session, currentuser);
            Customer user_customer = (getUserCustomers.Count == 0) ? null : getUserCustomers.First<Customer>();
            XPCollection<CompanyNew> getUserSuppliers = BOApplicationHelper.GetUserEntities<CompanyNew>(session, currentuser);
            CompanyNew user_supplier = (getUserSuppliers.Count == 0) ? null : getUserSuppliers.First<CompanyNew>();

            if (user_customer != null && user_customer.CompanyName != null)
            {
                HttpContext.Current.Session["currentCustomer"] = user_customer;
                HttpContext.Current.Session["TraderCompanyName"] = user_customer.CompanyName;
            }
            else if (user_supplier != null && user_supplier.CompanyName != null)
            {
                HttpContext.Current.Session["currentUserSupplier"] = user_supplier;
                HttpContext.Current.Session["TraderCompanyName"] = user_supplier.CompanyName;
            }
            List<StoreViewModel> svms = new List<StoreViewModel>();
            IEnumerable<Store> stores; 
            if ((bool)HttpContext.Current.Session["IsAdministrator"])
            {
                stores = new XPCollection<Store>(currentuser.Session).ToList();
            }
            else
            {
                stores = UserHelper.GetStoresThatUserOwns(currentuser).ToList();
            }
            foreach (Store store in stores)
            {
                StoreViewModel svm = new StoreViewModel();
                svm.LoadPersistent(store);
                svms.Add(svm);
            }
            HttpContext.Current.Session["StoresThatCurrentUserOwns"] = svms;

            stores = UserHelper.GetStoresWhereUserBuysFrom(currentuser);
            List<StoreViewModel> svmsbuy = new List<StoreViewModel>();
            foreach (Store store in stores)
            {
                StoreViewModel svm = new StoreViewModel();
                svm.LoadPersistent(store);
                svmsbuy.Add(svm);
            }
            HttpContext.Current.Session["StoresThatCurrentUserBuysFrom"] = svmsbuy;
        }

    }
}