// -----------------------------------------------------------------------
// <copyright file="IPoleDisplayTextInput.cs" company="Hewlett-Packard Company">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace ITS.POS.Client.UserControls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;

    /// <summary>
    /// Marks an input as a simple text pole display input that will be shown at the cashier's pole display
    /// </summary>
    public interface IPoleDisplayTextInput : IPoleDisplayInput
    {
        void AttachTextChangedEvent(EventHandler handler);
        void DetachTextChangedEvent(EventHandler handler);
        string GetText();
    }
}
