using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Common.ViewModel;
using ITS.Retail.Mobile.AuxilliaryClasses;
using ITS.Retail.Model;
using ITS.Retail.Model.NonPersistant;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.WebClient.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Model.SupportingClasses;
using ITS.Retail.WebClient.Helpers.Factories;
using ITS.POS.Hardware;

using System.Globalization;
using ITS.Hardware.RBSPOSEliot.CashRegister;

namespace ITS.Retail.WebClient.Controllers
{
    [StoreControllerEditable]
    public class CashierRegisterController : BaseObjController<PriceCatalogDetail>
    {
        //
        // GET: /CashierRegister/
        UnitOfWork uow;
        
        protected void GenerateUnitOfWork()
        {
            if (Session["uow"] == null)
            {
                uow = XpoHelper.GetNewUnitOfWork();
                Session["uow"] = uow;
            }
            else
            {
                uow = (UnitOfWork)Session["uow"];
            }
        }

        [Security(ReturnsPartial = false), Display(ShowSettings = false)]
        public ActionResult Index()
        {
            GenerateUnitOfWork();
            ToolbarOptions.ForceVisible = true;
            ToolbarOptions.ViewButton.Visible = false;
            ToolbarOptions.ExportToButton.Visible = false;
            ToolbarOptions.CustomButton.Visible = false;
            ToolbarOptions.OptionsButton.Visible = true;
            ToolbarOptions.ShowHideMenu.Visible = false;
            ToolbarOptions.DeleteButton.Visible = true;
            ToolbarOptions.DeleteButton.OnClick = "Cashier.ClearSelectedItems";
            ToolbarOptions.NewButton.Visible = false;
            ToolbarOptions.EditButton.Visible = false;
            ToolbarOptions.TransformButton.Visible = false;
            ToolbarOptions.SendPaymentMethodsButton.Visible = true;
            ToolbarOptions.SendPaymentMethodsButton.OnClick = "Cashier.SendPayments";
            ToolbarOptions.SendItemsButton.Visible = true;
            ToolbarOptions.SendItemsButton.OnClick = "Cashier.SendItems";
            ToolbarOptions.ClearAllItems.Visible = true;
            ToolbarOptions.ClearAllItems.OnClick = "Cashier.ClearAllItems";
            ToolbarOptions.IssueZ.Visible = true;
            ToolbarOptions.IssueZ.OnClick = "Cashier.IssueZ";
            ToolbarOptions.IssueX.Visible = true;
            ToolbarOptions.IssueX.OnClick = "Cashier.IssueX";
            ToolbarOptions.DailySales.Visible = true;
            ToolbarOptions.DailySales.OnClick = "Cashier.DailyTotal";
            ToolbarOptions.DailyItemsSales.Visible = true;
            ToolbarOptions.DailyItemsSales.OnClick = "Cashier.DailyItemsSales";

            this.CustomJSProperties.AddJSProperty("gridName", "grdLabels");
            List<PrintLabelSettings> printsettings = GetList<PrintLabelSettings>(uow, new BinaryOperator("Store.Oid", StoreControllerAppiSettings.CurrentStore.Oid)).ToList();
            ViewBag.Labels = printsettings.OrderByDescending(criteria => criteria.IsDefault);

            List<ITS.Retail.Model.POS> posDevices = GetList<ITS.Retail.Model.POS>(uow, new BinaryOperator("Store.Oid", StoreControllerAppiSettings.CurrentStore.Oid)).ToList();
            ViewBag.CashRegisterDevices = posDevices.Where(x => x.IsCashierRegister == true).ToList();
            return View((object)null);
        }
        public ActionResult CashierWithChangesFilters()
        {
            return PartialView();
        }

        public override ActionResult Grid()
        {
            GenerateUnitOfWork();            
            Guid posOid = Guid.Empty;
            Guid.TryParse(Request["Cashier"], out posOid);
            ITS.Retail.Model.POS pos = uow.GetObjectByKey<ITS.Retail.Model.POS>(posOid);
            if ( pos == null )
            {
                Session["Error"] = String.Format("{0}: {1}", Resources.InvalidValue, Resources.CashierRegister);
                return PartialView(new List<PriceCatalogDetail>());
            }
            POSDevice posDevice = pos.TerminalDeviceAssociations.FirstOrDefault().TerminalDevice as POSDevice;

            if (Request["DXCallbackArgument"].Contains("CLEAR"))
            {
                return PartialView(new List<PriceCatalogDetail>());
            }
            GenerateUnitOfWork();
            if (StoreControllerAppiSettings.CurrentStore.DefaultPriceCatalogPolicy == null)
            {
                Session["Error"] = String.Format(Resources.DefaultPriceCatalogPolicyIsNotDefinedForStore, StoreControllerAppiSettings.CurrentStore.Description);
                return PartialView();
            }
            CriteriaOperator CashierWithValueChangeFilter = new BinaryOperator("Oid", Guid.Empty);
            if (Request["DXCallbackArgument"].Contains("SEARCH"))
            {
                CashierSearchCriteria filter = new CashierSearchCriteria(posDevice);
                TryUpdateModel<CashierSearchCriteria>(filter);
                CashierWithValueChangeFilter = filter.BuildCriteria();
                Session["UserFilter"] = CashierWithValueChangeFilter;
            }
            else if (Session["UserFilter"] is CriteriaOperator)
            {
                CashierWithValueChangeFilter = (CriteriaOperator)Session["UserFilter"];
            }
            Store storeOnUow = uow.GetObjectByKey<Store>(StoreControllerAppiSettings.CurrentStoreOid);
            IEnumerable<PriceCatalogDetail> priceCatalogDetails = PriceCatalogHelper.GetAllSortedPriceCatalogDetails(storeOnUow, CashierWithValueChangeFilter);
            IEnumerable<Item> items = priceCatalogDetails.Select(x => x.Item).Distinct();
            List<ItemCashRegister> CashierRegisterItems = new List<ItemCashRegister>();
            if (items.Count() > 0)
            {
                //Store store = XpoSession.GetObjectByKey<Store>(StoreControllerAppiSettings.CurrentStoreOid);
                EffectivePriceCatalogPolicy effectivePriceCatalogPolicy = new EffectivePriceCatalogPolicy(storeOnUow);
                IEnumerable<PriceCatalogDetail> pricesFromPolicy = items.Select(item => PriceCatalogHelper.GetPriceCatalogDetailFromPolicy(storeOnUow.Session as UnitOfWork,
                                                                                                                                           effectivePriceCatalogPolicy,
                                                                                                                                           item))
                                                                         .Where(priceCatalogPolicyResult => priceCatalogPolicyResult != null
                                                                                                         && priceCatalogPolicyResult.PriceCatalogDetail != null
                                                                                                         && priceCatalogPolicyResult.PriceCatalogDetail.Value > 0
                                                                               )
                                                                         .Select(priceCatalogPolicyResult => priceCatalogPolicyResult.PriceCatalogDetail);

                //Calculate Item results

                Customer DefaultCustomer = StoreControllerAppiSettings.DefaultCustomer;
                int loop = 0;
                foreach (var currentItem in pricesFromPolicy)
                {
                    loop++;
                    if (currentItem.RetailValue < 0)
                    {
                        continue;
                    }

                    ItemCashRegister cashRegister = new ItemCashRegister()
                    {
                        Item = currentItem.Item,
                        RetailPriceValue = currentItem.RetailValue,
                        CashRegisterBarcode = ItemHelper.GetBarcodeCodeForCashRegister(currentItem.Item, posDevice, StoreControllerAppiSettings.Owner),
                        //CashRegisterQTY = 0, //TODO if we want to use item stock
                        eSItemtatus = eCashRegisterItemStatus.WAITING
                    };
                    List<MapVatFactor> deviceMapVatFactors = posDevice.MapVatFactors.ToList();

                    VatFactor vatFactor = cashRegister.GetVatFactor(currentItem.Item.VatCategory.Oid, DefaultCustomer.VatLevel.Oid);
                    MapVatFactor mapVATFactor = deviceMapVatFactors.FirstOrDefault(mapVatFactor => mapVatFactor.VatFactor.Oid == vatFactor.Oid);
                    if (mapVATFactor == null)
                    {
                        continue;//TODO display error message for this item
                    }
                    int intVatDeviceLevel = 0;
                    Int32.TryParse(mapVATFactor.DeviceVatLevel, out intVatDeviceLevel);
                    cashRegister.CashRegisterVatLevel = intVatDeviceLevel;
                    cashRegister.CashRegisterPoints = cashRegister.GetPointsOfItem(currentItem.Item, StoreControllerAppiSettings.OwnerApplicationSettings);
                    //itemCashRegister.Add(cashRegister);
                    CashierRegisterItems.Add(cashRegister);
                }
                Session["CashierRegisterItems"] = null;
                Session["CashierRegisterItems"] = CashierRegisterItems.AsEnumerable();
            }
            else
            {
                Session["CashierRegisterItems"] = null;
                Session["CashierRegisterItems"] = new List<ItemCashRegister>().AsEnumerable();
            }

            if (Request.Params.AllKeys.Contains("DXCallbackArgument"))
            {
                if (Request["DXCallbackArgument"].Contains("SELECTROWS") && Request["DXCallbackArgument"].Contains("|all"))
                {
                    //Select All
                    ViewBag.ValuesToSelect = CashierRegisterItems.Select(x => x.Item.Oid).ToList();
                }
                else if (Request["DXCallbackArgument"].Contains("SELECTROWS") && Request["DXCallbackArgument"].Contains("|unall"))
                {
                    //Deselect All
                    ViewBag.ValuesToDeselect = CashierRegisterItems.Select(x => x.Item.Oid).ToList();
                }
            }
            return PartialView(/*priceCatalogDetails*/Session["CashierRegisterItems"] as List<ItemCashRegister>);
        }
        public JsonResult SendItemsToDevice(string SelectedItems, string cashierOid)
        {
            List<ItemCashRegister> Items = Session["CashierRegisterItems"] as List<ItemCashRegister>;
            try
            {
                GenerateUnitOfWork();

                List<Guid> Oids = new List<Guid>();
                string[] OidArray = SelectedItems.Split(',');
                foreach (string currentOid in OidArray)
                {
                    Guid oid = new Guid(currentOid);
                    Oids.Add(oid);
                }
               

                List<ItemCashRegister> SelectedItemsToSend = Items.Where(x => Oids.Contains(x.Oid)).ToList();
                
                Guid posOid = Guid.Empty;
                Guid.TryParse(cashierOid, out posOid);
                
                ITS.Retail.Model.POS pos = uow.GetObjectByKey<ITS.Retail.Model.POS>(posOid);
                if (pos == null)
                {
                    return Json(new { success = false, error = String.Format("{0}: {1}", Resources.InvalidValue, Resources.CashierRegister) });
                }
                POSDevice posDevice = pos.TerminalDeviceAssociations.FirstOrDefault().TerminalDevice as POSDevice;
                                                
                CashRegisterHardware cashRegisterHardware = GetCashRegisterHardware(pos);

                List<ItemCashRegister> dataToSend = new List<ItemCashRegister>();
                dataToSend = SelectedItemsToSend;
                Dictionary<int, ItemCashRegister> dictionaryItems = new Dictionary<int, ItemCashRegister>();

                int deviceIndex = 0;
                int lastIndex = 0;

                CriteriaOperator itemsAlreadyOnDeviceCriteria = CriteriaOperator.And(new NotOperator(new NullOperator("CashierDeviceIndex")),
                                                                                     new BinaryOperator("CashierDeviceIndex", String.Empty, BinaryOperatorType.NotEqual)
                                                                                    );
                XPCollection<Item> itemsAlreadyOnDevice = new XPCollection<Item>(uow, itemsAlreadyOnDeviceCriteria);
                if (itemsAlreadyOnDevice.Where(x => String.IsNullOrEmpty(x.CashierDeviceIndex) == false).Count() > 0)
                {
                    itemsAlreadyOnDevice.OrderBy(x => x.CashierDeviceIndex).ToList();
                    lastIndex = itemsAlreadyOnDevice.Max(x => Convert.ToInt32(x.CashierDeviceIndex));
                    if (lastIndex == 0 && itemsAlreadyOnDevice.Count() > 0)
                    {
                        string errorMessage = String.Format(Resources.InvalidDeviceIndicesFoundNoItemsOnDeviceButMaxIndexFoundIs, itemsAlreadyOnDevice.Count, lastIndex );
                        return Json(new { success = false, error =  errorMessage });
                    }
                }

                List<ItemCashRegister> sourceList = SelectedItemsToSend;
                int counter = 0;
                foreach (var currentSelectedRow in SelectedItemsToSend)
                {
                    int row = counter++;
                    dictionaryItems.Add(row, currentSelectedRow);
                }

                string message = string.Empty;

                dataToSend = (cashRegisterHardware as RBSElioCashRegister).PreparationItemsToAdd(dictionaryItems, dataToSend, lastIndex, deviceIndex, uow);
                foreach (var currentItem in dataToSend)
                {

                    var itemID = Items.FirstOrDefault(x => x.Item.Oid == currentItem.Item.Oid);

                    if (itemID != null)
                    {
                        itemID.Item.CashierDeviceIndex = currentItem.Item.CashierDeviceIndex.ToString();
                    }
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                Session["CashierRegisterItems"] = null;
                Session["CashierRegisterItems"] = Items.AsEnumerable();
                string exceptionMessage = ex.GetFullMessage();
                return Json(new { success = false, error = exceptionMessage });
            }
            finally
            {
                Session["CashierRegisterItems"] = null;
                Session["CashierRegisterItems"] = Items.AsEnumerable();
            }
            
        }

        private CashRegisterHardware GetCashRegisterHardware(ITS.Retail.Model.POS cashierDevice)
        {
            try
            {
                DeviceSettings settings = (cashierDevice.TerminalDeviceAssociations.FirstOrDefault().TerminalDevice as POSDevice).DeviceSettings;
                CashRegisterFactory cashRegisterFactory = new CashRegisterFactory();
                ConnectionType connectionType = cashierDevice.TerminalDeviceAssociations.FirstOrDefault().TerminalDevice.ConnectionType;
                CashRegisterHardware cashRegisterHardware = cashRegisterFactory.GetCashRegisterHardware(settings.DeviceType, settings, cashierDevice.Name, cashierDevice.ID, connectionType,settings.LineChars,settings.CommandChars);
                return cashRegisterHardware;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public JsonResult ClearAllItems(string cashierOid)
        {
            try
            {
                GenerateUnitOfWork();

                Guid posOid = Guid.Empty;
                Guid.TryParse(cashierOid, out posOid);
                ITS.Retail.Model.POS pos = uow.GetObjectByKey<ITS.Retail.Model.POS>(posOid);
                if (pos == null)
                {
                    return Json(new { success = false, error = String.Format("{0}: {1}", Resources.InvalidValue, Resources.CashierRegister) });
                }
                
                CashRegisterHardware cashRegisterHardware = GetCashRegisterHardware(pos);

                POSDevice posDevice = pos.TerminalDeviceAssociations.FirstOrDefault().TerminalDevice as POSDevice;

                CriteriaOperator criteria = CriteriaOperator.And(new NotOperator(new NullOperator("CashierDeviceIndex")),
                                                        new ContainsOperator("ItemAnalyticTrees", new BinaryOperator("Node.Oid", posDevice.ItemCategory.Oid)));

                Store storeOnUow = uow.GetObjectByKey<Store>(StoreControllerAppiSettings.CurrentStoreOid);

                Customer DefaultCustomer = StoreControllerAppiSettings.DefaultCustomer;
                XPCollection<Item> items = new XPCollection<Item>(uow, criteria);

                string message = string.Empty;
                Dictionary<Guid, ItemCashRegister> dictItemsToRemove = new Dictionary<Guid, ItemCashRegister>();

                List<ItemCashRegister> itemCashRegister = new List<ItemCashRegister>();
                foreach (Item currentItem in items)
                {
                    itemCashRegister.Add(new ItemCashRegister() { Item = currentItem });
                }

                (cashRegisterHardware as RBSElioCashRegister).PreparationDeleteAllItems(itemCashRegister, ResourcesLib.Resources.Item, items, uow);

                List<ItemCashRegister> viewItems = Session["CashierRegisterItems"] as List<ItemCashRegister>;
                foreach(var currentItem in viewItems)
                {
                    currentItem.eSItemtatus = eCashRegisterItemStatus.WAITING;
                }
                Session["CashierRegisterItems"] = viewItems;
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                string exceptionMessage = ex.GetFullMessage();
                return Json(new { error = exceptionMessage });
            }
        }
        public JsonResult ClearSelectedItems(string SelectedItems, string cashierOid)
        {
            try
            {
                GenerateUnitOfWork();

                List<Guid> Oids = new List<Guid>();
                string[] OidArray = SelectedItems.Split(',');
                foreach (string currentOid in OidArray)
                {
                    Guid oid = new Guid(currentOid);
                    Oids.Add(oid);
                }
                List<ItemCashRegister> Items = Session["CashierRegisterItems"] as List<ItemCashRegister>;

                List<ItemCashRegister> itemsToRemove = Items.Where(x => Oids.Contains(x.Oid)).ToList();

                List<Item> faildToRemoveItemList = new List<Item>();


                Guid posOid = Guid.Empty;
                Guid.TryParse(cashierOid, out posOid);
                ITS.Retail.Model.POS pos = uow.GetObjectByKey<ITS.Retail.Model.POS>(posOid);
                if (pos == null)
                {
                    return Json(new { success = false, error = String.Format("{0}: {1}", Resources.InvalidValue, Resources.CashierRegister) });
                }
                POSDevice posDevice = pos.TerminalDeviceAssociations.FirstOrDefault().TerminalDevice as POSDevice;
                
                CashRegisterHardware cashRegisterHardware = GetCashRegisterHardware(pos);

                List<ItemCashRegister> resultSource = (cashRegisterHardware as RBSElioCashRegister).PreperationDeleteSpecificItems(itemsToRemove, ResourcesLib.Resources.Item, Items, uow);

                Session["CashierRegisterItems"] = null;
                Session["CashierRegisterItems"] = resultSource.AsEnumerable();
            }
            catch (Exception ex)
            {
                string exceptionMessage = ex.GetFullMessage();
                return Json(new { error = exceptionMessage });
            }
            return Json(new { success = true });
        }
        public JsonResult AddPaymentMethods(string cashierOid)
        {
            GenerateUnitOfWork();

            Guid posOid = Guid.Empty;
            Guid.TryParse(cashierOid, out posOid);
            ITS.Retail.Model.POS pos = uow.GetObjectByKey<ITS.Retail.Model.POS>(posOid);
            if (pos == null)
            {
                return Json(new { success = false, error = String.Format("{0}: {1}", Resources.InvalidValue, Resources.CashierRegister) });
            }
            CashRegisterHardware cashRegisterHardware = GetCashRegisterHardware(pos);

            string failurePaymentTypes = "";
            string HexMessage = String.Empty;
            XPCollection<PaymentMethod> paymentMethods = new XPCollection<PaymentMethod>(uow);
            foreach (PaymentMethod currentPaymentMethod in paymentMethods)
            {
                try
                {
                    int paymentCode = 0;
                    Int32.TryParse(currentPaymentMethod.Code, out paymentCode);
                    cashRegisterHardware.ProgramPaymentType(paymentCode, currentPaymentMethod.Description, true, out HexMessage);
                }
                catch (Exception ex)
                {
                    string msg = ResourcesLib.Resources.FaildToAddPaymentMethod;
                    msg = msg.Replace("\\n", "");
                    failurePaymentTypes += string.Format(msg, currentPaymentMethod.Code, currentPaymentMethod.Description);
                    continue;
                }
            }

            if (failurePaymentTypes.Length > 0)
            {
                return Json(new { error = failurePaymentTypes });
            }
            return Json(new { success = true });
        }
        public JsonResult ZIssue(string cashierOid)
        {
            try
            {
                GenerateUnitOfWork();

                Store storeOnUow = uow.GetObjectByKey<Store>(StoreControllerAppiSettings.CurrentStoreOid);
                User currentUser = uow.GetObjectByKey<User>(CurrentUser.Oid);
                
                Guid posOid = Guid.Empty;
                Guid.TryParse(cashierOid, out posOid);
                ITS.Retail.Model.POS pos = uow.GetObjectByKey<ITS.Retail.Model.POS>(posOid);
                if (pos == null)
                {
                    return Json(new { success = false, error = String.Format("{0}: {1}", Resources.InvalidValue, Resources.CashierRegister) });
                }
                CashRegisterHardware cashRegisterHardware = GetCashRegisterHardware(pos);

                List<ItemSales> itemSales = (cashRegisterHardware as RBSElioCashRegister).GetItemDailySales();
                List<CashRegisterPaymentMethods> paymentDevice = (cashRegisterHardware as RBSElioCashRegister).LoadDailyPaymentMethods(uow);
                bool isCreated = false;

                DocumentHeader documentHeader = CreateDocumentForZIssue(itemSales, paymentDevice,pos.TerminalDeviceAssociations.FirstOrDefault().TerminalDevice as POSDevice);

                string abcPath = pos.ABCDirectory;
                int zReportNumber = 0;
                string pathToEJFiles;
                

                //Get daily totals 
                string messageDailyTotals = "";
                cashRegisterHardware.GetTotalSalesOfDay(out messageDailyTotals);
                decimal Totals = 0;
                DailyTotal dailyTotal = new DailyTotal();

                string messageDevice = "";
                cashRegisterHardware.GetDeviceInfo(out messageDevice);

                //Issue Z rerport
                string message = string.Empty;
                string status = string.Empty;
                eDeviceCheckResult result = cashRegisterHardware.IssueZReportCashierRegister(abcPath, out zReportNumber, out pathToEJFiles, out message);


                //check succcess get information totals and calculate daily totals
                if (result == eDeviceCheckResult.SUCCESS && documentHeader != null)
                {
                    documentHeader.Save();
                    XpoHelper.CommitTransaction(uow);
                    String[] splittedString = message.Split('/');

                    //int.TryParse(splittedString[2], out zReportNumber);

                    string dt = splittedString[0];

                    string dd = dt.Substring(0, dt.Length - 4);
                    string mm = dt.Substring(2, dt.Length - 4);
                    string yy = dt.Substring(4, dt.Length - 4);

                    string tm = splittedString[1];

                    string HH = tm.Substring(0, tm.Length - 4);
                    string min = tm.Substring(2, tm.Length - 4);
                    string ss = tm.Substring(4, tm.Length - 4);

                    string NewZDate = string.Format("{0}-{1}-20{2} {3}:{4}:{5}", dd, mm, yy, HH, min, ss);
                    DateTime zDate = DateTime.Parse(String.Format("{0:dd-MM-yyyy HH:mm:ss}", NewZDate));

                    if (messageDailyTotals != "" && messageDevice != "")
                    {
                        string[] devicResult = messageDevice.Split('/');
                        dailyTotal = (cashRegisterHardware as RBSElioCashRegister).CalculateDaylyTotals(messageDailyTotals);
                        Totals = dailyTotal.DailyTotals;
                        (cashRegisterHardware as RBSElioCashRegister).CalculateDailyTotalsExplantionDetails(zDate, zReportNumber, dailyTotal, devicResult[0], pos, currentUser, uow);
                    }

                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, error = message });
                }
            }
            catch(Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        private DocumentHeader CreateDocumentForZIssue(List<ItemSales> itemSales, List<CashRegisterPaymentMethods> paymentMethods, POSDevice posDevice)
        {
            try
            {
                Store storeOnUow = uow.GetObjectByKey<Store>(StoreControllerAppiSettings.CurrentStoreOid);
                User currentUser = uow.GetObjectByKey<User>(CurrentUser.Oid);

                if (itemSales.Count < 0)
                {
                    return null;
                }

                DocumentType documentType = uow.GetObjectByKey<DocumentType>(posDevice.DocumentType.Oid);
                DocumentSeries documentSeries = uow.GetObjectByKey<DocumentSeries>(posDevice.DocumentSeries.Oid);
                DocumentStatus documentStatus = uow.GetObjectByKey<DocumentStatus>(posDevice.DocumentStatus.Oid);
                DocumentHeader documentHeader = DocumentHelper.CreateDocumentHeaderForCashier(uow, itemSales, documentType, documentSeries, documentStatus, StoreControllerAppiSettings.DefaultCustomer, storeOnUow, CurrentUser, paymentMethods, posDevice);
                return documentHeader;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public override ActionResult LoadViewPopup()
        {
            base.LoadViewPopup();
            return PartialView("LoadViewPopup");
        }

        public JsonResult XIssue(string cashierOid)
        {
            try
            {
                GenerateUnitOfWork();

                Store storeOnUow = uow.GetObjectByKey<Store>(StoreControllerAppiSettings.CurrentStoreOid);
                User currentUser = uow.GetObjectByKey<User>(CurrentUser.Oid);

                Guid posOid = Guid.Empty;
                Guid.TryParse(cashierOid, out posOid);
                ITS.Retail.Model.POS pos = uow.GetObjectByKey<ITS.Retail.Model.POS>(posOid);
                if (pos == null)
                {
                    return Json(new { success = false, error = String.Format("{0}: {1}", Resources.InvalidValue, Resources.CashierRegister) });
                }
                CashRegisterHardware cashRegisterHardware = GetCashRegisterHardware(pos);
                string message = string.Empty;
                cashRegisterHardware.IssueXReport(out message);
                if (message != "")
                {
                    return Json(new { success = false, error = message });
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        public ActionResult DailyTotalGrid(string cashierOid)
        {
            try
            {
                GenerateUnitOfWork();

                Store storeOnUow = uow.GetObjectByKey<Store>(StoreControllerAppiSettings.CurrentStoreOid);
                User currentUser = uow.GetObjectByKey<User>(CurrentUser.Oid);

                Guid posOid = Guid.Empty;
                Guid.TryParse(cashierOid, out posOid);
                ITS.Retail.Model.POS pos = uow.GetObjectByKey<ITS.Retail.Model.POS>(posOid);
                if (pos == null)
                {
                    return Json(new { success = false, error = String.Format("{0}: {1}", Resources.InvalidValue, Resources.CashierRegister) });
                }
                CashRegisterHardware cashRegisterHardware = GetCashRegisterHardware(pos);
               
                string message = string.Empty;
                cashRegisterHardware.GetTotalSalesOfDay(out message);
                DailyTotal total = (cashRegisterHardware as RBSElioCashRegister).CalculateDaylyTotals(message);
                List<DailyTotal> DailyTotalListSource = new List<DailyTotal>();
                DailyTotalListSource.Add(total);
                Session["DailyTotal"] = DailyTotalListSource;

                List<VatCategory> vatCategories = GetList<VatCategory>(uow).OrderBy(x=>x.Description).ToList();
                ViewBag.VatCategories = vatCategories;
                return PartialView("DailyTotalGrid");
            }
            catch(Exception ex)
            {
                Session["Error"] = ex.GetFullMessage();
                return null;
            }
        }

        public ActionResult DailyItemTotalGrid(string cashierOid)
        {
            try
            {
                GenerateUnitOfWork();

                Store storeOnUow = uow.GetObjectByKey<Store>(StoreControllerAppiSettings.CurrentStoreOid);
                User currentUser = uow.GetObjectByKey<User>(CurrentUser.Oid);

                Guid posOid = Guid.Empty;
                Guid.TryParse(cashierOid, out posOid);
                ITS.Retail.Model.POS pos = uow.GetObjectByKey<ITS.Retail.Model.POS>(posOid);
                if (pos == null)
                {
                    return Json(new { success = false, error = String.Format("{0}: {1}", Resources.InvalidValue, Resources.CashierRegister) });
                }
                CashRegisterHardware cashRegisterHardware = GetCashRegisterHardware(pos);
                
                string message = string.Empty;
                
                List<string> mashineItemSalesResults = new List<string>();

                bool ExceptionHappent = false;
                do
                {
                    try
                    {
                        cashRegisterHardware.GetDailySalesOfItem(out message);
                        if (message.Contains("The requested fiscal record number is wrong"))
                        {
                            break;
                        }
                        mashineItemSalesResults.Add(message);
                    }
                    catch (Exception ex)
                    {
                        ExceptionHappent = true;
                        break;
                    }
                }
                while (!ExceptionHappent);
                List<ItemSales> itemSales = CalculateItemSales(mashineItemSalesResults);
                Session["DailyItemTotal"] = itemSales;
                return PartialView("DailyItemTotalGrid");
            }
            catch (Exception ex)
            {
                throw;
            }

        }


        public override ActionResult Dialog(List<string> arguments)
        {
            if (arguments != null)
            {
                if (arguments[0].ToUpper() == "DAILY")
                {
                    this.DialogOptions.AdjustSizeOnInit = true;
                    this.DialogOptions.HeaderText = Resources.DailyTotals;
                    this.DialogOptions.Height = 530;
                    this.DialogOptions.Width = 930;
                    this.DialogOptions.OKButton.Visible = false;
                    //-- The name of the partial to render in the Dialog
                    this.DialogOptions.BodyPartialView = "../CashierRegister/LoadViewPopup";
                    Session["CallBackFrom"] = "DAILY";
                }
                else if (arguments[0].ToUpper() == "DAILY_ITEMS")
                {
                    this.DialogOptions.AdjustSizeOnInit = true;
                    this.DialogOptions.HeaderText = Resources.DailyItemSales;
                    this.DialogOptions.Height = 530;
                    this.DialogOptions.Width = 930;
                    this.DialogOptions.OKButton.Visible = false;
                    //-- The name of the partial to render in the Dialog
                    this.DialogOptions.BodyPartialView = "../CashierRegister/LoadViewPopup";
                    Session["CallBackFrom"] = "DAILY_ITEMS";
                }
                    
            }
            return PartialView();
        }
        private List<ItemSales> CalculateItemSales(List<string> items)
        {
            List<ItemSales> itemSales = new List<ItemSales>();
            try
            {
                foreach (var item in items)
                {
                    if (item == "")
                    {
                        continue;
                    }
                    string[] currentItem = item.Split('/');
                    ItemSales currentItemSale = new ItemSales();

                    int vatCode = 0;
                    int deviceIndex = 0;
                    int points = 0;
                    decimal price = 0;
                    decimal Qty = 0;
                    decimal soldQty = 0;
                    decimal totalSalesAmount = 0;
                    //convertions
                    int.TryParse(currentItem[2], out vatCode);
                    int.TryParse(currentItem[3], out deviceIndex);
                    int.TryParse(currentItem[7], out points);
                    decimal.TryParse(currentItem[4].Replace(',', '.'), NumberStyles.Any, new CultureInfo("en-US"), out price);
                    decimal.TryParse(currentItem[8].Replace(',', '.'), NumberStyles.Any, new CultureInfo("en-US"), out Qty);
                    decimal.TryParse(currentItem[9].Replace(',', '.'), NumberStyles.Any, new CultureInfo("en-US"), out soldQty);
                    decimal.TryParse(currentItem[10].Replace(',', '.'), NumberStyles.Any, new CultureInfo("en-US"), out totalSalesAmount);

                    currentItemSale.deviceIndex = deviceIndex;
                    currentItemSale.Code = currentItem[0];
                    currentItemSale.Description = currentItem[1];
                    currentItemSale.VatCode = vatCode;
                    currentItemSale.price = price;
                    currentItemSale.Points = points;
                    currentItemSale.SoldQTY = soldQty;
                    currentItemSale.TotalSalesAmount = totalSalesAmount;
                    currentItemSale.Qty = Qty;
                    itemSales.Add(currentItemSale);
                }
                return itemSales;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}
