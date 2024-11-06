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
    public partial class frmInsertEDPSAuthID : frmInputFormBase, INotifyPropertyChanged
    {
        private string _AuthorizationKey;

        protected frmInsertEDPSAuthID()
        {
            InitializeComponent();
        }

        public frmInsertEDPSAuthID(IPosKernel kernel): base(kernel)
        {
            InitializeComponent();
            lblTitle.Text = POSClientResources.AUTHORIZATION_KEY;
            btnOK.Text = POSClientResources.OK;
            btnCancel.Text = POSClientResources.CANCEL;
            edtAuthorizationKey.DataBindings.Add("EditValue", this, "AuthorizationKey");

        }

        public string AuthorizationKey
        {
            get
            {
                return _AuthorizationKey;
            }
            set
            {
                _AuthorizationKey = value;
                if(PropertyChanged !=null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("AuthorizationKey"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
