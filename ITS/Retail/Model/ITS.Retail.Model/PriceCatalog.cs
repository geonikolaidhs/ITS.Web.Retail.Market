 //-----------------------------------------------------------------------
// <copyright file="PriceCatalog.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using System.Linq;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;
namespace ITS.Retail.Model
{
    [Updater(Order = 51,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("PriceCatalog", typeof(ResourcesLib.Resources))]

    public class PriceCatalog : LookupField, IRequiredOwner, IPriceCatalog
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

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew supplier, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    if (supplier == null)
                    {
                        throw new Exception("PriceCatalog.GetUpdaterCriteria(); Error: Supplier is null");
                    }
                    crop = new BinaryOperator("Owner.Oid", supplier.Oid);
                    break;
            }
            return crop;
        }

        private string _Code;
        private DateTime _StartDate;
        private CompanyNew _Owner;
        private bool _SupportLoyalty;
        private Store _IsEditableAtStore;
        private DateTime _EndDate;
        private Guid? _ParentCatalogOid;
        private bool _IsRoot;
        private int _Level;
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

        
        [Persistent("ParentCatalog")]
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

        [NonPersistent]
        [UpdaterIgnoreField]
        public PriceCatalog ParentCatalog
        {
            get
            {
                return this.Session.FindObject<PriceCatalog>(PersistentCriteriaEvaluationBehavior.InTransaction, new BinaryOperator("Oid", this.ParentCatalogOid));
            }
            set
            {
                if (value == null)
                {
                    this.ParentCatalogOid = null;
                }
                else
                {
                    this.ParentCatalogOid = value.Oid;
                }
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

        public int Level
        {
            get
            {
                return _Level;
            }
            set
            {
                SetPropertyValue("Level", ref _Level, value);
            }
        }

        public XPCollection<PriceCatalog> PriceCatalogs
        {
            get
            {
                return new XPCollection<PriceCatalog>(PersistentCriteriaEvaluationBehavior.InTransaction, this.Session
                                                      ,new BinaryOperator("ParentCatalogOid", this.Oid)
                                                     );
            }
        }

        [Association("PriceCatalog-PriceCatalogDetails"), Aggregated]
        public XPCollection<PriceCatalogDetail> PriceCatalogDetails
        {
            get
            {
                return GetCollection<PriceCatalogDetail>("PriceCatalogDetails");
            }
        }

        [Association("PriceCatalog-StorePriceLists"), Aggregated]
        public XPCollection<StorePriceList> StorePriceLists
        {
            get
            {
                return GetCollection<StorePriceList>("StorePriceLists");
            }
        }

        [Association("PriceCatalog-CustomerCategoryDiscounts")]
        public XPCollection<CustomerCategoryDiscount> CustomerCategoryDiscounts
        {
            get
            {
                return GetCollection<CustomerCategoryDiscount>("CustomerCategoryDiscounts");
            }
        }

        [Association("PriceCatalog-PriceCatalogPolicyDetails"), Aggregated]
        public XPCollection<PriceCatalogPolicyDetail> PriceCatalogPolicyDetails
        {
            get
            {
                return GetCollection<PriceCatalogPolicyDetail>("PriceCatalogPolicyDetails");
            }
        }

        [Association("PriceCatalog-Offers")]
        public XPCollection<Offer> Offers
        {
            get
            {
                return GetCollection<Offer>("Offers");
            }
        }


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



        [Indexed("Code;GCRecord", Unique = true), Association("Supplier-PriceCatalogs")]
        public CompanyNew Owner
        {
            get
            {
                return _Owner;
            }
            set
            {
                SetPropertyValue("Owner", ref _Owner, value);
            }
        }

        protected override void OnSaving()
        {
            if (StartDate == null)  // Intiallize 
                StartDate = DateTime.MinValue;  //Den douleuei to int
            if (EndDate == null)
                EndDate = DateTime.MaxValue;    //Den douleuei to int
            if (ParentCatalog == null)
            {
                Level = 0;
            }
            else
            {
                Level = ParentCatalog.Level + 1;
            }
            base.OnSaving();
        }
        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            if (propertyName == "Level")
            {
                foreach (PriceCatalog pc in PriceCatalogs)
                {
                    pc.Level = (int)newValue + 1;
                }
            }
            else if (propertyName == "ParentCatalog")
            {
                if (newValue == null)
                {
                    Level = 0;
                }
                else
                {
                    Level = ((PriceCatalog)newValue).Level + 1;
                }
            }
            base.OnChanged(propertyName, oldValue, newValue);
        }

        /// <summary>
        /// Χρησιμοποιείται μόνο για συμβάτότα με παλίες εκδόσεις
        /// </summary>
        /// <returns></returns>
        public XPCollection<Item> GetItems()
        {
            return GetItems<Item>(DateTime.Now);
        }

        /// <summary>
        /// Χρησιμοποιείται μόνο για συμβάτότα με παλίες εκδόσεις
        /// </summary>
        /// <returns></returns>
        public XPCollection<Barcode> GetBarcodes()
        {
            return GetItems<Barcode>(DateTime.Now);
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
            if (ParentCatalog != null) return true;
            if (DateInsideRange(StartDate, ParentCatalog.StartDate, ParentCatalog.EndDate) && StartDate <= EndDate)
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
            if (ParentCatalog == null) return false;
            foreach (PriceCatalog item in ParentCatalog.PriceCatalogs)
            {
                if (item.Oid == Oid) continue;
                if (DateInsideRange(StartDate, item.StartDate, item.EndDate) &&
                     DateInsideRange(EndDate, item.StartDate, item.EndDate) &&
                     DateInsideRange(item.StartDate, StartDate, EndDate) &&
                     DateInsideRange(item.EndDate, StartDate, EndDate)) return true;
            }
            return false;

        }

        /// <summary>
        /// Δημιουργεί μία λίστα μέ όλα τι είδη που είναι ενεργά για την τρέχουσα ημερομηνία από τρέχοντα τιμοκατάλογο και τα 
        /// παιδία του.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="currentDate"></param>
        /// <returns></returns>
        public List<Guid> GetAllOids<T>(DateTime currentDate)
        {
            List<Guid> ItemOids = new List<Guid>();
            XPQuery<PriceCatalogDetail> pcdt = new XPQuery<PriceCatalogDetail>(this.Session);
            XPQuery<Offer> offers = new XPQuery<Offer>(this.Session);


            if (DateInsideRange(currentDate, StartDate, EndDate))
            {
                if (typeof(T) == typeof(Barcode))
                {
                    var ids = from item in pcdt
                              where item.PriceCatalog.Oid == Oid && item.Item.IsActive && !ItemOids.Contains(item.Item.Oid)
                              select item.Barcode.Oid;
                    ids = ids.Distinct().DefaultIfEmpty(Guid.Empty);
                    if (ids != null) ItemOids.AddRange(ids.ToList());
                    //   return ItemOids;
                }
                else if (typeof(T) == typeof(Item))
                {
                    var ids = from item in pcdt
                              where item.PriceCatalog.Oid == Oid && item.Item.IsActive && !ItemOids.Contains(item.Item.Oid)
                              select item.Item.Oid;
                    if (ids != null) ItemOids.AddRange(ids.ToList());
                }
                else if (typeof(T) == typeof(PriceCatalogDetail))
                {
                    var ids = from item in pcdt
                              where item.PriceCatalog.Oid == Oid && item.Item.IsActive && !ItemOids.Contains(item.Item.Oid)
                              select item.Oid;
                    if (ids != null) ItemOids.AddRange(ids.ToList());
                }
                else if (typeof(T) == typeof(Offer))
                {
                    var ids = from item in offers
                              where item.PriceCatalog.Oid == Oid && item.IsActive && item.StartDate <= currentDate && item.EndDate >= currentDate && !ItemOids.Contains(item.Oid)
                              select item.Oid;
                    if (ids != null) ItemOids.AddRange(ids.ToList());
                }
            }
            if (!IsRoot || ParentCatalog != null)
            {
                ItemOids.AddRange(ParentCatalog.GetAllOids<T>(currentDate));
            }

            if (ItemOids.Count() == 0)
                return null;
            else
                return ItemOids;
        }

        /// <summary>
        /// Επιστρέφει ένα collection ειδών ή barcode που υπάρχουν στον τρέχον τιμοκατάλογο για την συγκεκριμένη ημερομηνία
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="currentDate"></param>
        /// <param name="SortPropertyName"></param>
        /// <returns></returns>
        public XPCollection<T> GetItems<T>(DateTime currentDate, string SortPropertyName = "Oid")
        {
            XPCursor cursor = new XPCursor(this.Session, typeof(T), GetAllOids<T>(currentDate));

            XPCollection<T> tcol = new XPCollection<T>(this.Session, false);
            tcol.AddRange(cursor.OfType<T>());
            return tcol;
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

        [Association("PriceCatalog-PriceCatalogPromotions"), Aggregated]
        public XPCollection<PriceCatalogPromotion> PriceCatalogPromotions
        {
            get
            {
                return GetCollection<PriceCatalogPromotion>("PriceCatalogPromotions");
            }
        }

        IEnumerable<IPriceCatalogPromotion> IPriceCatalog.PriceCatalogPromotions
        {
            get
            {
                return PriceCatalogPromotions;
            }
        }

        public Store IsEditableAtStore
        {
            get
            {
                return _IsEditableAtStore;
            }
            set
            {
                SetPropertyValue("IsEditableAtStore", ref _IsEditableAtStore, value);
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