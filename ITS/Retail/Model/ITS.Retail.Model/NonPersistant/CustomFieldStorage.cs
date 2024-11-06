using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    [NonPersistent]
    public class CustomFieldStorage : BaseObj
    {
        public CustomFieldStorage()
        {
            
        }
        public CustomFieldStorage(Session session)
            : base(session)
        {
            
        }


        // Fields...
        private DateTime _DateField5;
        private DateTime _DateField4;
        private DateTime _DateField3;
        private DateTime _DateField2;
        private DateTime _DateField1;
        private decimal _DecimalField5;
        private decimal _DecimalField4;
        private decimal _DecimalField3;
        private decimal _DecimalField2;
        private decimal _DecimalField1;
        private string _StringField5;
        private string _StringField4;
        private string _StringField3;
        private string _StringField2;
        private string _StringField1;
        private int _IntegerField5;
        private int _IntegerField4;
        private int _IntegerField3;
        private int _IntegerField2;
        private int _IntegerField1;
        private CustomEnumerationValue _CustomEnumerationValue1;
        private CustomEnumerationValue _CustomEnumerationValue2;
        private CustomEnumerationValue _CustomEnumerationValue3;
        private CustomEnumerationValue _CustomEnumerationValue4;
        private CustomEnumerationValue _CustomEnumerationValue5;



        public int IntegerField1
        {
            get
            {
                return _IntegerField1;
            }
            set
            {
                SetPropertyValue("IntegerField1", ref _IntegerField1, value);
            }
        }


        public int IntegerField2
        {
            get
            {
                return _IntegerField2;
            }
            set
            {
                SetPropertyValue("IntegerField2", ref _IntegerField2, value);
            }
        }


        public int IntegerField3
        {
            get
            {
                return _IntegerField3;
            }
            set
            {
                SetPropertyValue("IntegerField3", ref _IntegerField3, value);
            }
        }


        public int IntegerField4
        {
            get
            {
                return _IntegerField4;
            }
            set
            {
                SetPropertyValue("IntegerField4", ref _IntegerField4, value);
            }
        }


        public int IntegerField5
        {
            get
            {
                return _IntegerField5;
            }
            set
            {
                SetPropertyValue("IntegerField5", ref _IntegerField5, value);
            }
        }

        [Size(SizeAttribute.Unlimited)]
        public string StringField1
        {
            get
            {
                return _StringField1;
            }
            set
            {
                SetPropertyValue("StringField1", ref _StringField1, value);
            }
        }

        [Size(SizeAttribute.Unlimited)]
        public string StringField2
        {
            get
            {
                return _StringField2;
            }
            set
            {
                SetPropertyValue("StringField2", ref _StringField2, value);
            }
        }

        [Size(SizeAttribute.Unlimited)]
        public string StringField3
        {
            get
            {
                return _StringField3;
            }
            set
            {
                SetPropertyValue("StringField3", ref _StringField3, value);
            }
        }

        [Size(SizeAttribute.Unlimited)]
        public string StringField4
        {
            get
            {
                return _StringField4;
            }
            set
            {
                SetPropertyValue("StringField4", ref _StringField4, value);
            }
        }

        [Size(SizeAttribute.Unlimited)]
        public string StringField5
        {
            get
            {
                return _StringField5;
            }
            set
            {
                SetPropertyValue("StringField5", ref _StringField5, value);
            }
        }



        public decimal DecimalField1
        {
            get
            {
                return _DecimalField1;
            }
            set
            {
                SetPropertyValue("DecimalField1", ref _DecimalField1, value);
            }
        }


        public decimal DecimalField2
        {
            get
            {
                return _DecimalField2;
            }
            set
            {
                SetPropertyValue("DecimalField2", ref _DecimalField2, value);
            }
        }


        public decimal DecimalField3
        {
            get
            {
                return _DecimalField3;
            }
            set
            {
                SetPropertyValue("DecimalField3", ref _DecimalField3, value);
            }
        }


        public decimal DecimalField4
        {
            get
            {
                return _DecimalField4;
            }
            set
            {
                SetPropertyValue("DecimalField4", ref _DecimalField4, value);
            }
        }


        public decimal DecimalField5
        {
            get
            {
                return _DecimalField5;
            }
            set
            {
                SetPropertyValue("DecimalField5", ref _DecimalField5, value);
            }
        }



        public DateTime DateField1
        {
            get
            {
                return _DateField1;
            }
            set
            {
                SetPropertyValue("DateField1", ref _DateField1, value);
            }
        }


        public DateTime DateField2
        {
            get
            {
                return _DateField2;
            }
            set
            {
                SetPropertyValue("DateField2", ref _DateField2, value);
            }
        }


        public DateTime DateField3
        {
            get
            {
                return _DateField3;
            }
            set
            {
                SetPropertyValue("DateField3", ref _DateField3, value);
            }
        }


        public DateTime DateField4
        {
            get
            {
                return _DateField4;
            }
            set
            {
                SetPropertyValue("DateField4", ref _DateField4, value);
            }
        }


        public DateTime DateField5
        {
            get
            {
                return _DateField5;
            }
            set
            {
                SetPropertyValue("DateField5", ref _DateField5, value);
            }
        }


        public CustomEnumerationValue CustomEnumerationValue1
        {
            get
            {
                return _CustomEnumerationValue1;
            }
            set
            {
                SetPropertyValue("CustomEnumerationValue1", ref _CustomEnumerationValue1, value);
            }
        }

        public CustomEnumerationValue CustomEnumerationValue2
        {
            get
            {
                return _CustomEnumerationValue2;
            }
            set
            {
                SetPropertyValue("CustomEnumerationValue2", ref _CustomEnumerationValue2, value);
            }
        }

        public CustomEnumerationValue CustomEnumerationValue3
        {
            get
            {
                return _CustomEnumerationValue3;
            }
            set
            {
                SetPropertyValue("CustomEnumerationValue3", ref _CustomEnumerationValue3, value);
            }
        }

        public CustomEnumerationValue CustomEnumerationValue4
        {
            get
            {
                return _CustomEnumerationValue4;
            }
            set
            {
                SetPropertyValue("CustomEnumerationValue4", ref _CustomEnumerationValue4, value);
            }
        }

        public CustomEnumerationValue CustomEnumerationValue5
        {
            get
            {
                return _CustomEnumerationValue5;
            }
            set
            {
                SetPropertyValue("CustomEnumerationValue5", ref _CustomEnumerationValue5, value);
            }
        }
    }
}
