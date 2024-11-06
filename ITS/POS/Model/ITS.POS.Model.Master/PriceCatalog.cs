using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Kernel.Model;

namespace ITS.POS.Model.Master
{
    public class PriceCatalog : Lookup2Fields, IPriceCatalog
    {
        public PriceCatalog()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public PriceCatalog(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
            _IsRoot = true;
            IsActive = true;
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

        private DateTime _StartDate;
        [Indexed(Unique = false)]
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

        private DateTime _EndDate;
        [Indexed(Unique=false)]
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

        private Guid? _ParentCatalogOid;
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


        private bool _IsRoot;
        private bool _SupportLoyalty;
        private bool _IgnoreZeroPrices;

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

        public XPCollection<PriceCatalog> PriceCatalogs
        {
            get
            {
                return new XPCollection<PriceCatalog>(this.Session, new BinaryOperator("ParentCatalogOid", Oid));
            }
        }

        public XPCollection<PriceCatalogDetail> PriceCatalogDetails
        {
            get
            {
                return new XPCollection<PriceCatalogDetail>(this.Session, new BinaryOperator("PriceCatalog", Oid));
            }
        }




        protected override void OnSaving()
        {
            if (StartDate == null)
            {
                // Intiallize 
                StartDate = Convert.ToDateTime(0);
            }
            if (EndDate == null)
            {
                EndDate = Convert.ToDateTime(20000);
            }
            base.OnSaving();
        }
        /// <summary>
        /// Έλεγχος αν η ημερομηνία βρίσκεται στο ημερομηνιακό διαστημα FromDate, ToDate 
        /// </summary>
        /// <param name="CheckedDate"></param>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <returns></returns>
        public bool DateInsideRange(DateTime CheckedDate, DateTime FromDate, DateTime ToDate)
        {
            return CheckedDate >= FromDate && CheckedDate <= ToDate ? true : false;
        }
        /// <summary>
        /// Έλεγχος αν ημερομηνία το τιμοκαταλόγου βρίσκεται μέσα στο range ημερομηνίων του πατέρα του ( ParanetCatalog )
        /// </summary>
        /// <returns></returns>
        public bool CheckParentDateRange()
        {
            if (ParentCatalogOid != null )
                return true;
            PriceCatalog parent = this.Session.GetObjectByKey<PriceCatalog>(ParentCatalogOid);
            if (DateInsideRange(StartDate, parent.StartDate, parent.EndDate) && StartDate <= EndDate)
                return true;
            return false;
        }

        /// <summary>
        /// Έλέχει αν τιμοκατάλογος έχει επικαλυπτόμενο διαστημά ημερομηνίας ισχύος με κάποιο απο τους τιμοκατάλογους που βρίσκονται 
        /// στο ίδιο επίπεδο του ParentCatalog
        /// </summary>
        /// <returns></returns>
        public bool HasChildOverlapingDateRange()
        {
            if (ParentCatalogOid == null)
            {
                return false;
            }
            PriceCatalog parent = this.Session.GetObjectByKey<PriceCatalog>(ParentCatalogOid);
            foreach (PriceCatalog item in parent.PriceCatalogs)
            {
                if (item.Oid == Oid)
                {
                    continue;
                }
                if (DateInsideRange(StartDate, item.StartDate, item.EndDate)
                    && DateInsideRange(EndDate, item.StartDate, item.EndDate)
                    && DateInsideRange(item.StartDate, StartDate, EndDate)
                    && DateInsideRange(item.EndDate, StartDate, EndDate)
                )
                {
                    return true;
                }
            }
            return false;
        }

        public IEnumerable<IPriceCatalogPromotion> PriceCatalogPromotions
        {
            get
            {
                return new XPCollection<PriceCatalogPromotion>(this.Session, new BinaryOperator("PriceCatalog", Oid));
            }
        }

        public IEnumerable<IPriceCatalog> SubPriceCatalogs
        {
            get
            {
                return this.PriceCatalogs;
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
    }
}
