using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ITS.Retail.Platform.Kernel
{
    public class DocumentNegativeTotalException : Exception
    {
        public DocumentNegativeTotalException()
        {

        }
        public DocumentNegativeTotalException(string message)
            : base(message)
        {

        }
        public DocumentNegativeTotalException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
        protected DocumentNegativeTotalException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
