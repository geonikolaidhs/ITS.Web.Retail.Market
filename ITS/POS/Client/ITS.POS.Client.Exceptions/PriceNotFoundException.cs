using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Exceptions
{
    public class PriceNotFoundException : POSException
    {
        public PriceNotFoundException()
        {
            
        }
        public PriceNotFoundException(string message)
            : base(message)
        {
            
        }
        public PriceNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
            
        }

    }
}
