using ITS.POS.Client.Kernel;
using ITS.POS.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ITS.POS.Client.Forms
{
    public partial class frmReprintZ : frmInputFormBase
    {
        public bool UseDateFilter { get; set; }

        public int FromZFilter
        {
            get
            {
                int value;
                int.TryParse(edtFromZ.Text, out value);
                return value;
            }
        }

        public int ToZFilter
        {
            get
            {
                int value;
                int.TryParse(edtToZ.Text, out value);
                return value;
            }
        }

        public DateTime FromZDateFilter
        {
            get
            {
                return edtFromDate.DateTime;
            }
        }

        public DateTime ToZDateFilter
        {
            get
            {
                return edtToDate.DateTime;
            }
        }

        public frmReprintZ(bool useDateFilter, IPosKernel Kernel)
            : base(Kernel)
        {
            InitializeComponent();
            UseDateFilter = useDateFilter;
            lblTitle.Text = POSClientResources.REPRINT_Z_REPORTS;
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();

            if (this.UseDateFilter)
            {
                this.lblFromFilter.Text = POSClientResources.FROM_DATE + ":";
                this.lblToFilter.Text = POSClientResources.TO_DATE + ":";
                edtFromZ.Visible = false;
                edtToZ.Visible = false;
                edtFromDate.Visible = true;
                edtToDate.Visible = true;
                edtFromDate.DateTime = DateTime.Now;
                edtToDate.DateTime = DateTime.Now;
                if (config.UsesTouchScreen)
                {
                    edtFromDate.Properties.CalendarView = DevExpress.XtraEditors.Repository.CalendarView.TouchUI;
                    edtToDate.Properties.CalendarView = DevExpress.XtraEditors.Repository.CalendarView.TouchUI;
                }
                else
                {
                    edtFromDate.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
                    edtFromDate.Properties.Mask.UseMaskAsDisplayFormat = true;
                    edtFromDate.Properties.Mask.EditMask = "([012]?[1-9]|[123]0|31)/(0?[1-9]|1[012])/([123][0-9])?[0-9][0-9]";

                    edtToDate.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
                    edtToDate.Properties.Mask.UseMaskAsDisplayFormat = true;
                    edtToDate.Properties.Mask.EditMask = "([012]?[1-9]|[123]0|31)/(0?[1-9]|1[012])/([123][0-9])?[0-9][0-9]";
                }
            }
            else
            {
                this.lblFromFilter.Text = POSClientResources.FROM_Z + ":";
                this.lblToFilter.Text = POSClientResources.TO_Z + ":";
                //edtFromZ.Visible = true;
                //edtToZ.Visible = true;
                //edtFromDate.Visible = false;
                //edtToDate.Visible = false;
            }
        }

        private void frmReprintZ_Load(object sender, EventArgs e)
        {

        }

        private void edtFromZ_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.edtToZ.Focus();
            }
        }

        private void edtToZ_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.btnOK.PerformClick();
            }
        }

        private void frmReprintZ_Shown(object sender, EventArgs e)
        {
            if (this.UseDateFilter)
            {
                this.edtFromDate.Focus();
            }
            else
            {
                this.edtFromZ.Focus();
            }
        }

        private void edtFromDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.edtToDate.Focus();
            }
        }

        private void edtToDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.btnOK.PerformClick();
            }
        }
    }
}
