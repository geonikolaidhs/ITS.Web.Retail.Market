#if _RETAIL_WEBCLIENT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ITS.Retail.Model;
using ITS.Retail.Common;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Enumerations;
using System.Web.Script.Serialization;
using ITS.Retail.WebClient.Helpers;

namespace ITS.Retail.WebClient.Controllers
{
    public class StoreControllerStatusController : BaseObjController<StoreControllerSettings>
    {
        //
        // GET: /StoreControllerStatus/

        public ActionResult Index()
        {
            ToolbarOptions.ForceVisible = false;
            ToolbarOptions.ViewButton.Visible = false;
            ToolbarOptions.ExportToButton.Visible = false;
            ToolbarOptions.CustomButton.Visible = false;
            ToolbarOptions.OptionsButton.Visible = false;
            ToolbarOptions.ShowHideMenu.Visible = false;
            ToolbarOptions.DeleteButton.Visible = false;
            ToolbarOptions.NewButton.Visible = false;
            ToolbarOptions.EditButton.Visible = false;
            FillLookupComboBoxes();
            this.CustomJSProperties.AddJSProperty("gridName", "grdStoreControllerSettings");
            return View(GetList<StoreControllerSettings>(this.XpoSession, null, "ID"));
            //return View();
        }

        public override ActionResult Grid()
        {

            return base.Grid();
        }
        public ActionResult StoreControllerListCheck()
        {
            
             try
             {
                 String[] selectedStores = new JavaScriptSerializer().Deserialize<String[]>(Request["stores"].ToString());
                 eStoreControllerCommand storeCommand = (eStoreControllerCommand)Enum.Parse(typeof(eStoreControllerCommand), Request["command"].ToString());
                 string parameters = Request["parameters"];
                 foreach (String sstore in selectedStores)
                 {
                     Guid store = Guid.Empty;
                     if (Guid.TryParse(sstore, out store) == true && store != Guid.Empty)
                     {
                         StoreHelper.AddStoreCommand(store, storeCommand, parameters);
                     }
                 }
             }
             catch (Exception exception)
             {
                 string errorMessage = exception.GetFullMessage();
             }

            return PartialView("StoreControllerListCheck", GetList<StoreControllerSettings>(this.XpoSession, null, "ID"));
        }

    }
}
#endif