using DevExpress.XtraWaitForm;
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
    public partial class frmWaitForm : WaitForm
    {
        public frmWaitForm()
        {

            InitializeComponent();
            progressPanel1.Caption = ITS.POS.Resources.POSClientResources.PLEASE_WAIT;
            progressPanel1.Description = ITS.POS.Resources.POSClientResources.PLEASE_WAIT;

        }

    }
}
