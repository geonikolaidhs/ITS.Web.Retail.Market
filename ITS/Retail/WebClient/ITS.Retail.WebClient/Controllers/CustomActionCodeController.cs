using ITS.Retail.Common;
using DevExpress.Xpo;
using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.WebClient.Controllers
{
    [StoreControllerEditable]
    public class CustomActionCodeController : BaseObjController<CustomActionCode>
    {

        [Security(ReturnsPartial = false), Display(ShowSettings = true)]
        public ActionResult Index()
        {
            ToolbarOptions.ExportToButton.Visible = false;
            ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            ToolbarOptions.EditButton.OnClick = "EditSelectedRowsFromGrid";
            ToolbarOptions.NewButton.OnClick = "AddNewFromGrid";
            ToolbarOptions.OptionsButton.Visible = false;
            CustomJSProperties.AddJSProperty("gridName", "grdCustomActionCode");

            FillLookupComboBoxes();
            return View(GetList<CustomActionCode>(XpoHelper.GetNewUnitOfWork()));
        }

        protected override void FillLookupComboBoxes()
        {
            base.FillLookupComboBoxes();
            XPCollection<CustomActionCode> existing = GetList<CustomActionCode>(XpoSession);
            List<eActions> excludeFromCombo = existing.Select(x => x.Action).ToList();
            if (IncludeCurrent != null && IncludeCurrent != Guid.Empty)
            {
                eActions typeToExclude = XpoSession.GetObjectByKey<CustomActionCode>(IncludeCurrent).Action;
                excludeFromCombo.Remove(typeToExclude);
            }
            List<eActions> externalActions = POSHelper.GetExternalActions();
            ViewBag.ExternalActions = externalActions.Where(x => excludeFromCombo.Contains(x) == false).OrderBy(x => x.ToLocalizedString()).ToDictionary(x => x, y => y.ToLocalizedString());
        }

        public override ActionResult Grid()
        {
            if (Request["DXCallbackArgument"].Contains("STARTEDIT"))
            {
                String editOid = Request["DXCallbackArgument"].Split('|').Last().Trim().Trim(';');
                Guid editGuid;
                if (Guid.TryParse(editOid, out editGuid))
                {
                    this.IncludeCurrent = editGuid;
                }
            }
            return base.Grid();
        }

        protected Guid IncludeCurrent { get; set; }

    }
}
