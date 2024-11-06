//-----------------------------------------------------------------------
// <copyright file="Company.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Model.Attributes;
using System.Linq;
using ITS.Retail.ResourcesLib;

namespace ITS.Retail.Model
{
    [DataViewParameter]
    [Updater(Order = 410, Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("Supplier", typeof(ResourcesLib.Resources))]
    public class SupplierNew : BaseObj, IOwner
    {
        public SupplierNew()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public SupplierNew(Session session)
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


        public string Description
        {
            get
            {
                return CompanyName;
            }
        }

        private Trader _Trader;
        [Association("Trader-Companies")]
        public Trader Trader
        {
            get
            {
                return _Trader;
            }
            set
            {
                SetPropertyValue("Trader", ref _Trader, value);
            }
        }

        private string _Code;
        [Indexed(Unique = true)]
        public string Code
        {
            get
            {
                return _Code;
            }
            set
            {
                SetPropertyValue("Code", ref _Code, value);
            }
        }

        private string _CompanyName;
        [DescriptionField]
        public string CompanyName
        {
            get
            {
                return _CompanyName;
            }
            set
            {
                SetPropertyValue("CompanyName", ref _CompanyName, value);
            }
        }
        private Address _DefaultAddress;
        public Address DefaultAddress
        {
            get
            {
                return _DefaultAddress;
            }
            set
            {
                SetPropertyValue("DefaultAddress", ref _DefaultAddress, value);
            }
        }
        private string _Profession;
        public string Profession
        {
            get
            {
                return _Profession;
            }
            set
            {
                SetPropertyValue("Profession", ref _Profession, value);
            }
        }
        private CompanyNew _Owner;
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

        private VatLevel _VatLevel;
        public VatLevel VatLevel
        {
            get
            {
                return _VatLevel;
            }
            set
            {
                SetPropertyValue("VatLevel", ref _VatLevel, value);
            }
        }

        [Association("Supplier-Items")]
        public XPCollection<Item> Items
        {
            get
            {
                return GetCollection<Item>("Items");
            }
        }

        [Association("Supplier-SupplierImportFilesSets")]
        public XPCollection<SupplierImportFilesSet> SupplierImportFilesSets
        {
            get
            {
                return GetCollection<SupplierImportFilesSet>("SupplierImportFilesSets");
            }
        }

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew owner, Store store, string deviceID)
        {
            //TO CHECK
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                case eUpdateDirection.STORECONTROLLER_TO_POS:
                    if (owner == null)
                    {
                        throw new Exception("Supplier.GetUpdaterCriteria(); Error: Owner is null");
                    }
                    crop = CriteriaOperator.Or(new BinaryOperator("Owner.Oid", owner.Oid), new NullOperator("Owner"));
                    break;
            }

            return crop;
        }

        public VatLevel GetVatLevel(Address address)
        {
            if (address != null
                && address.Trader == this.Trader
                && address.VatLevel != null
                )
            {
                return address.VatLevel;
            }
            return VatLevel;
        }

        protected override void OnSaving()
        {
            if (this.SkipOnSavingProcess)
            {
                base.OnSaving();
                return;
            }

            if (VatLevel == null)
            {
                VatLevel = this.Session.FindObject<VatLevel>(new BinaryOperator("IsDefault", true));
            }
            if (this.DefaultAddress == null && this.Trader.Addresses.Count > 0)
            {
                this.DefaultAddress = this.Trader.Addresses.OrderBy(adr => adr.AutomaticNumbering).FirstOrDefault();
            }
            base.OnSaving();
        }

        [Association("Supplier-DocumentHeaders")]
        public XPCollection<DocumentHeader> DocumentHeaders
        {
            get
            {
                return GetCollection<DocumentHeader>("DocumentHeaders");
            }
        }

        private DateTime? _GDPRAnonymizationDate;
        private DateTime? _GDPRExportDate;
        public DateTime? GDPRAnonymizationDate
        {
            get
            {
                return _GDPRAnonymizationDate;
            }
            set
            {
                SetPropertyValue("GDPRAnonymizationDate", ref _GDPRAnonymizationDate, value);
            }
        }
        public DateTime? GDPRExportDate
        {
            get
            {
                return _GDPRExportDate;
            }
            set
            {
                SetPropertyValue("GDPRExportDate", ref _GDPRExportDate, value);
            }
        }
        #region "GDPRComments"
        private string _GDPRComments;
        [DbType("varchar(500)")]
        public string GDPRComments
        {
            get { return _GDPRComments; }
            set { SetPropertyValue("GDPRComments", ref _GDPRComments, value); }
        }
        #endregion
        #region "GDPRExportUser"
        private User _GDPRExportUser;
        public User GDPRExportUser
        {
            get { return _GDPRExportUser; }
            set { SetPropertyValue("GDPRExportUser", ref _GDPRExportUser, value); }
        }
        #endregion
        #region "GDPRExportProtocolNumber"
        private int _GDPRExportProtocolNumber;
        public int GDPRExportProtocolNumber
        {
            get { return _GDPRExportProtocolNumber; }
            set { SetPropertyValue("GDPRExportProtocolNumber", ref _GDPRExportProtocolNumber, value); }
        }
        #endregion
        #region "GDPRAnonymizationUser"
        private User _GDPRAnonymizationUser;
        public User GDPRAnonymizationUser
        {
            get { return _GDPRAnonymizationUser; }
            set { SetPropertyValue("GDPRAnonymizationUser", ref _GDPRAnonymizationUser, value); }
        }
        #endregion
        #region "GDPRAnonymizationProtocolNumber"
        private int _GDPRAnonymizationProtocolNumber;
        public int GDPRAnonymizationProtocolNumber
        {
            get { return _GDPRAnonymizationProtocolNumber; }
            set { SetPropertyValue("GDPRAnonymizationProtocolNumber", ref _GDPRAnonymizationProtocolNumber, value); }
        }
        #endregion
    }
}