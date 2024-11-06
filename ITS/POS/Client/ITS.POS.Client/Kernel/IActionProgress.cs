using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Kernel
{
    public delegate void ActionProgressStartedEventHandler(object sender, int minimum, int maximum, string initialMessage);
    public delegate void ActionProgressChangedEventHandler(object sender, int currentvalue, string message);
    public delegate void ActionProgressCompletedEventHandler(object sender,string messageAppend, object result);


    public interface IActionProgress
    {
        event ActionProgressStartedEventHandler ProgressStarted;
        event ActionProgressChangedEventHandler ProgressChanged;
        event ActionProgressCompletedEventHandler ProgressCompleted;

        void DoWork();
    }
}
