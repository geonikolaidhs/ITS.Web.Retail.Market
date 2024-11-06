using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Enumerations;
using System.Collections;


namespace ITS.Retail.Common.ViewModel
{
    public class DocumentSearchFilter: BaseSearchFilter
    {
        public DocumentSearchFilter()
        {
            ApplicableStores = new List<Guid>();
            ProformaTypes = new List<Guid>();
            ResetDateFilters();
        }

        private void ResetDateFilters()
        {
            FromDate = DateTime.Now.Date;
            FromDateTime = FromDate.Value.AddDays(-1).AddMilliseconds(1);
            ToDate = FromDate;
            ToDateTime = FromDate.Value.AddDays(1).AddMilliseconds(-1);
            FromFiscalDate = null;
            FromFiscalDateTime = null;
            ToFiscalDate = null;
            ToFiscalDateTime = null;
        }


        [CriteriaField("FinalizedDate", OperatorType = CustomBinaryOperatorType.GreaterOrEqual)]
        public DateTime? FromDateFilter
        {
            get
            {
                return DateTimeHelper.GetDateTimeValue(FromDate, FromDateTime);
            }
        }

        [Binding("Fromdate")]
        public DateTime? FromDate
        {
            get
            {
                return _FromDate;
            }
            set
            {
                SetPropertyValue("FromDate", ref _FromDate, value);
            }
        }

        [Binding("FromDateTime")]
        public DateTime? FromDateTime
        {
            get
            {
                return _FromDateTime;
            }
            set
            {
                SetPropertyValue("FromDateTime", ref _FromDateTime, value);
            }
        }

        [CriteriaField("FinalizedDate", OperatorType = CustomBinaryOperatorType.LessOrEqual, PreProcessorType = typeof(DateNight))]
        public DateTime? ToDateFilter
        {
            get
            {
                return DateTimeHelper.GetDateTimeValue(ToDate, ToDateTime);
            }
        }

        [Binding("todate")]
        public DateTime? ToDate
        {
            get
            {
                return _ToDate;
            }
            set
            {
                SetPropertyValue("ToDate", ref _ToDate, value);
            }
        }

        [Binding("ToDateTime")]
        public DateTime? ToDateTime
        {
            get
            {
                return _ToDateTime;
            }
            set
            {
                SetPropertyValue("ToDateTime", ref _ToDateTime, value);
            }
        }

        [CriteriaField("FiscalDate", OperatorType = CustomBinaryOperatorType.GreaterOrEqual)]
        public DateTime? FromFiscalDateFilter
        {
            get
            {
                return DateTimeHelper.GetDateTimeValue(FromFiscalDate, FromFiscalDateTime);
            }
        }

        [Binding("FromFiscaldate")]
        public DateTime? FromFiscalDate
        {
            get
            {
                return _FromFiscalDate;
            }
            set
            {
                SetPropertyValue("FromFiscalDate", ref _FromFiscalDate, value);
            }
        }

        [Binding("FromFiscalDateTime")]
        public DateTime? FromFiscalDateTime
        {
            get
            {
                return _FromFiscalDateTime;
            }
            set
            {
                SetPropertyValue("FromFiscalDateTime", ref _FromFiscalDateTime, value);
            }
        }

        [CriteriaField("FiscalDate", OperatorType = CustomBinaryOperatorType.LessOrEqual)]
        public DateTime? ToFiscalDateFilter
        {
            get
            {
                return DateTimeHelper.GetDateTimeValue(ToFiscalDate, ToFiscalDateTime);
            }
        }

        [Binding("toFiscaldate")]
        public DateTime? ToFiscalDate
        {
            get
            {
                return _ToFiscalDate;
            }
            set
            {
                SetPropertyValue("ToFiscalDate", ref _ToFiscalDate, value);
            }
        }

        public DateTime? ToFiscalDateTime
        {
            get
            {
                return _ToFiscalDateTime;
            }
            set
            {
                SetPropertyValue("ToFiscalDateTime", ref _ToFiscalDateTime, value);
            }
        }

        [Binding("statuscombo_VI")]
        [CriteriaField("Status.Oid")]
        public Guid? DocumentStatus
        {
            get
            {
                return _DocumentStatus;
            }
            set
            {
                SetPropertyValue("DocumentStatus", ref _DocumentStatus, value);
            }
        }

        [Binding("typecombo_VI")]
        [CriteriaField("DocumentType.Oid")]
        public Guid? DocumentType
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

        [Binding("seriescombo_VI")]
        [CriteriaField("DocumentSeries.Oid")]
        public Guid? DocumentSeries
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

        
        
        public Guid? User
        {
            get
            {
                return _User;
            }
            set
            {
                SetPropertyValue("User", ref _User, value);
            }
        }

        [Binding("Users_VI")]
        [CriteriaField("CreatedBy.Oid")]
        public Guid? CreatedBy
        {
            get
            {
                return _CreatedBy;
            }
            set
            {
                SetPropertyValue("CreatedBy", ref _CreatedBy, value);
            }
        }

        [Binding("createdByDevice", EnumerableType=typeof(Guid))]
        public IEnumerable CreatedByDevice
        {
            get
            {
                return _CreatedByDevice;
            }
            set
            {
                SetPropertyValue("CreatedByDevice", ref _CreatedByDevice, value);
            }
        }       
        

        [Binding("sell_from_VI")]
        [CriteriaField("Store.Oid")]
        public Guid? Store
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
        
        [CriteriaField("SecondaryStore.Oid")]
        public Guid? SecondaryStore
        {
            get
            {
                return _SecondaryStore;
            }
            set
            {
                SetPropertyValue("SecondaryStore", ref _SecondaryStore, value);
            }
        }

        [CriteriaField("Store.Owner.Oid")]
        public Guid? Owner
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

        [CriteriaField("Division")]
        [Binding("Mode")]
        public eDivision Division
        {
            get
            {
                return _Division;
            }
            set
            {
                SetPropertyValue("Division", ref _Division, value);
            }
        }

        [CriteriaField("Customer.Oid")]
        [Binding("DocumentCustomerFilter_VI")]
        public Guid? Customer
        {
            get
            {
                return _Customer;
            }
            set
            {
                SetPropertyValue("Customer", ref _Customer, value);
            }
        }

        [CriteriaField("Supplier.Oid")]
        [Binding("DocumentSupplierFilter_VI")]
        public Guid? Supplier
        {
            get
            {
                return _Supplier;
            }
            set
            {
                SetPropertyValue("Supplier", ref _Supplier, value);
            }
        }

        public eTransformationStatus? TransformationStatus
        {
            get
            {
                return _TransformationStatus;
            }
            set
            {
                SetPropertyValue("TransformationStatus", ref _TransformationStatus, value);
            }
        }


        [Binding("txtnumberFrom")]
        [CriteriaField("DocumentNumber", OperatorType = CustomBinaryOperatorType.GreaterOrEqual)]
        public int? DocumentNumberFrom
        {
            get
            {
                return _DocumentNumberFrom;
            }
            set
            {
                SetPropertyValue("DocumentNumberFrom", ref _DocumentNumberFrom, value);
            }
        }


        [Binding("txtnumberTo")]
        [CriteriaField("DocumentNumber", OperatorType = CustomBinaryOperatorType.LessOrEqual)]
        public int? DocumentNumberTo
        {
            get
            {
                return _DocumentNumberTo;
            }
            set
            {
                SetPropertyValue("DocumentNumberTo", ref _DocumentNumberTo, value);
            }
        }    
    


        public List<Guid> ApplicableStores { get; protected set; }

        public List<Guid> ProformaTypes { get; set;}

        public string Proforma
        {
            get
            {
                return _Proforma;
            }
            set
            {
                SetPropertyValue("Proforma", ref _Proforma, value);
            }
        }
       
        protected override List<CriteriaOperator> BuildExtraCriteria()
        {
            List<CriteriaOperator> list = base.BuildExtraCriteria();
            if (CreatedByDevice is IEnumerable && (CreatedByDevice as IEnumerable).Cast<object>().Count() > 0)
            {
                list.Add(new InOperator("CreatedByDevice", CreatedByDevice));
            }

            if (this.Proforma == "Proforma" || this.Proforma == "SpecialProforma")
            {
                if (this.ProformaTypes.Count <= 0)
                {
                    ProformaTypes.Add(Guid.Empty);
                }
                list.Add(new InOperator("DocumentType.Oid", ProformaTypes));
                list.Add(new BinaryOperator("IsCanceled", false));
            }
     
            //TO DO User Criteria

            if (TransformationStatus.HasValue)
            {
                CriteriaOperator cancelledDocCriteria = CriteriaOperator.Or(new BinaryOperator("IsCanceled", true),
                                                                      CriteriaOperator.And(new NotOperator(new NullOperator("CancelsDocumentOid")),
                                                                                           new BinaryOperator("CancelsDocumentOid",
                                                                                           Guid.Empty,BinaryOperatorType.NotEqual)));

                BinaryOperator derivedDocumentCriteria = new BinaryOperator(new AggregateOperand("DerivedDocuments","Oid", Aggregate.Count,
                    new BinaryOperator("DerivedDocument.IsCanceled", false)),0, BinaryOperatorType.Greater);

                CriteriaOperator detailTransformedCriteria = new ContainsOperator("DocumentDetails",
                                CriteriaOperator.And(
                                        new BinaryOperator("LinkedLine",Guid.Empty),//new BinaryOperator("IsLinkedLine", false),
                                        new BinaryOperator("IsCanceled", false),
                                        new BinaryOperator("IsReturn", false),
                                        new NullOperator("SpecialItem"),
                                        CriteriaOperator.Or(
                                            new BinaryOperator(new OperandProperty("Qty"),
                                                               new AggregateOperand("DerivedRelativeDocumentDetails", "Qty", Aggregate.Sum,
                                                               CriteriaOperator.And(new BinaryOperator("RelativeDocument.DerivedDocument.IsCanceled", false))), 
                                                               BinaryOperatorType.NotEqual),                                     
                                            CriteriaOperator.Parse("DerivedRelativeDocumentDetails.Count = 0"))));

                switch (TransformationStatus.Value)
                {
                    case eTransformationStatus.CANNOT_BE_TRANSFORMED:
                        list.Add(cancelledDocCriteria);                            
                        break;
                    case eTransformationStatus.NOT_TRANSFORMED:
                        list.Add(CriteriaOperator.And(new NotOperator(derivedDocumentCriteria),
                                                      new NotOperator(cancelledDocCriteria)));
                        break;
                    case eTransformationStatus.PARTIALLY_TRANSFORMED:
                        list.Add(CriteriaOperator.And(detailTransformedCriteria,
                                                      derivedDocumentCriteria));
                        break;
                    case eTransformationStatus.FULLY_TRANSFORMED:
                        list.Add(CriteriaOperator.And(new NotOperator(detailTransformedCriteria),
                                                      derivedDocumentCriteria));
                        break;
                }
            }
            return list;
        }


        // Fields...
        private string _Proforma;
        private eDivision _Division;
        private int? _DocumentNumberTo;
        private int? _DocumentNumberFrom;
        private eTransformationStatus? _TransformationStatus;
        private Guid? _Customer;
        private Guid? _Store;
        private IEnumerable _CreatedByDevice;
        private Guid? _User;
        private Guid? _DocumentSeries;
        private Guid? _DocumentType;
        private Guid? _DocumentStatus;
        private DateTime? _ToFiscalDate;
        private DateTime? _FromFiscalDate;
        private DateTime? _ToDate;
        private DateTime? _FromDate;
        private Guid? _CreatedBy;
        private DateTime? _FromDateTime;
        private DateTime? _ToDateTime;
        private DateTime? _FromFiscalDateTime;
        private DateTime? _ToFiscalDateTime;
        private Guid? _Supplier;
        private Guid? _Owner;
        private Guid? _SecondaryStore;

        public void ClearFilter()
        {
            DocumentNumberTo = null;
            DocumentNumberFrom = null;
            TransformationStatus = null;
            Customer = null;
            Store = null;
            SecondaryStore = null;
            CreatedByDevice = null;
            User = null;
            DocumentSeries = null;
            DocumentType = null;
            DocumentStatus = null;
            CreatedBy = null;

            ResetDateFilters();
        }
    }
}