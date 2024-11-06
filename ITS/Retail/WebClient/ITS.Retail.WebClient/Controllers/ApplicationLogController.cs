using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.Model;
using DevExpress.Xpo;
using ITS.Retail.Common;
using DevExpress.Data.Filtering;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Data.Linq;
using DevExpress.Data;


namespace ITS.Retail.WebClient.Controllers
{
    public class ApplicationLogController : BaseObjController<ApplicationLog>
    {
        [Security(ReturnsPartial = false), Display(ShowSettings = true)]
        public ActionResult Index()
        {
            CriteriaOperator crop = Session["logCriteria"] as CriteriaOperator;

            ToolbarOptions.ForceVisible = false;
            this.CustomJSProperties.AddJSProperty("gridName", "grdLogView");
            XPQuery<ApplicationLog> results = new XPQuery<ApplicationLog>(XpoHelper.GetNewUnitOfWork());
            CriteriaToExpressionConverter conv = new CriteriaToExpressionConverter();
            ServerModeOrderDescriptor sortByCreatedOnTicks = new ServerModeOrderDescriptor(new OperandProperty("CreatedOnTicks"), true);
            return View(results.AppendWhere(conv,null).MakeOrderBy(conv, sortByCreatedOnTicks));
        }

        public override ActionResult Grid()
        {
            CriteriaOperator crop = Session["logCriteria"] as CriteriaOperator;
            XPQuery<ApplicationLog> results = new XPQuery<ApplicationLog>(XpoSession);
            IQueryable<ApplicationLog> finalResult = results;
            if (Request["action"] == "CLEARLOG")
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    XPCollection<ApplicationLog> logs = GetList<ApplicationLog>(uow);
                    uow.Delete(logs);
                    XpoHelper.CommitChanges(uow);
                }
            }
            
            if ((Request["IP"] != null && Request["IP"] != "")
                || (Request["controller"] != null && Request["controller"] != "")
                || (Request["created"] != null && Request["created"] != "")
                || (Request["user"] != null && Request["user"] != "")
                )
            {
                crop = null;

                //CriteriaOperator ip_crop, controler_crop, created_crop, user_crop = null;
                if (Request["IP"] != null && Request["IP"] != "")
                {
                    finalResult = finalResult.Where(g => g.IPAddress.Contains(Request["IP"].ToString()));
                }

                if (Request["controller"] != null && Request["controller"] != "")
                {
                    finalResult = finalResult.Where(g => g.Controller.Contains(Request["controller"].ToString()));
                }

                if (Request["created"] != null && Request["created"] != "")
                {
                    DateTime now = DateTime.Parse(Request["created"]);
                    finalResult = finalResult.Where(g => g.CreatedOnTicks >= now.Ticks);
                }

                if (Request["user"] != null && Request["user"] != "")
                {
                    finalResult = finalResult.Where(g => g.CreatedBy.UserName.Contains(Request["user"].ToString()));
                }                
            }            
            return PartialView(finalResult);//.MakeOrderBy(conv, sortByCreatedOnTicks));
        }


        public ActionResult LoadViewPopupLog(String LogID, String view_column)
        {
            Guid LogGuid;
            if(!Guid.TryParse(LogID, out LogGuid)){
                LogGuid = Guid.Empty;
            }
            ApplicationLog entry = XpoSession.FindObject<ApplicationLog>(new BinaryOperator("Oid", LogGuid));
            ViewData["view_column"] = view_column;
            ViewData["entry"] = entry;
            return PartialView();
        }
    }
}