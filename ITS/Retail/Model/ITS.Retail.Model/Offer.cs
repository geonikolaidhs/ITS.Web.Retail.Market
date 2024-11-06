//-----------------------------------------------------------------------
// <copyright file="Offer.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using System.Linq;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using ITS.Retail.Platform.Enumerations;
namespace ITS.Retail.Model
{
    [Updater(Order = 630,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER)]// | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("Offer", typeof(ResourcesLib.Resources))]
    public class Offer : Lookup2Fields
    {
        public Offer()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Offer(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
            IsActive = true;
        }

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew owner, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    if (owner == null)
                    {
                        throw new Exception("Offer.GetUpdaterCriteria(); Error: Owner is null");
                    }
                    crop = new BinaryOperator("PriceCatalog.Owner.Oid", owner.Oid);
                    break;
            }

            return crop;
        }

        private string _Description2;
        [Size(SizeAttribute.Unlimited)]
        public string Description2
        {
            get
            {
                return _Description2;
            }
            set
            {
                SetPropertyValue("Description2", ref _Description2, value);
            }
        }

        private DateTime _StartDate;
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

        private DateTime _EndDate;
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
        private PriceCatalog _PriceCatalog;
        [Association("PriceCatalog-Offers")]
        public PriceCatalog PriceCatalog
        {
            get
            {
                return _PriceCatalog;
            }
            set
            {
                SetPropertyValue("PriceCatalog", ref _PriceCatalog, value);
            }
        }


        [Association("Offer-OfferDetails"), Aggregated]
        public XPCollection<OfferDetail> OfferDetails
        {
            get
            {
                return GetCollection<OfferDetail>("OfferDetails");
            }
        }

        protected override void OnSaving()
        {
            if (this.SkipOnSavingProcess)
            {
                base.OnSaving();
                return;
            }
            if (StartDate == null)  // Intiallize 
            {
                StartDate = Convert.ToDateTime(0);
            }
            if (EndDate == null)
            {
                EndDate = Convert.ToDateTime(9000);
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
            if (DateInsideRange(StartDate, StartDate, EndDate) && StartDate <= EndDate)
                return true;
            return false;

        }
    }

}