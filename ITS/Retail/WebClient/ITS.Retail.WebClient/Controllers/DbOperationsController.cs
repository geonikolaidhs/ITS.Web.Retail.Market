using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using ITS.Retail.Common;
using ITS.Retail.WebClient.Providers;
using ITS.Retail.WebClient.ViewModel;
using DevExpress.Xpo.DB;
using ITS.Retail.WebClient.Helpers;


namespace ITS.Retail.WebClient.Controllers
{
    public class DbOperationsController : BaseController
    {
        // GET: /DbOperations/
        public ActionResult Index()
        {
            if (UserHelper.IsSystemAdmin(this.CurrentUser))
            {
                return View();
            }
            return new RedirectResult("~");
        }

        public ActionResult TableComboPartialCallbackPanel(string ExtraData)
        {
            IEnumerable<XPClassInfo> xp = XpoHelper.dict.Classes.Cast<XPClassInfo>();
            IEnumerable<String> tables = xp.OrderBy(g => g.TableName).Where(g => g.IsPersistent).Select(g => g.TableName);
            ViewData["Tables"] = tables;
            ViewData["Table"] = ExtraData;
            return PartialView();
        }

        public ActionResult Grid()
        {
            return PartialView("ResultGrid",Session["ResultGridData"]);
        }

        public ActionResult Operation([ModelBinder(typeof(RetailModelBinder))] DbOperationsViewModel request)
        {
            if (UserHelper.IsSystemAdmin(this.CurrentUser)==false)
            {
                return new RedirectResult("~");
            }           
            string sqlQuery = "";
            ViewData["UpdatedOnTick"] = request.UpdatedOnTick;
            ViewData["Where"] = request.Where;                        
            ViewData["Operation"] = request.Operation;

            Session["ResultGridData"] = null;

            bool selectQuery = false;
            switch (request.Operation)
            {
                case eOperation.SELECT:
                    ViewData["Select"] = request.Select;
                    ViewData["SetQuery"] = "";
                    ViewData["Table"] = "";
                    if (String.IsNullOrEmpty(request.Select))
                    {
                        Session["Error"] = "Field 'Select' is Empty";
                        return View("Index");
                    }
                    if (String.IsNullOrEmpty(request.Where))
                    {
                        Session["Error"] = "Field 'Where' is Empty";
                        return View("Index");
                    }
                    sqlQuery = string.Format("SELECT {0} WHERE {1}", request.Select, request.Where);
                    selectQuery = true;
                    break;
                case eOperation.UPDATE:
                    ViewData["Select"] = "";
                    ViewData["SetQuery"] = request.Set;
                    ViewData["Table"] = request.Table;
                    if (String.IsNullOrEmpty(request.Set))
                    {
                        Session["Error"] = "Field 'Set' is Empty";
                        return View("Index");
                    }
                    if (String.IsNullOrEmpty(request.Where))
                    {
                        Session["Error"] = "Field 'Where' is Empty";
                        return View("Index");
                    }
                    if (String.IsNullOrEmpty(request.Table))
                    {
                        Session["Error"] = "Field 'Table' is Empty";
                        return View("Index");
                    }

                    if (request.UpdatedOnTick == false)
                    {
                        sqlQuery = string.Format("UPDATE \"{0}\" SET {1} WHERE {2}", request.Table, request.Set, request.Where);
                    }
                    else
                    {
                        sqlQuery = string.Format("UPDATE \"{0}\" SET {1}, \"UpdatedOnTicks\" = \"{2}\" WHERE {3}", request.Table, request.Set, DateTime.Now.Ticks, request.Where);
                    }
                    break;
            }
            try
            {
                if (selectQuery)
                {
                    SelectedData result = XpoSession.ExecuteQueryWithMetadata(sqlQuery);
                    XPDataView dv = new XPDataView();
                    foreach (var row in result.ResultSet[0].Rows)
                    {
                        dv.AddProperty((string)row.Values[0], DBColumn.GetType((DBColumnType)Enum.Parse(typeof(DBColumnType), (string)row.Values[2])));
                    }
                    dv.LoadData(new SelectedData(result.ResultSet[1]));
                    ViewData["PreviewQuery"] = sqlQuery;
                    Session["ResultGridData"] = dv;
                    return View("Index");
                }
                else
                {
                    if (String.IsNullOrEmpty(request.Preview))
                    {
                        var result = XpoSession.ExecuteNonQuery(sqlQuery);
                        ViewData["RowsAffected"] = result;
                    }
                    else
                    {
                        ViewData["PreviewQuery"] = sqlQuery;
                    }
                    return View("Index");
                }
            }
            catch (Exception ex)
            {
                Session["Error"] = ex.Message + Environment.NewLine + ((ex.InnerException == null) ? "" : ex.InnerException.Message);
                return View("Index");
            }
        }

    }
}
