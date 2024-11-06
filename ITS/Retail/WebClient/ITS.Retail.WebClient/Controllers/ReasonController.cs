using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;

namespace ITS.Retail.WebClient.Controllers
{
    public class ReasonController : BaseObjController<Reason>
    {
        private static readonly Dictionary<PropertyInfo, string> propMapping = new Dictionary<PropertyInfo, string>()
        {
            { typeof(Reason).GetProperty("Category"), "ReasonCategory_VI" }
        };

        protected override Dictionary<PropertyInfo, string> PropertyMapping
        {
            get
            {
                return propMapping;
            }
        }
        
        [Security(ReturnsPartial = false), Display(ShowSettings = true)]
        public ActionResult Index()
        {
            ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            ToolbarOptions.EditButton.OnClick = "EditSelectedRowsFromGrid";
            ToolbarOptions.NewButton.OnClick = "AddNewFromGrid";
            ToolbarOptions.ViewButton.Visible = true;
            ToolbarOptions.ViewButton.OnClick = "ShowGenericView";
            ToolbarOptions.OptionsButton.Visible = false;
            CustomJSProperties.AddJSProperty("gridName", "grdReasons");
            return View("Index", GetList<Reason>(XpoSession));
        }

        protected override void FillLookupComboBoxes()
        {
            ViewBag.CategoriesForEdit = GetList<ReasonCategory>(XpoHelper.GetNewUnitOfWork());
        }

        protected override GenericViewRuleset GenerateGenericViewRuleset()
        {
            GenericViewRuleset ruleset = base.GenerateGenericViewRuleset();
            ruleset.PropertiesToIgnore.AddRange(new List<string>() { "IsActive", "IsDefault" });
            ruleset.ShowDetails = false;
            ruleset.NumberOfColumns = 1;
            return ruleset;
        }
    }
}
