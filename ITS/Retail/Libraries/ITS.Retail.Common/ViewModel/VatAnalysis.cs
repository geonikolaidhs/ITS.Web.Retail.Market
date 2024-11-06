using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
namespace ITS.Retail.Common.ViewModel
{
    public class VatAnalysis: BasicViewModel
    {

        public decimal Qty
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


        public decimal NetTotal
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

        public decimal TotalVatAmount
        {
            get
            {
                return _TotalVatAmount;
            }
            set
            {
                SetPropertyValue("TotalVatAmount", ref _TotalVatAmount, value);
            }
        }


        public decimal GrossTotal
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

        public decimal VatFactor
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


        public Guid VatFactorGuid
        {
            get
            {
                return _VatFactorGuid;
            }
            set
            {
                SetPropertyValue("VatFactorGuid", ref _VatFactorGuid, value);
            }
        }

        // Fields...
        private Guid _VatFactorGuid;
        private decimal _GrossTotal;
        private decimal _TotalVatAmount;
        private decimal _NetTotal;
        private decimal _Qty;
        private decimal _VatFactor;
    }
}
