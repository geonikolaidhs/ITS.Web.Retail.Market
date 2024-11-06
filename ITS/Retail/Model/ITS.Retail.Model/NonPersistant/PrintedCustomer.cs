using DevExpress.Xpo;
using Newtonsoft.Json;

namespace ITS.Retail.Model.NonPersistant
{
    [NonPersistent]
    public class PrintedCustomer
    {
        public string TaxOfficeLookup { get; set; }
        public string TaxOfficeDescription { get; set; }
        public string Code { get; set; }
        public string TaxCode { get; set; }
        public string TaxOffice { get; set; }
        public string CompanyName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Profession { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string CustomerOid { get; set; }
        public string AddressOid { get; set; }
        public string IsNew { get; set; }
    }
}
