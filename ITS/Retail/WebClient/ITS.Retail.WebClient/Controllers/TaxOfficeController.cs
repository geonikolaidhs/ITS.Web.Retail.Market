using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.Model;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;

namespace ITS.Retail.WebClient.Controllers
{
    public class TaxOfficeController : BaseObjController <TaxOffice>
    {
        //
        // GET: /TaxOffice/
        [Security(ReturnsPartial = false), Display(ShowSettings = true)]
        public ActionResult Index()
        {
            this.ToolbarOptions.ExportToButton.Visible = false;
            this.ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            this.ToolbarOptions.EditButton.OnClick = "EditSelectedRowsFromGrid";
            this.ToolbarOptions.NewButton.OnClick = "AddNewFromGrid";
            this.ToolbarOptions.ViewButton.Visible = true;
            this.ToolbarOptions.ViewButton.OnClick = "ShowGenericView";
            ToolbarOptions.OptionsButton.Visible = false;

            this.CustomJSProperties.AddJSProperty("gridName", "grdTaxOffices");
            return View("Index", GetList<TaxOffice>(XpoSession, GridFilter, GridSortingField).AsEnumerable<TaxOffice>());
        }

        protected override GenericViewRuleset GenerateGenericViewRuleset()
        {
            GenericViewRuleset ruleset = base.GenerateGenericViewRuleset();
            ruleset.DetailsToIgnore.Add("Traders");
            ruleset.PropertiesToIgnore.Add("IsDefault");
            ruleset.PropertiesToIgnore.Add("IsActive");
            ruleset.NumberOfColumns = 1;
            return ruleset;
        }

    }
}
