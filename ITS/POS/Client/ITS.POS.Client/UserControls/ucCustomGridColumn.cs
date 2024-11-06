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
    public partial class ucCustomGridColumn : GridColumn
    {
        public ucCustomGridColumn()
        {
            InitializeComponent();
        }

        public ucCustomGridColumn(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
