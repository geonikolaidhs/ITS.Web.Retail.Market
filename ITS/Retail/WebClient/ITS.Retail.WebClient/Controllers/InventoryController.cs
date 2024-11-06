using DevExpress.Data.Filtering;
using ITS.Retail.Model.MobileInventory;
using System.Web.Mvc;

namespace ITS.Retail.WebClient.Controllers
{
    [StoreControllerEditable]
    public class InventoryController : BaseObjController<MobileInventoryEntry>
    {
        public ActionResult Index()
        {
            CustomJSProperties.AddJSProperty("gridName", "grdMobileInventory");
            ToolbarOptions.DeleteButton.Visible = true;
            ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            ToolbarOptions.EditButton.Visible = false;
            ToolbarOptions.NewButton.Visible = false;
            ToolbarOptions.ExportToButton.Visible = false;
            ToolbarOptions.OptionsButton.Visible = false;
            GridFilter = CriteriaOperator.Parse("IsExactType(This,?)", typeof(MobileInventoryEntry).FullName);
            return View(GetList<MobileInventoryEntry>(XpoSession, GridFilter, GridSortingField));
        }

        public override ActionResult Grid()
        {
            GridFilter = CriteriaOperator.Parse("IsExactType(This,?)", typeof(MobileInventoryEntry).FullName);
            return base.Grid();
        }

    }
}
