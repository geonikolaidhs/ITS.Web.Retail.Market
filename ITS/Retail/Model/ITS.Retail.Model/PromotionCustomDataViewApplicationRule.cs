using System;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;
using DevExpress.Data.Filtering;
using ITS.Retail.ResourcesLib;

namespace ITS.Retail.Model
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    [Updater(Order = 1170,
     Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("PromotionCustomDataViewApplicationRule", typeof(ResourcesLib.Resources))]
    public class PromotionCustomDataViewApplicationRule : PromotionApplicationRule, IPromotionCustomDataViewApplicationRule
    {
        public PromotionCustomDataViewApplicationRule()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public PromotionCustomDataViewApplicationRule(Session session)
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
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    if (supplier == null)
                    {
                        throw new Exception("PromotionCustomDataViewApplicationRule.GetUpdaterCriteria(); Error: Supplier is null");
                    }
                    return new BinaryOperator("PromotionApplicationRuleGroup.Promotion.Owner.Oid", supplier.Oid);
                default:
                    return null;
            }
        }

        private CustomDataView _CustomDataView;

        public CustomDataView CustomDataView
        {
            get
            {
                return _CustomDataView;
            }
            set
            {
                SetPropertyValue("CustomDataView", ref _CustomDataView, value);
            }
        }

        public override string Description
        {
            get { return string.Format("{0} = {1}", Resources.DataView, this.CustomDataView == null ? string.Empty : this.CustomDataView.Description); }
        }

        public Guid CustomDataViewOid
        {
            get
            {
                return CustomDataView != null ? CustomDataView.Oid : Guid.Empty;
            }
        }
    }
}
