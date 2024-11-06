using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace ITS.Retail.WebClient
{
    public delegate void GenericThreadStart<T>(object value);

    public class GenericThreadT<T> : GenericThread
    {
        private GenericThreadStart<T> genericThreadStart;


        public GenericThreadT(GenericThreadStart<T> start)
        {
            genericThreadStart = start;
            thread = new Thread(internalThreadStart);
        }

        private void internalThreadStart(object obj)
        {
            genericThreadStart.Invoke(obj);
        }
    }
}

