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

namespace ITS.Retail.DesktopClient.StoreControllerClient.Controls
{
    public partial class SupplierFilter : BaseFilterControl
    {
        public SupplierFilter()
        {
            InitializeComponent();
            SecondaryFilterControl = new SupplierSecondaryFilterControl();
            SecondaryFilterControl.ParentFilterControl = this;
            this.SearchFilter = new SupplierSearchFilter() { Owner = Program.Settings.StoreControllerSettings.Owner.Oid };
            this.SearchFilter.Set();
        }

        private void textEditCreatedOn_ButtonPressed(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            DeleteValue(sender as BaseEdit, e);
        }

        private void textEditUpdatedOn_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            DeleteValue(sender as BaseEdit, e);
        }
    }
}
