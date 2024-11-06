using DevExpress.Xpo;
using ITS.POS.Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA_Model.NonPersistant
{
    [NonPersistent]
    public class CustomFieldStorage: BaseObj
    {
        public CustomFieldStorage()
        {

        }
        public CustomFieldStorage(Session session)
            : base(session)
        {

        }
        public DateTime DateField5 { get; set; }
        public DateTime DateField4 { get; set; }
        public DateTime DateField3 { get; set; }
        public DateTime DateField2 { get; set; }
        public DateTime DateField1 { get; set; }
        public decimal DecimalField5 { get; set; }
        public decimal DecimalField4 { get; set; }
        public decimal DecimalField3 { get; set; }
        public decimal DecimalField2 { get; set; }
        public decimal DecimalField1 { get; set; }
        public string StringField5 { get; set; }
        public string StringField4 { get; set; }
        public string StringField3 { get; set; }
        public string StringField2 { get; set; }
        public string StringField1 { get; set; }
        public int IntegerField5 { get; set; }
        public int IntegerField4 { get; set; }
        public int IntegerField3 { get; set; }
        public int IntegerField2 { get; set; }
        public int IntegerField1 { get; set; }
        [NonPersistent]
        public CustomEnumerationValue CustomEnumerationValue1 { get; set; }
        [Persistent("CustomEnumerationValue1")]
        public Guid CustomEnumerationValue1Oid { get; set; }
        [NonPersistent]
        public CustomEnumerationValue CustomEnumerationValue2 { get; set; }
        [Persistent("CustomEnumerationValue2")]
        public Guid CustomEnumerationValue1Oid2 { get; set; }
        public Guid CustomEnumerationValue1Oid3 { get; set; }
        public Guid CustomEnumerationValue1Oid4 { get; set; }
        public Guid CustomEnumerationValue1Oid5 { get; set; }
    }
}