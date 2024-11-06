using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;

namespace ITS.Licensing.LicenseModel
{
    public class Rule : BasicObj
    {
        public Rule()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Rule(Session session)
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

        private String _Entity;
        public String Entity
        {
            get
            {
                return _Entity;
            }
            set
            {
                SetPropertyValue("Entity", ref _Entity, value);
            }
        }

        private String _Field;
        public String Field
        {
            get
            {
                return _Field;
            }
            set
            {
                SetPropertyValue("Field", ref _Field, value);
            }
        }

        private BinaryOperatorType _Operator;
        public BinaryOperatorType Operator
        {
            get
            {
                return _Operator;
            }
            set
            {
                SetPropertyValue("Operator", ref _Operator, value);
            }
        }

        private String _Value;
        public String Value
        {
            get
            {
                return _Value;
            }
            set
            {
                SetPropertyValue("Value", ref _Value, value);
            }
        }

        private String _Description;
        public String Description
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

        private ApplicationInfo _ApplicationInfo;
        [Association("Rule-ApplicationInfo")]
        public ApplicationInfo ApplicationInfo
        {
            get
            {
                return _ApplicationInfo;
            }
            set
            {
                SetPropertyValue("ApplicationInfo", ref _ApplicationInfo, value);
            }
        }

    }
}
