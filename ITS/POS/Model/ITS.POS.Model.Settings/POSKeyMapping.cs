using System;
using System.Windows.Forms;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Model.Settings
{
    
    public class POSKeyMapping : BaseObj
    {
        public POSKeyMapping()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public POSKeyMapping(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        private Guid _POSKeysLayout;
        public Guid POSKeysLayout
        {
            get
            {
                return _POSKeysLayout;
            }
            set
            {
                SetPropertyValue("POSKeysLayout", ref _POSKeysLayout, value);
            }
        }

        private Keys _KeyData;
        [Indexed("POSKeysLayout;GCRecord", Unique = true)]
        public Keys KeyData
        {
            get
            {
                return _KeyData;
            }
            set
            {
                SetPropertyValue("KeyData", ref _KeyData, value);
            }
        }

        private eNotificationsTypes _NotificationType;
        public eNotificationsTypes NotificationType
        {
            get
            {
                return _NotificationType;
            }
            set
            {
                SetPropertyValue("NotificationType", ref _NotificationType, value);
            }
        }

        private eActions _ActionCode;
        public eActions ActionCode
        {
            get
            {
                return _ActionCode;
            }
            set
            {
                SetPropertyValue("ActionCode", ref _ActionCode, value);
            }
        }


        private Keys _RedirectTo;
        public Keys RedirectTo
        {
            get
            {
                return _RedirectTo;
            }
            set
            {
                SetPropertyValue("RedirectTo", ref _RedirectTo, value);
            }
        }

        private string _ActionParameters;
        [Size(SizeAttribute.Unlimited)]
        public string ActionParameters
        {
            get
            {
                return _ActionParameters;
            }
            set
            {
                SetPropertyValue("ActionParameters", ref _ActionParameters, value);
            }
        }

    }
}
