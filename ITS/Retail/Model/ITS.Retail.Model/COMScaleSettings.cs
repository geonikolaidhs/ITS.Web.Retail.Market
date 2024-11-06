using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{
    public class COMScaleSettings : COMDeviceSettings
    {

        public COMScaleSettings()
            : base() {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public COMScaleSettings(Session session)
            : base(session) {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            // Place here your initialization code.
            this.ScaleReadPattern = @"AB\r(\d{3}.\d{3})\r\d{3}.\d{3}\r";
        }


        public ScaleCommunicationType CommunicationType
        {
            get
            {
                return _CommunicationType;
            }
            set
            {
                SetPropertyValue("CommunicationType", ref _CommunicationType, value);
            }
        }

        public string ScaleReadPattern
        {
            get
            {
                return _ScaleReadPattern;
            }
            set
            {
                SetPropertyValue("ScaleReadPattern", ref _ScaleReadPattern, value);
            }
        }

        private ScaleCommunicationType _CommunicationType;
        private string _ScaleReadPattern;
    }
}
