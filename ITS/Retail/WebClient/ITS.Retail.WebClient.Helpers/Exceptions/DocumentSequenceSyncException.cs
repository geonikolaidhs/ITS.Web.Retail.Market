using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ITS.Retail.WebClient.Helpers.Exceptions
{
    public class DocumentSequenceSyncException : Exception
    {
        public DocumentSequenceSyncException()
        {

        }
        public DocumentSequenceSyncException(string message)
            : base(message)
        {

        }
        public DocumentSequenceSyncException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
        protected DocumentSequenceSyncException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
