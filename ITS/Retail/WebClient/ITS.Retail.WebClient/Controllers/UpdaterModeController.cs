using ITS.Retail.Common;
using DevExpress.Xpo;
using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.WebClient.Controllers
{
    [StoreControllerEditable]
    public class UpdaterModeController : BaseObjController<UpdaterMode>
    {

        [Security(ReturnsPartial = false), Display(ShowSettings = true)]
        public ActionResult Index()
        {
            ToolbarOptions.ExportToButton.Visible = false;
            ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            ToolbarOptions.EditButton.OnClick = "EditSelectedRowsFromGrid";
            ToolbarOptions.NewButton.OnClick = "AddNewFromGrid";
            ToolbarOptions.OptionsButton.Visible = false;
            CustomJSProperties.AddJSProperty("gridName", "grdUpdaterMode");

            FillLookupComboBoxes();
            return View(GetList<UpdaterMode>(XpoHelper.GetNewUnitOfWork()));
        }

        protected override void FillLookupComboBoxes()
        {
            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();
            base.FillLookupComboBoxes();
            XPCollection<UpdaterMode> existing = GetList<UpdaterMode>(uow);
            List<string> excludeFromCombo = existing.Select(x => x.EntityName).ToList();
            if (IncludeCurrent != null && IncludeCurrent != Guid.Empty)
            {
                string typeToExclude = uow.GetObjectByKey<UpdaterMode>(IncludeCurrent).EntityName;
                excludeFromCombo.Remove(typeToExclude);
            }
            IEnumerable<Type> types = typeof(Customer).Assembly.GetTypes().Where
                                                                    (g => g.IsSubclassOf(typeof(BaseObj))
                                                                    && g.GetCustomAttributes(typeof(UpdaterAttribute), false).Count() > 0
                                                                    && ((UpdaterAttribute)(g.GetCustomAttributes(typeof(UpdaterAttribute), false).First())).Permissions.HasFlag(eUpdateDirection.STORECONTROLLER_TO_POS)
                                                                    && excludeFromCombo.Contains(g.Name) == false);
            ViewBag.Entities = types.Select(p => new KeyValuePair<string, string>(p.Name, p.ToLocalizedString())).OrderBy(p => p.Value).ToDictionary(p => p.Key, p => p.Value);

        }

        public override ActionResult Grid()
        {
            if (Request["DXCallbackArgument"].Contains("STARTEDIT"))
            {
                string editOid = Request["DXCallbackArgument"].Split('|').Last().Trim().Trim(';');
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
