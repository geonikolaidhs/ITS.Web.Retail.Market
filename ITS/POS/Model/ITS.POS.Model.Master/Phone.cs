using DevExpress.Xpo;
using ITS.POS.Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Model.Master
{
    public class Phone : BaseObj 
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

       

        private Guid _PhoneType;
        [Association("PhoneType-Phones")]
        public Guid PhoneType
        {
            get
            {
                return _PhoneType;
            }
            set
            {
                SetPropertyValue("PhoneType", ref _PhoneType, value);
            }
        }

        private string _Number;
        [Indexed("GCRecord",Unique = false)]
        public string Number
        {
            get
            {
                return _Number;
            }
            set
            {
                SetPropertyValue("Number", ref _Number, value);
            }
        }

        private Guid _Address;
        public Guid Address
        {
            get
            {
                return _Address;
            }
            set
            {
                SetPropertyValue("Address", ref _Address, value);
            }
        }

       
    }

}