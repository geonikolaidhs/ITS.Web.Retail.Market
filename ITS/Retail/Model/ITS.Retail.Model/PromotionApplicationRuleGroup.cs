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
    [Updater(Order = 1010,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("PromotionApplicationRuleGroup", typeof(ResourcesLib.Resources))]

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

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew supplier, Store store, string deviceID)
        {
            CriteriaOperator crop = null;

            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    if (supplier == null)
                    {
                        throw new Exception("PromotionApplicationRuleGroup.GetUpdaterCriteria(); Error: Supplier is null");
                    }
                    crop = new BinaryOperator("Promotion.Owner.Oid", supplier.Oid);
                    break;
            }

            return crop;
        }

        private Promotion _Promotion;
        //[Association("Promotion-PromotionApplicationRuleGroups")]
        public Promotion Promotion
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
        [Persistent("ParentPromotionApplicationRuleGroup")]
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

        [NonPersistent]
        [UpdaterIgnoreField]
        public PromotionApplicationRuleGroup ParentPromotionApplicationRuleGroup
        {
            get
            {
                return this.Session.FindObject<PromotionApplicationRuleGroup>(PersistentCriteriaEvaluationBehavior.InTransaction, new BinaryOperator("Oid", this.ParentPromotionApplicationRuleGroupOid));
            }
            set
            {
                //SetPropertyValue("ParentCatalog", ref _ParentCatalog, value);
                if (value == null)
                    this.ParentPromotionApplicationRuleGroupOid = null;
                else
                    this.ParentPromotionApplicationRuleGroupOid = value.Oid;
            }
        }

        [Aggregated]
        public XPCollection<PromotionApplicationRuleGroup> ChildPromotionApplicationRuleGroups
        {
            get
            {
                return new XPCollection<PromotionApplicationRuleGroup>(PersistentCriteriaEvaluationBehavior.InTransaction, this.Session,
                    new BinaryOperator("ParentPromotionApplicationRuleGroupOid", this.Oid));
            }
        }

        [Association("PromotionApplicationRuleGroup-PromotionApplicationRules")]
        [Aggregated]
        public XPCollection<PromotionApplicationRule> PromotionApplicationRules
        {
            get
            {
                return GetCollection<PromotionApplicationRule>("PromotionApplicationRules");
            }
        }

        public string Description
        {
            get
            {
                return GetDescription();
            }
        }

        public string HtmlDescription
        {
            get
            {
                return GetDescription(useHtmlMarkUp: true);
            }
        }

        public string GetDescription(string indent = "", bool useHtmlMarkUp = false)
        {
            string format = useHtmlMarkUp ? "\t{3}{0}" + Environment.NewLine + "{3}{1}" + Environment.NewLine + "\t{3}{2}" : "{0} {1} {2}";
            string operatorFormat = useHtmlMarkUp ? "{3}(" + Environment.NewLine + "{0}" + Environment.NewLine + "{3}{1}" + Environment.NewLine + "{2}" + Environment.NewLine + "{3})" : "({0} {1} {2})";
            string singleFormat = useHtmlMarkUp ? "{1}(" + Environment.NewLine + "{0}" + Environment.NewLine + "{1})" : "({0})";
            string singleComplexFormat = useHtmlMarkUp ? "{1}(" + Environment.NewLine + "{1}{0}" + Environment.NewLine + "{1})" : "({0})";

            string result = "";

            if (this.Operator == eGroupOperatorType.Not && this.PromotionApplicationRules.Count == 1)
            {
                result = string.Format(format, "", this.Operator, this.PromotionApplicationRules[0].Description, indent);
            }
            else
            {
                result = this.PromotionApplicationRules.Count == 0 ?
                    "" : this.PromotionApplicationRules.Select(x => string.Format("{1}{0}", x.Description, indent))
                        .Aggregate((f, s) => string.Format(format, f, this.Operator, s, indent));
            }

            string result2 = this.ChildPromotionApplicationRuleGroups.Count == 0 ?
                "" : this.ChildPromotionApplicationRuleGroups.Select(x => x.GetDescription(indent + "\t\t", useHtmlMarkUp))
                    .Aggregate((f, s) => string.Format(format, f, this.Operator, s, indent));
            string final;

            if (string.IsNullOrWhiteSpace(result))
            {
                final = string.Format(singleComplexFormat, result2, indent);
            }
            else if (string.IsNullOrWhiteSpace(result2))
            {
                final = string.Format(singleFormat, result, indent);
            }
            else
            {
                final = string.Format(operatorFormat, result, this.Operator, result2, indent);
            }
            return final;
            /*string result = indent + (PromotionApplicationRules.Count > 0 ?
                                         PromotionApplicationRules.Select(x => x.Description).Aggregate((x, y) => x + " " + this.Operator + " " + y)
                                         : "");

            foreach(PromotionApplicationRuleGroup ruleGroup in ChildPromotionApplicationRuleGroups)
            {
                int tabsCount = indent.Count(x => x == '\t');
                string tabs = "".PadLeft(tabsCount, '\t');
                string newTabs = "".PadLeft(tabsCount + 2, '\t');
                indent = tabsCount == 0 ? newTabs : indent.Replace(tabs, newTabs);

                result += "  " + this.Operator + " [" + Environment.NewLine + ruleGroup.GetDescription(indent, useHtmlMarkUp) + "]";
            }

            return result;*/
        }

        public void SetPromotion(Promotion promo)
        {
            this.Promotion = promo;
            foreach (var child in this.ChildPromotionApplicationRuleGroups)
            {
                child.SetPromotion(promo);
            }
        }

        public bool HasCustomerRule
        {
            get
            {
                if (this.Operator == eGroupOperatorType.Or)
                    return false;
                if (PromotionApplicationRules.Any(x => x is PromotionCustomerApplicationRule))
                    return true;
                if (ChildPromotionApplicationRuleGroups.Any(x => x.HasCustomerRule))
                    return true;
                return false;
            }
        }

        public bool HasCustomerGroupRule
        {
            get
            {
                if (this.Operator == eGroupOperatorType.Or)
                    return false;
                if (PromotionApplicationRules.Any(x => x is PromotionCustomerCategoryApplicationRule))
                    return true;
                if (ChildPromotionApplicationRuleGroups.Any(x => x.HasCustomerGroupRule))
                    return true;
                return false;
            }
        }
    }
}