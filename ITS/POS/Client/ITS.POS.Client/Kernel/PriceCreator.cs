using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Model.Master;
using ITS.POS.Client.Exceptions;
using ITS.POS.Resources;
using ITS.Retail.Platform;
using System.Threading;
using System.Globalization;
using NLog;
using ITS.POS.Client.Helpers;
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Client.Kernel
{
    /// <summary>
    /// Handles the price creation, when the POS can create prices
    /// </summary>
    public class PriceCreator : IPriceCreator
    {
        private IItemService ItemService { get; set; }
        private IConfigurationManager ConfigurationManager { get; set; }
        private Logger LogFile { get; set; }

        public PriceCreator(IItemService itemService, IConfigurationManager configurationManager, Logger logFile)
        {
            ItemService = itemService;
            ConfigurationManager = configurationManager;
            LogFile = logFile;
        }

        public string CreateOrUpdatePrice(PriceCatalogPolicy priceCatalogPolicy, Item item, decimal priceToSet, bool vatIncluded, PriceCatalogSearchMethod searchMethod = PriceCatalogSearchMethod.PRICECATALOG_TREE)
        {
            try
            {
                PriceCatalogDetail pcdToSet = null;
                PriceCatalogPolicyDetail defaultPolicyDetail = priceCatalogPolicy.PriceCatalogPolicyDetails.FirstOrDefault(policyDetail => policyDetail.IsDefault);
                if(defaultPolicyDetail == null)
                {
                    throw new POSException(POSClientResources.UNDEFINED_POLICY);
                }
                Guid DefaultPriceCatalogOfPolicy = defaultPolicyDetail.PriceCatalog;
                try
                {
                    ItemService.GetUnitPriceFromPolicies(null, priceCatalogPolicy, item, out pcdToSet);
                }
                catch (PriceNotFoundException)
                {

                }

                if (pcdToSet != null)
                {
                    pcdToSet.IsActive = true;
                    pcdToSet.DatabaseValue = priceToSet;
                    pcdToSet.VATIncluded = vatIncluded;
                    pcdToSet.Save();
                }
                else
                {
                    bool foundButInactive;
                    KeyValuePair<Item, Barcode> itemAndBarcode = ItemService.GetItemAndBarcodeByCode(item.Code, false, out foundButInactive);
                    if (itemAndBarcode.Value == null)
                    {
                        throw new POSException(POSClientResources.ITEM_CODE_AS_BARCODE_IS_UNDEFINED);
                    }

                    pcdToSet = new PriceCatalogDetail(item.Session);
                    pcdToSet.IsActive = true;
                    pcdToSet.Item = item.Oid;
                    pcdToSet.PriceCatalog = DefaultPriceCatalogOfPolicy;
                    pcdToSet.Barcode = itemAndBarcode.Value.Oid;
                    pcdToSet.CreatedByDevice = ConfigurationManager.CurrentTerminalOid.ToString();
                    pcdToSet.DatabaseValue = priceToSet;
                    pcdToSet.VATIncluded = vatIncluded;
                    pcdToSet.UpdatedOnTicks = DateTime.Now.Ticks;
                    pcdToSet.Save();
                }

                pcdToSet.Session.CommitTransaction();
                Thread.CurrentThread.CurrentCulture = PlatformConstants.DefaultCulture;
                return pcdToSet.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS);
            }
            catch (Exception ex)
            {
                LogFile.Error(ex, "Error creating Price");
                return null;
            }
            finally
            {
                LocaleHelper.SetLocale(LocaleHelper.GetLanguageCode(ConfigurationManager.Locale), ConfigurationManager.CurrencySymbol, ConfigurationManager.CurrencyPattern);
            }
        }

        
    }
}
