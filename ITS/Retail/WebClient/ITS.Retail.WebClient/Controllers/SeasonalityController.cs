using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.Common;
using ITS.Retail.Model;
using System.Reflection;

namespace ITS.Retail.WebClient.Controllers
{
    public class SeasonalityController : BaseObjController<Seasonality>
    {
        private static readonly Dictionary<PropertyInfo, String> propMapping = new Dictionary<PropertyInfo, string>() 
        { 
                    { typeof(Seasonality).GetProperty("Code"), "SeasonalityCode" }, 
                    { typeof(Seasonality).GetProperty("Description"), "SeasonalityDescription" } 
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
            //this.ToolbarOptions.Visible = true;
            this.ToolbarOptions.ViewButton.Visible = false;
            this.ToolbarOptions.ShowHideMenu.Visible = false;
            this.ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            this.ToolbarOptions.EditButton.OnClick = "EditSelectedRowsFromGrid";
            this.ToolbarOptions.NewButton.OnClick = "AddNewFromGrid";

            this.CustomJSProperties.AddJSProperty("gridName", "grdSeasonality");
            this.ToolbarOptions.ExportToButton.Visible = false;
            return View("Index", GetList<Seasonality>(XpoHelper.GetNewUnitOfWork()).AsEnumerable<Seasonality>());
        }
    }

}
