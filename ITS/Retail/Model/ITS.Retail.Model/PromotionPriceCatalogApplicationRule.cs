using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    [Updater(Order = 1026,
     Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    public class PromotionPriceCatalogApplicationRule: PromotionApplicationRule, IPromotionPriceCatalogApplicationRule
    {
        

        public PromotionPriceCatalogApplicationRule()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public PromotionPriceCatalogApplicationRule(Session session)
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
                        throw new Exception("PromotionPriceCatalogApplicationRule.GetUpdaterCriteria(); Error: Owner is null");
                    }

                    crop = new BinaryOperator("PromotionApplicationRuleGroup.Promotion.Owner.Oid", owner.Oid);
                    break;

            }

            return crop;
        }

        private string _PriceCatalogs;
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
                string description = ResourcesLib.Resources.PriceCatalogIn;
                if (String.IsNullOrWhiteSpace(this.PriceCatalogs) == false)
                {
                    try
                    {
                        List<Guid> guids = this.PriceCatalogs.Split(',').Select(oid => Guid.Parse(oid)).ToList();
                        XPCollection<PriceCatalog> priceCatalogs = new XPCollection<PriceCatalog>(this.Session, new InOperator("Oid", guids));
                        if (priceCatalogs != null && priceCatalogs.Count > 0)
                        {
                            description += priceCatalogs.Select(priceCatalog => priceCatalog.Description).Aggregate((first, second) => first + ',' + second);
                        }
                    }
                    catch(Exception exception)
                    {
                        string errorMessage = exception.GetFullMessage();
                    }
                }
                return description;
            }
        }
    }
}
