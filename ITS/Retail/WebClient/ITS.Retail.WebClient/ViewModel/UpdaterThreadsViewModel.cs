using ITS.Retail.Common.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace ITS.Retail.WebClient.ViewModel
{
    public class UpdaterThreadsViewModel 
    {
       public string ThreadName { get; set; } 
       public string ThreadState { get; set; }
        public UpdaterThreadsViewModel(string Threadname,string  Threadstate)
        {
            ThreadName = Threadname;
            ThreadState = Threadstate;
        }
    }
}