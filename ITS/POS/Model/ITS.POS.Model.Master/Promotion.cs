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
    public class Promotion : Lookup2Fields, IPromotion
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

        private DaysOfWeek _ActiveDaysOfWeek;
        private DateTime _EndTime;
        private DateTime _StartTime;
        private DateTime _EndDate;
        private DateTime _StartDate;
        private Guid _PromotionApplicationRuleGroupOid;
        private int _MaxExecutionsPerReceipt;
        private bool _TestMode;

    }
}
