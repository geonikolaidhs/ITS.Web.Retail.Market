using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ITS.Retail.WebClient.Controllers
{
    public class TransferPurposeController : BaseObjController<TransferPurpose>
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
            CustomJSProperties.AddJSProperty("gridName", "grdTransferPurpose");
            return View(GetList<TransferPurpose>(XpoHelper.GetNewUnitOfWork()).AsEnumerable<TransferPurpose>());
        }

        protected override GenericViewRuleset GenerateGenericViewRuleset()
        {
            GenericViewRuleset ruleset = base.GenerateGenericViewRuleset();
            ruleset.PropertiesToIgnore.Add("IsActive");
            ruleset.NumberOfColumns = 1;
            return ruleset;
        }
    }
}
