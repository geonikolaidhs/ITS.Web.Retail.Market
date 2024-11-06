using DevExpress.Data.Filtering;
using DevExpress.Data.Linq;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.Model.Attributes;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.WebClient.Attributes;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.WebClient.ViewModel.Totalisers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ITS.Retail.WebClient.Controllers
{
    [CustomDataViewShow]
    public class VariableValuesDisplayController : BaseController
    {
        [Security(ReturnsPartial = false), Display(ShowSettings = true)]
        public ActionResult Index()
        {
            return View();
        }

        private List<CustomDataViewCategoryViewModel> ReturnCategories()
        {
            List<CustomDataViewCategoryViewModel> Categories = new List<CustomDataViewCategoryViewModel>();
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                XPCollection<CustomDataViewCategory> customDataViewCategories = GetList<CustomDataViewCategory>(uow, new ContainsOperator("DataViews", null));
                foreach (CustomDataViewCategory customDataViewCategory in customDataViewCategories)
                {
                    CustomDataViewCategoryViewModel customDataViewCategoryViewModel = new CustomDataViewCategoryViewModel()
                    {
                        label = customDataViewCategory.Description,
                        value = customDataViewCategory.Oid.ToString(),
                        isDefault = customDataViewCategory.IsDefault
                    };
                    Categories.Add(customDataViewCategoryViewModel);
                }
            }
            return Categories;
        }


        public ActionResult ShowVariableValues(string typeName, string objectID)
        {
            Session["EntityName"] = typeName;
            Session["objectID"] = objectID;
            return PartialView(ReturnCategories());
        }

        public JsonResult GetDataView(Guid? categoryOid)
        {
            try
            {
                string typeName = (string)Session["EntityName"];
                XPCollection<CustomDataView> dataViews = null;
                if (categoryOid.HasValue)
                {
                    dataViews = GetList<CustomDataView>(XpoHelper.GetNewUnitOfWork(),
                                                CriteriaOperator.And(new BinaryOperator("Category", categoryOid),
                                                    UserHelper.IsAdmin(CurrentUser) ?
                                                    null : new ContainsOperator("Roles", new BinaryOperator("Type", CurrentUser.Role.Type)),
                                                    new ContainsOperator("ShowSettings", new BinaryOperator("EntityType",typeName))));
                }
                if (dataViews != null)
                {
                    var result = from dataView in dataViews
                                 select new
                                 {
                                     value = dataView.Oid,
                                     label = dataView.Description,
                                     isDefault = dataView.ShowSettings.FirstOrDefault(set => set.EntityType == typeName
                                     ).IsDefault
                                 };
                    return Json(new { result = dataViews == null ? null : result });
                }
                return Json(new { result = "" });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.InnerException != null ? ex.InnerException.Message : ex.Message });
            }
        }

        public JsonResult GetViewParameters(Guid? dataViewOid)
        {
            try
            {
                UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
                CustomDataView dataView = uow.GetObjectByKey<CustomDataView>(dataViewOid);
                List<dynamic> result = new List<dynamic>();
                if (dataView != null)
                {
                    Type[] builtInTypes = new Type[] { typeof(decimal), typeof(string), typeof(DateTime), typeof(bool) };
                    IEnumerable<Type> types = builtInTypes.Union(typeof(BasicObj).Assembly.GetTypes().Where(type => type.GetCustomAttributes(typeof(DataViewParameterAttribute), false).FirstOrDefault() != null));
                    foreach (CustomDataViewParameter param in dataView.Parameters.Where(par => dataView.Query.Contains(par.Name)))
                    {
                        Type type = types.First(x => x.Name == param.ParameterType);
                        result.Add(new { description = param.Description, name = param.Name, type = param.ParameterType });
                    }
                    return Json(new { result = result });
                }
                return Json(new { result = "" });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.InnerException != null ? ex.InnerException.Message : ex.Message });
            }
        }

        public JsonResult GetVariableValues(Guid? customDataViewOid, string gridOids, string paramValues)
        {
            try
            {
                List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();

                CustomDataView dataView = XpoHelper.GetNewUnitOfWork().GetObjectByKey<CustomDataView>(customDataViewOid);
                if (dataView != null)
                {
                    JArray Jparameters = JsonConvert.DeserializeObject(paramValues) as JArray;
                    Dictionary<string, string> parameters = new Dictionary<string, string>();
                    foreach (JToken param in Jparameters)
                    {
                        parameters.Add(string.Format("{{{0}}}", param.First.First), param.Last.First.ToString());
                    }
                    result = ActionTypeHelper.ShowDataViewDataWeb(dataView.CreateDataView(dataView.Session, gridOids, parameters));
                }

                return Json(new { result = result });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.InnerException != null ? ex.InnerException.Message : ex.Message });
            }
        }

        public JsonResult SearchLookUpObject(string typeStr, string searchTxt = "")
        {
            try
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    CriteriaToExpressionConverter converter = new CriteriaToExpressionConverter();
                    Type type = typeof(BasicObj).Assembly.GetTypes().First(objtype => objtype.GetCustomAttributes(typeof(DataViewParameterAttribute), false).FirstOrDefault() != null
                                                                            && objtype.Name == typeStr);
                    string searchPropertyName = "";
                    if (type.GetProperty("Description") != null)
                    {
                        searchPropertyName = "Description";
                    }
                    else if (type.GetProperty("Name") != null)
                    {
                        searchPropertyName = "Name";
                    }
                    else if (type.GetProperty("Code") != null)
                    {
                        searchPropertyName = "Code";
                    }
                    else
                    {
                        searchPropertyName = "Oid";
                    }
                    XPCollection rows = new XPCollection(uow, type, new BinaryOperator(searchPropertyName, string.Format("%{0}%", searchTxt), BinaryOperatorType.Like)) { TopReturnedObjects = 100 };

                    List<dynamic> result = new List<dynamic>();
                    foreach (BasicObj obj in rows)
                    {
                        result.Add(new { value = obj.GetType().GetProperty("Oid").GetValue(obj, null), label = obj.GetType().GetProperty(searchPropertyName).GetValue(obj, null) });
                    }
                    return Json(new { result = result });
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.InnerException != null ? ex.InnerException.Message : ex.Message });
            }
        }
    }
}