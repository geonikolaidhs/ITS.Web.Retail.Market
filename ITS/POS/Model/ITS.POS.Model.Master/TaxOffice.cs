using DevExpress.Xpo;
using ITS.POS.Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Model.Master
{
   public class TaxOffice : Lookup2Fields
    {
        private string _Street;
        private string _PostCode;
        private string _Municipality;
        
        public TaxOffice()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public TaxOffice(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public string Street
        {
            get
            {
                return _Street;
            }
            set
            {
                SetPropertyValue("Street", ref _Street, value);
            }
        }

        public string PostCode
        {
            get
            {
                return _PostCode;
            }
            set
            {
                SetPropertyValue("PostCode", ref _PostCode, value);
            }
        }

        public string Municipality
        {
            get
            {
                return _Municipality;
            }
            set
            {
                SetPropertyValue("Municipality", ref _Municipality, value);
            }
        }
    }
}
