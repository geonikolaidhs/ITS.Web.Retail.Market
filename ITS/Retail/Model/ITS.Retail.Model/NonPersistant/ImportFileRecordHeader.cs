using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model.NonPersistant
{
    [NonPersistent]
    public class ImportFileRecordHeader : BaseObj
    {
         public ImportFileRecordHeader()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

         public ImportFileRecordHeader(Session session)
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


        private string _HeaderCode;
        private int _Position; 
        private string _TabDelimitedString;
        private bool _IsTabDelimited;
        private string _EntityName;
        private int _Length;
        private int _Order;

        [DisplayOrder(Order = 3)]
        public string EntityName
        {
            get
            {
                return _EntityName;
            }
            set
            {
                SetPropertyValue("EntityName", ref _EntityName, value);
            }
        }
        [DisplayOrder(Order = 2)]
        public string HeaderCode
        {
            get
            {
                return _HeaderCode;
            }
            set
            {
                SetPropertyValue("HeaderCode", ref _HeaderCode, value);
            }
        }
        [DisplayOrder(Order = 9)]
        public int Position
        {
            get
            {
                return _Position;
            }
            set
            {
                SetPropertyValue("Position", ref _Position, value);
            }
        }
        [DisplayOrder(Order = 4)]
        public int Length
        {
            get
            {
                return _Length;
            }
            set
            {
                SetPropertyValue("Length", ref _Length, value);
            }
        }
        [DisplayOrder(Order = 5)]
        public bool IsTabDelimited
        {
            get
            {
                return _IsTabDelimited;
            }
            set
            {
                SetPropertyValue("IsTabDelimited", ref _IsTabDelimited, value);
            }
        }
        [DisplayOrder(Order = 6)]
        public string TabDelimitedString
        {
            get
            {
                return _TabDelimitedString;
            }
            set
            {
                SetPropertyValue("TabDelimitedString", ref _TabDelimitedString, value);
            }
        }


        [DisplayOrder(Order = 8)]
        public int Order
        {
            get
            {
                return _Order;
            }
            set
            {
                SetPropertyValue("Order", ref _Order, value);
            }
        }

    }
}
