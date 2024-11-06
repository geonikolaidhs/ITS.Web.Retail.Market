using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using SFA_Model.NonPersistant;
using ITS.WRM.Model.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 53, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class PriceCatalog : LookUp2Fields, IPriceCatalog
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
        
        private bool _IsRoot;
        public bool IgnoreZeroPrices { get; set; }

        public Guid IsEditableAtStore { get; set; }



        public int Level { get; set; }

        public Guid? ParentCatalogOid { get; set; }
        [NonPersistent]
        public DateTime StartDate {

            get
            {
                return new DateTime(lngStartDate);
            }
            set
            {
                if (value == null)
                {
                    lngStartDate = DateTime.MinValue.Ticks;
                }
                else
                {
                    lngStartDate = value.Ticks;
                }
            }
        }
        [NonPersistent]
        public DateTime EndDate 
        {

            get
            {
                return new DateTime(lngEndDate);
            }
            set
            {
                if (value == null)
                {
                    lngEndDate = DateTime.MinValue.Ticks;
                }
                else
                {
                    lngEndDate = value.Ticks;
                }
            }
        }
        [Persistent("StartDate")]   
        public long lngStartDate { get; set; }

        [Persistent("EndDate")]
        public long lngEndDate { get; set; }
        public bool SupportLoyalty { get; set; }
        [NonPersistent]
        IStore IPriceCatalog.IsEditableAtStore
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
    }
}