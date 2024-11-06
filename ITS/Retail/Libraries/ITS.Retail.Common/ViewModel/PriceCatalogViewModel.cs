using ITS.Retail.Platform.Kernel.Model;
using System;
using System.Collections.Generic;
using ITS.Retail.Model;
using DevExpress.Xpo;
using System.ComponentModel.DataAnnotations;

namespace ITS.Retail.Common.ViewModel
{
    public class PriceCatalogViewModel : BasePersistableViewModel, IPriceCatalog
    {
        public override Type PersistedType
        {
            get
            {
                return typeof(PriceCatalog);
            }
        }


        private DateTime _StartDate;
        private DateTime _EndDate;
        private bool _IsRoot;
        private Guid? _ParentCatalogOid;
        private bool _SupportLoyalty;
        private string _Code;
        private string _Description;
        private bool _IgnoreZeroPrices;

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

        public bool IsRoot
        {
            get
            {
                return _IsRoot;
            }
            set
            {
                SetPropertyValue("IsRoot", ref _IsRoot, value);
            }
        }

        [Binding("ParentCatalog_VI")]
        public Guid? ParentCatalogOid
        {
            get
            {
                return _ParentCatalogOid;
            }
            set
            {
                SetPropertyValue("ParentCatalogOid", ref _ParentCatalogOid, value);
            }
        }

        public bool SupportLoyalty
        {
            get
            {
                return _SupportLoyalty;
            }
            set
            {
                SetPropertyValue("SupportLoyalty", ref _SupportLoyalty, value);
            }
        }

        public bool IgnoreZeroPrices
        {
            get
            {
                return _IgnoreZeroPrices;
            }
            set
            {
                SetPropertyValue("IgnoreZeroPrices", ref _IgnoreZeroPrices, value);
            }
        }

        [Required]
        public string Code
        {
            get
            {
                return _Code;
            }
            set
            {
                SetPropertyValue("Code", ref _Code, value);
            }
        }

        [Required]
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                SetPropertyValue("Description", ref _Description, value);
            }
        }

        public IEnumerable<IPriceCatalogPromotion> PriceCatalogPromotions
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<IPriceCatalog> SubPriceCatalogs
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Session Session
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Save()
        {
            throw new NotImplementedException();
        }
    }
}
