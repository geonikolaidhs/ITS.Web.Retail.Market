using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;

namespace ITS.Licensing.LicenseModel
{
    public class User : BasicObj
    {
        public User()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public User(Session session)
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

        private string _UserName;
        [Indexed(Unique = true)]
        public string UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                SetPropertyValue("UserName", ref _UserName, value);
            }
        }
        private string _FullName;
        public string FullName
        {
            get
            {
                return _FullName;
            }
            set
            {
                SetPropertyValue("FullName", ref _FullName, value);
            }
        }
        private string _Password;
        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                SetPropertyValue("Password", ref _Password, value);
            }
        }

        //TODO
        //private Role _Role;
        //[Association("Role-Users")]
        //public Role Role
        //{
        //    get
        //    {
        //        return _Role;
        //    }
        //    set
        //    {
        //        SetPropertyValue("Role", ref _Role, value);
        //    }
        //}

        //private string _Key;
        //public string Key
        //{
        //    get
        //    {
        //        return _Key;
        //    }
        //    set
        //    {
        //        SetPropertyValue("Key", ref _Key, value);
        //    }
        //}
    }
}
