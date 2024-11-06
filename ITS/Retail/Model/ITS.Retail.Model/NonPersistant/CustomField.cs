using DevExpress.Xpo;
using ITS.Retail.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model.NonPersistant
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
        private CustomEnumerationDefinition _CustomEnumeration;

        [DescriptionField]
        [DisplayOrder (Order = 3)]
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
        [DisplayOrder(Order = 4)]
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

        public CustomEnumerationDefinition CustomEnumeration
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
