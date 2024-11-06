using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.UserControls
{
    /// <summary>
    /// Marks an input that can receive scan results
    /// </summary>
    public interface IScannerInput
    {
        void SetText(string text);
        void SendEnter();
    }
}
