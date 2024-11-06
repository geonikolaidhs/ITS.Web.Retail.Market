//-----------------------------------------------------------------------
// <copyright file="Customer.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Model.Attributes;
using Newtonsoft.Json;
using ITS.Retail.Platform.Kernel.Model;
using ITS.Retail.Platform.Enumerations.ViewModel;

namespace ITS.Retail.Model
{
    [DataViewParameter]
    [Updater(Order = 210, Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [Indices("CompanyName;Oid;GCRecord;Owner", "GCRecord;Owner;Code;CompanyName;Trader")]
    [EntityDisplayName("Customer", typeof(ResourcesLib.Resources))]
    public class Customer : BaseObj, IRequiredOwner, ICustomer
    {
        public Customer()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Customer(Session session)
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
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                case eUpdateDirection.STORECONTROLLER_TO_POS:

                    Type thisType = typeof(Customer);
                    if (owner == null)
                    {
                        throw new Exception(thisType.Name + ".GetUpdaterCriteria(); Owner: Owner is null");
                    }

                    crop = new BinaryOperator("Owner.Oid", owner.Oid);
                    break;
            }
            return crop;
        }

        private string _Email;
        private decimal _OtherPets;
        private decimal _Cats;
        private decimal _Dogs;
        private eMaritalStatus _MaritalStatus;
        private eSex _Sex;
        private string _FatherName;
        private long _BirthDateTicks;
        private decimal _Balance;
        private string _CardID;
        private Store _RefundStore;
        private Trader _Trader;
        private CompanyNew _Owner;

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

        [Association("Trader-Customers"), Indexed(Unique = false)]
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
        [Indexed("Owner;GCRecord", Unique = true)]
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

        public Store DefaultStore
        {
            get
            {
                if (CustomerStorePriceLists.Count == 0)
                {
                    return null;
                }
                IEnumerable<CustomerStorePriceList> cspls = CustomerStorePriceLists.Where(g => g.IsDefault == true);
                if (cspls.Count() == 1)
                {
                    return cspls.First().StorePriceList.Store;
                }
                return CustomerStorePriceLists.First().StorePriceList.Store;
            }
        }

        public List<Store> Stores
        {
            get
            {
                return CustomerStorePriceLists.Select(g => g.StorePriceList.Store).ToList();
            }
        }

        private string _CompanyName;
        [DescriptionField, Indexed("Owner;GCRecord", Unique = false)]
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

        private string _CompanyBrandName;
        public string CompanyBrandName
        {
            get
            {
                return _CompanyBrandName;
            }
            set
            {
                SetPropertyValue("CompanyBrandName", ref _CompanyBrandName, value);
            }
        }

        private Address _DefaultAddress;
        public Address DefaultAddress
        {
            get
            {
                if (_DefaultAddress == null)
                {
                    if (this.Trader != null && this.Trader.Addresses != null && this.Trader.Addresses.Count == 1)
                    {
                        return this.Trader.Addresses[0];
                    }
                }
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

        private string _Loyalty;
        [Indexed(Unique = false)]
        public string Loyalty
        {
            get
            {
                return _Loyalty;
            }
            set
            {
                SetPropertyValue("Loyalty", ref _Loyalty, value);
            }
        }

        private double _Discount;
        public double Discount
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


        private PaymentMethod _PaymentMethod;
        public PaymentMethod PaymentMethod
        {
            get
            {
                return _PaymentMethod;
            }
            set
            {
                SetPropertyValue("PaymentMethod", ref _PaymentMethod, value);
            }
        }

        private VatLevel _VatLevel;
        [Association("VatLevel-Customer")]
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


        private bool _BreakOrderToCentral;
        private decimal _CollectedPoints;
        private decimal _TotalEarnedPoints;
        private decimal _TotalConsumedPoints;
        private PriceCatalogPolicy _PriceCatalogPolicy;

        public bool BreakOrderToCentral
        {
            get
            {
                return _BreakOrderToCentral;
            }
            set
            {
                SetPropertyValue("BreakOrderToCentral", ref _BreakOrderToCentral, value);
            }
        }


        public decimal Balance
        {
            get
            {
                return _Balance;
            }
            set
            {
                SetPropertyValue("Balance", ref _Balance, value);
            }
        }

        public string Description
        {
            get
            {
                return CompanyName;
            }
        }

        [Aggregated, Association("Customer-CustomerStorePriceLists")]
        public XPCollection<CustomerStorePriceList> CustomerStorePriceLists
        {
            get
            {
                return GetCollection<CustomerStorePriceList>("CustomerStorePriceLists");
            }
        }

        public PriceCatalogPolicy PriceCatalogPolicy
        {
            get
            {
                return _PriceCatalogPolicy;
            }
            set
            {
                SetPropertyValue("PriceCatalogPolicy", ref _PriceCatalogPolicy, value);
            }
        }

        [Aggregated, Association("Customer-CustomerAnalyticTrees")]
        public XPCollection<CustomerAnalyticTree> CustomerAnalyticTrees
        {
            get
            {
                return GetCollection<CustomerAnalyticTree>("CustomerAnalyticTrees");
            }
        }

        [Association("Customer-DocumentHeaders")]
        public XPCollection<DocumentHeader> DocumentHeaders
        {
            get
            {
                return GetCollection<DocumentHeader>("DocumentHeaders");
            }
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
            if (this.IsDeleted == false && this.DefaultAddress == null && this.Trader.Addresses.Count > 0)
            {
                this.DefaultAddress = this.Trader.Addresses.OrderBy(adr => adr.AutomaticNumbering).FirstOrDefault();
            }
            base.OnSaving();
        }

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            if (propertyName == "Code" && Loyalty == null)
            {
                Loyalty = Code;
            }
            base.OnChanged(propertyName, oldValue, newValue);
        }

        /*
        public PriceCatalog GetPriceCatalog(Store store)
        {
            if (store != null)
            {
                XPQuery<StorePriceList> storePricelists = new XPQuery<StorePriceList>(this.Session);
                var storePriceListOids = from storePriceList in storePricelists
                              where storePriceList.Store.Oid == store.Oid
                              select storePriceList.Oid;

                XPCollection<CustomerStorePriceList> customerPriceLists = new XPCollection<CustomerStorePriceList>(this.Session,
                                                                    CriteriaOperator.And(new BinaryOperator("Customer.Oid", this.Oid),
                                                                                        new InOperator("StorePriceList.Oid", storePriceListOids.ToList())));
                //customerPriceLists.Sorting = new SortingCollection(new SortProperty("Sort", SortingDirection.Ascending));

                PriceCatalog toReturn = (customerPriceLists.Count == 0) ?
                                        store.DefaultPriceCatalog : 
                                        customerPriceLists.FirstOrDefault().StorePriceList.PriceList;

                return toReturn;
            }
            else
            {
                throw new Exception("Error at Customer.GetPriceCatalog(). Store is null");
                //return null;
            }
        }
        */

        public PriceCatalog GetDefaultPriceCatalog()
        {
            CustomerStorePriceList customerPriceList = this.Session.FindObject<CustomerStorePriceList>(CriteriaOperator.And(new BinaryOperator("Customer.Oid", this.Oid),
                                                                                                                            new BinaryOperator("IsDefault", true)));
            if (customerPriceList == null)
            {
                customerPriceList = this.Session.FindObject<CustomerStorePriceList>(new BinaryOperator("Customer.Oid", this.Oid));
            }

            PriceCatalog toReturn = (customerPriceList == null) ? null : customerPriceList.StorePriceList.PriceList;
            return toReturn;
        }

        public VatLevel GetVatLevel(Address address)
        {
            if (address != null && address.Trader == this.Trader && address.VatLevel != null)
            {
                return address.VatLevel;
            }
            return VatLevel;
        }

        public decimal CollectedPoints
        {
            get
            {
                return _CollectedPoints;
            }
            set
            {
                SetPropertyValue("CollectedPoints", ref _CollectedPoints, value);
            }
        }


        public string CardID
        {
            get
            {
                return _CardID;
            }
            set
            {
                SetPropertyValue("CardID", ref _CardID, value);
            }
        }

        public Store RefundStore
        {
            get
            {
                return _RefundStore;
            }
            set
            {
                SetPropertyValue("RefundStore", ref _RefundStore, value);
            }
        }

        [GDPR]
        public long BirthDateTicks
        {
            get
            {
                return _BirthDateTicks;
            }
            set
            {
                SetPropertyValue("BirthDateTicks", ref _BirthDateTicks, value);
            }
        }

        [NonPersistent]
        public DateTime BirthDate
        {
            get
            {
                return new DateTime(BirthDateTicks);
            }
            set
            {
                BirthDateTicks = value.Ticks;
            }
        }
        [GDPR]
        public string FatherName
        {
            get
            {
                return _FatherName;
            }
            set
            {
                SetPropertyValue("FatherName", ref _FatherName, value);
            }
        }
        [GDPR]
        public eSex Sex
        {
            get
            {
                return _Sex;
            }
            set
            {
                SetPropertyValue("Sex", ref _Sex, value);
            }
        }

        [GDPR]
        public eMaritalStatus MaritalStatus
        {
            get
            {
                return _MaritalStatus;
            }
            set
            {
                SetPropertyValue("MaritalStatus", ref _MaritalStatus, value);
            }
        }

        [GDPR]
        public decimal Dogs
        {
            get
            {
                return _Dogs;
            }
            set
            {
                SetPropertyValue("Dogs", ref _Dogs, value);
            }
        }

        [GDPR]
        public decimal Cats
        {
            get
            {
                return _Cats;
            }
            set
            {
                SetPropertyValue("Cats", ref _Cats, value);
            }
        }
        [GDPR]
        public decimal OtherPets
        {
            get
            {
                return _OtherPets;
            }
            set
            {
                SetPropertyValue("OtherPets", ref _OtherPets, value);
            }
        }

        public decimal TotalPets
        {
            get
            {
                return Dogs + Cats + OtherPets;
            }
        }

        public bool Pets
        {
            get
            {
                return TotalPets > 0;
            }
        }
        [GDPR]
        public string Email
        {
            get
            {
                return _Email;
            }
            set
            {
                SetPropertyValue("Email", ref _Email, value);
            }
        }
        public decimal TotalEarnedPoints
        {
            get
            {
                return _TotalEarnedPoints;
            }
            set
            {
                SetPropertyValue("TotalEarnedPoints", ref _TotalEarnedPoints, value);
            }
        }
        public decimal TotalConsumedPoints
        {
            get
            {
                return _TotalConsumedPoints;
            }
            set
            {
                SetPropertyValue("TotalEarnedPoints", ref _TotalConsumedPoints, value);
            }
        }
        [Association("Customer-CustomerChilds"), Aggregated]
        public XPCollection<CustomerChild> CustomerChilds
        {
            get
            {
                return GetCollection<CustomerChild>("CustomerChilds");
            }
        }
        public string FullJson(JsonSerializerSettings settings, bool includeType = false)
        {
            Dictionary<string, object> dict = this.GetDict(settings, includeType, true);
            if (dict.ContainsKey("Trader") == false)
            {
                dict.Add("Trader", Trader.FullJson(settings, includeType));
            }
            else
            {
                dict["Trader"] = Trader.FullJson(settings, includeType);
            }
            if (dict.ContainsKey("DefaultAddress") == false)
            {
                dict.Add("DefaultAddress", Trader.FullJson(settings, includeType));
            }
            else
            {
                dict["DefaultAddress"] = Trader.FullJson(settings, includeType);
            }
            if (dict.ContainsKey("CustomerStorePriceLists") == false)
            {
                dict.Add("CustomerStorePriceLists", CustomerStorePriceLists.Select(g => g.GetDict(settings, includeType, true)).ToList());
            }
            else
            {
                dict["CustomerStorePriceLists"] = CustomerStorePriceLists.Select(g => g.GetDict(settings, includeType, true)).ToList();
            }
            if (dict.ContainsKey("CustomerChilds") == false)
            {
                dict.Add("CustomerChilds", this.CustomerChilds.Select(g => g.GetDict(settings, includeType, true)).ToList());
            }
            else
            {
                dict["CustomerChilds"] = CustomerChilds.Select(g => g.GetDict(settings, includeType, true)).ToList();
            }
            if (dict.ContainsKey("CustomerAnalyticTrees") == false)
            {
                dict.Add("CustomerAnalyticTrees", this.CustomerAnalyticTrees.Select(g => g.GetDict(settings, includeType, true)).ToList());
            }
            else
            {
                dict["CustomerAnalyticTrees"] = CustomerAnalyticTrees.Select(g => g.GetDict(settings, includeType, true)).ToList();
            }
            return JsonConvert.SerializeObject(dict, Formatting.Indented, settings);
        }
        public InsertedCustomerViewModel InsertedCustomerViewModel
        {
            get
            {
                return new InsertedCustomerViewModel()
                {
                    TaxOfficeLookup = this.Trader.TaxOfficeLookUpOid ?? Guid.Empty,
                    TaxOfficeDescription = this.Trader.TaxOfficeLookUp == null ? "" : this.Trader.TaxOfficeLookUp.Description,
                    Code = this.Code,
                    TaxCode = this.Trader != null ? this.Trader.TaxCode : string.Empty,
                    CompanyName = this.CompanyName,
                    FirstName = this.Trader != null ? this.Trader.FirstName : "",
                    LastName = this.Trader != null ? this.Trader.LastName : "",
                    Profession = this.Profession,
                    Street = this.DefaultAddress != null ? this.DefaultAddress.Street : "",
                    AddressProfession = this.DefaultAddress != null ? this.DefaultAddress.Profession : "",
                    PostalCode = this.DefaultAddress != null ? this.DefaultAddress.PostCode : "",
                    City = this.DefaultAddress != null ? this.DefaultAddress.City : "",
                    Phone = this.DefaultAddress != null ? (this.DefaultAddress.DefaultPhone != null ? this.DefaultAddress.DefaultPhone.Number : "") : "",
                    CustomerOid = this.Oid,
                    AddressOid = this.DefaultAddress != null ? this.DefaultAddress.Oid : Guid.Empty,
                    IsNew = false,
                    AddressIsNew = false,
                    IsSupplier = false
                };
            }
        }
        public InsertedCustomerViewModel GetInsertedCustomerViewModel(Address address = null)
        {
            if (address == null)
            {
                address = this.DefaultAddress;
            }

            return new InsertedCustomerViewModel()
            {
                TaxOfficeLookup = this.Trader.TaxOfficeLookUpOid ?? Guid.Empty,
                TaxOfficeDescription = this.Trader.TaxOfficeLookUp == null ? "" : this.Trader.TaxOfficeLookUp.Description,
                Code = this.Code,
                TaxCode = this.Trader != null ? this.Trader.TaxCode : string.Empty,
                CompanyName = this.CompanyName,
                FirstName = this.Trader != null ? this.Trader.FirstName : "",
                LastName = this.Trader != null ? this.Trader.LastName : "",
                Profession = this.Profession,
                Street = address.Street,
                AddressProfession = address.Profession,
                PostalCode = address.PostCode,
                City = address.City,
                Phone = address.DefaultPhone != null ? address.DefaultPhone.Number : "",
                CustomerOid = this.Oid,
                AddressOid = address.Oid,
                IsNew = false,
                AddressIsNew = false,
                IsSupplier = false
            };
        }
        public string FullDescription
        {
            get
            {
                return string.Format("{0} ({1})", this.Description, this.Code);
            }
        }
        public bool HasCustomerPolicy
        {
            get
            {
                if (this.PriceCatalogPolicy != null && this.PriceCatalogPolicy.PriceCatalogPolicyDetails.Count > 0)
                {
                    return true;
                }
                return false;
            }
        }
        public string AllProfessions
        {
            get
            {
                List<string> professions = new List<string>();

                if (!String.IsNullOrEmpty(this.Profession))
                {
                    professions.Add(this.Profession);
                }
                if (this.Trader != null)
                {
                    try
                    {
                        if (this.Trader.Addresses.Count > 0)
                        {
                            List<string> traderProfessions = this.Trader.Addresses.Where(address => String.IsNullOrEmpty(address.Profession) == false).Select(address => address.Profession).ToList();
                            if (traderProfessions.Count > 0)
                            {
                                professions.AddRange(traderProfessions);
                            }
                        }
                    }
                    catch { }

                }
                if (professions.Count <= 0)
                {
                    return String.Empty;
                }
                return String.Join(",", professions);
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
