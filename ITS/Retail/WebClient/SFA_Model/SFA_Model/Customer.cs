using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using SFA_Model.NonPersistant;
using ITS.WRM.Model.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ITS.WRM.Model.Interface.Model.SupportingClasses;

namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 210, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class Customer : BasicObj, ICustomer
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
        public decimal Balance { get; set; }
        
        public long BirthDateTicks { get; set; }

        public bool BreakOrderToCentral { get; set; }

        public string CardID { get; set; }
        public decimal Cats { get; set; }
        public string Code { get; set; }
        public decimal CollectedPoints { get; set; }
        public string CompanyBrandName { get; set; }
        public string CompanyName { get; set; }
        [NonPersistent]
        public List<ICustomerAnalyticTree> CustomerAnalyticTrees { get; set; }
        [NonPersistent]
        public List<ICustomerStorePriceList> CustomerStorePriceLists { get; set; }
        public Guid DefaultAddress { get; set; }
        public string Description { get; set; }
        public double Discount { get; set; }
        public decimal Dogs { get; set; }
        public string Email { get; set; }
        public string FatherName { get; set; }
        public string Loyalty { get; set; }
        public eMaritalStatus MaritalStatus { get; set; }
        public decimal OtherPets { get; set; }
        public Guid Owner { get; set; }

        public Guid PaymentMethod { get; set; }
       
        public Guid PriceCatalogPolicy { get; set; }
        
        public string Profession { get; set; }

        public Guid RefundStore { get; set; }
        
        public eSex Sex { get; set; }
        
        public decimal TotalConsumedPoints { get; set; }

        public decimal TotalEarnedPoints { get; set; }

        public Guid Trader { get; set; }
       
       
        public Guid VatLevel { get; set; }
        [NonPersistent]
        IAddress ICustomer.DefaultAddress
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        ICompanyNew IOwner.Owner
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        IPaymentMethod ICustomer.PaymentMethod
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        IPriceCatalogPolicy ICustomer.PriceCatalogPolicy
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        IStore ICustomer.RefundStore
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        ITrader ICustomer.Trader
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        IVatLevel ICustomer.VatLevel
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
    }
}