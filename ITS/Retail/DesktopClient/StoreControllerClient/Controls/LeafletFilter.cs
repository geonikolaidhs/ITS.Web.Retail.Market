using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.Retail.Common.ViewModel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using ITS.Retail.ResourcesLib;
using DevExpress.Data.Filtering;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Controls
{
    public partial class LeafletFilter : BaseFilterControl
    {
        public LeafletFilter()
        {
            InitializeComponent();
            SecondaryFilterControl = new LeafletSecondaryFilterControl();
            SecondaryFilterControl.ParentFilterControl = this;

            this.SearchFilter = new LeafletSearchFilter();
            this.SearchFilter.Set();
            Expandable = false;
        }

        private void lueIsActive_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            this.DeleteValue(sender as BaseEdit, e);
        }
    }
}
