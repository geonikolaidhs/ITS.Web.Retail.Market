using DevExpress.Data.Filtering;
using ITS.Retail.Model.MobileInventory;
using System.Web.Mvc;

namespace ITS.Retail.WebClient.Controllers
{
    public class EslInventoryController : BaseObjController<MobileInventoryEslEntry>
    {
        public ActionResult Index()
        {
            this.CustomJSProperties.AddJSProperty("gridName", "grdMobileInventoryEsl");
            ToolbarOptions.DeleteButton.Visible = true;
            ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            ToolbarOptions.EditButton.Visible = false;
            ToolbarOptions.NewButton.Visible = false;
            ToolbarOptions.ExportToButton.Visible = false;
            ToolbarOptions.OptionsButton.Visible = false;
            GridFilter = CriteriaOperator.Parse("IsExactType(This,?)", typeof(MobileInventoryEslEntry).FullName);
            return View(GetList<MobileInventoryEslEntry>(XpoSession, GridFilter, GridSortingField));
        }

        public override ActionResult Grid()
        {
            GridFilter = CriteriaOperator.Parse("IsExactType(This,?)", typeof(MobileInventoryEslEntry).FullName);
            return base.Grid();
        }

    }
}
