using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ITS.POS.Client.UserControls
{
    /// <summary>
    /// Marks an input control as a pole display input that will be shown at the cashier's pole display
    /// </summary>
    public interface IPoleDisplayInput
    {
        /// <summary>
        /// The text to display at the pole display
        /// </summary>
        string PoleDisplayName { get; set; }
    }
}
