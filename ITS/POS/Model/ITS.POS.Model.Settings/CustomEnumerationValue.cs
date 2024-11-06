using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Model.Settings
{
    public class CustomEnumerationValue : BaseObj
    {
        public CustomEnumerationValue()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public CustomEnumerationValue(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        // Fields...
        private Guid _CustomEnumerationDefinition;
        private string _Description;
        private int _Ordering;

        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                SetPropertyValue("Description", ref _Description, value);
            }
        }

        public int Ordering
        {
            get
            {
                return _Ordering;
            }
            set
            {
                SetPropertyValue("Ordering", ref _Ordering, value);
            }
        }

        public Guid CustomEnumerationDefinition
        {
            get
            {
                return _CustomEnumerationDefinition;
            }
            set
            {
                SetPropertyValue("CustomEnumerationDefinition", ref _CustomEnumerationDefinition, value);
            }
        }
    }
}
