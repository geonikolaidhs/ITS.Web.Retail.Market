using ITS.Retail.Model;
using System;
using DevExpress.Xpo;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using ITS.Retail.Platform.Kernel.Model;
using System.Collections.Generic;

namespace ITS.Retail.Common.ViewModel
{
    public class PriceCatalogDetailViewModel : BasePersistableViewModel, IPriceCatalogDetail, IPersistableViewModel
    {
        public override Type PersistedType { get { return typeof(PriceCatalogDetail); } }

        public PriceCatalogDetailViewModel()
        {
            VATIncluded = true;
            TimeValues = new BindingList<PriceCatalogDetailTimeValueViewModel>();
            TimeValues.ListChanged += TimeValues_ListChanged;

        }

        private void TimeValues_ListChanged(object sender, ListChangedEventArgs e)
        {
            if(e.ListChangedType == ListChangedType.ItemAdded && TimeValues.Count > e.NewIndex && TimeValues[e.NewIndex] !=null)
            {
                TimeValues[e.NewIndex].PriceCatalogDetail = this.Oid;
            }
        }

        // Fields...

        private decimal _DatabaseValue;
        private Guid _Item;
        private Guid _PriceCatalog;
        private decimal _Discount;
        private bool _VATIncluded;
        private Guid _Barcode;
        private decimal _MarkUp;
        private bool _IsActive;

        public Guid PriceCatalog
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

        public Guid Item
        {
            get
            {
                return _Item;
            }
            set
            {
                SetPropertyValue("Item", ref _Item, value);
            }
        }


        public Guid Barcode
        {
            get
            {
                return _Barcode;
            }
            set
            {
                SetPropertyValue("Barcode", ref _Barcode, value);
            }
        }

        [Range(0, double.MaxValue)]
        public decimal DatabaseValue
        {
            get
            {
                return _DatabaseValue;
            }
            set
            {
                SetPropertyValue("DatabaseValue", ref _DatabaseValue, value);
            }
        }

        

        [Range(0, 1)]
        public decimal Discount
        {
            get
            {
                return _Discount;
            }
            set
            {
                SetPropertyValue("Discount", ref _Discount, value);
            }
        }

        [Range(0, double.MaxValue)]
        public decimal MarkUp
        {
            get
            {
                return _MarkUp;
            }
            set
            {
                SetPropertyValue("MarkUp", ref _MarkUp, value);
            }
        }

        public bool VATIncluded
        {
            get
            {
                return _VATIncluded;
            }
            set
            {
                SetPropertyValue("VATIncluded", ref _VATIncluded, value);
            }
        }

        public bool IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                SetPropertyValue("IsActive", ref _IsActive, value);
            }
        }

        public BindingList<PriceCatalogDetailTimeValueViewModel> TimeValues
        {
            get; set;
        }


        IEnumerable<IPriceCatalogDetailTimeValue> IPriceCatalogDetail.TimeValues
        {
            get
            {
                return TimeValues;
            }
        }
    }
}
