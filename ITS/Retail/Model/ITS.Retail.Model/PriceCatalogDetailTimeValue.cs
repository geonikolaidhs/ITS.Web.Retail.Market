//-----------------------------------------------------------------------
// <copyright file="PriceCatalogDetail.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;

namespace ITS.Retail.Model
{
    [Updater(Order = 665,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("PriceCatalogDetailTimeValue", typeof(ResourcesLib.Resources))]

    public class PriceCatalogDetailTimeValue : BaseObj, IPriceCatalogDetailTimeValue
    {
        public PriceCatalogDetailTimeValue()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public PriceCatalogDetailTimeValue(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.'
        }

        // Fields...
        private PriceCatalogDetail _PriceCatalogDetail;
        private decimal _TimeValue;
        private long _TimeValueValidUntil;
        private long _TimeValueValidFrom;
        private decimal _OldTimeValue;
        private long _TimeValueChangedOn;

        public long TimeValueValidFrom
        {
            get
            {
                return _TimeValueValidFrom;
            }
            set
            {
                long valueToSet = value - value % TimeSpan.TicksPerSecond;
                SetPropertyValue("TimeValueValidFrom", ref _TimeValueValidFrom, valueToSet);
            }
        }

        [NonPersistent]
        public DateTime TimeValueValidFromDate
        {
            get
            {
                return new DateTime(TimeValueValidFrom);
            }
            set
            {
                TimeValueValidFrom = value.Ticks;
            }
        }

        public decimal OldTimeValue
        {
            get
            {
                return _OldTimeValue;
            }
            set
            {
                SetPropertyValue("OldTimeValue", ref _OldTimeValue, value);
            }
        }


        public long TimeValueValidUntil
        {
            get
            {
                return _TimeValueValidUntil;
            }
            set
            {
                long valueToSet = value - value % TimeSpan.TicksPerSecond;
                SetPropertyValue("TimeValueValidUntil", ref _TimeValueValidUntil, valueToSet);
            }
        }

        public long TimeValueRange
        {
            get
            {
                return TimeValueValidUntil - TimeValueValidFrom;
            }
        }



        [NonPersistent]
        public DateTime TimeValueValidUntilDate
        {
            get
            {
                return new DateTime(TimeValueValidUntil);
            }
            set
            {
                TimeValueValidUntil = value.Ticks;
            }
        }

        public decimal TimeValue
        {
            get
            {
                return _TimeValue;
            }
            set
            {
                SetPropertyValue("TimeValue", ref _TimeValue, value);
            }
        }


        [Association("PriceCatalogDetail-TimeValues")]
        public PriceCatalogDetail PriceCatalogDetail
        {
            get
            {
                return _PriceCatalogDetail;
            }
            set
            {
                SetPropertyValue("PriceCatalogDetail", ref _PriceCatalogDetail, value);
            }
        }

        /// <summary>
        /// Returns true if properties TimeValue, TimeValueValidFrom and TimeValueValidUntil have been set correctly.Otherwise false.
        /// </summary>
        public bool TimeValueIsValid
        {
            get
            {
                long now = DateTime.Now.Ticks;
                if (TimeValueValidFrom <= now && now <= TimeValueValidUntil && TimeValue <= 0)
                {
                    return false;
                }

                if (TimeValueValidFrom > TimeValueValidUntil)
                {
                    return false;
                }

                if (this.IsActive == false)
                {
                    return false;
                }
                return true;
            }
        }

        [NonPersistent]
        public DateTime TimeValueChangedOnDate
        {
            get
            {
                return new DateTime(TimeValueChangedOn);
            }
        }

        public long TimeValueChangedOn
        {
            get
            {
                return _TimeValueChangedOn;
            }
            set
            {
                SetPropertyValue("TimeValueChangedOn", ref _TimeValueChangedOn, value);
            }
        }

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);
            
            if (propertyName == "TimeValue" && (decimal)oldValue != (decimal)newValue)
            {
                this.OldTimeValue = (decimal)oldValue;
                this.TimeValueChangedOn = DateTime.Now.Ticks;
            }
        }
    }
}
