//-----------------------------------------------------------------------
// <copyright file="Address.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using System.Drawing;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Model.Attributes;
using ITS.Retail.Platform.Kernel.Model;

namespace ITS.Retail.Model
{
    [Updater(Order = 60,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("OwnerApplicationSettings", typeof(ResourcesLib.Resources))]

    public class OwnerApplicationSettings : BaseObj, IOwner, IOwnerApplicationSettings
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

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew owner, Store store, string deviceID)
        {
            //TO CHECK
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    if (owner == null)
                    {
                        throw new Exception("OwnerApplicationSettings.GetUpdaterCriteria(); Error: Owner is null");
                    }
                    crop = CriteriaOperator.Or(new BinaryOperator("Owner.Oid", owner.Oid), new NullOperator("Owner"));
                    break;
                case eUpdateDirection.STORECONTROLLER_TO_POS:
                    if (owner == null)
                    {
                        throw new Exception("OwnerApplicationSettings.GetUpdaterCriteria(); Error: Owner is null");
                    }
                    crop = new BinaryOperator("Owner.Oid", owner.Oid);
                    break;
            }

            return crop;
        }


        //Fields
        private Guid _PointsDocumentSeriesOid;
        private Guid _PointsDocumentTypeOid;
        private string _Fonts;
        private PayPalMode _PayPalMode;
        private string _PayPalEmail;
        private string _B2CProductsShipping;
        private string _B2CTransactionsSafety;
        private string _B2CCompany;
        private string _B2CUsefullInfo;
        private string _B2CFAQ;
        private string _MetaDescription;
        private string _GoogleAnalyticsID;
        private string _SmtpHost;
        private string _SmtpPort;
        private string _SmtpUsername;
        private string _SmtpPassword;
        private string _SmtpDomain;
        private string _SmtpEmailAddress;
        private bool _SmtpUseSSL;
        private string _EmailTemplateColor1;
        private string _EmailTemplateColor2;
        private eLoyaltyRefundType _LoyaltyRefundType;
        private decimal _MaximumAllowedDiscountPercentage;
        private string _FAX;
        private bool _OnlyRefundStore;
        private string _Phone;
        private string _LocationGoogleID;
        private string _LinkedInAccount;
        private string _FacebookAccount;
        private string _TwitterAccount;
        private string _Webpage;
        private string _EMail;
        private bool _TrimBarcodeOnDisplay;
        private CompanyNew _Owner;
        private double _DisplayValueDigits;
        private double _ComputeValueDigits;
        private int _BarcodeLength;
        private int _ItemCodeLength;
        private string _BarcodePaddingCharacter;
        private string _ItemCodePaddingCharacter;
        private double _ComputeDigits;
        private double _DisplayDigits;
        private Guid _OwnerImageOid;        
        private bool _RecomputePrices;
        private bool _DiscountPermited;
        private string _ApplicationTerms;
        private bool _AllowPriceCatalogSelection;
        private decimal _RefundPoints;
        private decimal _DiscountAmount;
        private decimal _DiscountPercentage;
        private bool _SupportLoyalty;
        private bool _UseBarcodeRelationFactor;
        private bool _ApplyDocumentDiscountToLinesWithoutDiscount;
        private bool _LoyaltyOnDocumentSum;
        private decimal _DocumentSumForLoyalty;
        private decimal _LoyaltyPointsPerDocumentSum;
        private Guid _LoyaltyPaymentMethodOid;
        private bool _EnablePurchases;
        private bool _PadItemCodes;
        private bool _PadBarcodes;
        private ePromotionExecutionPriority _PromotionExecutionPriority;
        private Guid _PointsDocumentStatusOid;
        private decimal _MarkupDefaultValueDifference;
        private bool _UseMarginInsteadMarkup;


        [DescriptionField]
        public String Description
        {
            get
            {
                return Owner.CompanyName + " Settings";
            }
        }

        public CompanyNew Owner
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
                return _BarcodeLength;
            }
            set
            {
                SetPropertyValue("BarcodeLenght", ref _BarcodeLength, value);
            }
        }

        public int ItemCodeLength
        {
            get
            {
                return _ItemCodeLength;
            }
            set
            {
                SetPropertyValue("ItemCodeLenght", ref _ItemCodeLength, value);
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

        public Guid OwnerImageOid
        {
            get
            {
                return _OwnerImageOid;
            }
            set
            {
                SetPropertyValue("OwnerImageOid", ref _OwnerImageOid, value);
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


        [Size(SizeAttribute.Unlimited)]
        public string ApplicationTerms
        {
            get
            {
                return _ApplicationTerms;
            }
            set
            {
                SetPropertyValue("ApplicationTerms", ref _ApplicationTerms, value);
            }
        }       

        public String formatCurrencyString
        {
            get
            {
                return "c" + DisplayDigits;
            }
        }

        public String formatItemValueString
        {
            get
            {
                return "c" + DisplayValueDigits;
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

        public bool AllowPriceCatalogSelection
        {
            get
            {
                return _AllowPriceCatalogSelection;
            }
            set
            {
                SetPropertyValue("AllowPriceCatalogSelection", ref _AllowPriceCatalogSelection, value);
            }
        }

        /// <summary>
        /// Gets or sets a flag that if set to true then a document discount will not apportion itself at lines that have a discount
        /// </summary>
        public bool ApplyDocumentDiscountToLinesWithoutDiscount
        {
            get
            {
                return _ApplyDocumentDiscountToLinesWithoutDiscount;
            }
            set
            {
                SetPropertyValue("ApplyDocumentDiscountToLinesWithoutDiscount", ref _ApplyDocumentDiscountToLinesWithoutDiscount, value);
            }
        }

        public Address DefaultAddress
        {
            get
            {
                return Owner.DefaultAddress;
            }
        }

        public string eMail
        {
            get
            {
                return _EMail;
            }
            set
            {
                SetPropertyValue("eMail", ref _EMail, value);
            }
        }


        public string Phone
        {
            get
            {
                return _Phone;
            }
            set
            {
                SetPropertyValue("Phone", ref _Phone, value);
            }
        }


        public string FAX
        {
            get
            {
                return _FAX;
            }
            set
            {
                SetPropertyValue("FAX", ref _FAX, value);
            }
        }


        public string Webpage
        {
            get
            {
                return _Webpage;
            }
            set
            {
                SetPropertyValue("Webpage", ref _Webpage, value);
            }
        }


        public string TwitterAccount
        {
            get
            {
                return _TwitterAccount;
            }
            set
            {
                SetPropertyValue("TwitterAccount", ref _TwitterAccount, value);
            }
        }


        public string FacebookAccount
        {
            get
            {
                return _FacebookAccount;
            }
            set
            {
                SetPropertyValue("FacebookAccount", ref _FacebookAccount, value);
            }
        }


        public string LinkedInAccount
        {
            get
            {
                return _LinkedInAccount;
            }
            set
            {
                SetPropertyValue("LinkedInAccount", ref _LinkedInAccount, value);
            }
        }

        [Size(SizeAttribute.Unlimited)]
        public string LocationGoogleID
        {
            get
            {
                return _LocationGoogleID;
            }
            set
            {
                SetPropertyValue("LocationGoogleID", ref _LocationGoogleID, value);
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


        public Guid LoyaltyPaymentMethodOid
        {
            get
            {
                return _LoyaltyPaymentMethodOid;
            }
            set
            {
                SetPropertyValue("LoyaltyPaymentMethodOid", ref _LoyaltyPaymentMethodOid, value);
            }
        }

        [NonPersistent]
        [UpdaterIgnoreField]
        public PaymentMethod LoyaltyPaymentMethod
        {
            get
            {
                return this.Session.FindObject<PaymentMethod>(PersistentCriteriaEvaluationBehavior.InTransaction, new BinaryOperator("Oid", this.LoyaltyPaymentMethodOid));
            }
            set
            {
                this.LoyaltyPaymentMethodOid = value.Oid;
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

        public bool EnablePurchases
        {
            get
            {
                return _EnablePurchases;
            }
            set
            {
                SetPropertyValue("EnablePurchases", ref _EnablePurchases, value);
            }
        }

        public string SmtpHost
        {
            get
            {
                return _SmtpHost;
            }
            set
            {
                SetPropertyValue("SmtpHost", ref _SmtpHost, value);
            }
        }


        public string SmtpPort
        {
            get
            {
                return _SmtpPort;
            }
            set
            {
                SetPropertyValue("SmtpPort", ref _SmtpPort, value);
            }
        }

        public string SmtpUsername
        {
            get
            {
                return _SmtpUsername;
            }
            set
            {
                SetPropertyValue("SmtpUsername", ref _SmtpUsername, value);
            }
        }

        public string SmtpPassword
        {
            get
            {
                return _SmtpPassword;
            }
            set
            {
                SetPropertyValue("SmtpPassword", ref _SmtpPassword, value);
            }
        }

        public string SmtpDomain
        {
            get
            {
                return _SmtpDomain;
            }
            set
            {
                SetPropertyValue("SmtpDomain", ref _SmtpDomain, value);
            }
        }

        public string SmtpEmailAddress
        {
            get
            {
                return _SmtpEmailAddress;
            }
            set
            {
                SetPropertyValue("SmtpEmailAddress", ref _SmtpEmailAddress, value);
            }
        }

        public bool SmtpUseSSL
        {
            get
            {
                return _SmtpUseSSL;
            }
            set
            {
                SetPropertyValue("SmtpUseSSL", ref _SmtpUseSSL, value);
            }
        }

        public string EmailTemplateColor1
        {
            get
            {
                return _EmailTemplateColor1;
            }
            set
            {
                SetPropertyValue("EmailTemplateColor1", ref _EmailTemplateColor1, value);
            }
        }

        public string EmailTemplateColor2
        {
            get
            {
                return _EmailTemplateColor2;
            }
            set
            {
                SetPropertyValue("EmailTemplateColor2", ref _EmailTemplateColor2, value);
            }
        }

        [Persistent("B2CPriceCatalog")]
        public Guid? B2CPriceCatalogOid
        {
            get
            {
                return _B2CPriceCatalogOid;
            }
            set
            {
                SetPropertyValue("B2CPriceCatalogOid", ref _B2CPriceCatalogOid, value);
            }
        }

        [NonPersistent]
        [UpdaterIgnoreField]
        public PriceCatalog B2CPriceCatalog
        {
            get
            {
                return this.Session.FindObject<PriceCatalog>(PersistentCriteriaEvaluationBehavior.InTransaction, new BinaryOperator("Oid", this.B2CPriceCatalogOid));
            }
            set
            {
                if (value == null)
                {
                    this.B2CPriceCatalogOid = null;
                }
                else
                {
                    this.B2CPriceCatalogOid = value.Oid;
                }
            }
        }
        
        public string GoogleAnalyticsID
        {
            get
            {
                return _GoogleAnalyticsID;
            }
            set
            {
                SetPropertyValue("GoogleAnalyticsID", ref _GoogleAnalyticsID, value);
            }
        }


        [Size(SizeAttribute.Unlimited)]
        public string MetaDescription
        {
            get
            {
                return _MetaDescription;
            }
            set
            {
                SetPropertyValue("MetaDescription", ref _MetaDescription, value);
            }
        }


        [Persistent("B2CDocumentType")]
        public Guid? B2CDocumentTypeOid
        {
            get
            {
                return _B2CDocumentTypeOid;
            }
            set
            {
                SetPropertyValue("B2CDocumentTypeOid", ref _B2CDocumentTypeOid, value);
            }
        }

        [NonPersistent]
        [UpdaterIgnoreField]
        public DocumentType B2CDocumentType
        {
            get
            {
                return this.Session.FindObject<DocumentType>(PersistentCriteriaEvaluationBehavior.InTransaction, new BinaryOperator("Oid", this.B2CDocumentTypeOid));        
            }
            set
            {
                if (value == null)
                {
                    this.B2CDocumentTypeOid = null;
                }
                else
                {
                    this.B2CDocumentTypeOid = value.Oid;
                }
            }
        }


        [Persistent("B2CDocumentSeries")]
        public Guid? B2CDocumentSeriesOid
        {
            get
            {
                return _B2CDocumentSeriesOid;
            }
            set
            {
                SetPropertyValue("B2CDocumentSeriesOid", ref _B2CDocumentSeriesOid, value);
            }
        }

        [NonPersistent]
        [UpdaterIgnoreField]
        public DocumentSeries B2CDocumentSeries
        {
            get
            {
                return this.Session.FindObject<DocumentSeries>(PersistentCriteriaEvaluationBehavior.InTransaction, new BinaryOperator("Oid", this.B2CDocumentSeriesOid));
            }
            set
            {
                if (value == null)
                {
                    this.B2CDocumentSeriesOid = null;
                }
                else
                {
                    this.B2CDocumentSeriesOid = value.Oid;
                }
            }
        }


        [Persistent("B2CStore")]
        public Guid? B2CStoreOid
        {
            get
            {
                return _B2CStoreOid;
            }
            set
            {
                SetPropertyValue("B2CStoreOid", ref _B2CStoreOid, value);
            }
        }

        [NonPersistent]
        [UpdaterIgnoreField]
        public Store B2CStore
        {
            get
            {
                return this.Session.FindObject<Store>(PersistentCriteriaEvaluationBehavior.InTransaction, new BinaryOperator("Oid", this.B2CStoreOid));
            }
            set
            {
                if (value == null)
                {
                    this.B2CStoreOid = null;
                }
                else
                {
                    this.B2CStoreOid = value.Oid;
                }
            }
        }

        [Size(SizeAttribute.Unlimited)]
        public string B2CProductsShipping
        {
            get
            {
                return _B2CProductsShipping;
            }
            set
            {
                SetPropertyValue("B2CProductsShipping", ref _B2CProductsShipping, value);
            }
        }

        [Size(SizeAttribute.Unlimited)]
        public string B2CTransactionsSafety
        {
            get
            {
                return _B2CTransactionsSafety;
            }
            set
            {
                SetPropertyValue("B2CTransactionsSafety", ref _B2CTransactionsSafety, value);
            }
        }

        [Size(SizeAttribute.Unlimited)]
        public string B2CCompany
        {
            get
            {
                return _B2CCompany;
            }
            set
            {
                SetPropertyValue("B2CCompany", ref _B2CCompany, value);
            }
        }

        [Size(SizeAttribute.Unlimited)]
        public string B2CUsefullInfo
        {
            get
            {
                return _B2CUsefullInfo;
            }
            set
            {
                SetPropertyValue("B2CUsefullInfo", ref _B2CUsefullInfo, value);
            }
        }
        [Size(SizeAttribute.Unlimited)]
        public string B2CFAQ
        {
            get
            {
                return _B2CFAQ;
            }
            set
            {
                SetPropertyValue("B2CFAQ", ref _B2CFAQ, value);
            }
        }

        public string PayPalEmail
        {
            get
            {
                return _PayPalEmail;
            }
            set
            {
                SetPropertyValue("PayPalEmail", ref _PayPalEmail, value);
            }
        }

        public PayPalMode PayPalMode
        {
            get
            {
                return _PayPalMode;
            }
            set
            {
                SetPropertyValue("PayPalMode", ref _PayPalMode, value);
            }
        }

        private Guid? _CashOnDeliveryOid;
        [Persistent("CashOnDelivery")]
        public Guid? CashOnDeliveryOid
        {
            get
            {
                return _CashOnDeliveryOid;
            }
            set
            {
                SetPropertyValue("CashOnDeliveryOid", ref _CashOnDeliveryOid, value);
            }
        }

        [NonPersistent]
        [UpdaterIgnoreField]
        public PaymentMethod CashOnDelivery
        {
            get
            {
                return this.Session.FindObject<PaymentMethod>(PersistentCriteriaEvaluationBehavior.InTransaction, new BinaryOperator("Oid", this.CashOnDeliveryOid));
            }
            set
            {
                if (value == null)
                {
                    this.CashOnDeliveryOid = null;
                }
                else
                {
                    this.CashOnDeliveryOid = value.Oid;
                }
            }
        }

        private Guid? _BankDepositOid;
        [Persistent("BankDeposit")]
        public Guid? BankDepositOid
        {
            get
            {
                return _BankDepositOid;
            }
            set
            {
                SetPropertyValue("BankDepositOid", ref _BankDepositOid, value);
            }
        }

        [NonPersistent]
        [UpdaterIgnoreField]
        public PaymentMethod BankDeposit
        {
            get
            {
                return this.Session.FindObject<PaymentMethod>(PersistentCriteriaEvaluationBehavior.InTransaction, new BinaryOperator("Oid", this.BankDepositOid));
            }
            set
            {
                if (value == null)
                {
                    this.BankDepositOid = null;
                }
                else
                {
                    this.BankDepositOid = value.Oid;
                }
            }
        }

        private Guid? _PayPalOid;
        private Guid? _B2CPriceCatalogOid;
        private Guid? _B2CDocumentTypeOid;
        private Guid? _B2CDocumentSeriesOid;
        private Guid? _B2CStoreOid;
        private Guid? _B2CDefaultCustomerOid;
        private int _NumberOfDaysDocumentCanBeCanceled;
        private decimal _PointCost;
        private string _AutoDeliveriesCode;
        private bool _POSCanChangePrices;
        private bool _POSCanSetPrices;
        private int _QuantityNumberOfDecimalDigits;
        private int _QuantityNumberOfIntegralDigits;

        [Persistent("PayPal")]
        public Guid? PayPalOid
        {
            get
            {
                return _PayPalOid;
            }
            set
            {
                SetPropertyValue("PayPalOid", ref _PayPalOid, value);
            }
        }

        [NonPersistent]
        [UpdaterIgnoreField]
        public PaymentMethod PayPal
        {
            get
            {
                return this.Session.FindObject<PaymentMethod>(PersistentCriteriaEvaluationBehavior.InTransaction, new BinaryOperator("Oid", this.PayPalOid));
            }
            set
            {
                if (value == null)
                {
                    this.PayPalOid = null;
                }
                else
                {
                    this.PayPalOid = value.Oid;
                }
            }
        }

        public string Fonts
        {
            get
            {
                return _Fonts;
            }
            set
            {
                SetPropertyValue("Fonts", ref _Fonts, value);
            }
        }

        public decimal PointCost
        {
            get
            {
                return _PointCost;
            }
            set
            {
                SetPropertyValue("PointCost", ref _PointCost, value);
            }
        }


        [Persistent("B2CDefaultCustomer")]
        public Guid? B2CDefaultCustomerOid
        {
            get
            {
                return _B2CDefaultCustomerOid;
            }
            set
            {
                SetPropertyValue("B2CDefaultCustomerOid", ref _B2CDefaultCustomerOid, value);
            }
        }

        [NonPersistent]
        [UpdaterIgnoreField]
        public Customer B2CDefaultCustomer
        {
            get
            {
                return this.Session.FindObject<Customer>(PersistentCriteriaEvaluationBehavior.InTransaction, new BinaryOperator("Oid", this.B2CDefaultCustomerOid));
            }
            set
            {
                if (value == null)
                {
                    this.B2CDefaultCustomerOid = null;
                }
                else
                {
                    this.B2CDefaultCustomerOid = value.Oid;
                }
            }
        }


        public int NumberOfDaysDocumentCanBeCanceled
        {
            get
            {
                //Only for rolling out, so that the default value 0
                //will not cause Document Cancel to be Aborted...
                if(_NumberOfDaysDocumentCanBeCanceled == 0)
                {
                    return int.MaxValue;
                }

                return _NumberOfDaysDocumentCanBeCanceled;
            }
            set
            {
                SetPropertyValue("NumberOfDaysDocumentCanBeCanceled", ref _NumberOfDaysDocumentCanBeCanceled, value);
            }
        }

        public String AutoDeliveriesCode
        {
            get
            {
                return _AutoDeliveriesCode;
            }
            set
            {
                SetPropertyValue("AutoDeliveriesCode", ref _AutoDeliveriesCode, value);
            }
        }



        public Guid PointsDocumentStatusOid
        {
            get
            {
                return _PointsDocumentStatusOid;
            }
            set
            {
                SetPropertyValue("PointsDocumentStatusOid", ref _PointsDocumentStatusOid, value);
            }
        }
        [NonPersistent]
        [UpdaterIgnoreField]
        public DocumentStatus PointsDocumentStatus
        {
            get
            {
                return this.Session.FindObject<DocumentStatus>(PersistentCriteriaEvaluationBehavior.InTransaction, new BinaryOperator("Oid", this.PointsDocumentStatusOid));
            }
        }

        public Guid PointsDocumentTypeOid
        {
            get
            {
                return _PointsDocumentTypeOid;
            }
            set
            {
                SetPropertyValue("PointsDocumentTypeOid", ref _PointsDocumentTypeOid, value);
            }
        }


        public decimal MarkupDefaultValueDifference
        {
            get
            {
                return _MarkupDefaultValueDifference;
            }
            set
            {
                SetPropertyValue("MarkupDefaultValueDifference", ref _MarkupDefaultValueDifference, value);
            }
        }


        public bool UseMarginInsteadMarkup
        {
            get
            {
                return _UseMarginInsteadMarkup;
            }
            set
            {
                SetPropertyValue("UseMarginInsteadMarkup", ref _UseMarginInsteadMarkup, value);
            }
        }
                    
        

        [NonPersistent]
        [UpdaterIgnoreField]
        public DocumentType PointsDocumentType
        {
            get
            {
                return this.Session.FindObject<DocumentType>(PersistentCriteriaEvaluationBehavior.InTransaction, new BinaryOperator("Oid", this.PointsDocumentTypeOid));
            }
        }


        public Guid PointsDocumentSeriesOid
        {
            get
            {
                return _PointsDocumentSeriesOid;
            }
            set
            {
                SetPropertyValue("PointsDocumentSeriesOid", ref _PointsDocumentSeriesOid, value);
            }
        }

        public int QuantityNumberOfDecimalDigits
        {
            get
            {
                return _QuantityNumberOfDecimalDigits;
            }
            set
            {
                SetPropertyValue("QuantityNumberOfDecimalDigits", ref _QuantityNumberOfDecimalDigits, value);
            }
        }

        public int QuantityNumberOfIntegralDigits
        {
            get
            {
                return _QuantityNumberOfIntegralDigits;
            }
            set
            {
                SetPropertyValue("QuantityNumberOfIntegralDigits", ref _QuantityNumberOfIntegralDigits, value);
            }
        }

        [NonPersistent]
        [UpdaterIgnoreField]
        public DocumentSeries PointsDocumentSeries
        {
            get
            {
                return this.Session.FindObject<DocumentSeries>(PersistentCriteriaEvaluationBehavior.InTransaction, new BinaryOperator("Oid", this.PointsDocumentSeriesOid));
            }
        }

        #region "CustomerExportProtocolReport"
        private CustomReport _CustomerExportProtocolReport;
        public CustomReport CustomerExportProtocolReport
        {
            get { return _CustomerExportProtocolReport; }
            set { SetPropertyValue("CustomerExportProtocolReport", ref _CustomerExportProtocolReport, value); }
        }
        #endregion
        #region "CustomerAnonymizationProtocoReport"
        private CustomReport _CustomerAnonymizationProtocolReport;
        public CustomReport CustomerAnonymizationProtocolReport
        {
            get { return _CustomerAnonymizationProtocolReport; }
            set { SetPropertyValue("CustomerAnonymizationProtocolReport", ref _CustomerAnonymizationProtocolReport, value); }
        }
        #endregion
        #region "SupplierExportProtocolReport"
        private CustomReport _SupplierExportProtocolReport;
        public CustomReport SupplierExportProtocolReport
        {
            get { return _SupplierExportProtocolReport; }
            set { SetPropertyValue("SupplierExportProtocolReport", ref _SupplierExportProtocolReport, value); }
        }
        #endregion
        #region "SupplierAnonymizationProtocolReport"
        private CustomReport _SupplierAnonymizationProtocolReport;
        public CustomReport SupplierAnonymizationProtocolReport
        {
            get { return _SupplierAnonymizationProtocolReport; }
            set { SetPropertyValue("SupplierAnonymizationProtocolReport", ref _SupplierAnonymizationProtocolReport, value); }
        }
        #endregion
    }
}
