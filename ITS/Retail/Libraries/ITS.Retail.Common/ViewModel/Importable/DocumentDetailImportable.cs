using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
namespace ITS.Retail.Common.ViewModel.Importable
{
    public class DocumentDetailImportable : ImportableViewModel<DocumentDetail>
    {
        public override string EntityName
        {
            get { return "DocumenDetail"; }
        }

        public override string ImportObjectUniqueKey
        {
            get
            {
                return this.DocumentHeaderUniqueKey + this.ItemCode + this.BarcodeCode + this.Qty.ToString();
            }
        }

        public string DocumentHeaderUniqueKey
        {
            get
            {
                return this.DocumentNumber + this.Year.ToString() + this.Month.ToString() + this.DayOfMonth.ToString();
            }
        }

        public override bool HasNewChildren
        {
            get { return false; }
        }

        public override bool CreateOrUpdatePeristant(UnitOfWork uow, Guid owner, Guid store)
        {
            return false;
        }

        public Guid ItemOid
        {
            get
            {
                return _ItemOid;
            }
            set
            {
                SetPropertyValue("Item", ref _ItemOid, value);
            }
        }

        [LookupImportable(typeof(Item), "Code", "ItemOid")]
        public string ItemCode
        {
            get
            {
                return _ItemCode;
            }
            set
            {
                SetPropertyValue("ItemCode", ref _ItemCode, value);
            }
        }


        public Guid BarcodeOid
        {
            get
            {
                return _BarcodeOid;
            }
            set
            {
                SetPropertyValue("BarcodeOid", ref _BarcodeOid, value);
            }
        }

        [LookupImportable(typeof(Barcode), "Code", "BarcodeOid")]
        public string BarcodeCode
        {
            get
            {
                return _BarcodeCode;
            }
            set
            {
                SetPropertyValue("BarcodeCode", ref _BarcodeCode, value);
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

        public decimal? Qty
        {
            get
            {
                return _Qty;
            }
            set
            {
                SetPropertyValue("Qty", ref _Qty, value);
            }
        }

        public decimal? VatFactor
        {
            get
            {
                return _VatFactor;
            }
            set
            {
                SetPropertyValue("VatFactor", ref _VatFactor, value);
            }
        }

        public decimal? UnitPrice
        {
            get
            {
                return _UnitPrice;
            }
            set
            {
                SetPropertyValue("UnitPrice", ref _UnitPrice, value);
            }
        }


        public decimal? NetTotalBeforeDiscount
        {
            get
            {
                return _NetTotalBeforeDiscount;
            }
            set
            {
                SetPropertyValue("NetTotalBeforeDiscount", ref _NetTotalBeforeDiscount, value);
            }
        }


        public decimal? DiscountAmount
        {
            get
            {
                return _DiscountAmount;
            }
            set
            {
                SetPropertyValue("DiscountAmount", ref _DiscountAmount, value);
            }
        }


        public decimal? NetTotal
        {
            get
            {
                return _NetTotal;
            }
            set
            {
                SetPropertyValue("NetTotal", ref _NetTotal, value);
            }
        }


        public decimal? VatTotal
        {
            get
            {
                return _VatTotal;
            }
            set
            {
                SetPropertyValue("VatTotal", ref _VatTotal, value);
            }
        }


        public decimal? GrossTotal
        {
            get
            {
                return _GrossTotal;
            }
            set
            {
                SetPropertyValue("GrossTotal", ref _GrossTotal, value);
            }
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

        public bool IsValid
        {
            get
            {
                return _IsValid;
            }
            set
            {
                SetPropertyValue("IsValid", ref _IsValid, value);
            }
        }


        public string Message
        {
            get
            {
                return _Message;
            }
            set
            {
                SetPropertyValue("Message", ref _Message, value);
            }
        }

        // Fields...
        private int _DocumentNumber;
        private Guid _BarcodeOid;
        private string _BarcodeCode;
        private string _ItemCode;
        private decimal? _NetTotalBeforeDiscount;
        private decimal? _GrossTotal;
        private decimal? _VatTotal;
        private decimal? _NetTotal;
        private decimal? _DiscountAmount;
        private decimal? _UnitPrice;
        private decimal? _Qty;
        private Guid _ItemOid;
        private int _DayOfMonth;
        private int _Month;
        private int _Year;
        private bool _IsValid;
        private string _Message;
        private decimal? _VatFactor;

        public override void CheckWithDatabase(UnitOfWork uow, Guid owner)
        {
            base.CheckWithDatabase(uow, owner);
            Message = string.Empty;
            IsValid = true;
            if(Qty.HasValue == false || Qty.Value == 0)
            {
                IsValid = false;
                Message += "No Qty ";
            }

            if (VatTotal.HasValue == false || GrossTotal.HasValue == false)
            {
                IsValid = false;
                Message += "No Vat/Gross ";
            }

            if(NetTotal.HasValue == false && UnitPrice.HasValue == false)
            {
                IsValid = false;
                Message += "No Price/NetTotal";
            }

            if(IsValid)
            {
                if(NetTotalBeforeDiscount.HasValue == false)
                {
                    if(NetTotal.HasValue)
                    {
                        NetTotalBeforeDiscount = NetTotal.Value + (DiscountAmount.HasValue ? DiscountAmount.Value : 0m);
                    }
                    else if(UnitPrice.HasValue)
                    {
                        NetTotalBeforeDiscount = UnitPrice * Qty;
                    }
                    else
                    {
                        IsValid = false;
                    }
                }
                if(NetTotal.HasValue == false)
                {
                    NetTotal = (NetTotalBeforeDiscount.HasValue ? NetTotalBeforeDiscount.Value : 0m) + NetTotalBeforeDiscount;
                }
                if(UnitPrice.HasValue == false)
                {
                    UnitPrice = NetTotalBeforeDiscount.Value / Qty.Value;
                }
                if(GrossTotal.HasValue == false)
                {
                    GrossTotal = NetTotal + VatTotal;
                }
                if(VatTotal.HasValue == false)
                {
                    VatTotal = GrossTotal - NetTotal;
                }
                if(DiscountAmount.HasValue == false)
                {
                    DiscountAmount = NetTotalBeforeDiscount - NetTotal;
                }
            }
        }

        
    }
}
