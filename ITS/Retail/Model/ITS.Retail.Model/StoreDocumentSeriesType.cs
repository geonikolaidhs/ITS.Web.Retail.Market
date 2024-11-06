//-----------------------------------------------------------------------
// <copyright file="StoreDocumentSeriesType.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Model.Attributes;

namespace ITS.Retail.Model
{
    [Updater(Order = 420, Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("StoreDocumentSeriesType", typeof(ResourcesLib.Resources))]

    public class StoreDocumentSeriesType : BaseObj
    {
        public StoreDocumentSeriesType()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public StoreDocumentSeriesType(Session session)
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
                    if (store == null)
                    {
                        throw new Exception("StoreDocumentSeriesType.GetUpdaterCriteria(); Error: Store is null");
                    }
                    CriteriaOperator moduleCriteria = CriteriaOperator.Or(new BinaryOperator("DocumentSeries.eModule", eModule.ALL),
                                                                          new BinaryOperator("DocumentSeries.eModule", eModule.STORECONTROLLER),
                                                                          new BinaryOperator("DocumentSeries.eModule", eModule.POS)
                                                                         );
                    crop = CriteriaOperator.And(new BinaryOperator("DocumentSeries.Store.Oid", store.Oid), moduleCriteria);
                    break;
                case eUpdateDirection.STORECONTROLLER_TO_POS:
                    crop =
                        CriteriaOperator.Or(
                            new BinaryOperator("DocumentSeries.eModule", eModule.POS),
                            new ContainsOperator("POSStoreDocumentSeriesTypes", new BinaryOperator("POS.Oid", deviceID))
                            );
                    break;
            }
            return crop;
        }

        //private POSDevice _FiscalDevice;
        //private bool _IsForPOS;
        private DocumentSeries _DocumentSeries;
        private Customer _DefaultCustomer;
        private SupplierNew _DefaultSupplier;
        private DocumentType _DocumentType;
        private CustomReport _DefaultCustomReport;
        private int _Duplicates;
        private UserType _UserType;
        private eStoreDocumentType _StoreDocumentType;
        private string _MenuDescription;
        private StoreDocumentSeriesTypePrintServiceSettings _PrintServiceSettings;
        private eDiscountType _DefaultDiscountType;
        private decimal _DefaultDiscountPercentage;


        [Association("DocumentType-StoreDocumentSeriesTypes")]
        public DocumentType DocumentType
        {
            get
            {
                return _DocumentType;
            }
            set
            {
                SetPropertyValue("DocumentType", ref _DocumentType, value);
            }
        }

        [Indexed("GCRecord;DocumentType", Unique = true)]
        [Association("DocumentSeries-StoreDocumentSeriesTypes")]
        public DocumentSeries DocumentSeries
        {
            get
            {
                return _DocumentSeries;
            }
            set
            {
                SetPropertyValue("DocumentSeries", ref _DocumentSeries, value);
            }
        }


        public eDiscountType DefaultDiscountType
        {
            get
            {
                return _DefaultDiscountType;
            }
            set
            {
                SetPropertyValue("DefaultDiscountType", ref _DefaultDiscountType, value);
            }
        }


        public decimal DefaultDiscountPercentage
        {
            get
            {
                return _DefaultDiscountPercentage;
            }
            set
            {
                SetPropertyValue("DefaultDiscountPercentage", ref _DefaultDiscountPercentage, value);
            }
        }



        public bool HasAutomaticNumbering
        {
            get
            {
                if (DocumentSeries == null)
                {
                    return false;
                }

                return DocumentSeries.HasAutomaticNumbering;
            }
        }

        public Customer DefaultCustomer
        {
            get
            {
                return _DefaultCustomer;
            }
            set
            {
                SetPropertyValue("DefaultCustomer", ref _DefaultCustomer, value);
            }
        }

        public SupplierNew DefaultSupplier
        {
            get
            {
                return _DefaultSupplier;
            }
            set
            {
                SetPropertyValue("DefaultSupplier", ref _DefaultSupplier, value);
            }
        }

        public CustomReport DefaultCustomReport
        {
            get
            {
                return _DefaultCustomReport;
            }
            set
            {
                SetPropertyValue("DefaultCustomReport", ref _DefaultCustomReport, value);
            }
        }

        [DescriptionField]
        public string Description
        {
            get
            {
                string documentSeriesFullName = DocumentSeries == null ? String.Empty : DocumentSeries.FullName;
                string documentTypeDescription = DocumentType == null ? String.Empty : DocumentType.Description;
                string description = String.IsNullOrWhiteSpace(MenuDescription) ?
                                     String.Format("{0} {1}", documentTypeDescription, documentSeriesFullName) :
                                     MenuDescription;
                return description;
            }
        }

        public int Duplicates
        {
            get
            {
                return _Duplicates;
            }
            set
            {
                SetPropertyValue("Duplicates", ref _Duplicates, value);
            }
        }

        public UserType UserType
        {
            get
            {
                return _UserType;
            }
            set
            {
                SetPropertyValue("UserType", ref _UserType, value);
            }
        }

        public eStoreDocumentType StoreDocumentType
        {
            get
            {
                return _StoreDocumentType;
            }
            set
            {
                SetPropertyValue("StoreDocumentType", ref _StoreDocumentType, value);
            }
        }

        public string MenuDescription
        {
            get
            {
                return _MenuDescription;
            }
            set
            {
                SetPropertyValue("MenuDescription", ref _MenuDescription, value);
            }
        }

        [Association("SDST-StoreDocumentSeriesType")]
        public XPCollection<POSDocumentSeries> POSStoreDocumentSeriesTypes
        {
            get
            {
                return GetCollection<POSDocumentSeries>("POSStoreDocumentSeriesTypes");
            }
        }

        public StoreDocumentSeriesTypePrintServiceSettings PrintServiceSettings
        {
            get
            {
                return _PrintServiceSettings;
            }
            set
            {
                if (_PrintServiceSettings == value)
                {
                    return;
                }

                StoreDocumentSeriesTypePrintServiceSettings previousPrintServiceSettings = _PrintServiceSettings;
                _PrintServiceSettings = value;

                if (IsLoading)
                {
                    return;
                }

                if (previousPrintServiceSettings != null && previousPrintServiceSettings.StoreDocumentSeriesType == this)
                {
                    previousPrintServiceSettings.StoreDocumentSeriesType = null;
                }


                if (_PrintServiceSettings != null)
                {
                    _PrintServiceSettings.StoreDocumentSeriesType = this;
                }

                OnChanged("PrintServiceSettings");
            }
        }
    }

}