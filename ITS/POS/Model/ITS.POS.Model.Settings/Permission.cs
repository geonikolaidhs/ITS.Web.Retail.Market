using System;
using DevExpress.Xpo;

namespace ITS.POS.Model.Settings
{
    [NonPersistent]
    public class Permission : BaseObj
    {
        public Permission()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public Permission(Session session)
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

        private bool _Visible;
        public bool Visible
        {
            get
            {
                return _Visible;
            }
            set
            {
                SetPropertyValue("Visible", ref _Visible, value);
            }
        }

        private double _CanInsert;
        public double CanInsert
        {
            get
            {
                return _CanInsert;
            }
            set
            {
                SetPropertyValue("CanInsert", ref _CanInsert, value);
            }
        }

        private double _CanUpdate;
        public double CanUpdate
        {
            get
            {
                return _CanUpdate;
            }
            set
            {
                SetPropertyValue("CanUpdate", ref _CanUpdate, value);
            }
        }

        private double _CanDelete;
        public double CanDelete
        {
            get
            {
                return _CanDelete;
            }
            set
            {
                SetPropertyValue("CanDelete", ref _CanDelete, value);
            }
        }


    }
}
