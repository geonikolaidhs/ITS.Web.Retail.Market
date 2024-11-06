using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Common.ViewModel.Importable
{
    public class ImportResult
    {
        public ImportResult()
        {
            Errors = new Dictionary<string, string>();
        }

        public bool Applicable { get; set; }

        public bool Successful { get; set; }

        public int FieldsSuccessful { get; set; }

        public Dictionary<string, string> Errors { get; set; }
    }
}
