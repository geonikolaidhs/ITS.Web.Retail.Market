// -----------------------------------------------------------------------
// <copyright file="IPoleDisplayLookupInput.cs" company="Hewlett-Packard Company">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace ITS.POS.Client.UserControls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Marks a lookup input as a pole display input that will be shown at the cashier's pole display
    /// </summary>
    public interface IPoleDisplayLookupInput : IPoleDisplayInput
    {
        void AttachOnValueChangedEvent(EventHandler handler);
        void DetachOnValueChangedEvent(EventHandler handler);
        string GetText();
    }
}
