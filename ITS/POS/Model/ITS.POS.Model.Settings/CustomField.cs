using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Model.Settings
{
    [NonPersistent]
    public class CustomField : BaseObj
    {
         public CustomField()
        {

        }

         public CustomField(Session session)
            : base(session)
        {

        }

        // Fields...
        private string _FieldName;
        private string _Label;
        private Guid _CustomEnumeration;

        public string Label
        {
            get
            {
                return _Label;
            }
            set
            {
                SetPropertyValue("Label", ref _Label, value);
            }
        }

        //[Indexed("PaymentMethod;GCRecord", Unique = true)]
        public string FieldName
        {
            get
            {
                return _FieldName;
            }
            set
            {
                SetPropertyValue("FieldName", ref _FieldName, value);
            }
        }


        public Guid CustomEnumeration
        {
            get
            {
                return _CustomEnumeration;
            }
            set
            {
                SetPropertyValue("CustomEnumeration", ref _CustomEnumeration, value);
            }
        }
    }
}
