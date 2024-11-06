using DevExpress.CodeParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace ITS.Retail.WebClient.AuxillaryClasses
{
    public  class ThreadHandleEvent
    {
        public ManualResetEvent PauseEventStartUpdateThread { get; set; }
        public  ManualResetEvent PauseEventPostSyncInfoThread { get; set; }
        public  ManualResetEvent PauseEventPostRecordsThread { get; set; }
        public bool ExceptionThrownStartUpdateThread { get; set; }
        public bool ExceptionThrownPostSyncInfoThread { get; set; }
        public bool ExceptionThrownPostRecordsThread { get; set; }
    }

    
}