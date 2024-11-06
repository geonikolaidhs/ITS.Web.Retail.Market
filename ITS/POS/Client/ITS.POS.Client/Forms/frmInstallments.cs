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
    public partial class frmInstallments : frmInputFormBase, INotifyPropertyChanged
    {
        private uint _Installments;

        public uint Installments { 
            get 
            {
                return _Installments;
            } 
            set 
            {
                _Installments = value;
                if(PropertyChanged !=null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Installments"));
                }
            } 
        }


        protected frmInstallments()
        {
            InitializeComponent();
        }

        public frmInstallments(IPosKernel kernel)
            : base(kernel)
        {
            InitializeComponent();
            btnOK.Text = POSClientResources.OK;
            btnCancel.Text = POSClientResources.CANCEL;
            lblTitle.Text = POSClientResources.INSTALLMENTS;
            Installments = 1;
            edtInstallmetns.DataBindings.Add("EditValue", this, "Installments");

        }

        private void edtInstallmetns_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnOK.PerformClick();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
