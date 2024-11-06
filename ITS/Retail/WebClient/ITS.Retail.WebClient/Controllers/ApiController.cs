using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Common.ViewModel;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.WebClient.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace ITS.Retail.WebClient.Controllers
{
    [AllowAnonymous]

    public class ApiController : BaseApiController
    {

        public JsonResult DailyItemSales(string codes, eDivision section = eDivision.Sales)
        {
            List<SaleItem> returnList = new List<SaleItem>();
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                try
                {
                    if (string.IsNullOrEmpty(codes))
                    {
                        return Json(returnList, JsonRequestBehavior.AllowGet);
                    }

                    HashSet<string> requestCodes = new HashSet<string>(!codes.Contains(",") ? new string[] { codes } : codes.Split(','));
                    DateTime startDate = DateTime.Now.Date;
                    DateTime enddate = startDate.AddHours(23).AddMinutes(59).AddSeconds(59);

                    CriteriaOperator criteria = CriteriaOperator.And(
                         new BinaryOperator("DocumentHeader.DocumentNumber", 0, BinaryOperatorType.Greater),
                         new BinaryOperator("DocumentHeader.IsCanceled", false, BinaryOperatorType.Equal),
                         new BinaryOperator("DocumentHeader.DocumentSeries.IsCancelingSeries", false, BinaryOperatorType.Equal),
                         new OperandProperty("DocumentHeader.GCRecord").IsNull(),
                         new OperandProperty("DocumentHeader.CanceledByDocument").IsNull(),
                         new OperandProperty("GCRecord").IsNull(),
                         new BinaryOperator("IsCanceled", false, BinaryOperatorType.Equal),
                         new InOperator("ItemCode", requestCodes),
                         new InOperator("DocumentHeader.DocumentType.Oid", AllDocumentTypes.Where(x => x.Division.Section == section && x.UpdateSalesRecords).Select(x => x.Oid)?.ToList() ?? new List<Guid>()),
                         CriteriaOperator.Or(
                             CriteriaOperator.And(
                                 new BinaryOperator("DocumentHeader.FinalizedDate", startDate, BinaryOperatorType.GreaterOrEqual),
                                 new BinaryOperator("DocumentHeader.FinalizedDate", enddate, BinaryOperatorType.LessOrEqual)),
                             CriteriaOperator.And(
                                 new BinaryOperator("DocumentHeader.FiscalDate", startDate, BinaryOperatorType.GreaterOrEqual),
                                 new BinaryOperator("DocumentHeader.FiscalDate", enddate, BinaryOperatorType.LessOrEqual))));

                    List<DocumentDetail> details = new XPCollection<DocumentDetail>(uow, criteria).ToList();
                    foreach (string code in requestCodes)
                    {
                        try
                        {
                            List<DocumentDetail> itemSales = details.Where(x => x.ItemCode == code)?.ToList();
                            SaleItem itm = new SaleItem(code);
                            foreach (DocumentDetail dtl in itemSales)
                            {
                                itm.Qty += !dtl.IsReturn ? (dtl.Qty * dtl.DocumentHeader.DocumentType.QuantityFactor) :
                                                         -(dtl.Qty * dtl.DocumentHeader.DocumentType.QuantityFactor);
                            }

                            returnList.Add(itm);
                        }
                        catch (Exception ex)
                        {
                            MvcApplication.WRMLogModule.Log(ex.Message, Platform.Enumerations.KernelLogLevel.Error);
                        }
                    }
                }
                catch (Exception ex2)
                {
                    MvcApplication.WRMLogModule.Log(ex2.Message, Platform.Enumerations.KernelLogLevel.Error);
                }
                return Json(returnList, JsonRequestBehavior.AllowGet);
            }

        }
    }
}
