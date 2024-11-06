//-----------------------------------------------------------------------
// <copyright file="DocumentSeries.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using System.Linq;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{
    [Updater(Order = 230,Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("DocumentSeries", typeof(ResourcesLib.Resources))]
    public class DocumentSeries : Lookup2Fields, IRequiredOwner
    {
        public DocumentSeries()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public DocumentSeries(Session session)
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


		public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew owner, Store store, string deviceID)
		{
			CriteriaOperator posCriteria = null;
            CriteriaOperator moduleCriteria = null;

			switch (direction)
			{
				case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    moduleCriteria = CriteriaOperator.Or(new BinaryOperator("eModule", eModule.POS), new BinaryOperator("eModule", eModule.STORECONTROLLER));
                    break;
                case eUpdateDirection.STORECONTROLLER_TO_POS:
                    Guid deviceOid;
                    if (Guid.TryParse(deviceID, out deviceOid))
                    {
                        posCriteria =
                            CriteriaOperator.Or(
                                new BinaryOperator("POS.Oid", deviceID),
                                new ContainsOperator("StoreDocumentSeriesTypes",
                                     new ContainsOperator("POSStoreDocumentSeriesTypes", new BinaryOperator("POS.Oid", deviceID))
                                )
                                );
                        moduleCriteria = CriteriaOperator.Or(new BinaryOperator("eModule", eModule.POS), new BinaryOperator("eModule", eModule.STORECONTROLLER));
                    }

                    break;
                default :
                    break;
            }

            if (store == null)
            {
                throw new Exception("DocumentSeries.GetUpdaterCriteria(); Error: Store is null");
            }
            CriteriaOperator crop = CriteriaOperator.And(new BinaryOperator("Store.Oid", store.Oid),
                                                         new BinaryOperator("Owner.Oid", owner.Oid),
                                                         posCriteria,
                                                         moduleCriteria
                                                        );
            return crop;
		}


        // Fields...
        private bool _HasAutomaticNumbering;
        private bool _IsCancelingSeries;
        private Store _Store;
        private string _Remarks;
        private string _PrintedCode;
        private Guid? _IsCanceledByOid;
        private DocumentSequence _DocumentSequence;
        private eModule _eModule;
        private bool _ShouldResetMenu;
        private POS _POS;

        [Size(SizeAttribute.Unlimited)]
        [DisplayOrder (Order = 4)]
        public string Remarks
        {
            get
            {
                return _Remarks;
            }
            set
            {
                SetPropertyValue("Remarks", ref _Remarks, value);
            }
        }

        [DisplayOrder (Order = 7)]
        public string PrintedCode
        {
            get
            {
                return _PrintedCode;
            }
            set
            {
                SetPropertyValue("PrintedCode", ref _PrintedCode, value);
            }
        }


        public string FullName
        {
            get
            {
                if (String.IsNullOrEmpty(Remarks))
                {
                    return Description;
                }
                return String.Format("{0}({1}", Description, Remarks) + ")";
            }
        }

        [Association("Store-DocumentSeries")]
        [DisplayOrder(Order = 3)]
        public Store Store
        {
            get
            {
                return _Store;
            }
            set
            {
                SetPropertyValue("Store", ref _Store, value);
            }
        }

        [Association("DocumentSeries-StoreDocumentSeriesTypes")]
        public XPCollection<StoreDocumentSeriesType> StoreDocumentSeriesTypes
        {
            get
            {
                return GetCollection<StoreDocumentSeriesType>("StoreDocumentSeriesTypes");
            }
        }

        [DisplayOrder(Order = 9)]
        public bool IsCancelingSeries
        {
            get
            {
                return _IsCancelingSeries;
            }
            set
            {
                SetPropertyValue("IsCancelingSeries", ref _IsCancelingSeries, value);
            }
        }


        [Persistent("IsCanceledBy")]
        public Guid? IsCanceledByOid
        {
            get
            {
                return _IsCanceledByOid;
            }
            set
            {
                SetPropertyValue("IsCanceledByOid", ref _IsCanceledByOid, value);
            }
        }

        [NonPersistent, UpdaterIgnoreField]
        [DisplayOrder(Order = 6)]
        public DocumentSeries IsCanceledBy
        {
            get
            {
                return this.Session.GetObjectByKey<DocumentSeries>(this.IsCanceledByOid);
            }
            set
            {
                if (value != null)
                {
                    IsCanceledByOid = value.Oid;
                }
                else
                {
                    IsCanceledByOid = null;
                }
            }
        }

        [DisplayOrder(Order = 10)]
        public bool HasAutomaticNumbering
        {
            get
            {
                return _HasAutomaticNumbering;
            }
            set
            {
                SetPropertyValue("HasAutomaticNumbering", ref _HasAutomaticNumbering, value);
            }
        }

        [UpdaterIgnoreField]
        [DisplayOrder(Order = 8)]
        public DocumentSequence DocumentSequence
        {
            get
            {
                return _DocumentSequence;
            }
            set
            {
                //SetPropertyValue("Address", ref _Address, value);
                if (_DocumentSequence == value)
                    return;

                // Store a reference to the former owner. 
                DocumentSequence docseq = _DocumentSequence;
                _DocumentSequence = value;

                if (IsLoading) return;

                // Remove an owner's reference to this building, if exists. 
                if (docseq != null && docseq.DocumentSeries == this)
                    docseq.DocumentSeries = null;

                // Specify that the building is a new owner's house. 
                if (_DocumentSequence != null)
                    _DocumentSequence.DocumentSeries = this;
                OnChanged("DocumentSequence");
            }
        }

        [NonPersistent]
        public bool ShouldResetMenu
        {
            get
            {
                return _ShouldResetMenu;
            }
            set
            {
                SetPropertyValue("ShouldResetMenu", ref _ShouldResetMenu, value);
            }
        }

        [DisplayOrder(Order = 5)]
        public eModule eModule
        {
            get
            {
                return _eModule;
            }
            set
            {
                SetPropertyValue("eModule", ref _eModule, value);
            }
        }

        [DisplayOrder(Order = 11)]
        public POS POS
        {
            get
            {
                return _POS;
            }
            set
            {
                SetPropertyValue("POS", ref _POS, value);
            }
        }

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);
            if (propertyName == "eModule")
            {
                ShouldResetMenu = true;
            }
        }
    }
}