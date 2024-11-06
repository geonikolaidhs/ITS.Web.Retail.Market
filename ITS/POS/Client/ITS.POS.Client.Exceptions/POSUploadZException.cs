using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ITS.POS.Client.Exceptions
{
    public class POSUploadZException : Exception
    {
        public POSUploadZException()
        {

        }
        public POSUploadZException(string message)
            : base(message)
        {

        }
        public POSUploadZException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

    }
}
