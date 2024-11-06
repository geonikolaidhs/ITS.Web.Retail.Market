using ITS.Retail.Common.ViewModel;
using ITS.Retail.WebClient.AuxillaryClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.WebClient.ViewModel
{
    public enum eOperation
    {
        UPDATE,
        SELECT
    }
    public class DbOperationsViewModel
    {
        [Binding("OperationCheckbox")]
        public eOperation Operation { get; set; }

        [Binding("TableComboBox")]
        public string Table { get; set; }

        public string Select { get; set; }

        public string Set { get; set; }

        public string Where { get; set; }

        public bool UpdatedOnTick { get; set; }

        public string Execute { get; set; }

        public string Preview { get; set; }


    }
}