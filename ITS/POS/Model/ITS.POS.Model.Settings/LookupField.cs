using System;
using DevExpress.Xpo;
using System.Collections.Generic;
using System.Linq;

namespace ITS.POS.Model.Settings
{
    [NonPersistent]
    public class LookupField : BaseObj
    {
        public LookupField()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public LookupField(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
            IsDefault = false;
        }

        public LookupField(string description)
            : base()
        {
            _Description = description;
        }

        public LookupField(Session session, string description)
            : base(session)
        {
            _Description = description;
            _IsDefault = false;
        }

        private string _Description;
        [Indexed, Size(300)]
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
        public bool Update(LookupField lf)
        {
            try
            {
                _Description = lf._Description;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool _IsDefault;
        public bool IsDefault
        {
            get
            {
                return _IsDefault;
            }
            set
            {
                SetPropertyValue("IsDefault", ref _IsDefault, value);
            }
        }


    }
}
