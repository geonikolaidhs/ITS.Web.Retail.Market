using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;
using System.Web.Mvc;

namespace ITS.Retail.WebClient.Controllers
{
    public class ReasonCategoryController : BaseObjController<ReasonCategory>
    {
        //
        // GET: /ReasonCategory/

        [Security(ReturnsPartial = false), Display(ShowSettings = true)]
        public ActionResult Index()
        {
            this.ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            this.ToolbarOptions.EditButton.OnClick = "EditSelectedRowsFromGrid";
            this.ToolbarOptions.NewButton.OnClick = "AddNewFromGrid";
            this.ToolbarOptions.ViewButton.OnClick = "ShowGenericView";
            this.CustomJSProperties.AddJSProperty("gridName", "grdReasonCategories");
            this.ToolbarOptions.ViewButton.Visible = true;
            return View("Index", GetList<ReasonCategory>(XpoSession));
        }

        protected override GenericViewRuleset GenerateGenericViewRuleset()
        {
            GenericViewRuleset ruleset = base.GenerateGenericViewRuleset();
            ruleset.PropertiesToIgnore.Add("IsActive");
            //ruleset.DetailsToIgnore.Add("DataViews");
            return ruleset;
        }
    }
}
