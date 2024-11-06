using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Common
{
    public abstract class ExtraReportScripts
    {
        public abstract void OnAfterLoad(XtraReportBaseExtension report);
    }
}
