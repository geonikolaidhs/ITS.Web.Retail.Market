using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Client.Exceptions
{
    public class InvalidFiscalMethodException : POSException
    {
        public eFiscalMethod ExpectedFiscalMethods { get; set; }
        public eFiscalMethod CurrentFiscalMethod { get; set; }

        public InvalidFiscalMethodException(eFiscalMethod currentFiscalMethod, eFiscalMethod expectedFiscalMethods)
            : base("Invalid Fiscal Method '" + currentFiscalMethod + "'. Expected '" + expectedFiscalMethods + "'")
        {
            ExpectedFiscalMethods = expectedFiscalMethods;
            CurrentFiscalMethod = currentFiscalMethod;
        }
    }
}
