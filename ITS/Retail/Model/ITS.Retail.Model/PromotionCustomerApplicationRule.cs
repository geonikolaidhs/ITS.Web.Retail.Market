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
    [EntityDisplayName("PromotionCustomerApplicationRule", typeof(ResourcesLib.Resources))]

    public class PromotionCustomerApplicationRule : PromotionApplicationRule, IPromotionCustomerApplicationRule
    {

        public PromotionCustomerApplicationRule()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public PromotionCustomerApplicationRule(Session session)
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
                        throw new Exception("PromotionCustomerApplicationRule.GetUpdaterCriteria(); Error: Supplier is null");
                    }

                    crop = new BinaryOperator("PromotionApplicationRuleGroup.Promotion.Owner.Oid", supplier.Oid);
                    break;
                    
            }

            return crop;
        }

        private Customer _Customer;


        public Customer Customer
        {
            get
            {
                return _Customer;
            }
            set
            {
                SetPropertyValue("Customer", ref _Customer, value);
            }
        }


        public override string Description
        {
            get { return string.Format("{0} = {1}", Resources.Customer, this.Customer == null ? "" : this.Customer.CompanyName); }
        }

        public Guid CustomerOid
        {
            get
            {
                if (this.Customer == null)
                {
                    return Guid.Empty;
                }
                return this.Customer.Oid;
            }
        }

    }
}
