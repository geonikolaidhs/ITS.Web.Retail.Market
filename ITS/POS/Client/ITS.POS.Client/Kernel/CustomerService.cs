using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.POS.Model.Master;
using ITS.Retail.Platform.Enumerations.ViewModel;
using ITS.POS.Model.Settings;

namespace ITS.POS.Client.Kernel
{
    /// <summary>
    /// Provides business logic for the customer entity.
    /// </summary>
    public class CustomerService : ICustomerService
    {
        ISessionManager SessionManager { get; set; }

        public CustomerService(ISessionManager sessionManager)
        {
            this.SessionManager = sessionManager;
        }

        /// <summary>
        /// Gets the price catalog of the given customer, for the given store.
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="store"></param>
        /// <returns></returns>
        public PriceCatalog GetPriceCatalog(Guid customer, Guid store)
        {
            XPCollection<StorePriceList> storePriceList = new XPCollection<StorePriceList>(SessionManager.GetSession<StorePriceList>(), new BinaryOperator("Store", store));
            XPCollection<CustomerStorePriceList> customerPriceList = new XPCollection<CustomerStorePriceList>(SessionManager.GetSession<CustomerStorePriceList>(), new BinaryOperator("Customer", customer));

            Guid priceCatalogOid = (from stpl in storePriceList
                                    join cstpl in customerPriceList on stpl.Oid equals cstpl.StorePriceList
                                    select stpl.PriceList).FirstOrDefault();

            PriceCatalog priceCatalog = priceCatalogOid != null ? SessionManager.GetObjectByKey<PriceCatalog>(priceCatalogOid) : null;
            if (priceCatalog == null)
            {
                Store storeobj = SessionManager.GetObjectByKey<Store>(store);
                if (storeobj != null)
                {
                    priceCatalog = SessionManager.GetObjectByKey<PriceCatalog>(storeobj.DefaultPriceCatalog);
                }
            }
            return priceCatalog;
        }

        public PriceCatalogPolicy GetPriceCatalogPolicy(Guid customerGuid, Guid storeGuid)
        {
            Customer customer = SessionManager.GetObjectByKey<Customer>(customerGuid);
            Store store = SessionManager.GetObjectByKey<Store>(storeGuid);
            PriceCatalogPolicy priceCatalogPolicy = null;
            if (customer != null)
            {
                priceCatalogPolicy = SessionManager.GetObjectByKey<PriceCatalogPolicy>(customer.PriceCatalogPolicy);
                if (priceCatalogPolicy != null)
                {
                    IEnumerable<Guid> storePriceCatalogGuids = SessionManager.GetSession<StorePriceList>().Query<StorePriceList>()
                                                                            .Where(spl =>spl.Store == storeGuid)
                                                                            .Select(storepricelist => storepricelist.PriceList);
                    IEnumerable<Guid> customerPriceCatalogGuids = priceCatalogPolicy.PriceCatalogPolicyDetails.Select(detail => detail.PriceCatalog);
                    if (customerPriceCatalogGuids.Any(cpc => storePriceCatalogGuids.Contains(cpc)) == false)
                    {
                        priceCatalogPolicy = SessionManager.GetObjectByKey<PriceCatalogPolicy>(store.DefaultPriceCatalogPolicy);
                    }                    
                }
                else
                {
                    priceCatalogPolicy = SessionManager.GetObjectByKey<PriceCatalogPolicy>(store.DefaultPriceCatalogPolicy);
                }
            }
            else
            {
                priceCatalogPolicy = SessionManager.GetObjectByKey<PriceCatalogPolicy>(store.DefaultPriceCatalogPolicy);
            }
            return priceCatalogPolicy;
        }

        /// <summary>
        /// Finds a customer using a lookup string. Searches the following customer fields, in the following order: CardID, TaxCode, Code.
        /// </summary>
        /// <param name="lookupString"></param>
        /// <returns></returns>
        public T SearchCustomer<T>(string lookupString) where T : BaseObj
        {
            T cust = null;
            if (!String.IsNullOrEmpty(lookupString))
            {
                BinaryOperator activeCriteria = new BinaryOperator("IsActive", true);
                ////Search by CardID

                if (typeof(T) == typeof(Customer))
                {
                    cust = SessionManager.FindObject<T>(CriteriaOperator.And(activeCriteria, new BinaryOperator("CardID", lookupString)));
                }
                ////Search by Tax Code
                if (cust == null)
                {
                    Trader trader = SessionManager.FindObject<Trader>(CriteriaOperator.And(activeCriteria, new BinaryOperator("TaxCode", lookupString)));
                    if (trader != null)
                    {
                        cust = SessionManager.FindObject<T>(CriteriaOperator.And(activeCriteria, new BinaryOperator("Trader", trader.Oid)));
                    }
                }
                ////Search by code
                if (cust == null)
                {
                    cust = SessionManager.FindObject<T>(CriteriaOperator.And(activeCriteria, new BinaryOperator("Code", lookupString)));
                }
                ////Search by Phone
                if (cust == null)
                {
                    Phone phone = SessionManager.FindObject<Phone>(new BinaryOperator("Number", lookupString));
                    if (phone != null)
                    {
                        Address address = SessionManager.FindObject<Address>(new BinaryOperator("Oid", phone.Address));
                        if (address != null)
                        {
                            cust = SessionManager.FindObject<T>(CriteriaOperator.And(activeCriteria, new BinaryOperator("Trader", address.Trader)));
                        }
                    }
                }
            }
            return cust;
        }

        public IEnumerable<InsertedCustomerViewModel> GetCustomerAddresses(BasicObj obj, Type type, Trader trader = null)
        {
            XPCollection<Address> addresses =
                new XPCollection<Address>(SessionManager.GetSession<Address>(), new BinaryOperator("Trader", type.GetProperty("Trader").GetValue(obj, null)));
            if (trader == null)
            {
                trader = SessionManager.GetSession<Trader>().GetObjectByKey<Trader>((Guid)type.GetProperty("Trader").GetValue(obj, null));
            }
            return addresses.Select(x => new InsertedCustomerViewModel()
            {
                AddressOid = x.Oid,
                City = x.City,
                Code = (string)type.GetProperty("Code").GetValue(obj, null),
                CompanyName = (string)type.GetProperty("CompanyName").GetValue(obj, null),
                CustomerOid = (Guid)type.GetProperty("Oid").GetValue(obj, null),
                FirstName = trader.FirstName,
                LastName = trader.LastName,
                PostalCode = x.PostCode,
                Profession = (string)type.GetProperty("Profession").GetValue(obj, null),
                AddressProfession = x.Profession,
                Street = x.Street,
                TaxCode = trader.TaxCode,
                TaxOfficeLookup = trader.TaxOfficeLookUpOid ?? Guid.Empty,
                TaxOfficeDescription = trader.TaxOfficeLookUp == null ? "" : trader.TaxOfficeLookUp.Description,
                Phone = this.GetPhone(x.Oid),
                IsSupplier = type == typeof(SupplierNew),
                ThirdPartNum = x.ThirdPartNum
                
            });
        }
        public IEnumerable<InsertedCustomerViewModel> GetValidCustomers(string lookupString)
        {
            List<InsertedCustomerViewModel> result = new List<InsertedCustomerViewModel>();
            if (!String.IsNullOrEmpty(lookupString))
            {
                BinaryOperator activeCriteria = new BinaryOperator("IsActive", true);

                Trader trader = SessionManager.FindObject<Trader>(new BinaryOperator("TaxCode", lookupString));

                if (trader == null)
                {
                    Phone phone = SessionManager.FindObject<Phone>(new BinaryOperator("Number", lookupString));
                    if (phone != null)
                    {
                        Address address = SessionManager.GetObjectByKey<Address>(phone.Address);
                        if (address != null)
                        {
                            trader = SessionManager.GetObjectByKey<Trader>(address.Trader);
                        }
                    }
                }

                Guid traderOid = Guid.Empty;
                if (trader != null)
                {
                    traderOid = trader.Oid;
                }

                CriteriaOperator commonCriteria = CriteriaOperator.Or(new BinaryOperator("Trader", traderOid),
                                                                        new BinaryOperator("Code", lookupString),
                                                                     //new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("CompanyName"), lookupString)
                                                                     new BinaryOperator("CompanyName", "%" + lookupString + "%", BinaryOperatorType.Like)
                                                                     );

                XPCollection<Customer> matchedCustomers = new XPCollection<Customer>(SessionManager.GetSession<Customer>(),
                    CriteriaOperator.And(CriteriaOperator.Or(commonCriteria
                                                             , new BinaryOperator("CardID", lookupString))
                                        , activeCriteria));

                XPCollection<SupplierNew> matchedSuppliers = new XPCollection<SupplierNew>(SessionManager.GetSession<SupplierNew>(), CriteriaOperator.And(commonCriteria, activeCriteria));

                foreach (Customer obj in matchedCustomers)
                {
                    result.AddRange(GetCustomerAddresses(obj, typeof(Customer), trader));
                }
                IEnumerable<Guid> addressesOfTradersWithCustomersAndSuppliers = result.Select(customerViewModel => customerViewModel.AddressOid);
                foreach (SupplierNew obj in matchedSuppliers)
                {
                    IEnumerable<InsertedCustomerViewModel> customersFromSuppliers = GetCustomerAddresses(obj, typeof(SupplierNew), trader);
                    result.AddRange(customersFromSuppliers.Where(customerViewModel => !addressesOfTradersWithCustomersAndSuppliers.Contains(customerViewModel.AddressOid)));
                }
            }

            return result;
        }

        public bool SearchDenormalizedCustomer(string lookupString, InsertedCustomerViewModel customerModel)
        {
            return customerModel == null ? false :
                    (customerModel.Code != null ? customerModel.Code.Contains(lookupString) : false ||
                    customerModel.CompanyName != null ? customerModel.CompanyName.Contains(lookupString) : false ||
                    customerModel.TaxCode != null ? customerModel.TaxCode.Contains(lookupString) : false ||
                    customerModel.Phone != null ? customerModel.Phone.Contains(lookupString) : false);

        }

        private string GetPhone(Guid addressOid)
        {
            Phone phone = SessionManager.FindObject<Phone>(new BinaryOperator("Address", addressOid));
            return (phone == null) ? "" : phone.Number;
        }
    }
}
