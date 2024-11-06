using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.WebClient.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;

namespace ITS.Retail.WebClient.Controllers
{
    [StoreControllerEditable]
    public class StoreDailyReportController : BaseObjController<StoreDailyReport>
    {
        UnitOfWork uow;

        protected void GenerateUnitOfWork()
        {

            if (Session["uow"] == null)
            {
                uow = XpoHelper.GetNewUnitOfWork();
                Session["uow"] = uow;
            }
            else
            {
                uow = (UnitOfWork)Session["uow"];
            }
        }

        public ActionResult Index()
        {
            this.ToolbarOptions.ViewButton.Visible = true;
            this.ToolbarOptions.FilterButton.Visible = true;
            this.ToolbarOptions.ExportButton.Visible = false;
            this.ToolbarOptions.ExportToButton.Visible = false;
            this.ToolbarOptions.OptionsButton.Visible = false;
           
            this.ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            this.ToolbarOptions.NewButton.OnClick = "AddNewCustomV2";
            this.ToolbarOptions.EditButton.OnClick = "EditSelectedRowsCustomV2";
            this.ToolbarOptions.ViewButton.Visible = true;
            this.ToolbarOptions.ViewButton.OnClick = "ShowGenericView";

            this.CustomJSProperties.AddJSProperty("gridName","grdStoreDailyReport");
            this.CustomJSProperties.AddJSProperty("editAction", "Edit");
            this.CustomJSProperties.AddJSProperty("editIDParameter", "storeDailyReportOid");
            return View("Index", GetList<StoreDailyReport>(XpoHelper.GetNewUnitOfWork()).AsEnumerable<StoreDailyReport>());
        }

        protected override GenericViewRuleset GenerateGenericViewRuleset()
        {
            GenericViewRuleset ruleset = base.GenerateGenericViewRuleset();
            ruleset.NumberOfColumns = 4;
            ruleset.PropertiesToIgnore.Add("Description");
            ruleset.PropertiesToIgnore.Add("IsActive");
            ruleset.PropertiesToIgnore.Add("Store");
            ruleset.PropertiesToIgnore.Add("CreditsTotal");
            //ruleset.ShowDetails = false;
            
            return ruleset;
        }

        public override ActionResult Grid()
        {
            CriteriaOperator criteria = null;
            if (Request["callbackparameter"] == "SEARCH")
            {
                int Code;
                //int StoreCode;
                DateTime startDate;
                DateTime endDate;


                if (Request.HttpMethod == "POST")
                {
                    if (int.TryParse(Request["ReportCode"], out Code))
                    {
                        criteria = CriteriaOperator.And(new BinaryOperator("Code", Code, BinaryOperatorType.Like));
                    }
                    if (!String.IsNullOrEmpty(Request["StoreCode"]))
                    {
                        criteria = CriteriaOperator.And(criteria, new BinaryOperator("Store.Code", Request["StoreCode"], BinaryOperatorType.Like));
                    }
                    if (DateTime.TryParse(Request["StartDate"], out startDate))
                    {
                        criteria = CriteriaOperator.And(criteria, new BinaryOperator("ReportDate", startDate, BinaryOperatorType.GreaterOrEqual));
                    }
                    if (DateTime.TryParse(Request["EndDate"], out endDate))
                    {
                        criteria = CriteriaOperator.And(criteria, new BinaryOperator("ReportDate", endDate, BinaryOperatorType.LessOrEqual));
                    }
                }

            }
            return PartialView("Grid", GetList<StoreDailyReport>(XpoHelper.GetNewUnitOfWork(), criteria));
        }


        public override ActionResult PopupEditCallbackPanel()
        {
            base.PopupEditCallbackPanel();

            return PartialView();

        }

        public override ActionResult LoadEditPopup()
        {
            base.LoadEditPopup();

            ViewBag.Title = Resources.EditStoreDailyReport;

            ActionResult rt = PartialView("LoadEditPopup");
            return rt;
        }


        public ActionResult Edit(String Oid)
        {
            GenerateUnitOfWork();
            StoreDailyReport storeDailyReport = null;
            Guid storeDailyReportGuid;
            if (Guid.TryParse(Oid, out storeDailyReportGuid))
            {
                if (storeDailyReportGuid == Guid.Empty)
                {
                    storeDailyReport = new StoreDailyReport(uow);
                    storeDailyReport.Store = uow.GetObjectByKey<Store>(StoreControllerAppiSettings.CurrentStore.Oid);
                    ViewData["IsNew"] = true;
                    ViewBag.Mode = Resources.AddStoreDailyReport;
                }
                else
                {
                    storeDailyReport = uow.GetObjectByKey<StoreDailyReport>(storeDailyReportGuid);
                    ViewData["IsNew"] = false;
                    ViewBag.Mode = Resources.EditStoreDailyReport;
                }
            }
            if (storeDailyReport == null)
            {
                return null;
            }

            Session["currentStoreDailyReport"] = storeDailyReport;
            FillLookupComboBoxes();
            return PartialView(storeDailyReport);
        }

        public ActionResult DailyTotalsGrid()
        {
            StoreDailyReport dailyReport = (StoreDailyReport)Session["currentStoreDailyReport"];
            return PartialView("DailyTotalsGrid", dailyReport.DailyTotals);
        }

        public ActionResult CreditsGrid()
        {
            StoreDailyReport dailyReport = (StoreDailyReport)Session["currentStoreDailyReport"];
            return PartialView("CreditsGrid", dailyReport.Lines.Where(line => line.Type == eStoreDailyReportDocHeaderType.Credit));
        }

        public ActionResult AutoDeliveriesGrid()
        {
            StoreDailyReport dailyReport = (StoreDailyReport)Session["currentStoreDailyReport"];
            return PartialView("AutoDeliveriesGrid", dailyReport.DocumentHeaders.Where(doc => doc.Type == eStoreDailyReportDocHeaderType.AutoDelivery));
        }

        public ActionResult PaymentsGrid()
        {
            StoreDailyReport dailyReport = (StoreDailyReport)Session["currentStoreDailyReport"];
            ViewBag.PaymentsCount = dailyReport.Lines.Where(payment => payment.Type == eStoreDailyReportDocHeaderType.Payment).Count();
            return PartialView("PaymentsGrid", dailyReport.Lines.Where(line => line.Type == eStoreDailyReportDocHeaderType.Payment));
        }

        public ActionResult StatisticsGrid()
        {
            StoreDailyReport dailyReport = (StoreDailyReport)Session["currentStoreDailyReport"];
            ViewBag.StatisticsCount = dailyReport.Lines.Where(payment => payment.Type == eStoreDailyReportDocHeaderType.Statistic).Count();
            return PartialView("StatisticsGrid", dailyReport.Lines.Where(line => line.Type == eStoreDailyReportDocHeaderType.Statistic).OrderBy(line => line.LineNumber));
        }

        public JsonResult jsonUpdateForm()
        {
            StoreDailyReport dailyReport = (StoreDailyReport)Session["currentStoreDailyReport"];

            decimal mainPOSWithDraws;
            decimal otherExpanses;
            decimal paperMoney;
            decimal coins;
            decimal collectionComplement;

            dailyReport.MainPOSWithdraws = Decimal.TryParse(Request["mainPOSWithDraws"], out mainPOSWithDraws) ? mainPOSWithDraws / StoreDailyReportHelper.QUANTITY_MULTIPLIER : 0;
            dailyReport.OtherExpanses = Decimal.TryParse(Request["OtherExpanses"], out otherExpanses) ? otherExpanses / StoreDailyReportHelper.QUANTITY_MULTIPLIER : 0;
            dailyReport.OtherExpansesText = Request["OtherExpansesText"];
            dailyReport.PaperMoney = Decimal.TryParse(Request["PaperMoney"], out paperMoney) ? paperMoney / StoreDailyReportHelper.QUANTITY_MULTIPLIER : 0;
            dailyReport.Coins = Decimal.TryParse(Request["Coins"], out coins) ? coins / StoreDailyReportHelper.QUANTITY_MULTIPLIER : 0;
            dailyReport.CollectionComplement = Decimal.TryParse(Request["CollectionComplement"], out collectionComplement) ? collectionComplement / StoreDailyReportHelper.QUANTITY_MULTIPLIER : 0;
            dailyReport.CollectionComplementText = Request["CollectionComplementText"];


            StoreDailyReportHelper.RecalculateReportTotals(dailyReport);

            return Json(new {
                DailyTotalsTotal = dailyReport.DailyTotalsTotal,
                InvoicesTotalCash = dailyReport.InvoicesTotalCash,
                InvoicesTotal = dailyReport.InvoicesTotal,
                AutoDeliveriesTotal = dailyReport.AutoDeliveriesTotal,
                CollectionComplementText = dailyReport.CollectionComplementText,
                CollectionComplement = dailyReport.CollectionComplement,
                CollectionsTotal = dailyReport.CollectionsTotal,
                PaymentsTotal = dailyReport.PaymentsTotal,
                PaymentsWithDrawsTotal = dailyReport.PaymentsWithDrawsTotal,
                CreditsGridTotal = dailyReport.CreditsGridTotal,
                CreditsPaymentsWithDrawsTotal = dailyReport.CreditsPaymentsWithDrawsTotal,
                CashDelivery = dailyReport.CashDelivery,
                MainPOSWithdraws = dailyReport.MainPOSWithdraws,
                OtherExpansesText = dailyReport.OtherExpansesText,
                OtherExpanses = dailyReport.OtherExpanses,
                Coins = dailyReport.Coins,
                PaperMoney = dailyReport.PaperMoney,
                ReportTotal = dailyReport.ReportTotal,
                POSDifference = dailyReport.POSDifference
            });
        }

        public JsonResult jsonInitializeDay()
        {
            StoreDailyReport dailyReport = (StoreDailyReport)Session["currentStoreDailyReport"];
            GenerateUnitOfWork();
            DateTime currentdate;
            try
            {
                if (dailyReport.ReportDate != DateTime.MinValue)
                {
                    currentdate = dailyReport.ReportDate;
                    foreach(StoreDailyReportPayment payment in dailyReport.Lines.ToList())
                    {
                        if(payment.Type == eStoreDailyReportDocHeaderType.Credit)
                        {
                            dailyReport.Session.Delete(payment);
                        }
                    }
                }
                else
                {
                    currentdate = DateTime.Parse(Request["date"]);
                    dailyReport.CollectionComplement = 0;
                    dailyReport.CollectionComplementText = String.Empty;
                    dailyReport.PaperMoney = 0;
                    dailyReport.Coins = 0;
                    dailyReport.MainPOSWithdraws = 0;
                    dailyReport.OtherExpansesText = String.Empty;
                    dailyReport.OtherExpanses = 0;              
                    dailyReport.Session.Delete(dailyReport.Lines);
                    dailyReport.PaymentsTotal = 0;
                }
             
                dailyReport.DailyTotalsTotal = 0;
                dailyReport.InvoicesTotalCash = 0;
                dailyReport.InvoicesTotal = 0;
                
                dailyReport.CreditsGridTotal = 0;
                
                dailyReport.Session.Delete(dailyReport.DailyTotals);
                dailyReport.Session.Delete(dailyReport.DocumentHeaders);

                StoreDailyReportHelper.GetDailyTotals(dailyReport, uow, currentdate);
                StoreDailyReportHelper.GetDocumentHeaders(dailyReport, uow, "101", currentdate);
                StoreDailyReportHelper.GetDocumentHeaders(dailyReport, uow, "927", currentdate);
                StoreDailyReportHelper.GetCredits(dailyReport, uow);
                StoreDailyReportHelper.RecalculateReportTotals(dailyReport);
                StoreDailyReportHelper.GetStatistics(dailyReport, uow, currentdate);
            }
            catch(Exception ex)
            {
                return Json(new {message = ex.Message});
            }
            return Json(new {
                DailyTotalsTotal = dailyReport.DailyTotalsTotal,
                InvoicesTotalCash = dailyReport.InvoicesTotalCash,
                InvoicesTotal = dailyReport.InvoicesTotal,
                AutoDeliveriesTotal = dailyReport.AutoDeliveriesTotal,
                CollectionComplement = dailyReport.CollectionComplement,
                CollectionComplementText = dailyReport.CollectionComplementText,
                CollectionsTotal = dailyReport.CollectionsTotal,
                PaymentsTotal = dailyReport.PaymentsTotal,
                PaymentsWithDrawsTotal = dailyReport.PaymentsWithDrawsTotal,
                CreditsGridTotal = dailyReport.CreditsGridTotal,
                CreditsPaymentsWithDrawsTotal = dailyReport.CreditsPaymentsWithDrawsTotal,
                CashDelivery = dailyReport.CashDelivery,
                MainPOSWithdraws = dailyReport.MainPOSWithdraws,
                OtherExpanses = dailyReport.OtherExpanses,
                OtherExpansesText = dailyReport.OtherExpansesText,
                Coins = dailyReport.Coins,
                PaperMoney = dailyReport.PaperMoney,
                ReportTotal = dailyReport.ReportTotal,
                POSDifference = dailyReport.POSDifference
            });
        }

        public ActionResult PaymentsAddNewPartial([ModelBinder(typeof(RetailModelBinder))] StoreDailyReportPayment ct)
        {
            StoreDailyReport dailyReport = (StoreDailyReport)Session["currentStoreDailyReport"];
            if(ModelState.IsValid)
            {
                StoreDailyReportPayment storeDailyReportPayment = new StoreDailyReportPayment(dailyReport.Session);
                storeDailyReportPayment.GetData(ct, new List<String>() { "Session" });
                storeDailyReportPayment.LineNumber = dailyReport.Lines.Where(payment => payment.Type == eStoreDailyReportDocHeaderType.Payment).Count() + 1;
                storeDailyReportPayment.DailyReport = dailyReport;
                storeDailyReportPayment.Type = eStoreDailyReportDocHeaderType.Payment;
                dailyReport.Lines.Add(storeDailyReportPayment);
                dailyReport.PaymentsTotal += ct.UserValue;
                StoreDailyReportHelper.RecalculateReportTotals(dailyReport);
            }
            return PartialView("PaymentsGrid", dailyReport.Lines.Where(payment => payment.Type == eStoreDailyReportDocHeaderType.Payment));
        }

        public ActionResult PaymentsUpdatePartial([ModelBinder(typeof(RetailModelBinder))] StoreDailyReportPayment ct)
        {
            StoreDailyReport dailyReport = (StoreDailyReport)Session["currentStoreDailyReport"];
            if (ModelState.IsValid)
            {
                dailyReport.Lines.Where(payment => payment.Oid == ct.Oid).FirstOrDefault().GetData(ct, new List<String>() { "Session", "DailyReport", "Type" });
                dailyReport.PaymentsTotal = dailyReport.Lines.Where(payment => payment.Type == eStoreDailyReportDocHeaderType.Payment).Sum(payment => payment.UserValue);
                StoreDailyReportHelper.RecalculateReportTotals(dailyReport);
            }
            return PartialView("PaymentsGrid", dailyReport.Lines.Where(payment => payment.Type == eStoreDailyReportDocHeaderType.Payment));
        }

        public ActionResult PaymentsDeletePartial([ModelBinder(typeof(RetailModelBinder))] StoreDailyReportPayment ct)
        {
            StoreDailyReport dailyReport = (StoreDailyReport)Session["currentStoreDailyReport"];
            if (ModelState.IsValid)
            {
                StoreDailyReportPayment storeDailyReportPayment = dailyReport.Lines.Where(payment => payment.Oid == ct.Oid).First();
                dailyReport.PaymentsTotal -= storeDailyReportPayment.UserValue;
                for (int i = storeDailyReportPayment.LineNumber + 1; i <= dailyReport.Lines.Where(payment => payment.Type == eStoreDailyReportDocHeaderType.Payment).Count(); i++)
                {
                    dailyReport.Lines.Where(payment => payment.Type == eStoreDailyReportDocHeaderType.Payment && payment.LineNumber == i).First().LineNumber--;
                }
                storeDailyReportPayment.Delete();
                StoreDailyReportHelper.RecalculateReportTotals(dailyReport);
            }
            return PartialView("PaymentsGrid", dailyReport.Lines.Where(payment => payment.Type == eStoreDailyReportDocHeaderType.Payment));
        }

        public ActionResult DailyTotalUpdatePartial([ModelBinder(typeof(RetailModelBinder))] StoreDailyReportDailyTotal ct)
        {
            StoreDailyReport dailyReport = (StoreDailyReport)Session["currentStoreDailyReport"];
            if (ModelState.IsValid)
            {
                StoreDailyReportDailyTotal storeDailyReportDailyTotal = dailyReport.DailyTotals.Where(dailyTotal => dailyTotal.Oid == ct.Oid).First();
                storeDailyReportDailyTotal.UserValue = ct.UserValue;
                storeDailyReportDailyTotal.UserSetsValue = ct.UserSetsValue;
                dailyReport.DailyTotalsTotal = dailyReport.DailyTotals.Sum(dailyTotal => dailyTotal.UserSetsValue
                                                                ? dailyTotal.UserValue//(dailyTotal.UserValue != null ? dailyTotal.UserValue : dailyTotal.Value) 
                                                                : dailyTotal.Value);
                StoreDailyReportHelper.RecalculateReportTotals(dailyReport);
            }
            return PartialView("DailyTotalsGrid", dailyReport.DailyTotals);
        }     

        public ActionResult AutoDeliveriesUpdatePartial([ModelBinder(typeof(RetailModelBinder))] StoreDailyReportDocumentHeader ct)
        {
            StoreDailyReport dailyReport = (StoreDailyReport)Session["currentStoreDailyReport"];
            if (ModelState.IsValid)
            {
                StoreDailyReportDocumentHeader storeDailyReportDocHeader = dailyReport.DocumentHeaders.Where(dailyTotal => dailyTotal.Oid == ct.Oid).First();
                storeDailyReportDocHeader.UserValue = ct.UserValue;
                storeDailyReportDocHeader.UserSetsValue = ct.UserSetsValue;
                dailyReport.AutoDeliveriesTotal = dailyReport.DocumentHeaders.Where(doc => doc.Type == eStoreDailyReportDocHeaderType.AutoDelivery).Sum(doc => doc.UserSetsValue
                    ? doc.UserValue//(doc.UserValue != null ? doc.UserValue : doc.DocumentHeader.GrossTotal) 
                    : doc.DocumentHeader.GrossTotal);
                StoreDailyReportHelper.RecalculateReportTotals(dailyReport);
            }
            return PartialView("AutoDeliveriesGrid", dailyReport.DocumentHeaders.Where(doc => doc.Type == eStoreDailyReportDocHeaderType.AutoDelivery));
        }

        public ActionResult CreditUpdatePartial([ModelBinder(typeof(RetailModelBinder))] StoreDailyReportPayment ct)
        {
            StoreDailyReport dailyReport = (StoreDailyReport)Session["currentStoreDailyReport"];
            if (ModelState.IsValid)
            {
                dailyReport.Lines.Where(payment => payment.Oid == ct.Oid).FirstOrDefault().GetData(ct, new List<String>() { "Session", "DailyReport", "Type" });
                dailyReport.CreditsGridTotal = dailyReport.Lines.Where(payment => payment.Type == eStoreDailyReportDocHeaderType.Credit).Sum(payment => payment.UserValue);
                StoreDailyReportHelper.RecalculateReportTotals(dailyReport);
            }
            return PartialView("CreditsGrid", dailyReport.Lines.Where(payment => payment.Type == eStoreDailyReportDocHeaderType.Credit));
        }




        public JsonResult Save()
        {
            GenerateUnitOfWork();
            StoreDailyReport dailyReport = (StoreDailyReport)Session["currentStoreDailyReport"];

            DateTime reportdate;
            string errorMessage = "";
            if (dailyReport.ReportDate == DateTime.MinValue)
            {
                if (DateTime.TryParse(Request["ReportDate"], out reportdate))
                {
                    dailyReport.ReportDate = reportdate;
                }

                else
                {
                    ModelState.AddModelError("ReportDate", Resources.DateIsEmpty);
                    errorMessage = Resources.DateIsEmpty;
                }
            }

            if(ModelState.IsValid)
            {
                dailyReport.Save();

                XpoHelper.CommitTransaction(uow);
                Session["uow"] = null;
                Session["currentStoreDailyReport"] = null;
                return Json(new { });
            }
            else
            {
                return Json(new { error = errorMessage });
                //return View("Edit", dailyReport);
            }
        }

 
        public ActionResult CloseReport()
        {
            Session["uow"] = null;
            Session["currentStoreDailyReport"] = null;
            return null;
        }
    }
}
