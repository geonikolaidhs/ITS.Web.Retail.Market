using DevExpress.XtraGrid.Columns;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.UserControls
{
    [DesignTimeVisible(true)]
    [Obsolete("DO NOT RENAME OR CHANGE NAMESPACE FOR BACKWARDS COMPATIBILITY!")]
    public partial class CustomGridColumn : GridColumn
    {
        public CustomGridColumn()
        {
            InitializeComponent();
        }

        public CustomGridColumn(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
