using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.Model;
using ITS.Retail.Common;
using ITS.Retail.ResourcesLib;
using DevExpress.Xpo;
using ITS.Retail.WebClient.Helpers;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.WebClient.Providers;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;

namespace ITS.Retail.WebClient.Controllers
{
    public class PaymentMethodController : BaseObjController<PaymentMethod>
    {
        public override ActionResult LoadEditPopup()
        {
            base.LoadEditPopup();

            ViewBag.Title = Resources.PaymentField;

            ActionResult rt = PartialView("LoadEditPopup");
            return rt;
        }

        [Security(ReturnsPartial = false), Display(ShowSettings = true)]
        public ActionResult Index()
        {
            ToolbarOptions.ExportToButton.Visible = false;
            ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            ToolbarOptions.NewButton.OnClick = "AddNewCustomV2";
            ToolbarOptions.EditButton.OnClick = "EditSelectedRowsCustomV2";
            ToolbarOptions.ViewButton.Visible = true;
            ToolbarOptions.ViewButton.OnClick = "ShowGenericView";
            ToolbarOptions.OptionsButton.Visible = false;

            CustomJSProperties.AddJSProperty("editAction", "PaymentMethodDetail");
            CustomJSProperties.AddJSProperty("editIDParameter", "paymentMethodOid");
            CustomJSProperties.AddJSProperty("gridName", "grdPaymentMethod");

            return View("Index", GetList<PaymentMethod>(XpoHelper.GetNewUnitOfWork()).AsEnumerable<PaymentMethod>());
        }

        protected override GenericViewRuleset GenerateGenericViewRuleset()
        {
            GenericViewRuleset ruleset = base.GenerateGenericViewRuleset();
            ruleset.DetailsToIgnore.AddRange(new List<string>() { "UserDailyTotalsDetails", "DailyTotalsDetails", "DocumentPayments" });
            ruleset.PropertiesToIgnore.AddRange(new List<string>() { "IsDefault", "IsActive", "NeedsValidation" });
            ruleset.DetailPropertiesToIgnore.Add(typeof(PaymentMethodField), new List<string>() { "IsActive", "CustomEnumeration" });
            ruleset.NumberOfColumns = 3;
            return ruleset;
        }


        public ActionResult Edit(string Oid)
        {
            Session["currentPaymentMethod"] = null;
            Guid paymentMethodGuid;
            if (Guid.TryParse(Oid, out paymentMethodGuid))
            {
                if (paymentMethodGuid == Guid.Empty)
                {
                    PaymentMethod newPaymentMethod = new PaymentMethod(XpoHelper.GetNewUnitOfWork());
                    AssignOwner(newPaymentMethod);
                    Session["currentPaymentMethod"] = ViewData["currentPaymentMethod"] = newPaymentMethod;
                }
                else
                {
                    Session["currentPaymentMethod"] = ViewData["currentPaymentMethod"] = XpoHelper.GetNewUnitOfWork().GetObjectByKey<PaymentMethod>(paymentMethodGuid);
                }
            }
            return PartialView();
        }



        public ActionResult PaymentFields([ModelBinder(typeof(RetailModelBinder))]PaymentMethod paymentMethod)
        {
            ViewBag.PaymentFields = PaymentFieldHelper.OptionalDocumentPaymentFields().Select(propertyInfo => propertyInfo.Name);
            ViewBag.CustomEnumerationDefinitions = GetList<CustomEnumerationDefinition>(XpoHelper.GetNewUnitOfWork());
            return PartialView((Session["currentPaymentMethod"] as PaymentMethod).PaymentMethodFields);
        }

        [HttpPost]
        public ActionResult InsertPaymentField([ModelBinder(typeof(RetailModelBinder))] PaymentMethodField ct)
        {

            Session["Error"] = "";
            if (String.IsNullOrEmpty(ct.Label))
            {
                ModelState.AddModelError("Label", Resources.LabelIsEmpty);
                Session["Error"] += Resources.LabelIsEmpty;
            }

            if (String.IsNullOrEmpty(ct.FieldName))
            {
                ModelState.AddModelError("FieldName", Resources.FieldNameIsEmpty);
                Session["Error"] += Resources.FieldNameIsEmpty;
            }

            if ((Session["currentPaymentMethod"] as PaymentMethod).PaymentMethodFields.Where(paymentField => paymentField.Oid != ct.Oid && paymentField.FieldName.Equals(ct.FieldName)).Count() > 0)
            {
                ModelState.AddModelError("FieldName", Resources.FieldNameHasAlreadyBeenSelectedForThisPaymentMethod);
                Session["Error"] += Resources.FieldNameHasAlreadyBeenSelectedForThisPaymentMethod;
            }

            if (ModelState.IsValid)
            {
                Session["Error"] = null;
                PaymentMethodField paymentMethodField = new PaymentMethodField((Session["currentPaymentMethod"] as PaymentMethod).Session as UnitOfWork);
                paymentMethodField.Label = ct.Label;
                paymentMethodField.FieldName = ct.FieldName;
                paymentMethodField.PaymentMethod = (Session["currentPaymentMethod"] as PaymentMethod);
                Guid customEnumGui;
                if (Guid.TryParse(Request["CustomEnumerationComboBox_VI"], out customEnumGui))
                {
                    paymentMethodField.CustomEnumeration = paymentMethodField.Session.GetObjectByKey<CustomEnumerationDefinition>(customEnumGui);
                }
                paymentMethodField.Save();
            }
            else
            {
                ViewBag.PaymentFields = PaymentFieldHelper.OptionalDocumentPaymentFields().Select(propertyInfo => propertyInfo.Name);
                ViewBag.CustomEnumerationDefinitions = GetList<CustomEnumerationDefinition>(XpoHelper.GetNewUnitOfWork());
                ViewBag.CurrentItem = ct;
            }
            return PartialView("PaymentFields", (Session["currentPaymentMethod"] as PaymentMethod).PaymentMethodFields);
        }

        [HttpPost]
        public ActionResult UpdatePaymentField([ModelBinder(typeof(RetailModelBinder))] PaymentMethodField ct)
        {
            Session["Error"] = "";
            if (String.IsNullOrEmpty(ct.Label))
            {
                ModelState.AddModelError("Label", Resources.LabelIsEmpty);
                Session["Error"] += Resources.LabelIsEmpty;
            }

            if (String.IsNullOrEmpty(ct.FieldName))
            {
                ModelState.AddModelError("FieldName", Resources.FieldNameIsEmpty);
                Session["Error"] += Resources.FieldNameIsEmpty;
            }

            if ((Session["currentPaymentMethod"] as PaymentMethod).PaymentMethodFields.Where(paymentField => paymentField.Oid != ct.Oid && paymentField.FieldName.Equals(ct.FieldName)).Count() > 0)
            {
                ModelState.AddModelError("FieldName", Resources.FieldNameHasAlreadyBeenSelectedForThisPaymentMethod);
                Session["Error"] += Resources.FieldNameHasAlreadyBeenSelectedForThisPaymentMethod;
            }

            if (ModelState.IsValid)
            {
                Session["Error"] = null;
                PaymentMethodField paymentMethodField = (Session["currentPaymentMethod"] as PaymentMethod).PaymentMethodFields.Where(paymMethodField => paymMethodField.Oid == ct.Oid).First();
                paymentMethodField.Label = ct.Label;
                paymentMethodField.FieldName = ct.FieldName;
                Guid customEnumGui;
                if (Guid.TryParse(Request["CustomEnumerationComboBox_VI"], out customEnumGui))
                {
                    paymentMethodField.CustomEnumeration = paymentMethodField.Session.GetObjectByKey<CustomEnumerationDefinition>(customEnumGui);
                }
                paymentMethodField.Save();
            }
            else
            {
                ViewBag.PaymentFields = PaymentFieldHelper.OptionalDocumentPaymentFields().Select(propertyInfo => propertyInfo.Name);
                ViewBag.CustomEnumerationDefinitions = GetList<CustomEnumerationDefinition>(XpoHelper.GetNewUnitOfWork());
                ViewBag.CurrentItem = ct;
            }

            return PartialView("PaymentFields", (Session["currentPaymentMethod"] as PaymentMethod).PaymentMethodFields);
        }

        [HttpPost]
        public ActionResult DeletePaymentField([ModelBinder(typeof(RetailModelBinder))] PaymentMethodField ct)
        {
            (Session["currentPaymentMethod"] as PaymentMethod).PaymentMethodFields.Where(paymentMethodField => paymentMethodField.Oid == ct.Oid).First().Delete();
            return PartialView("PaymentFields", (Session["currentPaymentMethod"] as PaymentMethod).PaymentMethodFields);
        }

        [Security(ReturnsPartial = false)]
        public ActionResult CancelEdit()
        {
            Session["currentPaymentMethod"] = null;
            return null;
        }

        public JsonResult Save()
        {
            PaymentMethod paymentMethod = Session["currentPaymentMethod"] as PaymentMethod;
            if (paymentMethod == null)
            {
                return Json(new { });
            }

            String code = Request["Code"];
            String ReferenceCode = Request["ReferenceCode"];
            String description = Request["Description"];
            String giveChange = Request["GiveChange"];
            string increasesDrawerAmount = Request["IncreasesDrawerAmount"];
            String opensDrawer = Request["OpensDrawer"];
            String canExceedTotal = Request["CanExceedTotal"];
            String paymentMethodType = Request["PaymentMethodType_VI"];
            String needsRatification = Request["NeedsRatification"];
            String UseEDPS = Request["UseEDPS"];
            String IsNegative = Request["IsNegative"];
            String UsesInstallments = Request["UsesInstallments"];
            String ForceEdpsOffline = Request["ForceEdpsOffline"];
            String AffectsCustomerBalance = Request["AffectsCustomerBalance"];
            string CashierDeviceCode = Request["CashierDeviceCode"];
            String UseCardlink = Request["UseCardlink"];
            String DisplayInCashCount = Request["DisplayInCashCount"];
            String HandelsCurrencies = Request["HandelsCurrencies"];
            

            Session["Error"] = "";
            if (String.IsNullOrWhiteSpace(code))
            {
                ModelState.AddModelError("Code", Resources.CodeIsEmpty);
                Session["Error"] += Resources.CodeIsEmpty;
            }
            if (String.IsNullOrWhiteSpace(description))
            {
                ModelState.AddModelError("Description", Resources.DescriptionIsEmpty);
                Session["Error"] += Resources.DescriptionIsEmpty;
            }


            paymentMethod.Code = code;
            paymentMethod.ReferenceCode = ReferenceCode;
            paymentMethod.Description = description;
            paymentMethod.GiveChange = giveChange.Equals("C");
            paymentMethod.OpensDrawer = opensDrawer.Equals("C");
            paymentMethod.CanExceedTotal = canExceedTotal.Equals("C");
            paymentMethod.NeedsRatification = needsRatification.Equals("C");
            paymentMethod.IncreasesDrawerAmount = increasesDrawerAmount.Equals("C");
            ePaymentMethodType parcedPaymentMethodType;
            Enum.TryParse<ePaymentMethodType>(paymentMethodType, true, out parcedPaymentMethodType);
            paymentMethod.PaymentMethodType = parcedPaymentMethodType;
            paymentMethod.UseEDPS = UseEDPS == "C";
            paymentMethod.IsNegative = IsNegative == "C";
            paymentMethod.UsesInstallments = UsesInstallments == "C";
            paymentMethod.ForceEdpsOffline = ForceEdpsOffline == "C";
            paymentMethod.AffectsCustomerBalance = AffectsCustomerBalance == "C";
            paymentMethod.UseCardlink = UseCardlink == "C";
            paymentMethod.DisplayInCashCount = DisplayInCashCount == "C";
            paymentMethod.HandelsCurrencies = HandelsCurrencies == "C";
            if (string.IsNullOrEmpty(CashierDeviceCode)) paymentMethod.CashierDeviceCode = 0; else paymentMethod.CashierDeviceCode = int.Parse(CashierDeviceCode);


            if (HasDuplicate(paymentMethod))
            {
                ModelState.AddModelError("Code", Resources.CodeAlreadyExists);
                Session["Error"] += Resources.CodeAlreadyExists;
            }

            if (ModelState.IsValid)
            {
                Session["Error"] = null;
                try
                {
                    paymentMethod.Save();
                    paymentMethod.Session.CommitTransaction();
                    Session["currentPaymentMethod"] = null;
                    Session["Notice"] = Resources.SavedSuccesfully;
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }

            if (ModelState.IsValid == false || Session["Error"] != null)
            {
                //return View("PaymentMethodDetail", paymentMethod);
                return Json(new { error = Session["Error"] });
            }

            return Json(new { });
        }
    }
}
