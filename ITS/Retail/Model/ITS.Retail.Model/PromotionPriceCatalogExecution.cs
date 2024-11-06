using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;
using System;

namespace ITS.Retail.Model
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    [Updater(Order = 1041, Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    public class PromotionPriceCatalogExecution : PromotionExecution, IPromotionPriceCatalogExecution
    {
        public PromotionPriceCatalogExecution()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public PromotionPriceCatalogExecution(Session session)
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

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew owner, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    if (owner == null)
                    {
                        throw new Exception("PromotionPriceCatalogExecution.GetUpdaterCriteria(); Error: Owner is null");
                    }
                    crop = new BinaryOperator("Promotion.Owner.Oid", owner.Oid);
                    break;
            }

            return crop;
        }

        private string _PriceCatalogs;
        private bool _OncePerItem;
        private eItemExecutionMode _ExecutionMode;

        [Size(SizeAttribute.Unlimited)]
        public string PriceCatalogs
        {
            get
            {
                return _PriceCatalogs;
            }
            set
            {
                SetPropertyValue("PriceCatalogs", ref _PriceCatalogs, value);
            }
        }

        public override string Description
        {
            get
            {
                if ( this.DiscountType == null )
                {
                    return String.Empty;
                }
                string discount = this.DiscountType.eDiscountType == eDiscountType.VALUE ? this.Value.ToString() : this.Percentage + "%";
                return String.Format("{0}: {1} {2}", ResourcesLib.Resources.Discount,this.DiscountType.Description, discount);
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
    }
}
