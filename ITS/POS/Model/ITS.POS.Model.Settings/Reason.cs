using DevExpress.Xpo;
using System;

namespace ITS.POS.Model.Settings
{
    public class Reason : Lookup2Fields
    {
        private Guid _Category;

        public Reason()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Reason(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public string CustomDescription
        {
            get
            {
                return this.Code + " - " + this.Description;
            }
        }

        public Guid Category
        {
            get
            {
                return _Category;
            }
            set
            {
                SetPropertyValue("Category", ref _Category, value);
            }
        }
    }
}
