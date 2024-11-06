using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;
using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    [Updater(Order = 1060,
       Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("PromotionItemExecution", typeof(ResourcesLib.Resources))]
    public class PromotionItemExecution : PromotionExecution, IPromotionItemExecution
    {
        public PromotionItemExecution()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public PromotionItemExecution(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew supplier, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    if (supplier == null)
                    {
                        throw new Exception("PromotionItemExecution.GetUpdaterCriteria(); Error: Supplier is null");
                    }
                    crop = new BinaryOperator("Promotion.Owner.Oid", supplier.Oid);
                    break;
            }

            return crop;
        }

        private Item _Item;
        private decimal _Quantity;
        private bool _OncePerItem;
        private eItemExecutionMode _ExecutionMode;
        private decimal _FinalUnitPrice;

        public Item Item
        {
            get
            {
                return _Item;
            }
            set
            {
                SetPropertyValue("Item", ref _Item, value);
            }
        }

        public decimal Quantity
        {
            get
            {
                return _Quantity;
            }
            set
            {
                SetPropertyValue("Quantity", ref _Quantity, value);
            }
        }

        public override string Description
        {
            get
            {
                if ((Value > 0 || Percentage > 0) && DiscountType != null && Item != null)
                {
                    return (DiscountType.eDiscountType == eDiscountType.PERCENTAGE)
                        ? String.Format("{0}({1}) x {2} - {3}({4}) {5:P}", Resources.Item, Item.Name, this.Quantity, Resources.Discount, DiscountType.Description, Percentage)
                        : String.Format("{0}({1}) x {2} - {3}({4}) {5}", Resources.Item, Item.Name, this.Quantity, Resources.Discount, DiscountType.Description, Value);
                }
                else if (Item != null /*&& FinalUnitPrice != null*/ && this.ExecutionMode == eItemExecutionMode.SET_PRICE)
                {
                    return String.Format("{0}({1}){2} => {3}", Resources.ItemCategory, Item.Name, Resources.UnitPrice, this.FinalUnitPrice);
                }

                return base.Description;
            }
        }

        public bool OncePerItem
        {
            get
            {
                return _OncePerItem;
            }
            set
            {
                SetPropertyValue("OncePerItem", ref _OncePerItem, value);
            }
        }

        public eItemExecutionMode ExecutionMode
        {
            get
            {
                return _ExecutionMode;
            }
            set
            {
                SetPropertyValue("ExecutionMode", ref _ExecutionMode, value);
            }

        }

        public decimal FinalUnitPrice
        {
            get
            {
                return _FinalUnitPrice;
            }
            set
            {
                SetPropertyValue("FinalUnitPrice", ref _FinalUnitPrice, value);
            }
        }


        public Guid ItemOid
        {
            get
            {
                if (this.Item == null)
                {
                    return Guid.Empty;
                }
                return this.Item.Oid;
            }
        }

        
    }
}
