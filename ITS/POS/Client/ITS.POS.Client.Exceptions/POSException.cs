using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ITS.POS.Client.Exceptions
{
    public class POSException: Exception
    {
        public POSException()
        {
            
        }
        public POSException(string message)
            : base(message)
        {
            
        }
        public POSException(string message, Exception innerException)
            : base(message, innerException)
        {
            
        }
        protected POSException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            
        }

    }
}
