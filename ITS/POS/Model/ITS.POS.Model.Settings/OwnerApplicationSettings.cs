using System;
using DevExpress.Xpo;
using System.Drawing;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;

namespace ITS.POS.Model.Settings
{
    public class OwnerApplicationSettings : BaseObj, IOwnerApplicationSettings
    {
        public OwnerApplicationSettings()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public OwnerApplicationSettings(Session session)
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

        
        /// <summary>
        /// Private Attributes 
        /// </summary>



        public double MaxItemOrderQty
        {
            get
            {
                return _MaxItemOrderQty;
            }
            set
            {
                SetPropertyValue("MaxItemOrderQty", ref _MaxItemOrderQty, value);
            }
        }

        public double DisplayValueDigits
        {
            get
            {
                return _DisplayValueDigits;
            }
            set
            {
                SetPropertyValue("DisplayValueDigits", ref _DisplayValueDigits, value);
            }
        }


        public double ComputeValueDigits
        {
            get
            {
                return _ComputeValueDigits;
            }
            set
            {
                SetPropertyValue("ComputeValueDigits", ref _ComputeValueDigits, value);
            }
        }

        public int NewItemCurrentDateIndex
        {
            get
            {
                return _NewItemCurrentDateIndex;
            }
            set
            {
                SetPropertyValue("NewItemCurrentDateIndex", ref _NewItemCurrentDateIndex, value);
            }
        }

        public decimal RefundPoints
        {
            get
            {
                return _RefundPoints;
            }
            set
            {
                SetPropertyValue("RefundPoints", ref _RefundPoints, value);
            }
        }

        public decimal DiscountAmount
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

        public decimal DiscountPercentage
        {
            get
            {
                return _DiscountPercentage;
            }
            set
            {
                SetPropertyValue("DiscountPercentage", ref _DiscountPercentage, value);
            }
        }

        public bool SupportLoyalty
        {
            get
            {
                return _SupportLoyalty;
            }
            set
            {
                SetPropertyValue("SupportLoyalty", ref _SupportLoyalty, value);
            }
        }

        public bool DiscountPermited
        {
            get
            {
                return _DiscountPermited;
            }
            set
            {
                SetPropertyValue("DiscountPermited", ref _DiscountPermited, value);
            }
        }

        public bool RecomputePrices
        {
            get
            {
                return _RecomputePrices;
            }
            set
            {
                SetPropertyValue("RecomputePrices", ref _RecomputePrices, value);
            }
        }

        public bool POSCanSetPrices
        {
            get
            {
                return _POSCanSetPrices;
            }
            set
            {
                SetPropertyValue("POSCanSetPrices", ref _POSCanSetPrices, value);
            }
        }

        public bool POSCanChangePrices
        {
            get
            {
                return _POSCanChangePrices;
            }
            set
            {
                SetPropertyValue("POSCanChangePrices", ref _POSCanChangePrices, value);
            }
        }

        public Guid DefaultCustomer
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

        //public bool DoPadding
        //{
        //    get
        //    {
        //        return _DoPadding;
        //    }
        //    set
        //    {
        //        SetPropertyValue("DoPadding", ref _DoPadding, value);
        //    }
        //}

        public bool PadItemCodes
        {
            get
            {
                return _PadItemCodes;
            }
            set
            {
                SetPropertyValue("PadItemCodes", ref _PadItemCodes, value);
            }
        }

        public bool PadBarcodes
        {
            get
            {
                return _PadBarcodes;
            }
            set
            {
                SetPropertyValue("PadBarcodes", ref _PadBarcodes, value);
            }
        }

        public int BarcodeLength
        {
            get
            {
                return _BarcodeLenght;
            }
            set
            {
                SetPropertyValue("BarcodeLength", ref _BarcodeLenght, value);
            }
        }

        public int ItemCodeLength
        {
            get
            {
                return _ItemCodeLenght;
            }
            set
            {
                SetPropertyValue("ItemCodeLength", ref _ItemCodeLenght, value);
            }
        }
        public string BarcodePaddingCharacter
        {
            get
            {
                return _BarcodePaddingCharacter;
            }
            set
            {
                SetPropertyValue("BarcodePaddingCharacter", ref _BarcodePaddingCharacter, value);
            }
        }
        public string ItemCodePaddingCharacter
        {
            get
            {
                return _ItemCodePaddingCharacter;
            }
            set
            {
                SetPropertyValue("ItemCodePaddingCharacter", ref _ItemCodePaddingCharacter, value);
            }
        }
        public double ComputeDigits
        {
            get
            {
                return _ComputeDigits;
            }
            set
            {
                SetPropertyValue("ComputeDigits", ref _ComputeDigits, value);
            }
        }
        public double DisplayDigits
        {
            get
            {
                return _DisplayDigits;
            }
            set
            {
                SetPropertyValue("DisplayDigits", ref _DisplayDigits, value);
            }
        }
        [ValueConverter(typeof(DevExpress.Xpo.Metadata.ImageValueConverter))]
        public Image LogoSmall
        {
            get
            {
                return _LogoSmall;
            }
            set
            {
                SetPropertyValue<Image>("LogoSmall", ref _LogoSmall, value);
            }
        }
        [ValueConverter(typeof(DevExpress.Xpo.Metadata.ImageValueConverter))]
        public Image MenuLogo
        {
            get
            {
                return _MenuLogo;
            }
            set
            {
                SetPropertyValue<Image>("MenuLogo", ref _MenuLogo, value);
            }
        }
        [ValueConverter(typeof(DevExpress.Xpo.Metadata.ImageValueConverter))]
        public Image MainScreenImage
        {
            get
            {
                return _MainScreenImage;
            }
            set
            {
                SetPropertyValue<Image>("MainScreenImage", ref _MainScreenImage, value);
            }
        }
        public bool MergedSameDocumentLines
        {
            get
            {
                return _MergedSameDocumentLines;
            }
            set
            {
                SetPropertyValue("MergedSameDocumentLines", ref _MergedSameDocumentLines, value);
            }
        }

        public bool UseBarcodeRelationFactor
        {
            get
            {
                return _UseBarcodeRelationFactor;
            }
            set
            {
                SetPropertyValue("UseBarcodeRelationFactor", ref _UseBarcodeRelationFactor, value);
            }
        }


        public bool TrimBarcodeOnDisplay
        {
            get
            {
                return _TrimBarcodeOnDisplay;
            }
            set
            {
                SetPropertyValue("TrimBarcodeOnDisplay", ref _TrimBarcodeOnDisplay, value);
            }
        }

        public decimal MaximumAllowedDiscountPercentage
        {
            get
            {
                return _MaximumAllowedDiscountPercentage;
            }
            set
            {
                SetPropertyValue("MaximumAllowedDiscountPercentage", ref _MaximumAllowedDiscountPercentage, value);
            }
        }


        public Guid LoyaltyPaymentMethod
        {
            get
            {
                return _LoyaltyPaymentMethod;
            }
            set
            {
                SetPropertyValue("LoyaltyPaymentMethod", ref _LoyaltyPaymentMethod, value);
            }
        }

        [NonPersistent]
        public Guid LoyaltyPaymentMethodOid
        {
            get
            {
                return LoyaltyPaymentMethod;
            }
            set
            {
                LoyaltyPaymentMethod = value;
            }
        }


        public eLoyaltyRefundType LoyaltyRefundType
        {
            get
            {
                return _LoyaltyRefundType;
            }
            set
            {
                SetPropertyValue("LoyaltyRefundType", ref _LoyaltyRefundType, value);
            }
        }

        public ePromotionExecutionPriority PromotionExecutionPriority
        {
            get
            {
                return _PromotionExecutionPriority;
            }
            set
            {
                SetPropertyValue("PromotionExecutionPriority", ref _PromotionExecutionPriority, value);
            }
        }


        public bool LoyaltyOnDocumentSum
        {
            get
            {
                return _LoyaltyOnDocumentSum;
            }
            set
            {
                SetPropertyValue("LoyaltyOnDocumentSum", ref _LoyaltyOnDocumentSum, value);
            }
        }

        public decimal DocumentSumForLoyalty
        {
            get
            {
                return _DocumentSumForLoyalty;
            }
            set
            {
                SetPropertyValue("DocumentSumForLoyalty", ref _DocumentSumForLoyalty, value);
            }
        }

        public decimal LoyaltyPointsPerDocumentSum
        {
            get
            {
                return _LoyaltyPointsPerDocumentSum;
            }
            set
            {
                SetPropertyValue("LoyaltyPointsPerDocumentSum", ref _LoyaltyPointsPerDocumentSum, value);
            }
        }

        public bool OnlyRefundStore
        {
            get
            {
                return _OnlyRefundStore;
            }
            set
            {
                SetPropertyValue("OnlyRefundStore", ref _OnlyRefundStore, value);
            }
        }


        private decimal _MaximumAllowedDiscountPercentage;
        private bool _TrimBarcodeOnDisplay;
        private Guid _DefaultCustomer;
        private int _BarcodeLenght;
        private int _ItemCodeLenght;
        private string _BarcodePaddingCharacter;
        private string _ItemCodePaddingCharacter;
        private double _ComputeDigits;
        private double _DisplayDigits;
        private Image _LogoSmall;
        private Image _MainScreenImage;
        private bool _MergedSameDocumentLines;
        private bool _UseBarcodeRelationFactor;
        private Image _MenuLogo;
        private bool _RecomputePrices;
        private bool _DiscountPermited;
        private int _NewItemCurrentDateIndex;
        private double _MaxItemOrderQty;
        private double _DisplayValueDigits;
        private double _ComputeValueDigits;
        private bool _SupportLoyalty;
        private decimal _RefundPoints;
        private decimal _DiscountAmount;
        private decimal _DiscountPercentage;
        private decimal _LoyaltyPointsPerDocumentSum;
        private decimal _DocumentSumForLoyalty;
        //private Guid _LoyaltyDiscountType;
        private Guid _LoyaltyPaymentMethod;
        private eLoyaltyRefundType _LoyaltyRefundType;
        private bool _LoyaltyOnDocumentSum;
        private bool _OnlyRefundStore;
        private bool _PadItemCodes;
        private bool _PadBarcodes;
        private ePromotionExecutionPriority _PromotionExecutionPriority;
        private bool _POSCanSetPrices;
        private bool _POSCanChangePrices;
    }
}
