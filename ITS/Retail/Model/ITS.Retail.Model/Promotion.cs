using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;
using System;
using System.Linq;

namespace ITS.Retail.Model
{
    [Updater(Order = 1000,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("Promotion", typeof(ResourcesLib.Resources))]

    public class Promotion : Lookup2Fields, IRequiredOwner, IPromotion
    {

        public Promotion()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Promotion(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
            this.MaxExecutionsPerReceipt = 1;
        }

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew supplier, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    if (supplier == null)
                    {
                        throw new Exception("Promotion.GetUpdaterCriteria(); Error: Supplier is null");
                    }
                    crop = new BinaryOperator("Owner.Oid", supplier.Oid);
                    break;
            }

            return crop;
        }

        
        [Indexed]
        public DateTime StartDate
        {
            get
            {
                return _StartDate;
            }
            set
            {
                SetPropertyValue("StartDate", ref _StartDate, value);
            }
        }

        
        [Indexed]
        public DateTime EndDate
        {
            get
            {
                return _EndDate;
            }
            set
            {
                SetPropertyValue("EndDate", ref _EndDate, value);
            }
        }


        public DateTime StartTime
        {
            get
            {
                return _StartTime;
            }
            set
            {
                SetPropertyValue("StartTime", ref _StartTime, value);
            }
        }


        public DateTime EndTime
        {
            get
            {
                return _EndTime;
            }
            set
            {
                SetPropertyValue("EndTime", ref _EndTime, value);
            }
        }


        public DaysOfWeek ActiveDaysOfWeek
        {
            get
            {
                return _ActiveDaysOfWeek;
            }
            set
            {
                SetPropertyValue("ActiveDaysOfWeek", ref _ActiveDaysOfWeek, value);
            }
        }

        public int MaxExecutionsPerReceipt
        {
            get
            {
                return _MaxExecutionsPerReceipt;
            }
            set
            {
                SetPropertyValue("MaxExecutionsPerReceipt", ref _MaxExecutionsPerReceipt, value);
            }
        }

        public bool TestMode
        {
            get
            {
                return _TestMode;
            }
            set
            {
                SetPropertyValue("TestMode", ref _TestMode, value);
            }
        }

        private string _PrintedDescription;
        [Size(SizeAttribute.Unlimited)]
        public string PrintedDescription
        {
            get
            {
                return _PrintedDescription;
            }
            set
            {
                SetPropertyValue("PrintedDescription", ref _PrintedDescription, value);
            }
        }


        private Guid _PromotionApplicationRuleGroupOid;
        [Persistent("PromotionApplicationRuleGroup")]
        public Guid PromotionApplicationRuleGroupOid
        {
            get
            {
                return _PromotionApplicationRuleGroupOid;
            }
            set
            {
                SetPropertyValue("PromotionApplicationRuleGroupOid", ref _PromotionApplicationRuleGroupOid, value);
            }
        }

        [NonPersistent]
        [UpdaterIgnoreField]
        [Aggregated]
        public PromotionApplicationRuleGroup PromotionApplicationRuleGroup
        {
            get
            {
                return this.Session.FindObject<PromotionApplicationRuleGroup>(PersistentCriteriaEvaluationBehavior.InTransaction, new BinaryOperator("Oid", this.PromotionApplicationRuleGroupOid));
            }
            set
            {
                if (value == null)
                {
                    this.PromotionApplicationRuleGroupOid = Guid.Empty;
                }
                else
                {
                    this.PromotionApplicationRuleGroupOid = value.Oid;
                }
            }
        }


        [Association("Promotion-PromotionExecutions")]
        [Aggregated]
        public XPCollection<PromotionExecution> PromotionExecutions
        {
            get
            {
                return GetCollection<PromotionExecution>("PromotionExecutions");
            }
        }


        [Association("Promotion-PromotionResults")]
        [Aggregated]
        public XPCollection<PromotionResult> PromotionResults
        {
            get
            {
                return GetCollection<PromotionResult>("PromotionResults");
            }
        }

        [Association("Promotion-PriceCatalogPromotions")]
        [Aggregated]
        public XPCollection<PriceCatalogPromotion> PriceCatalogPromotions
        {
            get
            {
                return GetCollection<PriceCatalogPromotion>("PriceCatalogPromotions");
            }
        }

        [Association("Promotion-PriceCatalogPolicyPromotions")]
        [Aggregated]
        public XPCollection<PriceCatalogPolicyPromotion> PriceCatalogPolicyPromotions
        {
            get
            {
                return GetCollection<PriceCatalogPolicyPromotion>("PriceCatalogPolicyPromotions");
            }
        }

        private DaysOfWeek _ActiveDaysOfWeek;
        private DateTime _EndTime;
        private DateTime _StartTime;
        private DateTime _EndDate;
        private DateTime _StartDate;
        //private PromotionApplicationRuleGroup _PromotionApplicationRuleGroup;
        private int _MaxExecutionsPerReceipt;
        private bool _TestMode;

        public string PromotionResultsDescription
        {
            get
            {
                string description = "";

                if (PromotionExecutions.Count > 0)
                {
                    description += PromotionExecutions.Select(x => x.Description+ Environment.NewLine).Aggregate((x, y) => x + y);
                }

                //if (PromotionExecutions.Count == 1)
                //{
                //    description += Environment.NewLine;
                //}

                if(PromotionResults.Count > 0)
                {
                    description += PromotionResults.Select(x => x.Description+ Environment.NewLine).Aggregate((x, y) => x  + y);
                }


                return description.TrimEnd(Environment.NewLine.ToCharArray());
            }
        }

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);           
        }
        protected override void OnDeleting()
        {
            base.OnDeleting();
            PromotionApplicationRuleGroup.Delete();
        }

        public bool HasCustomerRule
        {
            get
            {
                return PromotionApplicationRuleGroup.HasCustomerRule;
            }
        }

        public bool HasCustomerGroupRule
        {
            get
            {
                return PromotionApplicationRuleGroup.HasCustomerGroupRule;
            }
        }
    }
}
