using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.WebClient.Helpers.AuxilliaryClasses
{

    public class Reevaluate
    {
        public const decimal INVALID_UNIT_PRICE = -1;
        public const decimal UNDEFINED_LAST_VALUE = -2;//for those cases where there is no previous value at all!!!

        public Guid Oid
        {
            get
            {
                return priceCatalogDetail.Oid;
            }
        }

        public PriceCatalogDetail priceCatalogDetail { get; set; }
        public DocumentHeader newDocumentHeader
        {
            get
            {
                return newDocumentDetail.DocumentHeader;
            }
        }
        public DocumentDetail newDocumentDetail { get; set; }
        public DocumentHeader documentHeaderOfLastValue
        {
            get
            {
                return documentDetailOfLastValue == null ? null : documentDetailOfLastValue.DocumentHeader;
            }
        }
        public DocumentDetail documentDetailOfLastValue { get; set; }

        public string lastValueStr
        {
            get
            {
                if (lastValue == UNDEFINED_LAST_VALUE)
                {
                    return "";
                }
                return lastValue.ToString();
            }
        }

        public decimal lastValue
        {
            get
            {
                return documentDetailOfLastValue == null ? UNDEFINED_LAST_VALUE : documentDetailOfLastValue.FinalUnitPrice;
            }
        }

        public decimal newValue
        {
            get
            {
                return newDocumentDetail == null ? INVALID_UNIT_PRICE : newDocumentDetail.FinalUnitPrice;
            }
        }

        public decimal ValueDifference
        {
            get
            {
                if (newValue == INVALID_UNIT_PRICE || lastValue == UNDEFINED_LAST_VALUE)
                {
                    return .0m;
                }
                return newValue - lastValue;
            }
        }

        public bool Selected { get; set; }

        private decimal _Markup, _UnitPrice;

        public decimal MarkUp
        {
            get
            {
                return _Markup;
            }
            set
            {
                _Markup = value;
                ComputeUnitPrice();
            }
        }
        public decimal UnitPrice
        {
            get
            {
                return _UnitPrice;
            }
            set
            {
                _UnitPrice = value;
                ComputeMarkup();
            }
        }

        protected decimal factor = 0;

        public bool UseMargin { get; private set; }

        public Reevaluate(PriceCatalogDetail priceCatalogDetailValue, DocumentDetail newDocumentDetailValue, DocumentDetail documentDetailOfLastValueValue)
        {
            this.priceCatalogDetail = priceCatalogDetailValue;
            this.newDocumentDetail = newDocumentDetailValue;
            this.documentDetailOfLastValue = documentDetailOfLastValueValue;
            this.Selected = true;
            UseMargin = newDocumentDetail.DocumentHeader.Owner.OwnerApplicationSettings.UseMarginInsteadMarkup;

            if (this.priceCatalogDetail != null)
            {
                if (priceCatalogDetail.VATIncluded)
                {
                    VatFactor vatFactor = priceCatalogDetail.Item.VatCategory.VatFactors.Where(vFactor => vFactor.VatLevel.IsDefault).FirstOrDefault();
                    if (vatFactor != null)
                    {
                        factor = vatFactor.Factor;
                    }
                }
                this.UnitPrice = priceCatalogDetail.Value;
                this.MarkUp = priceCatalogDetail.MarkUp;                
            }
            else
            {
                this.UnitPrice =
                this.MarkUp = .0m;
            }
        }

        private void ComputeUnitPrice()
        {
            decimal marginMarkupFactor = 1;
            if (this.UseMargin )
            {
                decimal denominator = 1.0m - this.MarkUp;
                if ( denominator != 0)
                {
                    marginMarkupFactor = 1.0m / denominator;
                }
            }
            else{
                marginMarkupFactor = (1.0m + this.MarkUp);
            }
            this._UnitPrice = this.newValue * (1 + factor) * marginMarkupFactor;
        }

        private void ComputeMarkup()
        {
            decimal UnitPriceWithoutVAT = this.UnitPrice / (1 + factor);
            decimal denominator = (this.UseMargin) ? UnitPriceWithoutVAT : this.newValue;
            _Markup = (denominator == 0) ? 0 : (UnitPriceWithoutVAT - this.newValue) / denominator;            
        }
    }
}