using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.Common.ViewModel;
using DevExpress.Xpo;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.WebClient.Controllers
{
    public class CouponCategoryController : BaseObjController<CouponCategory>
    {
        public ActionResult Index()
        {
            this.ToolbarOptions.ExportButton.Visible = false;
            this.ToolbarOptions.ExportToButton.Visible = false;

            bool isRetailInstance = MvcApplication.ApplicationInstance == eApplicationInstance.RETAIL || MvcApplication.ApplicationInstance == eApplicationInstance.DUAL_MODE;

            this.ToolbarOptions.NewButton.Visible = isRetailInstance;
            this.ToolbarOptions.NewButton.OnClick = "AddNewFromGrid";
            this.ToolbarOptions.EditButton.Visible = isRetailInstance;
            this.ToolbarOptions.EditButton.OnClick = "EditSelectedRowsFromGrid";
            this.ToolbarOptions.OptionsButton.Visible = false;
            this.ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            this.ToolbarOptions.ViewButton.Visible = true;
            this.ToolbarOptions.ViewButton.OnClick = "ShowGenericView";
            this.ToolbarOptions.CustomButton.Visible = false;
            this.CustomJSProperties.AddJSProperty("gridName", "grdCouponCategories");

            XPCollection<CouponCategory> couponCategories = GetList<CouponCategory>(XpoHelper.GetNewUnitOfWork());
            return View(couponCategories);
        }

        public override ActionResult Grid()
        {
            return base.Grid();
        }

        public ActionResult Edit(Guid? Oid)
        {
            if( !Oid.HasValue)
            {
                return Json(new { error = Resources.AnErrorOccurred });
            }

            CouponCategory couponCategory = null;
            if (Oid.Value == Guid.Empty)
            {
                couponCategory = new CouponCategory(XpoSession);
                couponCategory.Owner = XpoSession.GetObjectByKey<CompanyNew>(EffectiveOwner.Oid);
            }
            else
            {
                couponCategory = XpoSession.GetObjectByKey<CouponCategory>(Oid.Value);
                if( couponCategory == null )
                {
                    return Json(new { error = Resources.InvalidValue });
                }
            }
            CouponCategoryViewModel couponCategoryViewModel = new CouponCategoryViewModel();
            couponCategoryViewModel.LoadPersistent(couponCategory);
            return PartialView(couponCategoryViewModel);
        }

        protected override GenericViewRuleset GenerateGenericViewRuleset()
        {
            GenericViewRuleset ruleset = base.GenerateGenericViewRuleset();
            ruleset.DetailsToIgnore.Add("Coupons");
            ruleset.PropertiesToIgnore.AddRange(new List<string>() { "IsDefault", "IsActive" });
            return ruleset;
        }

        public JsonResult Save(CouponCategoryViewModel couponCategoryViewModel)
        {
            try
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    CouponCategory couponCategory = uow.GetObjectByKey<CouponCategory>(couponCategoryViewModel.Oid);
                    if (couponCategory == null)
                    {
                        couponCategory = new CouponCategory(uow);
                    }
                    couponCategoryViewModel.Persist<CouponCategory>(couponCategory);
                    couponCategory.Owner = uow.GetObjectByKey<CompanyNew>(couponCategoryViewModel.Owner);
                    couponCategory.Save();
                    XpoHelper.CommitTransaction(couponCategory.Session);
                }
                return Json(new { success = true, message = Resources.SavedSuccesfully });
            }
            catch (Exception exception)
            {
                return Json(new { success = false, message = exception.Message });
            }

        }
    }
}
