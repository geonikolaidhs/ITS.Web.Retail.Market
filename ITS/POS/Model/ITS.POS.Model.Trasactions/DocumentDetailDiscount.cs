using DevExpress.Xpo;
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;
using System;

namespace ITS.POS.Model.Transactions
{
    [SyncInfoIgnoreAttribute]
    public class DocumentDetailDiscount : CustomFieldStorage, IDocumentDetailDiscount
    {
        public DocumentDetailDiscount()
        {

        }
        public DocumentDetailDiscount(Session session)
            : base(session)
        {

        }

        private DocumentDetail _DocumentDetail;
        private eDiscountSource _DiscountSource;
        private int _Priority;
        private eDiscountType _DiscountType;
        private bool _DiscardsOtherDiscounts;
        private decimal _DiscountDeviation;
        private decimal _Percentage;
        private decimal _Value;
        private Guid _Type;
        private string _TypeDescription;
        private string _Description;
        private Guid _Promotion;
        private TransactionCoupon _TransactionCoupon;
        private decimal _DiscountWithVAT;
        private decimal _DiscountWithoutVAT;

        public Guid Type
        {
            get
            {
                return _Type;
            }
            set
            {
                SetPropertyValue("Type", ref _Type, value);
            }
        }

        public Guid Promotion
        {
            get
            {
                return _Promotion;
            }
            set
            {
                SetPropertyValue("Promotion", ref _Promotion, value);
            }
        }

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

        public string TypeDescription
        {
            get
            {
                return _TypeDescription;
            }
            set
            {
                SetPropertyValue("TypeDescription", ref _TypeDescription, value);
            }
        }

        public decimal Value
        {
            get
            {
                return _Value;
            }
            set
            {
                SetPropertyValue("Value", ref _Value, value);
            }
        }

        public decimal Percentage
        {
            get
            {
                return _Percentage;
            }
            set
            {
                SetPropertyValue("Percentage", ref _Percentage, value);
            }
        }

        public decimal DiscountDeviation
        {
            get
            {
                return _DiscountDeviation;
            }
            set
            {
                SetPropertyValue("DiscountDeviation", ref _DiscountDeviation, value);
            }
        }

        public eDiscountSource DiscountSource
        {
            get
            {
                return _DiscountSource;
            }
            set
            {
                SetPropertyValue("DiscountSource", ref _DiscountSource, value);
            }
        }

        public bool DiscardsOtherDiscounts
        {
            get
            {
                return _DiscardsOtherDiscounts;
            }
            set
            {
                SetPropertyValue("DiscardsOtherDiscounts", ref _DiscardsOtherDiscounts, value);
            }
        }


        public eDiscountType DiscountType
        {
            get
            {
                return _DiscountType;
            }
            set
            {
                SetPropertyValue("DiscountType", ref _DiscountType, value);
            }
        }

        public int Priority
        {
            get
            {
                return _Priority;
            }
            set
            {
                SetPropertyValue("Priority", ref _Priority, value);
            }
        }


        [Association("DocumentDetail-DocumentDetailDiscounts"), Indexed(Unique = false)]
        public DocumentDetail DocumentDetail
        {
            get
            {
                return _DocumentDetail;
            }
            set
            {
                SetPropertyValue("DocumentDetail", ref _DocumentDetail, value);
            }
        }

        public string DisplayName
        {
            get
            {
                string displayName = "";
                if (String.IsNullOrWhiteSpace(this.Description) == false)
                {
                    displayName = this.Description;
                }
                else if (String.IsNullOrWhiteSpace(this.TypeDescription) == false)
                {
                    displayName = this.TypeDescription;
                }
                else
                {
                    displayName = this.DiscountSource.ToLocalizedString().ToUpperGR();
                }

                return displayName;
            }
        }

        [NonPersistent]
        public Guid PromotionOid
        {
            get
            {
                return Promotion;
            }
            set
            {
                Promotion = value;
            }
        }

        [NonPersistent]
        public Guid TypeOid
        {
            get
            {
                return Type;
            }
            set
            {
                Type = value;
            }
        }

        IDocumentDetail IDocumentDetailDiscount.DocumentDetail
        {
            get
            {
                return DocumentDetail;
            }
            set
            {
                DocumentDetail = (DocumentDetail)value;
            }
        }

        public TransactionCoupon TransactionCoupon
        {
            get
            {
                return _TransactionCoupon;
            }
            set
            {
                if (_TransactionCoupon == value)
                {
                    return;
                }

                // Store a reference to the former value. 
                TransactionCoupon transactionCoupon = _TransactionCoupon;
                _TransactionCoupon = value;

                if (IsLoading)
                {
                    return;
                }

                // Remove a reference if needed. 
                if (transactionCoupon != null && transactionCoupon.DocumentDetailDiscount == this)
                {
                    transactionCoupon.DocumentDetailDiscount = null;
                }

                // Specify the new owner value.
                if (_TransactionCoupon != null)
                {
                    _TransactionCoupon.DocumentDetailDiscount = this;
                }

                OnChanged("TransactionCoupon");
            }
        }

        public decimal DiscountWithVAT
        {
            get
            {
                return _DiscountWithVAT;
            }
            set
            {
                SetPropertyValue("DiscountWithVAT", ref _DiscountWithVAT, value);
            }
        }

        public decimal DiscountWithoutVAT
        {
            get
            {
                return _DiscountWithoutVAT;
            }
            set
            {
                SetPropertyValue("DiscountWithoutVAT", ref _DiscountWithoutVAT, value);
            }
        }
    }
}
