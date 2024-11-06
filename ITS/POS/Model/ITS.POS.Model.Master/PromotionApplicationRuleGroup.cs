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
    public class PromotionApplicationRuleGroup : BaseObj, IPromotionApplicationRuleGroup
    {
        public PromotionApplicationRuleGroup()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public PromotionApplicationRuleGroup(Session session)
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

        private Guid _Promotion;
        public Guid Promotion
        {
            get
            {
                return _Promotion;
            }
            set
            {
                SetPropertyValue("Promotion", ref _Promotion, value);
            }
        }

        private eGroupOperatorType _Operator;
        public eGroupOperatorType Operator
        {
            get
            {
                return _Operator;
            }
            set
            {
                SetPropertyValue("Operator", ref _Operator, value);
            }
        }

        private Guid? _ParentPromotionApplicationRuleGroupOid;
        [Indexed("GCRecord", Unique = false)]
        public Guid? ParentPromotionApplicationRuleGroupOid
        {
            get
            {
                return _ParentPromotionApplicationRuleGroupOid;
            }
            set
            {
                SetPropertyValue("ParentPromotionApplicationRuleGroupOid", ref _ParentPromotionApplicationRuleGroupOid, value);
            }
        }
    }
}