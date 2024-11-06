using DevExpress.Data.Filtering;
using DevExpress.Web;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.Model.Attributes;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Attributes;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;
using ITS.Retail.WebClient.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace ITS.Retail.WebClient.Controllers
{
    public class CustomDataViewController : BaseObjController<CustomDataView>
    {
        [Security(ReturnsPartial = false), Display(ShowSettings = true)]
        public ActionResult Index()
        {
            this.ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            this.ToolbarOptions.EditButton.OnClick = "EditSelectedRowsFromGrid";
            this.ToolbarOptions.NewButton.OnClick = "AddNewFromGrid";
            this.ToolbarOptions.ViewButton.OnClick = "ShowGenericView";
            this.CustomJSProperties.AddJSProperty("gridName", "grdCustomDataViews");
            this.ToolbarOptions.ViewButton.Visible = true;
            ViewBag.Categories = GetList<CustomDataViewCategory>(XpoSession);
            return View();
        }

        public override ActionResult Grid()
        {
            ViewBag.Categories = GetList<CustomDataViewCategory>(XpoSession);
            if (Request["DXCallbackArgument"] != null)
            {
                if (Request["DXCallbackArgument"].Contains("STARTEDIT") || Request["DXCallbackArgument"].Contains("ADDNEWROW"))
                {
                    UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
                    Session["CustomDataView"] = uow.GetObjectByKey<CustomDataView>(RetailHelper.GetOidToEditFromDxCallbackArgument(Request["DXCallbackArgument"])) ?? new CustomDataView(XpoHelper.GetNewUnitOfWork());
                    ViewBag.Roles = GetList<Role>(uow);
                }
            }         
            Guid categoryGuid;
            GridFilter = Guid.TryParse(Request["CustomDataViewsCategory"], out categoryGuid) ? new BinaryOperator("Category", categoryGuid) :
            new BinaryOperator("Oid", Guid.Empty);
            return base.Grid();
        }

        protected override GenericViewRuleset GenerateGenericViewRuleset()
        {
            GenericViewRuleset ruleset = base.GenerateGenericViewRuleset();
            ruleset.PropertiesToIgnore.Add("IsActive");
            ruleset.DetailPropertiesToIgnore.Add(typeof(CustomDataViewParameter),new List<string>() {"IsActive", "Value" });
            ruleset.DetailPropertiesToIgnore.Add(typeof(CustomDataViewShowSettings), new List<string>() { "IsActive", "Description" });
            ruleset.DetailPropertiesToIgnore.Add(typeof(Role), new List<string>() { "IsActive", "IsDefault", "Type" });
            return ruleset;
        }

        public static object DataViewCategoryRequestedByFilterCondition(ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            XPCollection<CustomDataViewCategory> collection = GetList<CustomDataViewCategory>(XpoHelper.GetNewUnitOfWork());
            CustomDataView dataView = (CustomDataView)System.Web.HttpContext.Current.Session["CustomDataView"];
            collection.SkipReturnedObjects = e.BeginIndex;
            collection.TopReturnedObjects = e.EndIndex - e.BeginIndex + 1;
            collection.Criteria = CriteriaOperator.Or(new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Description"), e.Filter),
                                                      new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Code"), e.Filter)
                                                      //new BinaryOperator("Description", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like),
                                                      //new BinaryOperator("Code", String.Format("%{0}%", e.Filter), BinaryOperatorType.Like)
                                                     );
            collection.Sorting.Add(new SortProperty("Description", SortingDirection.Ascending));
            return collection;
        }

        public ActionResult DataViewUpdatePartial([ModelBinder(typeof(RetailModelBinder))] CustomDataView ct)
        {
            if(ModelState.IsValid)
            {
                CustomDataView dataView = Session["CustomDataView"] as CustomDataView;
                dataView.GetData(ct, new List<string>() { "Session" });
                if (!ActionTypeHelper.AllQueryParametersAreDefined(dataView))
                {
                    Session["Error"] = Resources.NotAllSqlParametersDefined;
                    ModelState.AddModelError("Query", Resources.NotAllSqlParametersDefined);
                }
                
                Guid categoryOid;
                dataView.Category = Guid.TryParse(Request["CustomDataViewCategoryCb_VI"].ToString(), out categoryOid) ? 
                                    dataView.Session.GetObjectByKey<CustomDataViewCategory>(categoryOid) : null;
                List<string> roles = Request["lstRoles"].Split('|').ToList();
                roles.Remove(roles.First());
                roles = roles.Select(oid => oid.Substring(0, Guid.Empty.ToString().Length)).ToList();
                foreach (Role role in dataView.Roles.ToList())
                {
                    dataView.Roles.Remove(role);
                }
                foreach (string oid in roles)
                {
                    Guid guid;
                    if (Guid.TryParse(oid, out guid))
                    {
                        dataView.Roles.Add(dataView.Session.GetObjectByKey<Role>(guid));
                    }
                }

                AssignOwner(dataView);
                dataView.Save();
                XpoHelper.CommitTransaction(dataView.Session);
            }
            return base.Grid();
        }

        public ActionResult CustomDataViewShowSettingsGrid()
        {
            ViewBag.ControllerTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => type.GetCustomAttributes(typeof(CustomDataViewShowAttribute), false).FirstOrDefault() != null).Select(type => type.Name);
            return PartialView((Session["CustomDataView"] as CustomDataView).ShowSettings);
        }

        public ActionResult DataViewShowSettingsUpdatePartial([ModelBinder(typeof(RetailModelBinder))] CustomDataViewShowSettings ct)
        {
            CustomDataView dataView = Session["CustomDataView"] as CustomDataView;
            if (ModelState.IsValid)
            {
                CustomDataViewShowSettings existingDefault = null;
                if (ct.IsDefault)
                {
                    existingDefault = dataView.Session.FindObject<CustomDataViewShowSettings>(PersistentCriteriaEvaluationBehavior.InTransaction,
                                                            RetailHelper.ApplyOwnerCriteria(CriteriaOperator.And(new BinaryOperator("EntityType", ct.EntityType),
                                                                                                                 //new BinaryOperator("DisplayValuesMode", ct.DisplayValuesMode),
                                                                                                                  new BinaryOperator("IsDefault", true),
                                                                                                                  new BinaryOperator("Oid", ct.Oid, BinaryOperatorType.NotEqual)),
                                                                                                                 typeof(CustomDataViewShowSettings), CurrentOwner));
                }


               

                if(existingDefault == null)
                {
                    CustomDataViewShowSettings customDataViewShowSettings = dataView.Session.GetObjectByKey<CustomDataViewShowSettings>(ct.Oid);
                    if (customDataViewShowSettings == null)
                    {
                        customDataViewShowSettings = new CustomDataViewShowSettings(dataView.Session);
                    }
                    customDataViewShowSettings.GetData(ct, new List<string>() {"Session"});
                    AssignOwner(customDataViewShowSettings);
                    dataView.ShowSettings.Add(customDataViewShowSettings);
                    customDataViewShowSettings.Save();
                }
                else
                {
                    Session["Error"] = Resources.DefaultCustomDataViewSettingAlreadyExists;
                    ModelState.AddModelError("IsDefault", Resources.DefaultCustomDataViewSettingAlreadyExists);
                }
            }
            ViewBag.ControllerTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => type.GetCustomAttributes(typeof(CustomDataViewShowAttribute), false).FirstOrDefault() != null).Select(type => type.Name);
            return PartialView("CustomDataViewShowSettingsGrid", dataView.ShowSettings);
        }

        public ActionResult DataViewShowSettingsDeletePartial([ModelBinder(typeof(RetailModelBinder))] CustomDataViewShowSettings ct)
        {
            CustomDataView dataView = Session["CustomDataView"] as CustomDataView;
            CustomDataViewShowSettings customDataViewShowSettings = dataView.ShowSettings.FirstOrDefault(x=> x.Oid == ct.Oid);

            if (customDataViewShowSettings != null)
            {
                dataView.Session.Delete(customDataViewShowSettings);
            }
            
            return PartialView("CustomDataViewShowSettingsGrid", dataView.ShowSettings);
        }

        public ActionResult CustomDataViewParametersGrid()
        {
            ViewBag.ParameterTypes = builtInTypes.Union(modelTypes).Select(type => type.Name);
            return PartialView((Session["CustomDataView"] as CustomDataView).Parameters);
        }

        public ActionResult DataViewParameterUpdatePartial([ModelBinder(typeof(RetailModelBinder))] CustomDataViewParameter ct)
        {
            CustomDataView dataView = Session["CustomDataView"] as CustomDataView;
            if (ModelState.IsValid)
            {
                ct.Name = ct.Name.TrimStart().TrimEnd();

                if(ct.Name != "{OIDS}")
                {
                    CustomDataViewParameter customDataViewParameter = dataView.Session.GetObjectByKey<CustomDataViewParameter>(ct.Oid);
                    if (customDataViewParameter == null)
                    {
                        customDataViewParameter = new CustomDataViewParameter(dataView.Session);
                    }
                    customDataViewParameter.GetData(ct, new List<string>() { "Session" });
                    AssignOwner(customDataViewParameter);
                    dataView.Parameters.Add(customDataViewParameter);
                    customDataViewParameter.Save();
                }
                else
                {
                    Session["Error"] = Resources.ReservedNameParameter;
                }
            }
            return PartialView("CustomDataViewParametersGrid", dataView.Parameters);
        }

        public ActionResult DataViewParameterDeletePartial([ModelBinder(typeof(RetailModelBinder))] CustomDataViewParameter ct)
        {
            CustomDataView dataView = Session["CustomDataView"] as CustomDataView;
            CustomDataViewParameter customDataViewParameter = dataView.Parameters.FirstOrDefault(x => x.Oid == ct.Oid);
            if (customDataViewParameter != null)
            {
                dataView.Session.Delete(customDataViewParameter);
            }
            
            return PartialView("CustomDataViewParametersGrid", dataView.Parameters);
        }
        public ActionResult CustomDataViewCategoryComboBox()
        {
            ViewBag.CategoriesForEdit = GetList<CustomDataViewCategory>(XpoHelper.GetNewUnitOfWork());
            return PartialView(((CustomDataView)Session["CustomDataView"]).Category);
        }

        Type[] builtInTypes = new Type[] { typeof(decimal), typeof(string), typeof(DateTime), typeof(bool) };
        IEnumerable<Type> modelTypes = typeof(BasicObj).Assembly.GetTypes().Where(type => type.GetCustomAttributes(typeof(DataViewParameterAttribute), false).FirstOrDefault() != null);
    }
}
