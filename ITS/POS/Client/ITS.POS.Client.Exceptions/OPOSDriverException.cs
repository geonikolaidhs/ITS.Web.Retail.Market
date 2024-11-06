using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Client.Exceptions
{
    public class OPOSDriverException : POSException
    {
        private OPOSErrorCode _errorCode;
        public OPOSErrorCode ErrorCode
        {
            get
            {
                return _errorCode;
            }
        }

        private int _extendedErrorCode;
        public int ExtendedErrorCode
        {
            get
            {
                return _extendedErrorCode;
            }
        }

        public OPOSDriverException(OPOSErrorCode errorCode,int extendedErrorCode) : base(""+errorCode + " Extended Code: " +extendedErrorCode)
        {
            _errorCode = errorCode;
            _extendedErrorCode = extendedErrorCode;
        }

        public OPOSDriverException(OPOSErrorCode errorCode, int extendedErrorCode, string message)
            : base(message)
        {
            _errorCode = errorCode;
            _extendedErrorCode = extendedErrorCode;
        }

    }
}
