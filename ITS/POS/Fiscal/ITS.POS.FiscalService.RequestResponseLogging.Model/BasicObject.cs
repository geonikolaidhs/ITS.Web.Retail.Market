using DevExpress.Xpo;
using System;

namespace ITS.POS.FiscalService.RequestResponseLogging.Model
{
    [NonPersistent]
    public class BasicObject : XPCustomObject, IXPObject, IComparable
    {
        public BasicObject()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public BasicObject(Session session)
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
            CreatedOn = DateTime.Now;
        }

        protected override void OnSaving()
        {
            base.OnSaving();
            this.UpdatedOn = DateTime.Now;
        }

        private Guid _Oid;
        [Key]
        public Guid Oid
        {
            get
            {
                return _Oid;
            }
            set
            {
                SetPropertyValue("Oid", ref _Oid, value);
            }
        }
        
        private DateTime _CreatedOn;
        public DateTime CreatedOn
        {
            get
            {
                return _CreatedOn;
            }
            set
            {
                SetPropertyValue("CreatedOn", ref _CreatedOn, value);
            }
        }

        private DateTime _UpdatedOn;
        public DateTime UpdatedOn
        {
            get
            {
                return _UpdatedOn;
            }
            set
            {
                SetPropertyValue("UpdatedOn", ref _UpdatedOn, value);
            }
        }
    }
}