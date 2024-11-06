using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Data.Filtering;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Helpers;
using ITS.POS.Model.Settings;
using ITS.POS.Resources;
using ITS.POS.Hardware;
using ITS.POS.Hardware.Common;
using ITS.POS.Client.Kernel;

namespace ITS.POS.Client.Forms
{
    public partial class frmElevatePrivileges : frmInputFormBase
    {
        public eKeyStatus RequiredKeyStatus { get; private set; }

        public frmElevatePrivileges(eKeyStatus requiredKeyStatus,IPosKernel kernel)
            : base(kernel)
        {
            InitializeComponent();
            tePassword.EditValue = "";
            teUserName.EditValue = "";

            RequiredKeyStatus = requiredKeyStatus;
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            if (config.UsesKeyLock)
            {
                this.lblMessage.Text = POSClientResources.TURN_KEY_TO_POSITION + " " + (int)this.RequiredKeyStatus;
                this.lblMessage.Appearance.TextOptions.VAlignment = VertAlignment.Default;
                this.teUserName.Visible = false;
                this.tePassword.Visible = false;
                this.lblUserName.Visible = false;
                this.lblPassword.Visible = false;
            }
            else
            {
                this.lblMessage.Appearance.TextOptions.VAlignment = VertAlignment.Top;
                this.lblMessage.Text = POSClientResources.HIGHER_LEVEL_USER_IS_REQUIRED;
                this.teUserName.Visible = true;
                this.tePassword.Visible = true;
                this.lblUserName.Visible = true;
                this.lblPassword.Visible = true;

                
                
            }
        }

        private void frmElevatePrivileges_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Escape)
            //{
            //    this.Invoke(new MethodInvoker(delegate { this.DialogResult = System.Windows.Forms.DialogResult.Cancel; Close(); }));
            //}
            //if(GlobalContext.UsesKeyLock)
            //{
            //    IEnumerable<KeyLock> indirectKeylocks = GlobalContext.DeviceManager.Devices.Where(x => typeof(KeyLock).IsAssignableFrom(x.GetType()) && x.ConType == ConnectionType.INDIRECT).Cast<KeyLock>();

            //    foreach (KeyLock keylock in indirectKeylocks)
            //    {
            //        e.Handled = keylock.IndirectConnectionHandleKey(e);
            //    }

            //    if (e.Handled)
            //    {
            //        return;
            //    }
            //}
        }

        private void OnKeyStatusChange(eKeyStatus newKeyStatus)
        {
            if (newKeyStatus == RequiredKeyStatus)
            {
                this.Invoke(new MethodInvoker(delegate { this.DialogResult = System.Windows.Forms.DialogResult.OK; Close(); }));
            }
        }

        private void frmElevatePrivileges_Load(object sender, EventArgs e)
        {
            IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();
            deviceManager.OnKeyStatusChange += OnKeyStatusChange;
        }

        private void frmElevatePrivileges_FormClosed(object sender, FormClosedEventArgs e)
        {
            IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();
            deviceManager.OnKeyStatusChange -= OnKeyStatusChange;
            this.teUserName.HideTouchPad();
            this.tePassword.HideTouchPad();
        }

        private void teUserName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                tePassword.Focus();
            }
        }

        private void tePassword_KeyDown(object sender, KeyEventArgs e)
        {
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            IFormManager formManager = Kernel.GetModule<IFormManager>();
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            if (e.KeyCode == Keys.Enter)
            {
                User user = sessionManager.FindObject<User>(CriteriaOperator.And(new BinaryOperator("POSUserName", teUserName.EditValue.ToString()),
                       new BinaryOperator("POSPassword", tePassword.EditValue.ToString())));
                if (user == null)
                {
                    formManager.ShowMessageBox(POSClientResources.USER_NOT_FOUND);
                    //MessageBox.Show(POSClientResources.USER_NOT_FOUND);
                    teUserName.Text = "";
                    tePassword.Text = "";
                    teUserName.Focus();
                }
                else if ((int)user.POSUserLevel < (int)RequiredKeyStatus)
                {
                    formManager.ShowMessageBox(POSClientResources.USER_NOT_ENOUGH_PRIVILEGES);
                    //MessageBox.Show(POSClientResources.USER_NOT_ENOUGH_PRIVILEGES);
                    teUserName.Text = "";
                    tePassword.Text = "";
                    teUserName.Focus();
                }
                else
                {
                    this.Invoke(new MethodInvoker(delegate { this.DialogResult = System.Windows.Forms.DialogResult.OK; Close(); }));
                }
            }
        }
    }
}
