using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ITS.Retail.Model.Exceptions
{
    public class SignatureFailureException : Exception
    {
        public SignatureFailureException()
        {
            
        }
        public SignatureFailureException(string message)
            : base(message)
        {
            
        }
        public SignatureFailureException(string message, Exception innerException)
            : base(message, innerException)
        {
            
        }
        protected SignatureFailureException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            
        }
    }
}
