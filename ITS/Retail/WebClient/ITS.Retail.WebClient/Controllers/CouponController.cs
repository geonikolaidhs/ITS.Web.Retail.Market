using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.ResourcesLib;
using DevExpress.Xpo;
using DevExpress.Xpo.DB.Exceptions;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;
using ITS.Retail.Common.ViewModel;
using ITS.Retail.Platform.Common.ViewModel;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.WebClient.Controllers
{
    public class CouponController : BaseObjController<Coupon>
    {
        public ActionResult Index()
        {
            this.ToolbarOptions.NewButton.Visible = false;
            this.ToolbarOptions.ExportButton.Visible = false;
            this.ToolbarOptions.ExportToButton.Visible = false;

            bool isRetailInstance = MvcApplication.ApplicationInstance == eApplicationInstance.RETAIL || MvcApplication.ApplicationInstance == eApplicationInstance.DUAL_MODE;

            this.ToolbarOptions.EditButton.Visible = isRetailInstance;
            this.ToolbarOptions.EditButton.OnClick = "EditSelectedRowsCustomV2";
            this.ToolbarOptions.OptionsButton.Visible = isRetailInstance;
            this.ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            this.ToolbarOptions.ViewButton.Visible = true;
            this.ToolbarOptions.ViewButton.OnClick = "ShowGenericView";
            this.ToolbarOptions.CustomButton.Visible = isRetailInstance;
            this.ToolbarOptions.CustomButton.CCSClass = "coupon";
            this.ToolbarOptions.CustomButton.Title = Resources.MassivelyCreateCoupons;
            this.ToolbarOptions.CustomButton.OnClick = "function(){Coupon.ShowMassivelyCreateCouponsForm('" + Url.Action("MassivelyCreateCoupons") + "')}";

            this.CustomJSProperties.AddJSProperty("gridName", "grdCoupons");
            return View(GetList<Coupon>(XpoSession));
        }

        public override ActionResult Grid()
        {
            return base.Grid();
        }

        public ActionResult MassivelyCreateCoupons()
        {
            bool isRetailInstance = MvcApplication.ApplicationInstance == eApplicationInstance.RETAIL || MvcApplication.ApplicationInstance == eApplicationInstance.DUAL_MODE;
            if (!isRetailInstance)
            {
                return RedirectToAction("Index", "Coupon");
            }
            FillLookupComboBoxes();
            return View();
        }

        protected override void FillLookupComboBoxes()
        {
            ViewBag.DiscountTypes = GetList<DiscountType>(XpoHelper.GetNewUnitOfWork(), ApplyOwnerCriteria(null, typeof(DiscountType)));
            ViewBag.PaymentMethods = GetList<PaymentMethod>(XpoHelper.GetNewUnitOfWork(), ApplyOwnerCriteria(null, typeof(PaymentMethod)));
            ViewBag.CouponCategories = GetList<CouponCategory>(XpoHelper.GetNewUnitOfWork(), ApplyOwnerCriteria(null, typeof(CouponCategory)));
        }

        public JsonResult CreateCoupons(MassivelyCreateCouponsViewModel massivelyCreateCouponsViewModel)
        {
            bool isRetailInstance = MvcApplication.ApplicationInstance == eApplicationInstance.RETAIL || MvcApplication.ApplicationInstance == eApplicationInstance.DUAL_MODE;
            if (!isRetailInstance)
            {
                return Json(new { success = false });
            }

            string errorMessage = string.Empty;


            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                Guid discountTypeGuid, paymentMethodGuid, couponCategoryGuid;
                if (Guid.TryParse(Request["DiscountType_VI"], out discountTypeGuid))
                {
                    massivelyCreateCouponsViewModel.DiscountType = uow.GetObjectByKey<DiscountType>(discountTypeGuid);
                }
                if (Guid.TryParse(Request["PaymentMethod_VI"], out paymentMethodGuid))
                {
                    massivelyCreateCouponsViewModel.PaymentMethod = uow.GetObjectByKey<PaymentMethod>(paymentMethodGuid);
                }
                if (Guid.TryParse(Request["CouponCategory_VI"], out couponCategoryGuid))
                {
                    massivelyCreateCouponsViewModel.CouponCategory = uow.GetObjectByKey<CouponCategory>(couponCategoryGuid);
                }

                if (!massivelyCreateCouponsViewModel.IsValid(out errorMessage))
                {
                    return Json(new { success = false, message = errorMessage });
                }
                try
                {
                    CompanyNew owner = uow.GetObjectByKey<CompanyNew>(EffectiveOwner.Oid);
                    string prefix = massivelyCreateCouponsViewModel.Prefix ?? string.Empty;
                    string suffix = massivelyCreateCouponsViewModel.Suffix ?? string.Empty;
                    for (int code = massivelyCreateCouponsViewModel.StartingNumber; code <= massivelyCreateCouponsViewModel.EndingNumber; code++)
                    {
                        string kode = code.ToString();
                        if (massivelyCreateCouponsViewModel.UsePadding)
                        {
                            if (massivelyCreateCouponsViewModel.PaddingDirection == Platform.Enumerations.PaddingDirection.LEFT)
                            {
                                kode = kode.PadLeft(massivelyCreateCouponsViewModel.PaddingLength, massivelyCreateCouponsViewModel.PaddingCharacter[0]);
                            }
                            if (massivelyCreateCouponsViewModel.PaddingDirection == Platform.Enumerations.PaddingDirection.RIGHT)
                            {
                                kode = kode.PadRight(massivelyCreateCouponsViewModel.PaddingLength, massivelyCreateCouponsViewModel.PaddingCharacter[0]);
                            }
                        }

                        Coupon coupon = new Coupon(uow)
                        {
                            Owner = owner,
                            Code = prefix + kode + suffix,
                            IsActive = true,
                            IsActiveFromDate = massivelyCreateCouponsViewModel.IsActiveFromDate,
                            IsActiveUntilDate = massivelyCreateCouponsViewModel.IsActiveUntilDate,
                            IsUnique = massivelyCreateCouponsViewModel.IsUnique,
                            CouponAmountIsAppliedAs = massivelyCreateCouponsViewModel.CouponAmountIsAppliedAs,
                            CouponAmountType = massivelyCreateCouponsViewModel.CouponAmountType,
                            CouponAppliesOn = massivelyCreateCouponsViewModel.CouponAppliesOn,
                            Amount = massivelyCreateCouponsViewModel.Amount,
                            DiscountType = massivelyCreateCouponsViewModel.DiscountType,
                            PaymentMethod = massivelyCreateCouponsViewModel.PaymentMethod,
                            CouponCategory = massivelyCreateCouponsViewModel.CouponCategory,
                        };
                        coupon.Save();
                    }

                    XpoHelper.CommitTransaction(uow);
                }
                catch (ConstraintViolationException ex)
                {
                    string exceptionMessage = ex.GetFullMessage();
                    return Json(new { success = false, message = Resources.CodeAlreadyExists });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }

            return Json(new { success = true, message = Resources.SavedSuccesfully });
        }

        public override ActionResult LoadEditPopup()
        {
            base.LoadEditPopup();

            ViewBag.Title = Resources.EditCoupon;

            ActionResult rt = PartialView("LoadEditPopup");
            return rt;
        }

        public ActionResult Edit(Guid? Oid)
        {
            Coupon coupon = null;
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            if (Oid.HasValue)
            {
                coupon = uow.GetObjectByKey<Coupon>(Oid.Value);
            }
            else
            {
                coupon = new Coupon(uow);
            }

            FillLookupComboBoxes();

            CouponViewModel couponViewModel = new CouponViewModel()
            {
                Oid = coupon.Oid,
                Code = coupon.Code,
                Owner = coupon.Owner.Oid,
                Description = coupon.Description,
                IsActiveUntilDate = coupon.IsActiveUntilDate,
                IsActiveFromDate = coupon.IsActiveFromDate,
                NumberOfTimesUsed = coupon.NumberOfTimesUsed,
                IsUnique = coupon.IsUnique,
                CouponAppliesOn = coupon.CouponAppliesOn,
                CouponAmountType = coupon.CouponAmountType,
                CouponAmountIsAppliedAs = coupon.CouponAmountIsAppliedAs,
                Amount = coupon.Amount,
                DiscountType = coupon.DiscountType == null ? Guid.Empty : coupon.DiscountType.Oid,
                PaymentMethod = coupon.PaymentMethod == null ? Guid.Empty : coupon.PaymentMethod.Oid,
                CouponCategory = coupon.CouponCategory == null ? Guid.Empty : coupon.CouponCategory.Oid
            };
            return PartialView(couponViewModel);
        }

        public JsonResult Save(CouponViewModel couponViewModel)
        {
            try
            {
                string errorResource;
                if (couponViewModel.IsValid(out errorResource) == false)
                {
                   return Json(new { success = false, error = Resources.ResourceManager.GetString(errorResource) ?? Resources.InvalidValue });
                }

                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    Coupon coupon = uow.GetObjectByKey<Coupon>(couponViewModel.Oid);
                    if (coupon == null)
                    {
                        coupon = new Coupon(uow);
                    }

                    coupon.Oid = couponViewModel.Oid;
                    coupon.Code = couponViewModel.Code;
                    coupon.Owner = uow.GetObjectByKey<CompanyNew>(couponViewModel.Owner);
                    coupon.Description = couponViewModel.Description;
                    coupon.IsActiveUntilDate = couponViewModel.IsActiveUntilDate;
                    coupon.IsActiveFromDate = couponViewModel.IsActiveFromDate;
                    coupon.IsUnique = couponViewModel.IsUnique;
                    coupon.CouponAppliesOn = couponViewModel.CouponAppliesOn;
                    coupon.CouponAmountType = couponViewModel.CouponAmountType;
                    coupon.CouponAmountIsAppliedAs = couponViewModel.CouponAmountIsAppliedAs;
                    coupon.Amount = couponViewModel.Amount;
                    coupon.DiscountType = uow.GetObjectByKey<DiscountType>(couponViewModel.DiscountType);
                    coupon.PaymentMethod = uow.GetObjectByKey<PaymentMethod>(couponViewModel.PaymentMethod);
                    coupon.CouponCategory = uow.GetObjectByKey<CouponCategory>(couponViewModel.CouponCategory);
                    coupon.Save();
                    XpoHelper.CommitTransaction(coupon.Session);
                }
                return Json(new { success = true, message = Resources.SavedSuccesfully });
            }
            catch (Exception exception)
            {
                return Json(new { success = false, message = exception.Message });
            }
        }

        protected override GenericViewRuleset GenerateGenericViewRuleset()
        {
            GenericViewRuleset ruleset = base.GenerateGenericViewRuleset();
            ruleset.PropertiesToIgnore.AddRange(new List<string>() { "IsDefault", "IsActive" });
            return ruleset;
        }
    }
}
