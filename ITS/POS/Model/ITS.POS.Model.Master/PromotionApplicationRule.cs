using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Model.Master
{
    [SyncInfoIgnore]
    public class PromotionApplicationRule : BaseObj, IPromotionApplicationRule
    {

        public PromotionApplicationRule()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public PromotionApplicationRule(Session session)
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

        private Guid _PromotionApplicationRuleGroup;
        public Guid PromotionApplicationRuleGroup
        {
            get
            {
                return _PromotionApplicationRuleGroup;
            }
            set
            {
                SetPropertyValue("PromotionApplicationRuleGroup", ref _PromotionApplicationRuleGroup, value);
            }
        }

        public Guid PromotionApplicationRuleGroupOid
        {
            get { return PromotionApplicationRuleGroup; }
        }
    }
}
