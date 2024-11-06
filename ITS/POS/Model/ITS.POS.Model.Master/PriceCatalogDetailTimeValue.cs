using ITS.POS.Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;

namespace ITS.POS.Model.Master
{

    public class PriceCatalogDetailTimeValue : BaseObj
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
        private Guid _PriceCatalogDetail;
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
                SetPropertyValue("TimeValueValidFrom", ref _TimeValueValidFrom, value);
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
                SetPropertyValue("TimeValueValidUntil", ref _TimeValueValidUntil, value);
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

        [Indexed("GCRecord", Unique = false)]
        public Guid PriceCatalogDetail
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
