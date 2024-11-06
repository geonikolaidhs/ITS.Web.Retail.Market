using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.Common;
using ITS.Retail.Model;
using System.Reflection;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;

namespace ITS.Retail.WebClient.Controllers
{
    public class BuyerController : BaseObjController<Buyer>
    {

        private static readonly Dictionary<PropertyInfo, String> propMapping = new Dictionary<PropertyInfo, string>() 
        { 
            { typeof(Buyer).GetProperty("Code"), "BuyerCode" }, 
            { typeof(Buyer).GetProperty("Description"), "BuyerDescription" } 
        };

        protected override Dictionary<PropertyInfo, String> PropertyMapping
        {
            get
            {
                return propMapping;
            }
        }

        [Security(ReturnsPartial = false)]
        public ActionResult Index()
        {
            //Visibility settings
            this.ToolbarOptions.ViewButton.Visible = true;
            this.ToolbarOptions.ExportToButton.Visible = false;
            this.ToolbarOptions.CustomButton.Visible = false;
            this.ToolbarOptions.ShowHideMenu.Visible = false;
            //Javascript Events
            this.ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            this.ToolbarOptions.EditButton.OnClick = "EditSelectedRowsFromGrid";
            this.ToolbarOptions.NewButton.OnClick = "AddNewFromGrid";
            this.ToolbarOptions.ViewButton.OnClick = "ShowGenericView";
            this.CustomJSProperties.AddJSProperty("gridName", "grdBuyer");

            return View("Index", GetList<Buyer>(XpoHelper.GetNewUnitOfWork()).AsEnumerable<Buyer>());
        }

        protected override GenericViewRuleset GenerateGenericViewRuleset()
        {
            GenericViewRuleset ruleset = base.GenerateGenericViewRuleset();
            ruleset.DetailsToIgnore.Add("Items");
            ruleset.PropertiesToIgnore.Add("IsDefault");
            ruleset.PropertiesToIgnore.Add("IsActive");
            return ruleset;
        }
    }
}
