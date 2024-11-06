using System;
using DevExpress.Xpo;
using ITS.POS.Model.Settings;
using ITS.POS.Model.Versions;


namespace ITS.POS.Model.Versions
{
    public class TableVersions : BaseObj
    {
        public TableVersions()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public TableVersions(Session session)
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
        private string _EntityName;
        [Indexed("GCRecord", Unique=false)]
        public string EntityName
        {
            get
            {
                return _EntityName;
            }
            set
            {
                SetPropertyValue("EntityName", ref _EntityName, value);
            }
        }

        private DateTime _Number;
        private bool _ForceReload;
        public DateTime Number 
        {
            get
            {
                return _Number;
            }
            set
            {
                SetPropertyValue("Number", ref _Number, value);
            }
        }

        public bool ForceReload
        {
            get
            {
                return _ForceReload;
            }
            set
            {
                SetPropertyValue("ForceReload", ref _ForceReload, value);
            }
        }

       
    }
}
