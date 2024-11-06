using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    [Updater(Order = 180,
       Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("MinistryDocumentType", typeof(ResourcesLib.Resources))]
    public class MinistryDocumentType : LookupField
    {
          public MinistryDocumentType()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
          public MinistryDocumentType(Session session)
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

        private string _Code;
        [Indexed("GCRecord", Unique = true)]
        public string Code
        {
            get
            {
                return _Code;
            }
            set
            {
                SetPropertyValue("Code", ref _Code, value);
            }
        }

        private string _Title;
        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                SetPropertyValue("Title", ref _Title, value);
            }
        }

        private string _ShortTitle;
        public string ShortTitle
        {
            get
            {
                return _ShortTitle;
            }
            set
            {
                SetPropertyValue("ShortTitle", ref _ShortTitle, value);
            }
        }


        private eDocumentValueFactor _DocumentValueFactor;
        public eDocumentValueFactor DocumentValueFactor
        {
            get
            {
                return _DocumentValueFactor;
            }
            set
            {
                SetPropertyValue("DocumentValueFactor", ref _DocumentValueFactor, value);
            }
        }

        private bool _IsSupported;
        public bool IsSupported
        {
            get
            {
                return _IsSupported;
            }
            set
            {
                SetPropertyValue("IsSupported", ref _IsSupported, value);
            }
        }

    }
}
