using System.Linq;
using System.Web.Mvc;
using ITS.Retail.Model;
using ITS.Retail.Common;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;

namespace ITS.Retail.WebClient.Controllers
{
    public class DocumentStatusController : BaseObjController<DocumentStatus>
    {
        [Security(ReturnsPartial = false), Display(ShowSettings = true)]
        public ActionResult Index()
        {
            ToolbarOptions.ViewButton.Visible = false;
            ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            ToolbarOptions.EditButton.OnClick = "EditSelectedRowsFromGrid";
            ToolbarOptions.NewButton.OnClick = "AddNewFromGrid";
            ToolbarOptions.ViewButton.Visible = true;
            ToolbarOptions.ViewButton.OnClick = "ShowGenericView";
            ToolbarOptions.OptionsButton.Visible = false;
            ToolbarOptions.ExportToButton.Visible = false;
            CustomJSProperties.AddJSProperty("gridName", "grdDocumentStatus");
            

            return View("Index", GetList<DocumentStatus>(XpoHelper.GetNewUnitOfWork()).AsEnumerable<DocumentStatus>());
        }


        protected override GenericViewRuleset GenerateGenericViewRuleset()
        {
            GenericViewRuleset ruleset = base.GenerateGenericViewRuleset();
            ruleset.PropertiesToIgnore.Add("IsActive");
            ruleset.ShowDetails = false;
            ruleset.NumberOfColumns = 1;
            return ruleset;
        }

    }
}
