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
    [Updater(Order = 1020,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("PromotionItemCategoryApplicationRule", typeof(ResourcesLib.Resources))]
    public class PromotionItemCategoryApplicationRule : PromotionApplicationRule, IPromotionItemCategoryApplicationRule
    {
        
         public PromotionItemCategoryApplicationRule()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

         public PromotionItemCategoryApplicationRule(Session session)
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
                        
                        throw new Exception("PromotionApplicationRule.GetUpdaterCriteria(); Error: Supplier is null");
					}
                    crop = new BinaryOperator("PromotionApplicationRuleGroup.Promotion.Owner.Oid", supplier.Oid);
					break;
			}

			return crop;
		}

        private ItemCategory _ItemCategory;
        private decimal _Quantity;
        private decimal _Value;


        public ItemCategory ItemCategory
        {
            get
            {
                return _ItemCategory;
            }
            set
            {
                SetPropertyValue("ItemCategory", ref _ItemCategory, value);
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

        public decimal Value
        {
            get
            {
                return _Value;
            }
            set
            {
                SetPropertyValue("Value", ref _Value, value);
            }
        }

        public override string Description
        {
            get
            {
                decimal valueToShow = (this.Quantity > 0 ? this.Quantity : this.Value);
                string stringToShow = (this.Quantity > 0 ? Resources.Quantity : Resources.Value);
                return string.Format("{0} >= {1} [{2}]", this.ItemCategory == null ? "" :this.ItemCategory.Description, valueToShow, stringToShow);
            }
        }

        public Guid ItemCategoryOid
        {
            get
            {
                if (this.ItemCategory == null)
                {
                    return Guid.Empty;
                }
                return this.ItemCategory.Oid;
            }
        }
    }

}
