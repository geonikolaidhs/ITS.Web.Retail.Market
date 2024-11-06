//-----------------------------------------------------------------------
// <copyright file="Store.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Model.Attributes;
using ITS.Retail.ResourcesLib;
using ITS.Retail.Platform.Kernel.Model;

namespace ITS.Retail.Model
{
    [DataViewParameter]
    [Updater(Order = 50, Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("Store", typeof(Resources))]

    public class Store : BaseObj, IRequiredOwner, IStore
    {

        private string _Name;
        private Guid? _CentralOid;
        private CompanyNew _Owner;
        private Guid _ImageOid;
        private PriceCatalogPolicy _DefaultPriceCatalogPolicy;
        private Address _Address;
        private bool _IsCentralStore;
        //private PriceCatalog _DefaultPriceCatalog;
        private string _Code;
        private Guid _ReferenceCompanyOid;
        //private ItemExtraInfo _ItemExtraInfo;
        public Store() : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Store(Session session) : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
        }


        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew supplier, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    if (store == null)
                    {
                        throw new Exception("Store.GetUpdaterCriteria(); Error: Store is null");
                    }
                    crop = CriteriaOperator.And(new BinaryOperator("Owner.Oid", supplier.Oid));
                    break;
            }
            return crop;
        }

        [DescriptionField]
        [DisplayOrder(Order = 3)]
        public string Description
        {
            get
            {
                return Name;
            }
        }


        [Indexed("Owner;GCRecord", Unique = true)]
        [DisplayOrder(Order = 1)]
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


        [DisplayOrder(Order = 2)]
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                SetPropertyValue("Name", ref _Name, value);
            }
        }

        [DisplayOrder(Order = 7)]
        public bool IsCentralStore
        {
            get
            {
                return _IsCentralStore;
            }
            set
            {
                SetPropertyValue("IsCentralStore", ref _IsCentralStore, value);
            }
        }


        [Persistent("Central")]
        public Guid? CentralOid
        {
            get
            {
                return _CentralOid;
            }
            set
            {
                SetPropertyValue("CentralOid", ref _CentralOid, value);
            }
        }


        [NonPersistent]
        [UpdaterIgnoreField]
        [DisplayOrder(Order = 5)]
        public Store Central
        {
            get
            {
                return this.Session.FindObject<Store>(PersistentCriteriaEvaluationBehavior.InTransaction, new BinaryOperator("Oid", CentralOid));
            }
            set
            {
                if (value == null)
                    CentralOid = null;
                else
                    CentralOid = value.Oid;
            }
        }



        [DisplayOrder(Order = 4)]
        public Address Address
        {
            get
            {
                return _Address;
            }
            set
            {
                if (_Address == value)
                    return;

                Address prevOwner = _Address;
                _Address = value;

                if (IsLoading) return;

                if (prevOwner != null && prevOwner.Store == this)
                    prevOwner.Store = null;

                if (_Address != null)
                    _Address.Store = this;
                OnChanged("Address");
            }
        }

        [Association("Store-DocumentSeries")]
        public XPCollection<DocumentSeries> DocumentSeries
        {
            get
            {
                return GetCollection<DocumentSeries>("DocumentSeries");
            }
        }

        [Association("Store-StoreBarcodeTypes")]
        public XPCollection<StoreBarcodeType> BarcodeTypes
        {
            get
            {
                return GetCollection<StoreBarcodeType>("BarcodeTypes");
            }
        }



        [Association("Owner-Stores")]
        [Indexed("GCRecord", Unique = false)]
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

        /* [DisplayOrder(Order = 7)]//TODO Migrate field
         public PriceCatalog DefaultPriceCatalog
         {
             get
             {
                 return _DefaultPriceCatalog;
             }
             set
             {
                 SetPropertyValue("DefaultPriceCatalog", ref _DefaultPriceCatalog, value);
             }
         }*/

        public XPCollection<Customer> Customers
        {
            get
            {
                return new XPCollection<Customer>(PersistentCriteriaEvaluationBehavior.BeforeTransaction, this.Session, new ContainsOperator("CustomerStorePriceLists", new BinaryOperator("StorePriceList.Store.Oid", this.Oid)));
            }
        }

        [Association("Store-DocumentHeaders")]
        public XPCollection<DocumentHeader> DocumentHeaders
        {
            get
            {
                return GetCollection<DocumentHeader>("DocumentHeaders");
            }
        }

        [Aggregated, Association("Store-StorePriceLists")]
        [DisplayName("Price Lists")]
        public XPCollection<StorePriceList> StorePriceLists
        {
            get
            {
                return GetCollection<StorePriceList>("StorePriceLists");
            }
        }

        [Aggregated, Association("Store-StorePriceCatalogPolicies")]
        [DisplayName("StorePriceCatalogPolicies")]
        public XPCollection<StorePriceCatalogPolicy> StorePriceCatalogPolicies
        {
            get
            {
                return GetCollection<StorePriceCatalogPolicy>("StorePriceCatalogPolicies");
            }
        }

        [Association("Store-ItemStocks")]
        public XPCollection<ItemStock> ItemStocks
        {
            get
            {
                return GetCollection<ItemStock>("ItemStocks");
            }
        }

        [Association("Store-Terminals")]
        public XPCollection<Terminal> Terminals
        {
            get
            {
                return GetCollection<Terminal>("Terminals");
            }
        }

        [Association("Store-UserDailyTotalss")]
        public XPCollection<UserDailyTotals> UserDailyTotalss
        {
            get
            {
                return GetCollection<UserDailyTotals>("UserDailyTotalss");
            }
        }

        [Association("Store-DailyTotalss")]
        public XPCollection<DailyTotals> DailyTotalss
        {
            get
            {
                return GetCollection<DailyTotals>("DailyTotalss");
            }
        }
        [Association("ItemStores-Store")]
        public XPCollection<ItemStore> Items
        {
            get
            {
                return GetCollection<ItemStore>("Items");
            }
        }

        [Association("CentralStore-Users")]
        public XPCollection<User> Users
        {
            get
            {
                return GetCollection<User>("Users");
            }
        }

        [Association("Store-StoreAnalyticTrees")]
        public XPCollection<StoreAnalyticTree> StoreAnalyticTrees
        {
            get
            {
                return GetCollection<StoreAnalyticTree>("StoreAnalyticTrees");
            }
        }

        public StoreControllerSettings StoreControllerSettings
        {
            get
            {
                return Session.FindObject<StoreControllerSettings>(PersistentCriteriaEvaluationBehavior.InTransaction, new BinaryOperator("Store.Oid", Oid));
            }
        }

        protected override void OnDeleting()
        {
            if (DocumentHeaders.Count > 0)
            {
                throw new Exception(Resources.StoreHasDocuments);
            }

            Customer customer = Session.FindObject<Customer>(CriteriaOperator.And(new BinaryOperator("RefundStore.Oid", this.Oid),
                                                                                  new BinaryOperator("Owner.Oid", this.Owner.Oid)));
            if (customer != null)
            {
                throw new Exception(String.Format(Resources.StoreIsDefinedAsRefund, customer.CompanyName));
            }
            Session.Delete(StoreControllerSettings);
            Address = null;
            base.OnDeleting();
        }

        public Guid ImageOid
        {
            get
            {
                return _ImageOid;
            }
            set
            {
                SetPropertyValue("ImageOid", ref _ImageOid, value);
            }
        }

        //[Association("Store-MobileTerminals")]
        //public XPCollection<MobileTerminal> MobileTerminals
        //{
        //    get
        //    {
        //        return GetCollection<MobileTerminal>("MobileTerminals");
        //    }
        //}

        //public PriceCatalog GetPriceCatalog()
        //{
        //    XPCollection<StorePriceList> storePricelists = new XPCollection<StorePriceList>(this.Session, new );
        //    var storePriceListOids = from storePriceList in storePricelists
        //                                where storePriceList.Store.Oid == store.Oid
        //                                select storePriceList.Oid;

        //    XPCollection<CustomerStorePriceList> customerPriceLists = new XPCollection<CustomerStorePriceList>(this.Session,
        //                                                        CriteriaOperator.And(new BinaryOperator("Customer.Oid", this.Oid),
        //                                                                            new InOperator("StorePriceList.Oid", storePriceListOids.ToList())));
        //    customerPriceLists.Sorting = new SortingCollection(new SortProperty("Sort", DevExpress.Xpo.DB.SortingDirection.Ascending));

        //    PriceCatalog toReturn = (customerPriceLists.Count == 0) ?
        //                            store.DefaultPriceCatalog :
        //                            customerPriceLists.FirstOrDefault().StorePriceList.PriceList;

        //    return toReturn;
        //}

        public PriceCatalogPolicy DefaultPriceCatalogPolicy
        {
            get
            {
                return _DefaultPriceCatalogPolicy;
            }
            set
            {
                SetPropertyValue("DefaultPriceCatalogPolicy", ref _DefaultPriceCatalogPolicy, value);
            }
        }

        public Guid ReferenceCompanyOid
        {
            get
            {
                return _ReferenceCompanyOid;
            }
            set
            {
                SetPropertyValue("ReferenceCompanyOid", ref _ReferenceCompanyOid, value);
            }
        }

        [NonPersistent]
        public CompanyNew ReferenceCompany
        {
            get
            {
                return this.Session.GetObjectByKey<CompanyNew>(ReferenceCompanyOid);
            }
            set
            {
                if (value == null)
                {
                    ReferenceCompanyOid = Guid.Empty;
                }
                else
                {
                    ReferenceCompanyOid = value.Oid;
                }
            }
        }

        [Aggregated, Association("Store-ItemExtraInfos")]
        [DisplayName("ItemExtraInfo")]
        public XPCollection<ItemExtraInfo> ItemExtraInfos
        {
            get
            {
                return GetCollection<ItemExtraInfo>("ItemExtraInfos");
            }
        }

        [Association("LeafletStores-Store")]
        public XPCollection<LeafletStore> Leaflets
        {
            get
            {
                return GetCollection<LeafletStore>("Leaflets");
            }
        }
    }

}
