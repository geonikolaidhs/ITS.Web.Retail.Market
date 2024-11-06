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
using ITS.Retail.Platform;
using ITS.Retail.Common.ViewModel;

namespace ITS.Retail.WebClient.ViewModel
{
    public class PromotionWizardModel: IPersistableViewModel
    {
        [System.ComponentModel.DataAnnotations.Display(Name = "Code", ResourceType = typeof(Resources))]
        [System.ComponentModel.DataAnnotations.Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "PLEASE_FILL_ALL_REQUIRED_FIELDS")]
        public string Code { get; set; }

        public Type PersistedType { get { return typeof(Promotion); } }

        [System.ComponentModel.DataAnnotations.Display(Name = "Description", ResourceType = typeof(Resources))]
        [System.ComponentModel.DataAnnotations.Required(ErrorMessageResourceType=typeof(Resources), ErrorMessageResourceName="PLEASE_FILL_ALL_REQUIRED_FIELDS")]
        public string Description { get; set; }

        [System.ComponentModel.DataAnnotations.Display(Name = "PrintedDescription", ResourceType = typeof(Resources))]
        [System.ComponentModel.DataAnnotations.Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "PLEASE_FILL_ALL_REQUIRED_FIELDS")]
        public string PrintedDescription { get; set; }

        [System.ComponentModel.DataAnnotations.Display(Name = "IsActive", ResourceType = typeof(Resources))]
        public bool? IsActive { get; set; }

        [System.ComponentModel.DataAnnotations.Display(Name = "StartDate", ResourceType = typeof(Resources))]
        //[System.ComponentModel.DataAnnotations.Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "PLEASE_FILL_ALL_REQUIRED_FIELDS")]
        public DateTime StartDate { get; set; }

        [System.ComponentModel.DataAnnotations.Display(Name = "EndDate", ResourceType = typeof(Resources))]
        [System.ComponentModel.DataAnnotations.Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "PLEASE_FILL_ALL_REQUIRED_FIELDS")]
        [CompareTo(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "EndDateShouldBeGreaterThanStartDate", OtherProperty = "StartDate", OperatorType = CompareOperatorType.GREATER | CompareOperatorType.EQUAL)]
        public DateTime EndDate { get; set; }

        [System.ComponentModel.DataAnnotations.Display(Name = "StartTimePerDay", ResourceType = typeof(Resources))]
        public DateTime? StartTime { get; set; }

        [System.ComponentModel.DataAnnotations.Display(Name = "EndTimePerDay", ResourceType = typeof(Resources))]
        [System.ComponentModel.DataAnnotations.Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "PLEASE_FILL_ALL_REQUIRED_FIELDS")]
        [CompareTo(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "EndTimeShouldBeGreaterThanStartTime", OtherProperty = "StartTime", OperatorType = CompareOperatorType.GREATER)]
        public DateTime? EndTime { get; set; }

        [System.ComponentModel.DataAnnotations.Display(Name = "TestMode", ResourceType = typeof(Resources))]
        public bool TestMode { get; set; }

        public DaysOfWeek ActiveDaysOfWeek { 
            get
            {              
                return activeDaysOfWeek;
            }
            set
            {
                activeDaysOfWeek = value;
                activeDaysOfWeekArray = activeDaysOfWeek.ToString();
            }
        }

        [PersistableViewModel(PersistantObjectPropertyName = "PromotionApplicationRuleGroup")]
        public PromotionApplicationRuleGroupWizardModel Rule { get; set; }

        public IEnumerable<PromotionRuleWizardModel> AllRules
        {
            get
            {
                return Rule.AllChilds;
            }
        }

        
        public List<PromotionExecutionWizardModel> PromotionExecutions { get; set; }

        public List<PromotionResultWizardModel> PromotionResults { get; set; }

        public List<PriceCatalogPromotionWizardModel> PriceCatalogPromotions { get; set; }

        public List<PriceCatalogPolicyPromotionWizardModel> PriceCatalogPolicyPromotions { get; set; }

        public Guid Oid { get; set; }

        [System.ComponentModel.DataAnnotations.Display(Name = "MaxExecutionsPerReceipt", ResourceType = typeof(Resources))]
        public int MaxExecutionsPerReceipt { get; set; }

        [System.ComponentModel.DataAnnotations.Display(Name = "PriceCatalogs", ResourceType = typeof(Resources))]
        [Binding("PriceCatalogsString_selected")]
        [PersistableViewModel(NotPersistant=true)]
        public string PriceCatalogsString
        {
            get
            {
                return this.priceCatalogsString;
            }
            set
            {
                this.priceCatalogsString = value;
                try
                {
                    List<Guid> priceCatalogs = String.IsNullOrWhiteSpace(value) ? new List<Guid>() : value.Split(',').Select(x => Guid.Parse(x.Trim())).ToList();
                    IEnumerable<PriceCatalogPromotionWizardModel> toDelete= this.PriceCatalogPromotions.Where(x => priceCatalogs.Contains(x.PriceCatalog) == false);
                    foreach(PriceCatalogPromotionWizardModel pcp in toDelete)
                    {
                        pcp.IsDeleted = true;
                    }

                    IEnumerable<PriceCatalogPromotionWizardModel> toUpdate = this.PriceCatalogPromotions.Where(x => priceCatalogs.Contains(x.PriceCatalog) == true);
                    foreach (PriceCatalogPromotionWizardModel pcp in toUpdate)
                    {
                        pcp.IsDeleted = false;
                    }

                    IEnumerable<PriceCatalogPromotionWizardModel> newPcps = priceCatalogs.Where(x => this.PriceCatalogPromotions.Select(y => y.PriceCatalog).Contains(x) == false).
                                                                             Select(x => new PriceCatalogPromotionWizardModel() { PriceCatalog = x, Promotion = this.Oid });
                    this.PriceCatalogPromotions.AddRange(newPcps);

                }
                catch
                {
                }

            }
        }

        [System.ComponentModel.DataAnnotations.Display(Name = "PriceCatalogPolicies", ResourceType = typeof(Resources))]
        [Binding("PriceCatalogPoliciesString_selected")]
        [PersistableViewModel(NotPersistant = true)]
        public string PriceCatalogPoliciesString
        {
            get
            {
                return this.priceCatalogsPoliciesString;
            }
            set
            {
                this.priceCatalogsPoliciesString = value;
                try
                {
                    List<Guid> priceCatalogPolicies = String.IsNullOrWhiteSpace(value) ? new List<Guid>() : value.Split(',').Select(x => Guid.Parse(x.Trim())).ToList();
                    IEnumerable<PriceCatalogPolicyPromotionWizardModel> toDelete = this.PriceCatalogPolicyPromotions.Where(x => priceCatalogPolicies.Contains(x.PriceCatalogPolicy) == false);
                    foreach (PriceCatalogPolicyPromotionWizardModel pcp in toDelete)
                    {
                        pcp.IsDeleted = true;
                    }

                    IEnumerable<PriceCatalogPolicyPromotionWizardModel> toUpdate = this.PriceCatalogPolicyPromotions.Where(x => priceCatalogPolicies.Contains(x.PriceCatalogPolicy) == true);
                    foreach (PriceCatalogPolicyPromotionWizardModel pcp in toUpdate)
                    {
                        pcp.IsDeleted = false;
                    }

                    IEnumerable<PriceCatalogPolicyPromotionWizardModel> newPcps = priceCatalogPolicies.Where(x => this.PriceCatalogPolicyPromotions.Select(y => y.PriceCatalogPolicy).Contains(x) == false).Select(x => new PriceCatalogPolicyPromotionWizardModel() { PriceCatalogPolicy = x, Promotion = this.Oid });
                    this.PriceCatalogPolicyPromotions.AddRange(newPcps);

                }
                catch
                {
                }

            }
        }

        [Binding("ActiveDaysOfWeekArray_selected")]
        [System.ComponentModel.DataAnnotations.Display(Name = "Days", ResourceType = typeof(Resources))]
        [PersistableViewModel(NotPersistant = true)]
        public string ActiveDaysOfWeekArray
        {
            get
            {
                return activeDaysOfWeekArray;
            }
            set
            {
                activeDaysOfWeekArray = value;
                try
                {
                    IEnumerable<DaysOfWeek> ds = value.Split(',').Select(x => x.Trim()).Select(x => (DaysOfWeek)Enum.Parse(typeof(DaysOfWeek), x));
                    activeDaysOfWeek = ds.Aggregate((f, s) => f | s);
                }
                catch
                {
                }
            }
        }


        private DaysOfWeek activeDaysOfWeek;
        private string activeDaysOfWeekArray;
        private string priceCatalogsString;
        private string priceCatalogsPoliciesString;



        public PromotionWizardModel()
        {
            Oid = Guid.NewGuid();
            ActiveDaysOfWeek = (DaysOfWeek)127;
            activeDaysOfWeekArray = ActiveDaysOfWeek.ToString().Replace(" ", "");
            StartTime = new DateTime(2000, 1, 1, 0, 0, 1);
            EndTime = new DateTime(2000, 1, 1, 23, 59, 59);
            MaxExecutionsPerReceipt = 1;

            Rule = new PromotionApplicationRuleGroupWizardModel() { Operator = eGroupOperatorType.And };
            PriceCatalogPromotions = new List<PriceCatalogPromotionWizardModel>();
            PriceCatalogPolicyPromotions = new List<PriceCatalogPolicyPromotionWizardModel>();
            this.PromotionExecutions = new List<PromotionExecutionWizardModel>();
            this.PromotionResults = new List<PromotionResultWizardModel>();
        }

        public virtual void UpdateModel(Session uow)
        {
            if(String.IsNullOrWhiteSpace(this.priceCatalogsPoliciesString))
            {
                IEnumerable<PriceCatalogPolicyPromotionWizardModel> priceCatalogPolicyPromotionsWhere = this.PriceCatalogPolicyPromotions.Where(x => x.IsDeleted == false);
                this.priceCatalogsPoliciesString = priceCatalogPolicyPromotionsWhere.Count() > 0 ?
                    priceCatalogPolicyPromotionsWhere.Select(x => x.PriceCatalogPolicy.ToString()).Aggregate((f, s) => string.Format("{0},{1}", f, s))
                    : null;
            }
            this.PriceCatalogPolicyPromotions.ForEach(x => x.UpdateModel(uow));
            this.Rule.Promotion = this.Oid;
        }

        [PersistableViewModel(NotPersistant = true)]
        public bool IsDeleted { get; set; }
       

        public bool Validate(out string message)
        {
            message = "";

            if(this.Rule.Validate(out message) == false)
            {
                return false;
            }
            if(this.PriceCatalogPolicyPromotions.Where(x=>x.IsDeleted==false).Count()==0)
            {
                message = Resources.NoPriceCatalogPoliciesDefined;
                return false;
            }
            if(this.PromotionExecutions.Where(exec => exec.IsDeleted == false).Count() == 0)
            {
                message = Resources.NoPromotionExecutionsDefined;
                return false;
            }
            //Get execution items
            Dictionary<Guid, decimal> itemsInConditions = Rule.GetItems();
            Dictionary<Guid, decimal> itemCategories = Rule.GetItemCategories();

            Dictionary<Guid, decimal> itemInExecutions = this.PromotionExecutions.Where(x => x is PromotionItemExecutionWizardModel && x.IsDeleted == false).Cast<PromotionItemExecutionWizardModel>()
                .ToDictionary(x => x.Item.Value, x => x.Quantity);

            Dictionary<Guid, decimal> itemCategoriesInExecutions = this.PromotionExecutions.Where(x => x is PromotionItemCategoryExecutionWizardModel && x.IsDeleted == false).Cast<PromotionItemCategoryExecutionWizardModel>()
                .ToDictionary(x => x.ItemCategory.Value, x => x.Quantity);
            foreach(KeyValuePair<Guid, decimal> pair in itemInExecutions)
            {
                if(itemsInConditions.ContainsKey(pair.Key)==false)
                {
                    PromotionItemExecutionWizardModel obj = this.PromotionExecutions.Where(x => x is PromotionItemExecutionWizardModel && x.IsDeleted == false).Cast<PromotionItemExecutionWizardModel>().FirstOrDefault(x => x.Item == pair.Key);
                    message += Resources.ItemNotFound;
                    if(obj != null)
                    {
                        message += ":" + obj.ItemDescription;
                    }
                    return false;
                }
                if(pair.Value > itemsInConditions[pair.Key])
                {
                    PromotionItemExecutionWizardModel obj = this.PromotionExecutions.Where(x => x is PromotionItemExecutionWizardModel && x.IsDeleted == false).Cast<PromotionItemExecutionWizardModel>().FirstOrDefault(x => x.Item == pair.Key);
                    message += Resources.InvalidItemQty;
                    if (obj != null)
                    {
                        message += ":" + obj.ItemDescription + string.Format(" {0} ({1}) - {2} ({3})", pair.Value, Resources.PromotionExecution, itemsInConditions[pair.Key], Resources.PromotionApplicationRuleGroup); ;
                    }
                    return false;
                }
            }
            foreach (KeyValuePair<Guid, decimal> pair in itemCategoriesInExecutions)
            {
                if (itemCategories.ContainsKey(pair.Key) == false)
                {
                    PromotionItemCategoryExecutionWizardModel obj = this.PromotionExecutions.Where(x => x is PromotionItemCategoryExecutionWizardModel && x.IsDeleted == false).Cast<PromotionItemCategoryExecutionWizardModel>().FirstOrDefault(x => x.ItemCategory == pair.Key);
                    message += Resources.ItemCategoryNotFound;
                    if (obj != null)
                    {
                        message += ":" + obj.ItemCategoryDescription;
                    }
                    return false;
                }
                if (pair.Value > itemCategories[pair.Key])
                {
                    PromotionItemCategoryExecutionWizardModel obj = this.PromotionExecutions.Where(x => x is PromotionItemCategoryExecutionWizardModel && x.IsDeleted == false).Cast<PromotionItemCategoryExecutionWizardModel>().FirstOrDefault(x => x.ItemCategory == pair.Key);
                    message += Resources.InvalidItemQty;
                    if (obj != null)
                    {
                        message += ":" + obj.ItemCategoryDescription + string.Format(" {0} ({1}) - {2} ({3})", pair.Value,Resources.PromotionExecution,itemCategories[pair.Key],Resources.PromotionApplicationRuleGroup);
                    }
                    return false;
                }
            }
            return true;
        }

    }
}