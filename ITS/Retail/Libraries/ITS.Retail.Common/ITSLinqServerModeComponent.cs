using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Data.Linq;
using DevExpress.Xpo;
using DevExpress.XtraReports.UserDesigner.Native;

namespace ITS.Retail.Common
{
    public class ITSLinqServerModeComponent : LinqServerModeSource
    {

        Type _queriableType;
        [EditorAttribute(typeof(ITSTypeEditor), typeof(UITypeEditor))]
        public Type QueriableType
        {
            get
            {
                return _queriableType;
            }
            set 
                {
                    _queriableType = value;
                    if (this.DesignMode)
                    {
                       // XtraReportExtension f = (this.Container as XRDesignerHost).RootComponent as XtraReportExtension;
                        //this.QueryableSource = f.SupportingIQueriables.Items.FirstOrDefault(g => g.ElementType == _queriableType);
                    }
                }
        }
    }
}
