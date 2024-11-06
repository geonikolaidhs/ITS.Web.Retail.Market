using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Model.Master
{
    [MapInheritance(MapInheritanceType.ParentTable)]
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

        private decimal _Value;
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

        private bool _ValueIsRepeating;
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

    }
}
