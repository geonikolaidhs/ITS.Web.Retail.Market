using ITS.Retail.Platform.Enumerations;
using ITS.POS.Model.Master;
using ITS.Retail.Platform.Enumerations.ViewModel;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionAddCustomerInternalParams : ActionParams
    {
        //private Customer cust;
        //private string lookupCode;

        public Customer Customer { get; set; }
        public string CustomerLookupCode { get; set; }

        public Address Address { get; set; }

        public InsertedCustomerViewModel CustomerViewModel { get; set; }

        public override eActions ActionCode
        {
            get { return eActions.ADD_CUSTOMER_INTERNAL; }
        }

        public ActionAddCustomerInternalParams(Customer customer,string customerLookupCode, Address address, InsertedCustomerViewModel customerViewModel)
        {
            this.Customer = customer;
            this.CustomerLookupCode = customerLookupCode;
            this.Address = address;
            this.CustomerViewModel = customerViewModel; 
        }

        public ActionAddCustomerInternalParams(Customer cust, string lookupCode, Address address)
        {
            this.Customer = cust;
            this.CustomerLookupCode = lookupCode;
            this.Address = address;
        }
    }
}
