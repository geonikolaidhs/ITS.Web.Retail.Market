using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.WRM.Model.Interface;
using ITS.WRM.Model.Interface.Model.NonPersistant;

using SFA_Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 40, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class Address : BaseObj, IAddress
    {

        public Address()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Address(Session session)
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

        public Guid AddressType { get; set; }
        public int? AutomaticNumbering { get; set; }
        public string City { get; set; }    
        public Guid? DefaultPhoneOid { get; set; }
        public string Email { get; set; }
        public bool IsDefault { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
        [NonPersistent]
        public List<IPhone> Phones { get; set; }
        public string POBox { get; set; }
        public string PostCode { get; set; }
        public string Profession { get; set; }
        public string Region { get; set; }
        public Guid Store { get; set; }
        public string Street { get; set; }
        public Guid Trader { get; set; }

        public Guid VatLevel { get; set; }
        
        [NonPersistent]
        ITrader IAddress.Trader
        {
            get
            {
                throw new NotImplementedException();
                //return null;
            }
            set
            {
                //this.Trader = value as Trader;
                throw new NotImplementedException();
            }
        }
        [NonPersistent]
        IStore IAddress.Store
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
        IVatLevel IAddress.VatLevel
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

        IAddressType IAddress.AddressType
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