using ITS.POS.Client.Helpers;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Common.ViewModel;
using ITS.Retail.Platform.Enumerations.ViewModel;
using ITS.Retail.Platform.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Kernel
{
    public interface ICustomerService : IKernelModule
    {
        PriceCatalog GetPriceCatalog(Guid customer, Guid store);
        PriceCatalogPolicy GetPriceCatalogPolicy(Guid customerGuid, Guid storeGuid);
        T SearchCustomer<T>(string lookupString) where T : BaseObj;
        IEnumerable<InsertedCustomerViewModel> GetValidCustomers(string lookupString);
        IEnumerable<InsertedCustomerViewModel> GetCustomerAddresses(BasicObj obj, Type type, Trader trader = null);
        bool SearchDenormalizedCustomer(string lookupString, InsertedCustomerViewModel customerModel);
    }
}
