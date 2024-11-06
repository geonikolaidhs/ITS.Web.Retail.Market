using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.Model;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;

namespace ITS.Retail.WebClient.Controllers
{
    public class AddressTypeController : BaseObjController<AddressType>
    {
        [Security(ReturnsPartial = false), Display(ShowSettings = true)]
        [OutputCache(Duration = 10, Location = System.Web.UI.OutputCacheLocation.Server)]
        public ActionResult Index()
        {
            ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            ToolbarOptions.EditButton.OnClick = "EditSelectedRowsFromGrid";
            ToolbarOptions.NewButton.OnClick = "AddNewFromGrid";
            ToolbarOptions.ViewButton.Visible = true;
            ToolbarOptions.ViewButton.OnClick = "ShowGenericView";
            CustomJSProperties.AddJSProperty("gridName", "grdAddressType");
            ToolbarOptions.ExportToButton.Visible = false;
            ToolbarOptions.OptionsButton.Visible = false;
            return View("Index", GetList<AddressType>(XpoSession).AsEnumerable<AddressType>());
        }

        protected override GenericViewRuleset GenerateGenericViewRuleset()
        {
            GenericViewRuleset ruleset = base.GenerateGenericViewRuleset();
            ruleset.PropertiesToIgnore.AddRange(new List<string>() { "IsDefault", "IsActive"});
            ruleset.ShowDetails = false;
            ruleset.NumberOfColumns = 1;
            return ruleset;
        }
    }
}
