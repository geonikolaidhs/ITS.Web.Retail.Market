using ITS.Retail.Common.ViewModel;
using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.WebClient.ViewModel
{
    public class CustomerViewModel : IPersistableViewModel
    {
        public CustomerViewModel()
        {

        }

        public CustomerViewModel(Customer customer)
        {
            this.LoadPersistent(customer);
            TaxCode = customer.Trader.TaxCode;
        }

        public string TaxCode { get; set; }

        public string Code { get; set; }

        public Guid Oid { get; set; }

        public Type PersistedType { get { return typeof(Customer); } }

        public bool IsDeleted { get; set; }

        public void UpdateModel(DevExpress.Xpo.Session uow)
        {
            //throw new NotImplementedException();
        }

        public bool Validate(out string message)
        {
            message = "";
            return true;
        }

        public string CardID { get; set; }

        public string Loyalty { get; set; }

        public string CompanyName { get; set; }
        
        public decimal CollectedPoints { get; set; }

        public decimal ComputedPoints { get; set; }

        public decimal Difference { get { return CollectedPoints - ComputedPoints; } }

        public decimal TotalEarnedPoints { get; set; }

        public decimal TotalConsumedPoints { get; set; }
    }
}