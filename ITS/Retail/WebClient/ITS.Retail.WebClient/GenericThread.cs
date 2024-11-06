using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace ITS.Retail.WebClient
{
    public abstract class GenericThread
    {
        protected Thread thread;
         

        public virtual Thread Thread
        {
            get { return thread; }
        }

        public virtual void Start(object value)
        {
            thread.Start(value);
        }

        public virtual bool IsAlive
        {
            get
            {
                return thread.IsAlive;
            }
        }
    }
}