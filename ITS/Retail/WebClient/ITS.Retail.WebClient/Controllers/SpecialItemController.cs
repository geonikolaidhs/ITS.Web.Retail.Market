using System.Web.Mvc;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;

namespace ITS.Retail.WebClient.Controllers
{
    public class SpecialItemController : BaseObjController<SpecialItem>
    {
        [Security(ReturnsPartial=false),Display(ShowSettings=true)]
        public ActionResult Index()
        {
            ToolbarOptions.ExportToButton.Visible = false;
            ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            ToolbarOptions.EditButton.OnClick = "EditSelectedRowsFromGrid";
            ToolbarOptions.NewButton.OnClick = "AddNewFromGrid";
            ToolbarOptions.ViewButton.Visible = true;
            ToolbarOptions.ViewButton.OnClick = "ShowGenericView";
            ToolbarOptions.OptionsButton.Visible = false;
            CustomJSProperties.AddJSProperty("gridName", "grdSpecialItems");
            return View("Index", GetList<SpecialItem>(XpoHelper.GetNewUnitOfWork()));
        }

        protected override GenericViewRuleset GenerateGenericViewRuleset()
        {
            GenericViewRuleset ruleset = base.GenerateGenericViewRuleset();
            ruleset.PropertiesToIgnore.Add("IsDefault");
            ruleset.PropertiesToIgnore.Add("IsActive");
            ruleset.NumberOfColumns = 1;
            return ruleset;
        }
    }
}
