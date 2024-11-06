using ITS.Retail.Common;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Model;
using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.WebClient.Providers;

namespace ITS.Retail.WebClient.Controllers
{
    public class DailyReportController : BaseObjController<DailyTotals>
    {
        //
        // GET: /DailyReport/
        [Security(ReturnsPartial=false)]
        public ActionResult Index()
        {
            return new RedirectResult("~/DailyReport/POS");
        }

        [Security(ReturnsPartial = false)]
        public ActionResult POS()
        {
            //this.ToolbarOptions.Visible = true;
            this.ToolbarOptions.ViewButton.Visible = true;
            //USE Component.ShowPopup and not the following
            this.ToolbarOptions.ViewButton.OnClick = "ShowPopups";
            //this.ToolbarOptions.FilterButton.Visible = true;
            this.ToolbarOptions.DeleteButton.Visible =
            this.ToolbarOptions.NewButton.Visible =
            this.ToolbarOptions.EditButton.Visible =
            this.ToolbarOptions.ExportToButton.Visible =
            this.ToolbarOptions.PrintButton.Visible =
            this.ToolbarOptions.TransformButton.Visible =
            this.ToolbarOptions.CopyButton.Visible = false;
            ViewData["DailyTotals"] = GetList<DailyTotals>(XpoHelper.GetNewUnitOfWork());
            return View();
        }

        [Security(ReturnsPartial = false)]
        public ActionResult User()
        {
            return View();
        }

        [Security(ReturnsPartial = false)]
        public ActionResult DailyGrid()
        {
            //Add criteria
            return PartialView(GetList<DailyTotals>(XpoHelper.GetNewUnitOfWork()));
        }


        public ActionResult UserDailyGrid()
        {
            //Add criteria
            return PartialView();
        }

        [HttpPost]
        public ActionResult AddNewPOS([ModelBinder(typeof(RetailModelBinder))] DailyTotals ct)
        {
            if (!TableCanInsert)
            {
                return null;
            }

            return PartialView("DailyGrid", GetList<DailyTotals>(XpoHelper.GetNewUnitOfWork()).AsEnumerable<DailyTotals>());
        }

        [HttpPost]
        public ActionResult UpdatePOS([ModelBinder(typeof(RetailModelBinder))] DailyTotals ct)
        {
            if (!TableCanUpdate)
            {
                return null;
            }

            return PartialView("DailyGrid", GetList<DailyTotals>(XpoHelper.GetNewUnitOfWork()).AsEnumerable<DailyTotals>());
        }

        [HttpPost]
        public ActionResult DeletePOS([ModelBinder(typeof(RetailModelBinder))] DailyTotals ct)
        {
            if (!TableCanDelete) return null;
            try
            {
                Delete(ct);
            }
            catch (Exception e)
            {
                Session["Error"] = e.Message;// +Environment.NewLine + e.StackTrace;
            }
            return PartialView("DailyGrid", GetList<DailyTotals>(XpoHelper.GetNewUnitOfWork()).AsEnumerable<DailyTotals>());
        }

        public ActionResult LoadPOSViewPopup(String DailyTotalID)
        {
            Guid dailyTotalGuid = (DailyTotalID == null || DailyTotalID == "null" || DailyTotalID == "-1") ? Guid.Empty : Guid.Parse(DailyTotalID);
            if (dailyTotalGuid != Guid.Empty)
            {
                DailyTotals dailyTotal = XpoHelper.GetNewUnitOfWork().FindObject<DailyTotals>(new BinaryOperator("Oid", dailyTotalGuid));
                ViewData["DailyTotalID"] = dailyTotal.Oid;
            }
            else
            {
                Session["Error"] = Resources.AnErrorOccurred;
            }

            return PartialView();
        }

        public ActionResult DailyTotalDetails(string DailyTotalID)
        {
            Guid dailyTotalGuid = (DailyTotalID == null || DailyTotalID == "null" || DailyTotalID == "-1") ? Guid.Empty : Guid.Parse(DailyTotalID);
            XPCollection<DailyTotalsDetail> dailyTotalsDetails = null;
            if (dailyTotalGuid != Guid.Empty)
            {
                DailyTotals dailyTotal = XpoHelper.GetNewUnitOfWork().FindObject<DailyTotals>(new BinaryOperator("Oid", dailyTotalGuid));
                ViewData["DailyTotalID"] = dailyTotal.Oid;
                dailyTotalsDetails = dailyTotal.DailyTotalsDetails;
                //TotalizorsHelper.CreateZReport(
            }
            else
            {
                Session["Error"] = Resources.AnErrorOccurred;
            }
            return PartialView(dailyTotalsDetails);
        }

    }
}
