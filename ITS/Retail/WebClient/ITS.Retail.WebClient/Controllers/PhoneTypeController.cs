using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.Model;
using ITS.Retail.Common;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;

namespace ITS.Retail.WebClient.Controllers
{
    public class PhoneTypeController : BaseObjController<PhoneType>
    {
        [Security(ReturnsPartial = false), Display(ShowSettings = true)]
        public ActionResult Index()
        {
            ToolbarOptions.ExportToButton.Visible = false;
            ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            ToolbarOptions.EditButton.OnClick = "EditSelectedRowsFromGrid";
            ToolbarOptions.NewButton.OnClick = "AddNewFromGrid";
            ToolbarOptions.ViewButton.Visible = true;
            ToolbarOptions.ViewButton.OnClick = "ShowGenericView";
            ToolbarOptions.OptionsButton.Visible = false;

            CustomJSProperties.AddJSProperty("gridName", "grdPhoneType");

            return View("Index", GetList<PhoneType>(XpoHelper.GetNewUnitOfWork()).AsEnumerable<PhoneType>());
        }

        protected override GenericViewRuleset GenerateGenericViewRuleset()
        {
            GenericViewRuleset ruleset = base.GenerateGenericViewRuleset();
            ruleset.PropertiesToIgnore.AddRange(new List<string>() { "IsDefault", "IsActive" });
            ruleset.ShowDetails = false;
            ruleset.NumberOfColumns = 1;
            return ruleset;
        }
    }
}
