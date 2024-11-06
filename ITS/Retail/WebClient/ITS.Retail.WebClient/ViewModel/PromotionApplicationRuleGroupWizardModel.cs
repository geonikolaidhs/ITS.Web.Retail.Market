using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Web;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.ResourcesLib;
using ITS.Retail.Model;
using ITS.Retail.WebClient.AuxillaryClasses;
using System.Reflection;
using System.Collections;

namespace ITS.Retail.WebClient.ViewModel
{
    
    public class PromotionApplicationRuleGroupWizardModel : PromotionRuleWizardModel
    {
        [System.ComponentModel.DataAnnotations.Display(ResourceType = typeof(Resources), Name = "ApplicationRuleGroup")]
        public override Type PersistedType { get { return typeof(PromotionApplicationRuleGroup); } }

        public PromotionApplicationRuleGroupWizardModel()
        {
            ChildPromotionApplicationRuleGroups = new List<PromotionApplicationRuleGroupWizardModel>();
            PromotionApplicationRules = new List<PromotionApplicationRuleWizardModel>();
        }

        public eGroupOperatorType Operator { get; set; }
        public List<PromotionApplicationRuleGroupWizardModel> ChildPromotionApplicationRuleGroups { get; set; }
        public List<PromotionApplicationRuleWizardModel> PromotionApplicationRules { get; set; }
        public Guid? ParentPromotionApplicationRuleGroupOid { get; set; }


        public override string Description
        {
            get { return this.Operator.ToString(); }
        }

        public Guid? Promotion { get; set; }

        public override IEnumerable<PromotionRuleWizardModel> AllChilds
        {
            get
            {
                return this.ChildPromotionApplicationRuleGroups.SelectMany(x => x.AllChilds)
                    .Union(this.PromotionApplicationRules).Union(new List<PromotionRuleWizardModel>() { this });
            }
        }

        public List<PromotionRuleWizardModel> ChildNodes
        {
            get
            {
                return this.PromotionApplicationRules.Where(x => x.IsDeleted == false).Cast<PromotionRuleWizardModel>()
                    .Union(
                        this.ChildPromotionApplicationRuleGroups.Where(y => y.IsDeleted == false).Cast<PromotionRuleWizardModel>()
                    ).ToList();
            }
        }

        public Dictionary<Guid, decimal> GetItems(Dictionary<Guid, decimal> initialDictionary = null)
        {
            Dictionary<Guid, decimal> intermed = this.PromotionApplicationRules.Where(x => x is PromotionItemApplicationRuleWizardModel && x.IsDeleted == false).Cast<PromotionItemApplicationRuleWizardModel>()
                    .Where(x => x.Item.HasValue).GroupBy(x => x.Item.Value).ToDictionary(x => x.Key, x => x.Min(y => y.Quantity));
            if(initialDictionary == null)
            {
                initialDictionary = intermed;
            }
            else
            {
                var commonIntermed = intermed.Where(x => initialDictionary.ContainsKey(x.Key));
                var newIntermed = intermed.Where(x => initialDictionary.ContainsKey(x.Key)==false);
                foreach(KeyValuePair<Guid,decimal> pair in newIntermed)
                {
                    initialDictionary.Add(pair.Key, pair.Value);                    
                }
                foreach (KeyValuePair<Guid, decimal> pair in commonIntermed)
                {
                    initialDictionary[pair.Key] = Math.Min(initialDictionary[pair.Key], pair.Value);
                }
            }
            return initialDictionary;
        }


        public Dictionary<Guid, decimal> GetItemCategories(Dictionary<Guid, decimal> initialDictionary = null)
        {
            Dictionary<Guid, decimal> intermed = this.PromotionApplicationRules.Where(x => x is PromotionItemCategoryApplicationRuleWizardModel && x.IsDeleted == false).Cast<PromotionItemCategoryApplicationRuleWizardModel>()
                    .Where(x => x.ItemCategory.HasValue).GroupBy(x => x.ItemCategory.Value).ToDictionary(x => x.Key, x => x.Min(y => y.Quantity));
            if (initialDictionary == null)
            {
                initialDictionary = intermed;
            }
            else
            {
                var commonIntermed = intermed.Where(x => initialDictionary.ContainsKey(x.Key));
                var newIntermed = intermed.Where(x => initialDictionary.ContainsKey(x.Key) == false);
                foreach (KeyValuePair<Guid, decimal> pair in newIntermed)
                {
                    initialDictionary.Add(pair.Key, pair.Value);
                }
                foreach (KeyValuePair<Guid, decimal> pair in commonIntermed)
                {
                    initialDictionary[pair.Key] = Math.Min(initialDictionary[pair.Key], pair.Value);
                }
            }
            return initialDictionary;
        }

        public List<Item> GetItems(UnitOfWork uow, List<Item> initialList = null)
        {            
            if(initialList == null)
            {
                initialList = new List<Item>();
            }
            if (this.Operator == eGroupOperatorType.Or)
            {
                return initialList;
            }
            IEnumerable<BinaryOperator> guids = this.PromotionApplicationRules.Where(x => x is PromotionItemApplicationRuleWizardModel && x.IsDeleted == false)
                .Cast<PromotionItemApplicationRuleWizardModel>().Where(x => x.Item.HasValue).Select(x => new BinaryOperator("Oid", x.Item.Value));
            XPCollection<Item> thisItems = new XPCollection<Item>(uow, guids.Count()==0?new BinaryOperator("Oid", Guid.Empty):CriteriaOperator.Or(guids));
            initialList.AddRange(thisItems);
            this.ChildPromotionApplicationRuleGroups.ForEach(x => x.GetItems(uow, initialList));
            return initialList;
        }

        public List<ItemCategory> GetItemCategories(UnitOfWork uow, List<ItemCategory> initialList = null)
        {
        
            if (initialList == null)
            {
                initialList = new List<ItemCategory>();
            }
            if (this.Operator == eGroupOperatorType.Or)
            {
                return initialList;
            }
            IEnumerable<BinaryOperator> guids = this.PromotionApplicationRules.Where(x => x is PromotionItemCategoryApplicationRuleWizardModel && x.IsDeleted == false)
            .Cast<PromotionItemCategoryApplicationRuleWizardModel>().Where(x => x.ItemCategory.HasValue).Select(x => new BinaryOperator("Oid", x.ItemCategory.Value));

            XPCollection<ItemCategory> thisItems = new XPCollection<ItemCategory>(uow, guids.Count() == 0 ? new BinaryOperator("Oid", Guid.Empty) : CriteriaOperator.Or(guids));
            initialList.AddRange(thisItems);
            this.ChildPromotionApplicationRuleGroups.ForEach(x => x.GetItemCategories(uow, initialList));
            return initialList;
        }

        public override bool Validate(out string message)
        {
            message = "";
            if(this.PromotionApplicationRules.Where(x=>x.IsDeleted == false).Count()>0)
            {
                return true;
            }
            if (this.PromotionApplicationRules.Where(x => x.IsDeleted == false).Count() > 1 && this.Operator == eGroupOperatorType.Not)
            {
                message = Resources.OnlyOneRuleForNotOperator;
                return false;
            }

            if (this.ChildPromotionApplicationRuleGroups.Where(x => x.IsDeleted == false).Count() == 0)
            {
                message = Resources.ApplicationRuleGroupMissing;
                return false;
            }

            string temp;
            if(ChildPromotionApplicationRuleGroups.FirstOrDefault(x => x.IsDeleted == false && x.Validate(out temp) == true)==null)
            {
                message = Resources.ApplicationRuleGroupMissing;
                return false;
            }
            return true;
        }

        public override void UpdateModel(Session uow)
        {
            base.UpdateModel(uow);
            this.PromotionApplicationRules.ForEach(x => { x.ParentOid = this.Oid; });
            this.ChildPromotionApplicationRuleGroups.ForEach(x => { x.UpdateModel(uow); x.ParentOid = this.Oid; x.Promotion = this.Promotion; });
            

        }


        public void RemoveInvalid()
        {
            string message = "";
            this.PromotionApplicationRules.RemoveAll(x => x.Validate(out message) == false);
            this.ChildPromotionApplicationRuleGroups.ForEach(x => x.RemoveInvalid());            
        }
    }
}
