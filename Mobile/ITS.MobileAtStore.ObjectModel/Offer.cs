using System;
using System.Collections.Generic;
using System.Text;
#if WindowsCE
using ITS.MobileAtStore.ObjectModel.Attributes;
#endif
namespace ITS.MobileAtStore.ObjectModel
{
    /// <summary>
    /// The class Action represents an offer which may be related to multiple products. An action has a description and a set of products
    /// </summary>
    public class Offer
    {
        private string _description = string.Empty;
        private int _offerId;
        private int _operationId;
        private int _aa; // incremental number used in aspxgridview
        private bool _onlyValidForMembers;
        private bool _isSingleItemDiscount;
        private decimal _discountPercent;
        private decimal _finalPrice = -1;
        private string _loyaltyText = string.Empty;

        /// <summary>
        /// Gets or sets the description of this action ie "Buy 2 katana get 1 for free"
        /// </summary>
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }

        /// <summary>
        /// Gets the processed description so it will be shown in the offer grid.
        /// </summary>
        private string _descriptionProcessed = string.Empty;
        /// <summary>
        /// 
        /// </summary>
#if WindowsCE
        [IgnoreFieldFromView]
#endif
        public string DescriptionProcessed
        {
            get
            {
                return _descriptionProcessed;
            }
            set
            {
                _descriptionProcessed = value;
            }
        }

        /// <summary>
        /// Gets or sets the final price that this offer of type discount sets. Returns -1 if its not set or its not valid.
        /// </summary>
        public decimal FinalPrice
        {
            get
            {
                return _finalPrice;
            }
            set
            {
                _finalPrice = value;
            }
        }

        /// <summary>
        /// Gets or sets whether or not this offer is a single item discount
        /// </summary>
        public bool IsSingleItemDiscount
        {
            get
            {
                return _isSingleItemDiscount;
            }
            set
            {
                _isSingleItemDiscount = value;
            }
        }

        /// <summary>
        /// Gets or sets the discount percent of this offer
        /// </summary>
        public decimal DiscountPercent
        {
            get
            {
                return _discountPercent;
            }
            set
            {
                _discountPercent = value;
            }
        }

        /// <summary>
        /// Gets or sets the target group of this offer. If true, then its only for loyalty members, otherwise for all.
        /// </summary>
        public bool ValidForMembers
        {
            get
            {
                return _onlyValidForMembers;
            }
            set
            {
                _onlyValidForMembers = value;
            }
        }
        /// <summary>
        /// Gets or sets the identification of this action in the data layer
        /// </summary>
        public int OfferId
        {
            get
            {
                return _offerId;
            }
            set
            {
                _offerId = value;
            }
        }

        /// <summary>
        /// Gets or sets the operation id or otherwise the grouping of offers that this offer belongs.
        /// </summary>
        public int OperationId
        {
            get
            {
                return _operationId;
            }
            set
            {
                _operationId = value;
            }
        }

        /// <summary>
        /// Gets or sets the incremental number for this object which is used as a key in aspxgridview
        /// </summary>
        public int AA
        {
            get
            {
                return _aa;
            }
            set
            {
                _aa = value;
            }
        }

        /// <summary>
        /// The text of the loyalty for the default offers
        /// </summary>
        public string LoyaltyText
        {
            get
            {
                return _loyaltyText;
            }
            set
            {
                _loyaltyText = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _descriptionProcessed;
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetProcessedDescription()
        {
            _descriptionProcessed = _description + (FinalPrice <= 0 ? "" : " ΤΕΛΙΚΗ ΤΙΜΗ : " + FinalPrice.ToString("0.00").Replace('.', ',') + "€");
            _descriptionProcessed = ReplaceDestroyedCharWithEURO(_descriptionProcessed);
        }

        private string ReplaceDestroyedCharWithEURO(string p)
        {
            if (string.IsNullOrEmpty(p))
            {
                return p;
            }
            char c = (char)128;//euro symbol in this encoding.
            p = p.Replace(c, '€');
            return p;
        }
    }
}
