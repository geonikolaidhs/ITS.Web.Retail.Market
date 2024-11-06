//-----------------------------------------------------------------------
// <copyright file="SystemSettings.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Model.Attributes;

namespace ITS.Retail.Model
{
    [Updater(Order = 220, Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER)]
    public class StoreControllerSettings : BaseObj
    {
        public StoreControllerSettings()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public StoreControllerSettings(Session session)
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

        // Fields...

        private DocumentStatus _DefaultDocumentStatus;
        private eCultureInfo _CultureInfo;
        private SpecialItem _DepositItem;
        private SpecialItem _WithdrawalItem;
        private int _MaximumNumberOfPOS;
        private DocumentType _WithdrawalDocumentType;
        private DocumentType _DepositDocumentType;
        private DocumentType _ProformaDocumentType;
        private DocumentType _ReceiptDocumentType;
        private string _ServerUrl;
        private Store _Store;
        private CompanyNew _Owner;
        private string _ServerName;
        private int _ID;
        private Customer _DefaultCustomer;
        private bool _POSSellsInactiveItems;
        private string _TransactionFilesFolder;
        private bool _IsOnline;
        private DocumentType _SpecialProformaDocumentType;

        [Indexed("Owner;GCRecord", Unique = true)]
        [DescriptionField]
        [DisplayOrder(Order = 1)]
        public int ID
        {
            get
            {
                return _ID;
            }
            set
            {
                SetPropertyValue("ID", ref _ID, value);
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

        public Store Store
        {
            get
            {
                return _Store;
            }
            set
            {
                SetPropertyValue("Store", ref _Store, value);
            }
        }

        [DisplayOrder(Order = 11)]
        public Customer DefaultCustomer
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

        [DisplayOrder(Order = 3)]
        public string ServerName
        {
            get
            {
                return _ServerName;
            }
            set
            {
                SetPropertyValue("ServerName", ref _ServerName, value);
            }
        }

        [DisplayOrder(Order = 2)]
        public string ServerUrl
        {
            get
            {
                return _ServerUrl;
            }
            set
            {
                SetPropertyValue("ServerUrl", ref _ServerUrl, value);
            }
        }

        [DisplayOrder(Order = 4)]
        public DocumentType ReceiptDocumentType
        {
            get
            {
                return _ReceiptDocumentType;
            }
            set
            {
                SetPropertyValue("ReceiptDocumentType", ref _ReceiptDocumentType, value);
            }
        }

        [DisplayOrder(Order = 5)]
        public DocumentType ProformaDocumentType
        {
            get
            {
                return _ProformaDocumentType;
            }
            set
            {
                SetPropertyValue("ProformaDocumentType", ref _ProformaDocumentType, value);
            }
        }

        [DisplayOrder(Order = 6)]
        public DocumentType DepositDocumentType
        {
            get
            {
                return _DepositDocumentType;
            }
            set
            {
                SetPropertyValue("DepositDocumentType", ref _DepositDocumentType, value);
            }
        }

        [DisplayOrder(Order = 7)]
        public DocumentType WithdrawalDocumentType
        {
            get
            {
                return _WithdrawalDocumentType;
            }
            set
            {
                SetPropertyValue("WithdrawalDocumentType", ref _WithdrawalDocumentType, value);
            }
        }

        [DisplayOrder(Order = 9)]
        public SpecialItem WithdrawalItem
        {
            get
            {
                return _WithdrawalItem;
            }
            set
            {
                SetPropertyValue("WithdrawalItem", ref _WithdrawalItem, value);
            }
        }

        [DisplayOrder(Order = 8)]
        public SpecialItem DepositItem
        {
            get
            {
                return _DepositItem;
            }
            set
            {
                SetPropertyValue("DepositItem", ref _DepositItem, value);
            }
        }

        [DisplayOrder(Order = 10)]
        public int MaximumNumberOfPOS
        {
            get
            {
                return _MaximumNumberOfPOS;
            }
            set
            {
                SetPropertyValue("MaximumNumberOfPOS", ref _MaximumNumberOfPOS, value);
            }
        }


        public eCultureInfo CultureInfo
        {
            get
            {
                return _CultureInfo;
            }
            set
            {
                SetPropertyValue("CultureInfo", ref _CultureInfo, value);
            }
        }

        [DisplayOrder(Order = 12)]
        public DocumentStatus DefaultDocumentStatus
        {
            get
            {
                return _DefaultDocumentStatus;
            }
            set
            {
                SetPropertyValue("DefaultDocumentStatus", ref _DefaultDocumentStatus, value);
            }
        }

        [DisplayOrder(Order = 13)]
        public bool POSSellsInactiveItems
        {
            get
            {
                return _POSSellsInactiveItems;
            }
            set
            {
                SetPropertyValue("POSSellsInactiveItems", ref _POSSellsInactiveItems, value);
            }
        }

        [Association("StoreControllerSettings-StoreControllerTerminaDeviceAssociations")]
        public XPCollection<StoreControllerTerminalDeviceAssociation> StoreControllerTerminalDeviceAssociations
        {
            get
            {
                return GetCollection<StoreControllerTerminalDeviceAssociation>("StoreControllerTerminalDeviceAssociations");
            }
        }

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew supplier, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    if (supplier == null || store == null)
                    {
                        throw new Exception("StoreControllerSettings.GetUpdaterCriteria(); Error: Supplier or Store is null");
                    }
                    crop = CriteriaOperator.Or(new BinaryOperator("Store.Oid", store.Oid));
                    break;
            }
            return crop;
        }


        [DisplayOrder(Order = 14)]
        public string TransactionFilesFolder
        {
            get
            {
                return _TransactionFilesFolder;
            }
            set
            {
                SetPropertyValue("TransactionFilesFolder", ref _TransactionFilesFolder, value);
            }
        }


        public bool IsOnline
        {
            get
            {
                return _IsOnline;
            }
            set
            {
                SetPropertyValue("IsOnline", ref _IsOnline, value);
            }
        }
                
        public DocumentType SpecialProformaDocumentType
        {
            get
            {
                return _SpecialProformaDocumentType;
            }
            set
            {
                SetPropertyValue("SpecialProformaDocumentType", ref _SpecialProformaDocumentType, value);
            }
        }
    }
}
