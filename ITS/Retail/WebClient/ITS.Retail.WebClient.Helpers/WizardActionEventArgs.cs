using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.WebClient.Helpers
{
    public class WizardActionEventArgs
    {
        public Wizard Wizard { get; set;}
        public eWizardAction WizardAction { get; set; }
        public bool CancelAction { get; set; }
    }
}
