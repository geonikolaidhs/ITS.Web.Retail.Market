using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;

namespace ITS.POS.Model.Settings
{
    public class EntityAccessPermision : Permission
    {
        public EntityAccessPermision()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public EntityAccessPermision(Session session)
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

        private string _EntityType;
        public string EntityType
        {
            get
            {
                return _EntityType;
            }
            set
            {
                SetPropertyValue("EntityType", ref _EntityType, value);
            }
        }
    }
}
