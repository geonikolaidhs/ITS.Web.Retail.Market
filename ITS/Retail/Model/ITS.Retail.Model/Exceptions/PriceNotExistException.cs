using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ITS.Retail.Model.Exceptions
{
    [Serializable]
    public class PriceNotExistException : Exception
    {
        public PriceNotExistException()
        {
    
        }
        public PriceNotExistException(string message)
            : base(message)
        {
    
        }
        public PriceNotExistException(string message, Exception innerException)
            : base(message, innerException)
        {
    
        }
        protected PriceNotExistException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            base.GetObjectData(info,context);
            if (info != null)
            {
                info.AddValue("_PriceDetail", "ΔΕΝ ΕΧΕΙ ΤΙΜΗ");
                this.PriceDetail = -1;
            }           
        }
        
        private int _PriceDetail;
        public int PriceDetail
        {
            get { return _PriceDetail; }
            set { this._PriceDetail = value; }
        }        
    }
}
