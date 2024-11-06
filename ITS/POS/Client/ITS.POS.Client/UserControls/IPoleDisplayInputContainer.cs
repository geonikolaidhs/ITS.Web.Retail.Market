// -----------------------------------------------------------------------
// <copyright file="IPoleDisplayInputContainer.cs" company="Hewlett-Packard Company">
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
    /// Marks a control as a container of IPoleDisplayInputs
    /// </summary>
    public interface IPoleDisplayInputContainer
    {
        bool ShowTitle { get; }
        bool ShowInputName { get; }
        string Title { get; }
        event FocusedPoleDisplayInputChangedEventHandler FocusedPoleDisplayInputChanged;
        void OnFocusedPoleDisplayInputChanged(object sender, FocusedPoleDisplayInputChangedArgs args);

    }
}
