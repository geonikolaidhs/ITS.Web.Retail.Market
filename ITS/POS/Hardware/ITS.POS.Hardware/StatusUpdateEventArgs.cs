using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Hardware
{
    /// <summary>
    /// A class for encapsulating the arguments for a StatusUpdateEvent
    /// </summary>
    public class StatusUpdateEventArgs : EventArgs
    {
        public int Status { get; set; }
    }
}
