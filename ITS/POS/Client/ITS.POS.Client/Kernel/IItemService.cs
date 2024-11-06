using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Kernel
{
    public interface IItemService : IKernelModule
    {
        /// <summary>
        /// Gets an item-barcode pair by code.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="includeInactive"></param>
        /// <param name="foundButInactive"></param>
        /// <param name="plu"></param>
        /// <returns></returns>
        KeyValuePair<Item, Barcode> GetItemAndBarcodeByCode(string code, bool includeInactive, out bool foundButInactive, string plu = null);

        /// <summary>
        /// Gets the unit price of the given item for the given price catalog.
        /// </summary>
        /// <param name="pricecatalog"></param>
        /// <param name="item"></param>
        /// <param name="barcode"></param>
        /// <returns></returns>
        //decimal GetUnitPrice(PriceCatalog pricecatalog, Item item, out PriceCatalogDetail priceCatalogDetail, Barcode barcode = null, bool includeVat = true, bool includeInactivePrices = false, PriceCatalogSearchMethod priceCatalogSearchMethod = PriceCatalogSearchMethod.PRICECATALOG_TREE);

        /// <summary>
        /// Returns all the points corresponding to an Item by suming recursively the points of all the categories of the item.
        /// </summary>
        /// <param name="item">The Item</param>
        /// <param name="type">The document type</param>
        /// <param name="pc">The price catalog</param>
        /// <returns>The sum of Points</returns>
        decimal GetPointsOfItem(Item item, DocumentType type);

       

        bool CheckIfItemIsValidForDocumentType(Item item, DocumentType type);

        decimal GetUnitPriceFromPolicies(PriceCatalogPolicy customerPriceCatalogPolicy, PriceCatalogPolicy storePriceCatalogPolicy, Item item, out PriceCatalogDetail priceCatalogDetail, Barcode barcode = null, bool includeVat = true, bool includeInactivePrices = false, PriceCatalogSearchMethod priceCatalogSearchMethod = PriceCatalogSearchMethod.PRICECATALOG_TREE);
        /// <summary>
        /// Gets an item-barcode pair by description.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="compareOperator"></param>
        /// <returns></returns>
        List<Item> GetItemFromDescription(string name);
       
    }
    
}
