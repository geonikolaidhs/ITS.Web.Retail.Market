using DevExpress.Xpo;
using ITS.POS.Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA_Model.NonPersistant
{
    [NonPersistent]
    public class CustomField:BaseObj
    {
        public CustomField()
        {

        }

        public CustomField(Session session)
            : base(session)
        {

        }

        public string FieldName { get; set; }
        public string Label { get; set; }
        [NonPersistent]
        public CustomEnumerationDefinition CustomEnumeration { get; set; }
    }
}