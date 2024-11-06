using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.POS.Model.Master
{
    [MapInheritance(MapInheritanceType.ParentTable)]
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

        private Guid _CustomerCategory;
        public Guid CustomerCategory
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



        public Guid CustomerCategoryOid
        {
            get { return CustomerCategory; }
        }
    }
}
