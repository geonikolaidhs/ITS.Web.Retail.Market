using DevExpress.XtraReports.Parameters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ITS.Retail.Common
{
    public class ReportParameterExtension : Parameter
    {
        private bool _multiSelect;
        [DisplayName("Is Multi-Select"), Browsable(false), DefaultValue(false)]
        [Obsolete("Supported officially by DevExpress. Use MultiValue")]
        public bool MultiSelect
        {
            get
            {
                return _multiSelect;
            }
            set
            {
                _multiSelect = value;
                
                ////Backwards Compatibility
                if (_multiSelect)
                {
                    this.MultiValue = true;
                    this._multiSelect = false;
                }
            }
        }

        [DisplayName("Width (px)"), Browsable(true), DefaultValue(null)]
        public int? Width { get; set; }

        [DisplayName("Height (px)"), Browsable(true), DefaultValue(null)]
        public int? Height { get; set; }

        public String SortingProperty { get; set; }

        [Browsable(false)]
        public Component LookUpEditor { get; set; }

        public ReportParameterExtension()
        {
           
        }

    }
}
