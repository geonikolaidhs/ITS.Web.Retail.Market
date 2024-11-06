//#if _RETAIL_STORECONTROLLER || _RETAIL_DUAL
using System.Text;
using System.Web.Mvc;
using DevExpress.Xpo;
using ITS.Retail.Model;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;

namespace ITS.Retail.WebClient.Controllers
{
    [StoreControllerEditable]
    public class ScaleController : BaseObjController<Scale>
    {
        [Display(ShowSettings = true)]
        public ActionResult Index()
        {
            ToolbarOptions.ExportToButton.Visible = false;
            ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            ToolbarOptions.EditButton.OnClick = "EditSelectedRowsFromGrid";
            ToolbarOptions.NewButton.OnClick = "AddNewFromGrid";
            ToolbarOptions.ViewButton.Visible = true;
            ToolbarOptions.ViewButton.OnClick = "ShowGenericView";
            ToolbarOptions.OptionsButton.Visible = false;
            CustomJSProperties.AddJSProperty("gridName", "grdScales");

            XPCollection<Scale> all = GetList<Scale>(this.XpoSession);
            return View(all);
        }

        public override ActionResult Grid()
        {
            return base.Grid();
        }

        protected override void FillLookupComboBoxes()
        {
            base.FillLookupComboBoxes();
            ViewBag.Encodings = Encoding.GetEncodings();
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
//#endif