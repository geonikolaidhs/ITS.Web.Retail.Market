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
    [Updater(Order = 1030,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("PromotionDocumentApplicationRule", typeof(ResourcesLib.Resources))]

    public class PromotionDocumentApplicationRule : PromotionApplicationRule, IPromotionDocumentApplicationRule
    {
        
         public PromotionDocumentApplicationRule()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

         public PromotionDocumentApplicationRule(Session session)
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

        private decimal _Value;
        private bool _ValueIsRepeating;

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

        public bool ValueIsRepeating
        {
            get
            {
                return _ValueIsRepeating;
            }
            set
            {
                SetPropertyValue("ValueIsRepeating", ref _ValueIsRepeating, value);
            }
        }

        public override string Description
        {

            get { return string.Format("{0} >= {1}", Resources.Document, this.Value) + (this.ValueIsRepeating ? "(" + Resources.ValueIsRepeating + ")" : ""); }
        }

    }
}
