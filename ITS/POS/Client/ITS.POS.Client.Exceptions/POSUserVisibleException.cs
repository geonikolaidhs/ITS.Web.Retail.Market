using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace ITS.POS.Client.Exceptions
{
    public class POSUserVisibleException: POSException
    {
        public POSUserVisibleException()
        {
            
        }
        public POSUserVisibleException(string message)
            : base(message)
        {
            
        }
        public POSUserVisibleException(string message, Exception innerException)
            : base(message, innerException)
        {
            
        }
        protected POSUserVisibleException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            
        }

    }
}
