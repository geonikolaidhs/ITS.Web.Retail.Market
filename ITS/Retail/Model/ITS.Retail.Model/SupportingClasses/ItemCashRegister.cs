using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model.SupportingClasses
{
    [NonPersistent]
    public class ItemCashRegister
    {
        public Guid Oid
        {
            get
            {
                return Item.Oid;
            }
        }

        private VatFactor _VatFactor;
        public int DeviceIndex { get; set; }

        public VatFactor VatFactor { get; set; }

        public Item Item { get; set; }

        public string CashRegisterBarcode { get; set; }

        public string CashierRegisterVat { get; set; }

        public decimal RetailPriceValue { get; set; }

        public int CashRegisterVatLevel { get; set; }

        public decimal CashRegisterQTY { get; set; }
        public decimal CashRegisterPoints
        {
            get;
            set;
        }
        public bool IsUpdated { get; set; }

        public Platform.Enumerations.eCashRegisterItemStatus eSItemtatus { get; set; }
        public VatFactor CashierVatFactor
        {
            get { return _VatFactor; }
        }
        public VatFactor GetVatFactor(Guid vatCategory, Guid vatLevel)
        {
            try
            {
                VatFactor vatFactor = this.Item.Session.FindObject<VatFactor>(CriteriaOperator.And(new BinaryOperator("VatCategory", vatCategory),
                                                                                                   new BinaryOperator("VatLevel", vatLevel)));
                _VatFactor = vatFactor;
                if (_VatFactor != null)
                {
                    return _VatFactor;
                }
                else
                {
                    _VatFactor = new VatFactor();
                    return _VatFactor;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public decimal GetPointsOfItem(Item item, OwnerApplicationSettings settings)
        {
            if (settings.SupportLoyalty)
            {
                decimal points = item.Points;
                foreach (ItemAnalyticTree iat in item.ItemAnalyticTrees)
                {
                    if (iat.Node != null)
                    {
                        ItemCategory CurrentCategory = item.Session.GetObjectByKey<ItemCategory>(iat.Node.Oid);
                        while (CurrentCategory != null)
                        {
                            points += CurrentCategory.Points;
                            CurrentCategory = item.Session.GetObjectByKey<ItemCategory>(CurrentCategory.ParentOid);
                        }
                    }
                }
                return points;
            }
            return 0;
        }
    }
}
