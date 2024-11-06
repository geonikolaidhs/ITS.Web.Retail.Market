using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.Retail.Common.ViewModel;
using ITS.Retail.ResourcesLib;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Controls
{
    public partial class CustomerFilter : BaseFilterControl
    {
        public CustomerFilter()
        {
            InitializeComponent();
            SecondaryFilterControl = new CustomerSecondaryFilterControl();
            SecondaryFilterControl.ParentFilterControl = this;

            this.SearchFilter = new CustomerSearchFilter() {Owner = Program.Settings.StoreControllerSettings.Owner.Oid };
            this.SearchFilter.Set();

            SetLookups();
        }

        private void SetLookups()
        {

            this.lueIsActive.Properties.DataSource = new Dictionary<string, bool?>() { { "", null }, { Resources.Yes, true }, { Resources.No, false } };
            this.lueIsActive.Properties.DisplayMember = "Key";
            this.lueIsActive.Properties.ValueMember = "Value";
            this.lueIsActive.Properties.Columns.Add(new LookUpColumnInfo("Key", Resources.Value));

        }

        private void textEditNewCustomersFrom_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            DeleteValue(sender as BaseEdit, e);
        }

        private void textEditUpdatedOn_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            DeleteValue(sender as BaseEdit, e);
        }
    }
}