using DevExpress.Data.Filtering;
using DevExpress.LookAndFeel.Design;
using DevExpress.Xpo;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Forms;
using ITS.POS.Client.Helpers;
using ITS.POS.Hardware;
using ITS.POS.Hardware.Common;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
using ITS.POS.Resources;
using ITS.Retail.Platform.Common.AuxilliaryClasses;
using ITS.Retail.Platform.Common.Helpers;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ITS.POS.Client.Kernel
{
    /// <summary>
    /// Handles the scanning of a code. Contains the logic of searching and parcing custom barcodes
    /// </summary>
    public class ScannedCodeHandler : IScannedCodeHandler
    {
        private IItemService ItemService { get; set; }
        private ISessionManager SessionManager { get; set; }
        private IActionManager ActionManager { get; set; }
        private IFormManager FormManager { get; set; }
        private IAppContext AppContext { get; set; }
        private ICustomerService CustomerService { get; set; }

        private IDeviceManager DeviceManager { get; set; }


        public ScannedCodeHandler(IItemService itemService, ISessionManager sessionManager, IActionManager actionManager,
                                  IFormManager formManager, IAppContext appContext, ICustomerService customerService, IDeviceManager deviceManager)
        {
            this.ItemService = itemService;
            this.SessionManager = sessionManager;
            this.ActionManager = actionManager;
            this.FormManager = formManager;
            this.AppContext = appContext;
            this.CustomerService = customerService;
            this.DeviceManager = deviceManager;
        }

        private void HandleQRCode(IGetItemPriceForm form, OwnerApplicationSettings appSettings, bool includeInactive, string scannedCode, bool checkForWeightedItem, bool isReturn, bool fromScanner, bool readWeight)
        {
            string originalCultureName = Thread.CurrentThread.CurrentCulture.Name;
            try
            {
                Customer customer = null;
                string data = scannedCode.Remove(0, 7);
                string[] codes = data.Split(';');
                if (codes != null && codes.Length > 0)
                {
                    string customerCode = codes[0].Replace("-", "");
                    //HandleScannedCode(form, appSettings, includeInactive, customerCode, 0, false, false, true, false);
                    customer = SessionManager.GetSession<Customer>().FindObject<Customer>(CriteriaOperator.And(new BinaryOperator("Code", customerCode, BinaryOperatorType.Equal)));
                    if (customer != null && customer != AppContext.CurrentCustomer)
                    {
                        Address address = SessionManager.GetSession<Address>().GetObjectByKey<Address>(customer.DefaultAddress);
                        ActionAddCustomerInternalParams parameters = new ActionAddCustomerInternalParams(customer, customer.Code, address);
                        try
                        {
                            ActionManager.GetAction(eActions.ADD_CUSTOMER_INTERNAL).Execute(parameters);
                        }
                        catch (Exception ex)
                        {
                            ActionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(ex.Message));
                            FormManager.ShowCancelOnlyMessageBox(ex.Message);
                        }
                    }
                    for (int i = 1; i < codes.Length; i++)
                    {
                        try
                        {
                            string[] splitData = codes[i].Split('[');
                            if (splitData != null && splitData.Length == 2)
                            {
                                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                                decimal qty;
                                if (decimal.TryParse(splitData[1], out qty))
                                {
                                    Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(originalCultureName);
                                    HandleScannedCode(form, appSettings, includeInactive, splitData[0], qty, false, false, true, false);
                                }
                                else
                                {
                                    throw new POSException(Resources.POSClientResources.INVALID_QUANTITY);
                                }
                            }
                            else
                            {
                                throw new POSException(Resources.POSClientResources.INVALID_PROPERTY_DETECTED);
                            }
                        }
                        catch (Exception ex)
                        {
                            ActionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(ex.Message));
                            FormManager.ShowCancelOnlyMessageBox(ex.Message);
                            continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ActionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(ex.Message));
                FormManager.ShowCancelOnlyMessageBox(ex.Message);
                return;
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(originalCultureName);
            }
        }

        public void HandleScannedCode(IGetItemPriceForm form, OwnerApplicationSettings appSettings, bool includeInactive, string scannedCode, decimal qty, bool checkForWeightedItem, bool isReturn, bool fromScanner, bool readWeight)
        {
            bool foundButInactive;

            if (!string.IsNullOrWhiteSpace(scannedCode) && scannedCode.Length > 7 && scannedCode.Substring(0, 7) == "-9-9-9-")
            {
                HandleQRCode(form, appSettings, includeInactive, scannedCode, checkForWeightedItem, isReturn, true, readWeight);
                return;
            }
            string plu = checkForWeightedItem == true ? scannedCode : null;
            KeyValuePair<Item, Barcode> pair = ItemService.GetItemAndBarcodeByCode(scannedCode, includeInactive, out foundButInactive, plu);

            if (pair.Key != null)
            {
                if (checkForWeightedItem)
                {
                    ItemBarcode ibc = pair.Key.ItemBarcodes.FirstOrDefault(itbc => itbc.Barcode == (pair.Value != null ? pair.Value.Oid : pair.Key.DefaultBarcode));
                    if (ibc == null)
                    {
                        ibc = pair.Key.ItemBarcodes.FirstOrDefault();
                    }
                    if (ibc != null)
                    {
                        MeasurementUnit mu = SessionManager.GetObjectByKey<MeasurementUnit>(ibc.MeasurementUnit);
                        if (mu == null || mu.SupportDecimal == false)
                        {
                            throw new POSException(scannedCode + " - " + POSClientResources.ITEM_DOES_NOT_SUPPORT_DECIMAL_QUANTITY);
                        }
                    }
                    else
                    {
                        throw new POSException(scannedCode + " - " + POSClientResources.ITEM_DOES_NOT_SUPPORT_DECIMAL_QUANTITY);
                    }
                }
                this.ProcessItemBarcodePair(form, pair, qty, isReturn, fromScanner, readWeight: readWeight);
            }
            else
            {
                if (foundButInactive)
                {
                    UtilsHelper.HardwareBeep();
                    FormManager.ShowCancelOnlyMessageBox(scannedCode + " - " + POSClientResources.ITEM_IS_INACTIVE);
                    return;
                }

                List<BarcodeType> barcodeTypes = CustomBarcodeHelper.GetMatchingMasks(scannedCode, string.Empty, SessionManager.GetSession<BarcodeType>());
                BarcodeParseResult result = CustomBarcodeHelper.ParseCustomBarcode(barcodeTypes,
                    scannedCode, appSettings.PadBarcodes, appSettings.BarcodeLength, (String.IsNullOrWhiteSpace(appSettings.BarcodePaddingCharacter) ? '0' : appSettings.BarcodePaddingCharacter[0]));
                switch (result.BarcodeParsingResult)
                {
                    case BarcodeParsingResult.NONE:
                        string message = (result.DecodedCode ?? scannedCode) + " - " + (result.BarcodeParsingResult == BarcodeParsingResult.CUSTOMER ? POSClientResources.CUSTOMER_NOT_FOUND : POSClientResources.ITEM_NOT_FOUND);
                        List<Device> comScanners = DeviceManager.Devices.Where(device => device.ConType == ConnectionType.COM && device is Scanner).ToList();
                        FormManager.ShowCancelOnlyMessageBoxWithSound(message, comScanners);
                        break;
                    case BarcodeParsingResult.CUSTOMER:
                        Customer cust = CustomerService.SearchCustomer<Customer>(result.DecodedCode);
                        if (cust == null)
                        {
                            goto case BarcodeParsingResult.NONE;
                        }
                        ActionManager.GetAction(eActions.ADD_CUSTOMER_INTERNAL).Execute(new ActionAddCustomerInternalParams(cust, result.DecodedCode, null));
                        break;
                    case BarcodeParsingResult.ITEM_CODE_QUANTITY:
                        pair = ItemService.GetItemAndBarcodeByCode(result.DecodedCode, includeInactive, out foundButInactive);
                        if (pair.Key == null)
                        {
                            goto case BarcodeParsingResult.NONE;
                        }
                        if (result.Quantity == 0)
                        {
                            throw new POSException(Resources.POSClientResources.INVALID_QUANTITY);
                        }
                        this.ProcessItemBarcodePair(form, pair, result.Quantity, isReturn, fromScanner);
                        break;
                    case BarcodeParsingResult.ITEM_CODE_VALUE:
                        pair = ItemService.GetItemAndBarcodeByCode(result.DecodedCode, includeInactive, out foundButInactive);
                        if (pair.Key == null)
                        {
                            goto case BarcodeParsingResult.NONE;
                        }
                        this.ProcessItemBarcodePair(form, pair, 1, isReturn, fromScanner, -1, result.CodeValue);
                        break;
                }
            }
        }

        private void ProcessItemBarcodePair(IGetItemPriceForm form, KeyValuePair<Item, Barcode> pair, decimal qty, bool isReturn, bool fromScanner, decimal price = -1, decimal scannedPrice = -1, bool readWeight = false)
        {
            PriceCatalogDetail pcd = null;
            bool hasCustomPrice = false;

            DocumentType documentType = SessionManager.GetObjectByKey<DocumentType>(AppContext.CurrentDocument.DocumentType);
            if (!documentType.AcceptsGeneralItems && pair.Key.IsGeneralItem)
            {
                throw new POSException(pair.Key.Code + " - " + POSClientResources.DOCUMENT_TYPE_NOT_USES_GENERAL_ITEMS);
            }
            PriceCatalogPolicy headerPolicy = SessionManager.GetObjectByKey<PriceCatalogPolicy>(AppContext.CurrentDocument.PriceCatalogPolicy), storePolicy;
            Store currentStore = SessionManager.GetObjectByKey<Store>(AppContext.CurrentDocument.Store);
            if (headerPolicy.Oid == currentStore.DefaultPriceCatalogPolicy)
            {
                storePolicy = headerPolicy;
                headerPolicy = null;
            }
            else
            {
                storePolicy = SessionManager.GetObjectByKey<PriceCatalogPolicy>(currentStore.DefaultPriceCatalogPolicy);
            }
            if (storePolicy == null)
            {
                throw new POSException(pair.Key.Code + " - " + POSClientResources.PRICECATALOG_DOES_NOT_EXIST);
            }

            if (pair.Key.CustomPriceOptions == eItemCustomPriceOptions.CUSTOM_PRICE_IS_MANDATORY)
            {
                hasCustomPrice = true;
                form.Item = pair.Key;
                form.ShowDialog();
                if (form.DialogResult != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }
                price = form.Price;
            }
            else if (pair.Key.CustomPriceOptions == eItemCustomPriceOptions.CUSTOM_PRICE_IS_OPTIONAL && price <= 0)
            {
                try
                {
                    decimal pcPrice = ItemService.GetUnitPriceFromPolicies(headerPolicy, storePolicy, pair.Key, out pcd, pair.Value);
                    price = pcPrice;
                }
                catch
                {
                    //Price not found, proceed to get item price
                }
                if (pcd == null || price <= 0)
                {
                    hasCustomPrice = true;
                    form.Item = pair.Key;
                    form.ShowDialog();
                    if (form.DialogResult != System.Windows.Forms.DialogResult.OK)
                    {
                        return;
                    }
                    price = form.Price;
                }
            }
            else if (price < 0.01m)
            {
                decimal pcPrice = ItemService.GetUnitPriceFromPolicies(headerPolicy, storePolicy, pair.Key, out pcd, pair.Value);
                price = pcPrice;
                if (pcd == null)
                {
                    string message = pair.Key.Code + " - " + POSClientResources.PRICE_DOES_NOT_EXIST;
                    ActionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(message));
                    FormManager.ShowCancelOnlyMessageBox(message);
                    return;
                }
                else if (price < 0.01m)
                {
                    string message = pair.Key.Code + " - " + POSClientResources.PRICE_IS_ZERO;
                    ActionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(message));
                    FormManager.ShowCancelOnlyMessageBox(message);
                    return;
                }
            }
            else
            {
                hasCustomPrice = true;
            }
            if (price == 0)
            {
                DocumentType docType = SessionManager.GetObjectByKey<DocumentType>(AppContext.CurrentDocument.DocumentType);
                if (docType != null && docType.AllowItemZeroPrices == false)
                {
                    ActionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(pair.Key.Code + " - " + POSClientResources.DOCUMENT_TYPE_DOES_NOT_ALLOW_ZERO_PRICES));
                    return;
                }
            }
            if (scannedPrice > 0)
            {
                qty = Math.Round(scannedPrice / price, 3, MidpointRounding.AwayFromZero) * (isReturn ? (-1) : 1);
            }
            else if (readWeight)
            {
                Scale primaryScale = DeviceManager.GetPrimaryDevice<Scale>();
                if (primaryScale == null)
                {
                    FormManager.ShowCancelOnlyMessageBox(POSClientResources.NO_PRIMARY_SCALE_FOUND);
                    return;
                }
                qty = primaryScale.ReadWeight(price, 0, pair.Key.Name);
            }

            ActionManager.GetAction(eActions.ADD_ITEM_INTERNAL).Execute(new ActionAddItemInternalParams(pair.Key, pcd, pair.Value, qty, hasCustomPrice, price, "", isReturn, fromScanner));
        }

    }
}
