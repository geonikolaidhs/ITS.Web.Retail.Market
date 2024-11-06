using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.Model;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Common;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Providers;

namespace ITS.Retail.WebClient.Controllers
{

    [StoreControllerEditable]
    public class FiscalDevicesController : BaseObjController<StoreControllerTerminalDeviceAssociation>
    {
        [Security(ReturnsPartial = false)]
        [Display(ShowSettings = true)]
        public ActionResult Index()
        {
            this.ToolbarOptions.ExportToButton.Visible = false;
            this.ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            this.ToolbarOptions.EditButton.OnClick = "EditSelectedRowsFromGrid";
            this.ToolbarOptions.NewButton.OnClick = "AddNewFromGrid";
            ToolbarOptions.OptionsButton.Visible = false;

            this.CustomJSProperties.AddJSProperty("gridName", "grdFiscalDevices");
            FillLookupComboBoxes();
            return View("Index", GetList<StoreControllerTerminalDeviceAssociation>(XpoHelper.GetNewUnitOfWork(), GridFilter, GridSortingField).AsEnumerable());
        }

        protected List<DeviceType> GetFiscalDeviceTypes()
        {
            IEnumerable<DeviceType> allDevicetypes = Enum.GetValues(typeof(DeviceType)).OfType<DeviceType>();
            return allDevicetypes.Where(x => x.GetFiscalDevice() != null && ((eFiscalDevice)x.GetFiscalDevice()).GetFiscalMethod() == eFiscalMethod.EAFDSS).ToList();
        }

        protected override void FillLookupComboBoxes()
        {
            base.FillLookupComboBoxes();
            CriteriaOperator documentSeriesCrop = null;
            switch (MvcApplication.ApplicationInstance)
            {
                case eApplicationInstance.DUAL_MODE:
                    documentSeriesCrop = null;
                    break;
                case eApplicationInstance.STORE_CONTROLER:
                    documentSeriesCrop = CriteriaOperator.Or(
                        new BinaryOperator("eModule", eModule.ALL),
                    new BinaryOperator("eModule", eModule.STORECONTROLLER));
                    break;
                case eApplicationInstance.RETAIL:
                    documentSeriesCrop = CriteriaOperator.Or(
                       new BinaryOperator("eModule", eModule.ALL),
                   new BinaryOperator("eModule", eModule.HEADQUARTERS));
                    break;
            }
            ViewBag.SignatureDevices = GetList<POSDevice>(XpoSession, new InOperator("DeviceSettings.DeviceType", GetFiscalDeviceTypes()));
            ViewBag.DocumentSeries = GetList<DocumentSeries>(XpoSession, documentSeriesCrop);
        }

        public override ActionResult InsertPartial([ModelBinder(typeof(RetailModelBinder))] StoreControllerTerminalDeviceAssociation ct)
        {
            if (String.IsNullOrWhiteSpace(Request["Device_VI"]))
            {
                ModelState.AddModelError("Device", Resources.SelectDevice);
            }
            if (ModelState.IsValid == false)
            {
                ViewBag.CurrentItem = ct;
                FillLookupComboBoxes();
                return PartialView("Grid", GetList<StoreControllerTerminalDeviceAssociation>(XpoSession).AsEnumerable());
            }
            else
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    StoreControllerTerminalDeviceAssociation objectToBeSaved = new StoreControllerTerminalDeviceAssociation(uow);
                    UpdateModel(objectToBeSaved);
                    switch (MvcApplication.ApplicationInstance)
                    {
                        case eApplicationInstance.DUAL_MODE:
                        case eApplicationInstance.STORE_CONTROLER:
                            Store store = uow.GetObjectByKey<Store>(this.CurrentStore.Oid);
                            objectToBeSaved.TerminalDevice = uow.GetObjectByKey<TerminalDevice>(Guid.Parse(Request["Device_VI"]));
                            break;
                        case eApplicationInstance.RETAIL:
                            objectToBeSaved.StoreControllerSettings = null;
                            break;
                    }
                    objectToBeSaved.TerminalDevice = uow.GetObjectByKey<TerminalDevice>(Guid.Parse(Request["Device_VI"]));
                    IEnumerable<Guid> selectedSeries;

                    if (String.IsNullOrEmpty(Request["fiscaldevices_selected"]))
                    {
                        selectedSeries = null;
                    }
                    else
                    {
                        selectedSeries = Request["fiscaldevices_selected"].Split(',').Select(x => Guid.Parse(x));
                        foreach (Guid series in selectedSeries)
                        {
                            DocumentSeries documentSeries = uow.GetObjectByKey<DocumentSeries>(series);
                            FiscalDeviceDocumentSeries fiscalDeviceDocumentSeries = new FiscalDeviceDocumentSeries(uow);
                            fiscalDeviceDocumentSeries.FiscalDevice = objectToBeSaved;
                            fiscalDeviceDocumentSeries.DocumentSeries = documentSeries;
                        }
                    }

                    AssignOwner(objectToBeSaved);
                    ViewBag.CurrentItem = objectToBeSaved;
                    try
                    {
                        XpoHelper.CommitChanges(uow);

                    }
                    catch (Exception ex)
                    {
                        Session["Error"] = ex.Message;
                    }
                }
                FillLookupComboBoxes();
                return PartialView("Grid", GetList<StoreControllerTerminalDeviceAssociation>(XpoSession).AsEnumerable());
            }
        }

        public override ActionResult UpdatePartial([ModelBinder(typeof(RetailModelBinder))] StoreControllerTerminalDeviceAssociation ct)
        {
            if (String.IsNullOrWhiteSpace(Request["Device_VI"]))
            {
                ModelState.AddModelError("Device", Resources.SelectDevice);
            }
            if (ModelState.IsValid == false)
            {
                FillLookupComboBoxes();
                ViewBag.CurrentItem = ct;
                return PartialView("Grid", GetList<StoreControllerTerminalDeviceAssociation>(XpoSession).AsEnumerable());
            }
            else
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    StoreControllerTerminalDeviceAssociation objectToBeSaved = uow.GetObjectByKey<StoreControllerTerminalDeviceAssociation>(ct.Oid);
                    UpdateModel(objectToBeSaved);
                    switch (MvcApplication.ApplicationInstance)
                    {
                        case eApplicationInstance.DUAL_MODE:
                        case eApplicationInstance.STORE_CONTROLER:
                            Store store = uow.GetObjectByKey<Store>(this.CurrentStore.Oid);
                            objectToBeSaved.StoreControllerSettings = uow.GetObjectByKey<StoreControllerSettings>(store.StoreControllerSettings.Oid);

                            break;
                        case eApplicationInstance.RETAIL:
                            objectToBeSaved.StoreControllerSettings = null;
                            break;
                    }
                    objectToBeSaved.TerminalDevice = uow.GetObjectByKey<TerminalDevice>(Guid.Parse(Request["Device_VI"]));
                    IEnumerable<Guid> selectedSeries, seriesToAdd, seriesToRemove, existingSeries;
                    existingSeries = objectToBeSaved.DocumentSeries.Select(x => x.DocumentSeries.Oid);

                    if (String.IsNullOrEmpty(Request["fiscaldevices_selected"])) //In case no Document Series are selected
                    {
                        seriesToRemove = existingSeries;
                    }
                    else
                    {
                        selectedSeries = Request["fiscaldevices_selected"].Split(',').Select(x => Guid.Parse(x));
                        seriesToAdd = selectedSeries.Except(existingSeries);
                        seriesToRemove = existingSeries.Except(selectedSeries);
                        foreach (Guid series in seriesToAdd)
                        {
                            DocumentSeries documentSeries = uow.GetObjectByKey<DocumentSeries>(series);
                            FiscalDeviceDocumentSeries fiscalDeviceDocumentSeries = new FiscalDeviceDocumentSeries(uow);
                            fiscalDeviceDocumentSeries.FiscalDevice = objectToBeSaved;
                            fiscalDeviceDocumentSeries.DocumentSeries = documentSeries;
                        }
                    }
                    objectToBeSaved.DocumentSeries.Where(x => seriesToRemove.Contains(x.DocumentSeries.Oid)).ToList().ForEach(x => x.Delete());

                    AssignOwner(objectToBeSaved);
                    ViewBag.CurrentItem = objectToBeSaved;
                    try
                    {
                        XpoHelper.CommitChanges(uow);

                    }
                    catch (Exception ex)
                    {
                        Session["Error"] = ex.Message;
                    }
                }
                FillLookupComboBoxes();
                return PartialView("Grid", GetList<StoreControllerTerminalDeviceAssociation>(XpoSession).AsEnumerable());
            }
        }
    }

}
