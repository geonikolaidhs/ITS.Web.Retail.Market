﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.WebClient.Helpers.AuxilliaryClasses
{
    public class LogErrorMessage
    {
        public string Result { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Error { get; set; }
    }
}
