using System;
using DevExpress.Xpo;

namespace ITS.Retail.Model
{

    public class FieldPermission : Permission
    {
        public FieldPermission()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public FieldPermission(Session session)
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

        private TablePermission _TableName;
        [Association("TablePermission-FieldPermissions")]
        public TablePermission TableName
        {
            get
            {
                return _TableName;
            }
            set
            {
                SetPropertyValue("TableName", ref _TableName, value);
            }
        }
        private string _FieldName;
        public string FieldName
        {
            get
            {
                return _FieldName;
            }
            set
            {
                SetPropertyValue("FieldName", ref _FieldName, value);
            }
        }
    }

}