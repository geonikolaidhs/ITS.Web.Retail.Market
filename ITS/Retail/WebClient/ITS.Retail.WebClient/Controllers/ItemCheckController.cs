using ITS.Retail.Model.MobileInventory;
using System.Web.Mvc;

namespace ITS.Retail.WebClient.Controllers
{
    public class ItemCheckController : BaseObjController<ItemCheck>
    {
        public ActionResult Index()
        {
            this.CustomJSProperties.AddJSProperty("gridName", "grdItemCheck");
            ToolbarOptions.DeleteButton.Visible = true;
            ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            ToolbarOptions.EditButton.Visible = false;
            ToolbarOptions.NewButton.Visible = false;
            ToolbarOptions.ExportToButton.Visible = false;
            ToolbarOptions.OptionsButton.Visible = false;
            return View(GetList<ItemCheck>(XpoSession, GridFilter, GridSortingField));
        }

        public override ActionResult Grid()
        {            
            return base.Grid();
        }

    }
}