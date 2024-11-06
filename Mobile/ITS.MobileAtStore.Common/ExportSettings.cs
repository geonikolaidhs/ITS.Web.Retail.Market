using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.MobileAtStore.Common
{
    public class ExportSettings : IXmlSubitems
    {
        public ConnectionSettings ExportConnectionSettings { get; set; }
        public ExportMode ExportMode { get; set; }
        public List<FileExportSetting> FileExportSettings { get; set; }

        public bool RemoveZeroQtyLines { get; set; }

        public ExportSettings()
        {            
            FileExportSettings = new List<FileExportSetting>();
        }
    }
}
