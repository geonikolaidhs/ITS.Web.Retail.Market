using ITS.Retail.Platform.Enumerations;
using ITS.WRM.Model.Interface.Model.NonPersistant;
using ITS.WRM.Model.Interface.Model.SupportingClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface ICustomer:  IRequiredOwner, ICustomerModel
    {
        string Email { get; set; }
        decimal OtherPets { get; set; }
        decimal Cats { get; set; }
        decimal Dogs { get; set; }
        eMaritalStatus MaritalStatus { get; set; }
        eSex Sex { get; set; }
        string FatherName { get; set; }
        long BirthDateTicks { get; set; }
        decimal Balance { get; set; }
        string CardID { get; set; }
        IStore RefundStore { get; set; }
        ITrader Trader { get; set; }
        string Code { get; set; }
        string CompanyName { get; set; }
        string CompanyBrandName { get; set; }
        IAddress DefaultAddress { get; set; }
        string Profession { get; set; }
        string Loyalty { get; set; }
        double Discount { get; set; }
        IPaymentMethod PaymentMethod { get; set; }
        IVatLevel VatLevel { get; set; }
        bool BreakOrderToCentral { get; set; }
        decimal CollectedPoints { get; set; }
        decimal TotalEarnedPoints { get; set; }
        decimal TotalConsumedPoints { get; set; }
        IPriceCatalogPolicy PriceCatalogPolicy { get; set; }
        List<ICustomerStorePriceList> CustomerStorePriceLists { get; set; }
        List<ICustomerAnalyticTree> CustomerAnalyticTrees { get; set; }
    }

}
