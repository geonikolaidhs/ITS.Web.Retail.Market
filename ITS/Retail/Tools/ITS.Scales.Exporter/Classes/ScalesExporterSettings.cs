using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Scales.Exporter.Classes
{
    public class ScalesExporterSettings
    {
        public string ServiceURL { get; set; }

        public int RepeatTimeInMinutes { get; set; }

        public int WebServiceCallTimeOut { get; set; }

        public bool Autorun { get; set; }
    }
}
