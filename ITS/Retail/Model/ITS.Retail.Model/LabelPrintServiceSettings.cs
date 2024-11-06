using DevExpress.Xpo;
using ITS.Retail.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    public class LabelPrintServiceSettings : PrintServiceSettings
    {
        public LabelPrintServiceSettings()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public LabelPrintServiceSettings(Session session)
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


        private Label _Label;

        public Label Label
        {
            get
            {
                return _Label;
            }
            set
            {
                if (_Label == value)
                {
                    return;
                }

                Label previousLabel = _Label;
                _Label = value;

                if (IsLoading)
                {
                    return;
                }

                if (previousLabel != null && previousLabel.PrintServiceSettings == this)
                {
                    previousLabel.PrintServiceSettings = null;
                }


                if (_Label != null)
                {
                    _Label.PrintServiceSettings = this;
                }

                OnChanged("Label");
            }
        }

        public string Description
        {
            get
            {
                string description = String.Empty;
                if ( this.Label != null )
                {
                    description = this.Label.Description + Environment.NewLine;
                }
                EthernetDeviceSettings ethernetDeviceSettings = this.RemotePrinterService.DeviceSettings as EthernetDeviceSettings;
                if ( this.RemotePrinterService != null && ethernetDeviceSettings != null)
                {
                    description += String.Format( "{0} {1}" ,ethernetDeviceSettings.IPAddress , ethernetDeviceSettings.Port);
                }
                return description;
            }
        }
    }
}
