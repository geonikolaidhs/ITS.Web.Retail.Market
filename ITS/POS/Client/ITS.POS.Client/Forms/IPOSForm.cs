using ITS.POS.Client.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ITS.POS.Client.Forms
{
    public interface IPOSForm : IDisposable
    {
        IPosKernel Kernel { get; set; }

        void Initialize(IPosKernel kernel);

        DialogResult ShowDialog();

        DialogResult DialogResult { get; set; }

        void Show();
    }
}
