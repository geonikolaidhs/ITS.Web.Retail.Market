using DevExpress.Xpo;

namespace ITS.POS.Model.Settings
{
    public class BarcodeType : BaseObj
    {
        public BarcodeType()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public BarcodeType(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        private string _Mask;
        private bool _HasMixInformation;
        private bool _PrefixIncluded;
        private bool _NonSpecialCharactersIncluded;
        private bool _IsWeighed;
        private string _Prefix;
        private string _EntityType;

        public bool IsWeighed     // αφορά barcode ζυγιζόμενου
        {
            get
            {
                return _IsWeighed;
            }
            set
            {
                SetPropertyValue("IsWeighed", ref _IsWeighed, value);
            }
        }

        
        [Indexed(Unique = false)]
        public string Prefix
        {
            get
            {
                return _Prefix;
            }
            set
            {
                SetPropertyValue("Prefix", ref _Prefix, value);
            }
        }

        
        public string Mask
        {
            get
            {
                return _Mask;
            }
            set
            {
                SetPropertyValue("Mask", ref _Mask, value);
            }
        }


        public bool HasMixInformation
        {
            get
            {
                return _HasMixInformation;
            }
            set
            {
                SetPropertyValue("HasMixInformation", ref _HasMixInformation, value);
            }
        }

        public int Length
        {
            get
            {
                return (Prefix == null ? 0 : Prefix.Length) + (Mask == null ? 0 : Mask.Length);
            }
        }

        public bool PrefixIncluded
        {
            get
            {
                return _PrefixIncluded;
            }
            set
            {
                SetPropertyValue("PrefixIncluded", ref _PrefixIncluded, value);
            }
        }


        public bool NonSpecialCharactersIncluded
        {
            get
            {
                return _NonSpecialCharactersIncluded;
            }
            set
            {
                SetPropertyValue("NonSpecialCharactersIncluded", ref _NonSpecialCharactersIncluded, value);
            }
        }

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
