using DevExpress.Xpo;
using ITS.Retail.Platform.Kernel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Model.Master
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class PromotionPriceCatalogApplicationRule : PromotionApplicationRule, IPromotionPriceCatalogApplicationRule
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

        private string _PriceCatalogs;
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
    }
}
