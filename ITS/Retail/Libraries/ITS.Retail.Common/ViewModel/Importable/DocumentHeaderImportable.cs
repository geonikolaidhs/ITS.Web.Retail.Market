using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace ITS.Retail.Common.ViewModel.Importable
{
    public class DocumentHeaderImportable : ImportableViewModel<DocumentHeader>
    {
        public override string ImportObjectUniqueKey
        {
            get
            {
                return this.DocumentNumber + this.Year.ToString() + this.Month.ToString() + this.DayOfMonth.ToString();
            }
        }

        public override void CheckWithDatabase(UnitOfWork uow, Guid owner)
        {
            base.CheckWithDatabase(uow, owner);
            this.DocumentDetails.ForEach(x => x.CheckWithDatabase(uow, owner));
        }

        public DocumentHeaderImportable()
        {
            this.DocumentDetails = new List<DocumentDetailImportable>();
        }

        public override string EntityName
        {
            get { return "DocumentHeader"; }
        }

        public override bool HasNewChildren
        {
            get { return DocumentDetails.Any(x => x.IsNew); }
        }

        public override bool CreateOrUpdatePeristant(UnitOfWork uow, Guid owner, Guid store)
        {
            if (!IsNew)
            {
                return false;
            }
            DocumentHeader header = new DocumentHeader(uow)
            {
                Store = uow.GetObjectByKey<Store>(store),
                FiscalDate = new DateTime(this.Year, this.Month, this.DayOfMonth),
                Supplier = uow.GetObjectByKey<SupplierNew>(this.Supplier),
                Division = Platform.Enumerations.eDivision.Purchase,
                DocumentType = uow.GetObjectByKey<DocumentType>(this.DocumentType),
                DocumentSeries = null,
                Status = uow.GetObjectByKey<DocumentStatus>(this.DocumentStatus),
                DocumentNumber = this.DocumentNumber
            };
            //StoreDocumentSeriesType sdst = 
            var sdsts = header.DocumentType.StoreDocumentSeriesTypes.Where(x => x.DocumentSeries.Store.Oid == store);
            if(sdsts.Count()>0)
            {
                header.DocumentSeries = sdsts.First().DocumentSeries;
            }

            this.DocumentDetails.ForEach(det =>
            {
                DocumentDetail detail = new DocumentDetail(uow)
                {
                    DocumentHeader = header,
                    Item = uow.GetObjectByKey<Item>(det.ItemOid),
                    Barcode = uow.GetObjectByKey<Barcode>(det.BarcodeOid),
                    CustomUnitPrice = det.UnitPrice.Value,
                    NetTotalBeforeDiscount = det.NetTotalBeforeDiscount.Value,
                    Qty = det.Qty.Value,
                    VatFactor = det.VatFactor.Value,
                    TotalVatAmount = det.VatTotal.Value,
                    GrossTotal = det.GrossTotal.Value,
                    NetTotal = det.NetTotal.Value,
                    HasCustomPrice = true

                };

                if(det.DiscountAmount.HasValue && det.DiscountAmount.Value !=0)
                {
                    DocumentDetailDiscount detailDiscount = new DocumentDetailDiscount(uow)
                    {
                        DocumentDetail = detail,
                        DiscountSource = Platform.Enumerations.eDiscountSource.CUSTOM,
                        DiscountType = Platform.Enumerations.eDiscountType.VALUE,
                        Value = det.DiscountAmount.Value
                    };
                }
            });
            return true;
        }


        public override ImportResult Import(string textToParse, SupplierImportFileRecordHeader header)
        {
            ImportResult result =  base.Import(textToParse, header);
            if(result.Applicable)
            {
                if(header.DocumentStatus !=null)
                {
                    this.DocumentStatus = header.DocumentStatus.Oid;
                }
                if (header.DocumentType != null)
                {
                    this.DocumentType = header.DocumentType.Oid;
                }
                if(header.SupplierImportSettingsSet.Supplier !=null)
                {
                    this.Supplier = header.SupplierImportSettingsSet.Supplier.Oid;
                }
            }
            return result;
        }

        public int DayOfMonth
        {
            get
            {
                return _DayOfMonth;
            }
            set
            {
                SetPropertyValue("DayOfMonth", ref _DayOfMonth, value);
            }
        }

        public int Month
        {
            get
            {
                return _Month;
            }
            set
            {
                SetPropertyValue("Month", ref _Month, value);
            }
        }

        public int Year
        {
            get
            {
                return _Year;
            }
            set
            {
                SetPropertyValue("Year", ref _Year, value);
            }
        }

        [Display(AutoGenerateField = false)]
        public Guid Supplier
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


        [Display(AutoGenerateField = false)]
        public Guid DocumentType
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

        [Display(AutoGenerateField = false)]
        public Guid DocumentStatus
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

        public int DocumentNumber
        {
            get
            {
                return _DocumentNumber;
            }
            set
            {
                SetPropertyValue("DocumentNumber", ref _DocumentNumber, value);
            }
        }

        [MasterDetailImportable(MasterProperty = "ImportObjectUniqueKey", DetailProperty = "DocumentHeaderUniqueKey")]
        public List<DocumentDetailImportable> DocumentDetails { get; set; }

        // Fields...
        private int _DocumentNumber;
        private int _Year;
        private int _Month;
        private int _DayOfMonth;
        private Guid _Supplier;
        private Guid _DocumentType;
        private Guid _DocumentStatus;
    }
}
