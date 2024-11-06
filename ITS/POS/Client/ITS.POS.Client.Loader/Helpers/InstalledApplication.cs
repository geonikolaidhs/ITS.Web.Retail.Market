using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Loader.Helpers
{
    public class InstalledApplication
    {
        public string Name { get; set; }
        public string AssignmentType { get; set; }
        public string Caption { get; set; }
        public string Description { get; set; }
        public string IdentifyingNumber { get; set; }
        public string InstallLocation { get; set; }
        public string InstallDate { get; set; }
        public string Version { get; set; }

    }
}
