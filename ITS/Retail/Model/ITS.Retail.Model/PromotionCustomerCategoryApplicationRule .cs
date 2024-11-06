using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;
using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.Model
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    [Updater(Order = 1025,
     Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("PromotionCustomerCategoryApplicationRule", typeof(ResourcesLib.Resources))]

    public class PromotionCustomerCategoryApplicationRule : PromotionApplicationRule, IPromotionCustomerCategoryApplicationRule
    {

        public PromotionCustomerCategoryApplicationRule ()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public PromotionCustomerCategoryApplicationRule(Session session)
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
                        throw new Exception("PromotionCustomerCategoryApplicationRule .GetUpdaterCriteria(); Error: Supplier is null");
                    }

                    crop = new BinaryOperator("PromotionApplicationRuleGroup.Promotion.Owner.Oid", supplier.Oid);
                    break;
                    
            }

            return crop;
        }

        private CustomerCategory _CustomerCategory;
        [Association("CustomerCategory-PromotionCustomerCategoryApplicationRules")]
        public CustomerCategory CustomerCategory
        {
            get
            {
                return _CustomerCategory;
            }
            set
            {
                SetPropertyValue("CustomerCategory", ref _CustomerCategory, value);
            }
        }


        public override string Description
        {
            get
            {
                return string.Format("{0} = {1}", Resources.CustomerCategory, this.CustomerCategory == null ? "" : this.CustomerCategory.Description);
            }
        }

        public Guid CustomerCategoryOid
        {
            get
            {
                if (this.CustomerCategory == null)
                {
                    return Guid.Empty;
                }
                return this.CustomerCategory.Oid;
            }
        }
    }
}
