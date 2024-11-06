using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ITS.Retail.Model;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.XtraEditors.Controls;
using ITS.Retail.ResourcesLib;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Controls
{
    public partial class PriceCatalogSecondaryFilterControl : BaseSecondaryFilterControl
    {
        public PriceCatalogSecondaryFilterControl()
        {
            InitializeComponent();
            this.lueIsActive.Properties.DataSource = new Dictionary<string, bool?>() { { "", null }, { Resources.Yes, true }, { Resources.No, false } };
            this.lueIsActive.Properties.Columns.Add(new LookUpColumnInfo("Key", Resources.Value));
            this.lueIsActive.Properties.DisplayMember = "Key";
            this.lueIsActive.Properties.ValueMember = "Value";

        }

        private void lueIsActive_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            DeleteValue(sender as BaseEdit, e);
        }
    }
}
