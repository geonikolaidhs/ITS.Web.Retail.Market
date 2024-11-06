using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;

namespace ITS.Retail.Model
{
    public class GridSettings : BaseObj
    {
        public GridSettings()
            : base() {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public GridSettings(Session session)
            : base(session) {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        // Fields...
        private string _GridName;
        private string _GridLayout;
        private User _User;
        [Association("User-GridSettings"), Indexed("GCRecord", Unique = false)]
        public User User
        {
            get
            {
                return _User;
            }
            set
            {
                SetPropertyValue("User", ref _User, value);
            }
        }


        [Size(SizeAttribute.Unlimited)]
        public string GridLayout
        {
            get
            {
                return _GridLayout;
            }
            set
            {
                SetPropertyValue("GridLayout", ref _GridLayout, value);
            }
        }


        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string GridName
        {
            get
            {
                return _GridName;
            }
            set
            {
                SetPropertyValue("GridName", ref _GridName, value);
            }
        }
    }
}
