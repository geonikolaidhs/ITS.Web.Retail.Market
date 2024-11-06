using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Model;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;

namespace ITS.Retail.Common.Helpers
{
    public static class BOItemHelper
    {
        public static ItemBarcode GetTaxCodeBarcode(UnitOfWork uow, Item item, CompanyNew owner)
        {

            if (item == null || owner == null)
            {
                throw new Exception(string.Format(ResourcesLib.Resources.ItemAndOwnerMustBeDefined, item == null ? "" : item.Name, owner == null ? "" : owner.CompanyName));
            }

            string barcodeCode = item.Code;
            if (owner.OwnerApplicationSettings != null && owner.OwnerApplicationSettings.PadBarcodes && string.IsNullOrEmpty(owner.OwnerApplicationSettings.BarcodePaddingCharacter)==false)
            {
                barcodeCode = item.Code.PadLeft(owner.OwnerApplicationSettings.BarcodeLength, owner.OwnerApplicationSettings.BarcodePaddingCharacter[0]);
            }
            Barcode taxCodeBarcode = uow.FindObject<Barcode>(new BinaryOperator("Code",barcodeCode));
            if(taxCodeBarcode==null)
            {
                return item.DefaultBarcode.ItemBarcode(owner);
            }

            CriteriaOperator crop = CriteriaOperator.And(new BinaryOperator("Item.Oid", item.Oid), new BinaryOperator("Barcode.Oid",taxCodeBarcode.Oid) , new BinaryOperator("Owner.Oid", owner.Oid));
            return uow.FindObject<ItemBarcode>(crop);
        }
    }
}
