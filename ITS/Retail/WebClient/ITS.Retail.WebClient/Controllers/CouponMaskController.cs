using DevExpress.Data.Filtering;
using DevExpress.Web;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Reflection;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.WebClient.Controllers
{
    public class CouponMaskController : BaseObjController<CouponMask>
    {
        private static readonly Dictionary<PropertyInfo, String> propertyMapping = new Dictionary<PropertyInfo, string>()
        {
            { typeof(CouponMask).GetProperty("DiscountType"), "discountTypesCallbackPanel_VI" },
            { typeof(CouponMask).GetProperty("PaymentMethod"), "paymentMethodsCallbackPanel_VI" },
            { typeof(CouponMask).GetProperty("CouponCategory"), "couponCategoriesCallbackPanel_VI" }
        };

        protected override Dictionary<PropertyInfo, string> PropertyMapping
        {
            get
            {
                return propertyMapping;
            }
        }

        public ActionResult Index()
        {
            ToolbarOptions.ExportButton.Visible = false;
            ToolbarOptions.ExportToButton.Visible = false;

            bool isRetailInstance = MvcApplication.ApplicationInstance == eApplicationInstance.RETAIL || MvcApplication.ApplicationInstance == eApplicationInstance.DUAL_MODE;

            ToolbarOptions.EditButton.Visible = isRetailInstance;
            ToolbarOptions.EditButton.OnClick = "EditSelectedRowsFromGrid";
            ToolbarOptions.OptionsButton.Visible = isRetailInstance;
            ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            ToolbarOptions.ViewButton.Visible = true;
            ToolbarOptions.ViewButton.OnClick = "ShowGenericView";
            ToolbarOptions.NewButton.OnClick = "AddNewFromGrid";
            ToolbarOptions.OptionsButton.Visible = false;

            CustomJSProperties.AddJSProperty("gridName", "grdCouponMasks");
            return View(GetList<CouponMask>(XpoSession));
        }

        public override ActionResult Grid()
        {
            return base.Grid();
        }

        protected override void FillLookupComboBoxes()
        {
            base.FillLookupComboBoxes();
            ViewBag.DiscountTypes = GetList<DiscountType>(XpoSession, ApplyOwnerCriteria(null, typeof(DiscountType)));
            ViewBag.PaymentMethods = GetList<PaymentMethod>(XpoSession, ApplyOwnerCriteria(null, typeof(PaymentMethod)));
            ViewBag.CouponCategories = GetList<CouponCategory>(XpoSession, ApplyOwnerCriteria(null, typeof(CouponCategory)));
        }

        public ActionResult DiscountTypesCallbackPanel()
        {
            return PartialView(GetList<DiscountType>(XpoSession));
        }

        public ActionResult PaymentMethodsCallbackPanel()
        {
            return PartialView(GetList<PaymentMethod>(XpoSession));
        }

        public ActionResult CouponCategoriesCallbackPanel()
        {
            return PartialView(GetList<CouponCategory>(XpoSession));
        }        

        public static object DiscountTypesAllRequestedByFilterCondition(ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            CriteriaOperator filterName = CriteriaOperator.Or(new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Code"), e.Filter),
                                                              new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Description"), e.Filter)
                                                             //new BinaryOperator("Code",e.Filter,BinaryOperatorType.Like),
                                                             //new BinaryOperator("Description", e.Filter, BinaryOperatorType.Like)
                                                             );

            XPCollection<DiscountType> discountTypes = GetList<DiscountType>(XpoHelper.GetNewUnitOfWork());

            int skipItems = e.BeginIndex;
            int takeItems = e.EndIndex - e.BeginIndex + 1;
            return discountTypes.Skip(skipItems).Take(takeItems).ToList();
        }
                        
        public static object PaymentMethodsAllRequestedByFilterCondition(ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            CriteriaOperator filterName = CriteriaOperator.Or(new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Code"), e.Filter),
                                                              new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Description"), e.Filter)
                                                             //new BinaryOperator("Code", e.Filter, BinaryOperatorType.Like),
                                                             //new BinaryOperator("Description", e.Filter, BinaryOperatorType.Like)
                                                             );

            XPCollection<PaymentMethod> paymentMethods = GetList<PaymentMethod>(XpoHelper.GetNewUnitOfWork());

            int skipItems = e.BeginIndex;
            int takeItems = e.EndIndex - e.BeginIndex + 1;
            return paymentMethods.Skip(skipItems).Take(takeItems).ToList();
        }

        public static object CouponCategoriesAllRequestedByFilterCondition(ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            CriteriaOperator filterName = CriteriaOperator.Or(new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Code"), e.Filter),
                                                              new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Description"), e.Filter)
                                                             //new BinaryOperator("Code", e.Filter, BinaryOperatorType.Like),
                                                             //new BinaryOperator("Description", e.Filter, BinaryOperatorType.Like)
                                                             );

            XPCollection<CouponCategory> couponCategories = GetList<CouponCategory>(XpoHelper.GetNewUnitOfWork());

            int skipItems = e.BeginIndex;
            int takeItems = e.EndIndex - e.BeginIndex + 1;
            return couponCategories.Skip(skipItems).Take(takeItems).ToList();
        }
    }
}

