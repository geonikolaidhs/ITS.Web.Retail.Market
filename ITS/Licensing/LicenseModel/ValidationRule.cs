using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using System.Reflection;
using DevExpress.Data.Filtering;

namespace ITS.Licensing.LicenseModel
{
    public class ValidationRule : BasicObj
    {
         public ValidationRule()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

         public ValidationRule(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
            Oid = Guid.NewGuid();
        }        

        private Rule _Rule;
        public Rule Rule
        {
            get
            {
                return _Rule;
            }
            set
            {
                SetPropertyValue("Rule", ref _Rule, value);
            }
        }

        private Int32 _limit;
        public Int32 limit
        {
            get
            {
                return _limit;
            }
            set
            {
                SetPropertyValue("Limit", ref _limit, value);
            }
        }

        private SerialNumber _SerialNumber;
        [Association("SerialNumber-ValidationRules")]
        public SerialNumber SerialNumber
        {
            get
            {
                return _SerialNumber;
            }
            set
            {
                SetPropertyValue("SerialNumber", ref _SerialNumber, value);
            }
        }
    }
}
