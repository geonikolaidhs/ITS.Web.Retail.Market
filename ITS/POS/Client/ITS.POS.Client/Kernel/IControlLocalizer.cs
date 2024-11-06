using ITS.Retail.Platform.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ITS.POS.Client.Kernel
{
    public interface IControlLocalizer : IKernelModule
    {
        void LocalizeControl(Control control);

        void ClearDisposedObjectsFromCache(bool clearAll);
    }
}
