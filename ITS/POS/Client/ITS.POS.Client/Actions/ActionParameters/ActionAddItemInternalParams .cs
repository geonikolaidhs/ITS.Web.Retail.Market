using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Model.Master;
using ITS.POS.Model.Transactions;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionAddItemInternalParams : ActionParams
    {
        //public DocumentHeader Header { get; set; }
        public decimal Quantity { get; set; }
        public PriceCatalogDetail PriceCatalogDetail { get; set; }
        public Barcode UserBarcode { get; set; }
        public bool HasCustomPrice { get; set; }
        public decimal CustomPrice { get; set; }
        //public decimal SecondDiscount { get; set; }
        public string CustomDescription { get; set; }
        public Item Item { get; set; }
        public bool IsReturn { get; set; }
        public bool FromScanner { get; set; }

        public override eActions ActionCode
        {
            get { return eActions.ADD_ITEM_INTERNAL; }
        }

        public ActionAddItemInternalParams(Item item, PriceCatalogDetail pcd, Barcode UserBarcode, decimal quantity, bool hasCustomPrice, decimal customPrice, string customDescription, bool isReturn, bool fromScanner)
        {
            PriceCatalogDetail = pcd;
            Quantity = quantity;
            HasCustomPrice = hasCustomPrice;
            CustomPrice = customPrice;
            this.UserBarcode = UserBarcode;
            CustomDescription = customDescription;
            Item = item;
            this.IsReturn = isReturn;
            this.FromScanner = fromScanner;
        }
    }
}