using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Kernel.Model;

namespace ITS.Retail.Model
{
    [Updater(Order = 1002,
           Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER )]
    [EntityDisplayName("Leaflet", typeof(ResourcesLib.Resources))]

    public class Leaflet : Lookup2Fields, IRequiredOwner, ILeaflet
    {
        public Leaflet()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Leaflet(Session session)
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

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew supplier, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    if (supplier == null)
                    {
                        throw new Exception("Leaflet.GetUpdaterCriteria(); Error: Owner is null");
                    }
                    if(store == null)
                    {
                        return CriteriaOperator.And(new BinaryOperator("Owner.Oid", supplier.Oid), new BinaryOperator("Store.Oid", store.Oid));
                    }
                    crop = new BinaryOperator("Owner.Oid", supplier.Oid);
                    break;
            }

            return crop;
        }
        [Association("Leaflet-LeafletDetails"), Aggregated]
        public XPCollection<LeafletDetail> LeafletDetails
        {
            get
            {
                return GetCollection<LeafletDetail>("LeafletDetails");
            }
        }
       
        private DateTime _EndTime;
        private DateTime _StartTime;
        private DateTime _EndDate;
        private DateTime _StartDate;
        private string _ImageDescription;
        private string _ImageInfo;
        private bool _ImportedFromERP;

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
        
        [ValueConverter(typeof(ImageValueConverter)), Delayed]
        [UpdaterIgnoreField]
        public Image Image
        {
            get
            {
                return GetDelayedPropertyValue<Image>("Image");
            }
            set
            {
                SetDelayedPropertyValue("Image", value);
            }
        }
        [Size(SizeAttribute.Unlimited)]
        public string ImageInfo
        {
            get
            {
                return _ImageInfo;
            }
            set
            {
                SetPropertyValue("ImageInfo", ref _ImageInfo, value);
            }
        }

        public string ImageDescription
        {
            get
            {
                return _ImageDescription;
            }
            set
            {
                SetPropertyValue("ImageDescription", ref _ImageDescription, value);
            }
        }

        [Association("LeafletStores-Leaflet"), Aggregated]
        public XPCollection<LeafletStore> Stores
        {
            get
            {
                return GetCollection<LeafletStore>("Stores");
            }
        }

        public bool ImportedFromERP
        {
            get
            {
                return _ImportedFromERP;
            }
            set
            {
                SetPropertyValue("ImportedFromERP", ref _ImportedFromERP, value);
            }
        }
    }
    }
