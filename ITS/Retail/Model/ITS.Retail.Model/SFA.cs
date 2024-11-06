using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Enumerations;
using Newtonsoft.Json;

namespace ITS.Retail.Model
{
    [Updater(Order = 1,
    Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class SFA : BaseObj
    {

        public SFA()
      : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public SFA(Session session)
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


        private string _Name;


        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                SetPropertyValue("Name", ref _Name, value);
            }
        }


        private int _ID;

        [Indexed(Unique = true)]
        public int ID
        {
            get
            {
                return _ID;
            }
            set
            {
                SetPropertyValue("ID", ref _ID, value);
            }
        }

        [JsonIgnore]
        [UpdaterIgnoreField]
        [Association("Tablet-DocumentHeaders")]
        public XPCollection<DocumentHeader> DocumentHeaders
        {
            get
            {
                return GetCollection<DocumentHeader>("DocumentHeaders");
            }
        }

    }
}
