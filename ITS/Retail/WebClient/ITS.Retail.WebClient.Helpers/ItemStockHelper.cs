using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Linq;

namespace ITS.Retail.WebClient.Helpers
{
    public static class ItemStockHelper
    {
        /// <summary>
        /// Recalculates ItemStock
        /// </summary>
        /// <param name="store">The store for which to recalculate Item Stock</param>
        /// <param name="fromDate">The from which and onwards Item Stock must be recalculated</param>
        /// <param name="itemBarcode">If Guid.Empty recalculation takes place for all items. If not recalculation takes place ONLY for the selected ItemBarcode.Item</param>
        /// <returns>String.Empty if everything went smooth otherwise a relative error message</returns>
        public static string RecalculateItemStock(Guid store, DateTime fromDate, Guid itemBarcode)
        {
            try
            {
                if (store == Guid.Empty)
                {
                    return ResourcesLib.Resources.PleaseSelectAStore;
                }

                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    Store theStore = uow.GetObjectByKey<Store>(store);
                    if (theStore == null)
                    {
                        return ResourcesLib.Resources.PleaseSelectAStore;
                    }


                    if (itemBarcode == Guid.Empty && fromDate == DateTime.MinValue)
                    {
                        return ResourcesLib.Resources.FillAllMissingFields;
                    }

                    if (itemBarcode != Guid.Empty && fromDate == DateTime.MinValue)
                    {
                        ItemBarcode theItemBarcode = uow.GetObjectByKey<ItemBarcode>(itemBarcode);
                        if (theItemBarcode == null)
                        {
                            return ResourcesLib.Resources.ItemNotFound;
                        }
                        RecalculateItemStockForItemForStore(theItemBarcode.Item, theStore);
                        return String.Empty;
                    }
                    else if (itemBarcode == Guid.Empty && fromDate != DateTime.MinValue)
                    {
                        CriteriaOperator documentHeaderCriteria = CriteriaOperator.And(new BinaryOperator("Store", store),
                                                                                       new BinaryOperator("DocumentType.ItemStockAffectionOptions", ItemStockAffectionOptions.INITIALISES),
                                                                                       new BinaryOperator("FinalizedDate", fromDate, BinaryOperatorType.GreaterOrEqual)
                                                                                      );
                        XPCollection<DocumentHeader> documentHeaders = new XPCollection<DocumentHeader>(uow, documentHeaderCriteria);
                        documentHeaders.SelectMany(document => document.DocumentDetails)
                                       .Select(documentDetail => documentDetail.Item)
                                       .Distinct()
                                       .ToList()
                                       .ForEach(item => 
                                       {
                                           RecalculateItemStockForItemForStore(item, theStore);
                                       });
                        return String.Empty;
                    }
                    else if (itemBarcode != Guid.Empty && fromDate != DateTime.MinValue)
                    {
                        ItemBarcode theItemBarcode = uow.GetObjectByKey<ItemBarcode>(itemBarcode);
                        if (theItemBarcode == null)
                        {
                            return ResourcesLib.Resources.ItemNotFound;
                        }
                        ItemStock itemStock = theItemBarcode.Item.GetItemStockForStore(theStore);
                        if (itemStock != null
                            && itemStock.LastDocumentHeaderInventory != null
                            && itemStock.LastDocumentHeaderInventory.FinalizedDate.Ticks <= fromDate.Ticks
                           )
                        {
                            return String.Empty;
                        }
                        RecalculateItemStockForItemForStore(theItemBarcode.Item, theStore);
                    }
                }
            }
            catch(Exception exception)
            {
                return exception.GetFullMessage();
            }
            return String.Empty;
        }

        private static void RecalculateItemStockForItemForStore(Item item, Store store)
        {
            ItemStock itemStock = item.GetItemStockForStore(store);
            if (itemStock == null || itemStock.LastDocumentHeaderInventory == null)
            {
                if (itemStock == null)
                {
                    itemStock = new ItemStock(item.Session)
                    {
                        Item = item,
                        Store = store
                    };
                }
                itemStock.Stock = item.RecalculateItemStockForItemWithoutInventory(store);
            }
            else
            {
                itemStock.Stock = item.RecalculateItemStockAfterInventory(itemStock.LastDocumentHeaderInventory);
            }
            itemStock.Save();
            item.Session.CommitTransaction();
        }
    }
}
