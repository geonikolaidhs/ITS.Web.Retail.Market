using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using SFA_Model.NonPersistant;
using ITS.WRM.Model.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 650, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class Phone : BasicObj, IPhone
    {
        public Phone()
           : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Phone(Session session)
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


        public Guid Address { get; set; }
        
        public string Number { get; set; }

        public Guid Owner { get; set; }
        
      
        public Guid PhoneType { get; set; }
        [NonPersistent]
        IPhoneType IPhone.PhoneType
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
        IAddress IPhone.Address
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
        ICompanyNew IPhone.Owner
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