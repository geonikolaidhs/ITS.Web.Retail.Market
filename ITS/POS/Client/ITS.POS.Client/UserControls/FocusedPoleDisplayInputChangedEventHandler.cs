// -----------------------------------------------------------------------
// <copyright file="FocusChangedEventHandler.cs" company="Hewlett-Packard Company">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace ITS.POS.Client.UserControls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public delegate void FocusedPoleDisplayInputChangedEventHandler(object sender, FocusedPoleDisplayInputChangedArgs args);

    public class FocusedPoleDisplayInputChangedArgs : EventArgs
    {
        public IPoleDisplayInput PreviousControlWithFocus;
        public IPoleDisplayInput CurrentControlWithFocus;

        public FocusedPoleDisplayInputChangedArgs(IPoleDisplayInput previous, IPoleDisplayInput currentControlWithFocus)
        {
            PreviousControlWithFocus = previous;
            CurrentControlWithFocus = currentControlWithFocus;
        }
    }

}
