using ITS.Retail.Model;
using ITS.Retail.Platform.Kernel.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ITS.Retail.Common.ViewModel
{
    public class PriceCatalogDetailTimeValueViewModel : BasePersistableViewModel, IPriceCatalogDetailTimeValue
    {


        private decimal _TimeValue;
        private DateTime _TimeValueValidFromDate;
        private DateTime _TimeValueValidUntilDate;
        private Guid _PriceCatalogDetail;
        private bool _IsActive;


        public PriceCatalogDetailTimeValueViewModel()
        {
            this.TimeValueValidFromDate = DateTime.Now.Date.AddDays(-1);
            this.TimeValueValidUntilDate = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
        }

        public override Type PersistedType { get { return typeof(PriceCatalogDetailTimeValue); } }

        [Range(0, double.MaxValue)]
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


        public DateTime TimeValueValidFromDate
        {
            get
            {
                return _TimeValueValidFromDate;
            }
            set
            {
                SetPropertyValue("TimeValueValidFromDate", ref _TimeValueValidFromDate, value);
            }
        }


        public DateTime TimeValueValidUntilDate
        {
            get
            {
                return _TimeValueValidUntilDate;
            }
            set
            {
                SetPropertyValue("TimeValueValidUntilDate", ref _TimeValueValidUntilDate, value);
            }
        }

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

        public bool IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                SetPropertyValue("IsActive", ref _IsActive, value);
            }
        }
    }
}
