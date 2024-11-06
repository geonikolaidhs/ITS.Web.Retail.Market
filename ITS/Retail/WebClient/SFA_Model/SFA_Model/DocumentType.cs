using ITS.Retail.Platform.Enumerations;
using SFA_Model.NonPersistant;
using ITS.WRM.Model.Interface;
using ITS.WRM.Model.Interface.Model.SupportingClasses;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ITS.Retail.Mobile.AuxilliaryClasses;
using ITS.WRM.Model.Interface.Model;
using DevExpress.Xpo;

namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 190, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class DocumentType : LookUp2Fields, ITS.WRM.Model.Interface.Model.SupportingClasses.IRequiredOwner, IDocumentType
    {
        public DocumentType() : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public DocumentType(Session session) : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public DocumentType(string code, string description) : base()
        {

        }

        public DocumentType(Session session, string code, string description)
            : base(session, code, description)
        {

        }
        public bool AcceptsGeneralItems { get; set; }

        public bool AllowItemZeroPrices { get; set; }
        public Guid DefaultDocumentStatus { get; set; }


        public Guid DefaultPaymentMethod { get; set; }


        public Guid DeficiencySettings { get; set; }


        public Guid Division { get; set; }

        public eDocTypeCustomerCategory DocTypeCustomerCategoryMode { get; set; }

        public bool DocumentHeaderCanBeCopied { get; set; }

        public eDocumentTypeItemCategory DocumentTypeItemCategoryMode { get; set; }

        public eDocumentTypeMeasurementUnit DocumentTypeProposedMeasurementUnit { get; set; }

        public string FormDescription { get; set; }

        public bool IsForWholesale { get; set; }

        public bool IsOfValues { get; set; }

        public bool IsPrintedOnStoreController { get; set; }

        public bool IsQuantitative { get; set; }

        public uint MaxCountOfLines { get; set; }

        public decimal MaxDetailQty { get; set; }

        public decimal MaxDetailTotal { get; set; }

        public decimal MaxDetailValue { get; set; }

        public decimal MaxPaymentAmount { get; set; }

        public bool MergedSameDocumentLines { get; set; }

        public Guid MinistryDocumentType { get; set; }

        public ePriceSuggestionType PriceSuggestionType { get; set; }

        public int QuantityFactor { get; set; }

        public Guid ReasonCategory { get; set; }

        public bool RecalculatePricesOnTraderChange { get; set; }

        public bool ReserveCoupons { get; set; }

        public bool ShouldResetMenu { get; set; }

        public Guid SpecialItem { get; set; }

        public bool SupportLoyalty { get; set; }

        public bool TakesDigitalSignature { get; set; }

        public eDocumentTraderType TraderType { get; set; }

        public eDocumentType Type { get; set; }

        public bool UsesMarkUp { get; set; }

        public bool UsesMarkUpForm { get; set; }

        public bool UsesPaymentMethods { get; set; }

        public bool UsesPrices { get; set; }

        public int ValueFactor { get; set; }
        public ItemStockAffectionOptions ItemStockAffectionOptions { get; set; }
        [NonPersistent]
        IDocumentStatus IDocumentType.DefaultDocumentStatus
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();

            }
        }
        [NonPersistent]
        IPaymentMethod IDocumentType.DefaultPaymentMethod
        {
            get
            {
                throw new NotImplementedException();

            }

            set
            {
                throw new NotImplementedException();

            }
        }
        [NonPersistent]
        IDeficiencySettings IDocumentType.DeficiencySettings
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        IDivision IDocumentType.Division
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        eDocumentType IDocumentType.DocumentTypeItemCategoryMode
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        [NonPersistent]
        IDocumentTypeMapping IDocumentType.DocumentTypeMapping
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        IMinistryDocumentType IDocumentType.MinistryDocumentType
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        IReasonCategory IDocumentType.ReasonCategory
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        ISpecialItem IDocumentType.SpecialItem
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
