using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using ITS.Retail.Common;
using ITS.Retail.DesktopClient.StoreControllerClient.Helpers;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms
{
    public partial class LoginForm : XtraLocalizedForm
    {
        protected bool ShowMainForm { get; set; }

        //ValidateUser is set to true if we need to assign a Document to a specific User
        protected bool ValidateUser { get; set; }
        
        protected string username { get; set; }
        public LoginForm(bool showMainForm, string username = null)
        {
            this.ShowMainForm = showMainForm;
            this.ValidateUser = username == null ? false : true;
            InitializeComponent();

            this.textEditUserName.Text = username != null ? username : null;
            this.textEditUserName.Enabled = username == null ? true : false;
            this.simpleButtonLogin.Text = (ValidateUser) ? "@@Validate" : "@@Login";
            this.Text = ValidateUser ? "@@Validate" : "@@Login";
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.Activate();
        }

        private void ResetFocus()
        {            
            textEditUserName.Focus();
            textEditUserName.SelectAll();
        }

        private void simpleButtonLogin_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(textEditUserName.Text) || string.IsNullOrEmpty(textEditPassword.Text) )
            {
                XtraMessageBox.Show(Resources.PleaseProvideUserName,Resources.Error,MessageBoxButtons.OK,MessageBoxIcon.Warning);
                ResetFocus();
                return;
            }
            SplashScreenManager.CloseForm(false, true);
            SplashScreenManager.ShowForm(typeof(ITSWaitForm));
            if (!ValidateUser)
            {
                using (UnitOfWork unitofWork = XpoHelper.GetNewUnitOfWork())
                {
                    string message = "";
                    Program.Settings.CurrentUser = Program.Settings.ReadOnlyUnitOfWork.FindObject<User>(new BinaryOperator("UserName", textEditUserName.Text));
                    if (LoginHelper.ValidateUser(textEditUserName.Text, textEditPassword.Text)
                        && UserHelper.UserCanLoginToCurrentStore(Program.Settings.CurrentUser, eApplicationInstance.STORE_CONTROLER,
                        Program.Settings.StoreControllerSettings.Store.Oid, out message)
                        )
                    {

                        if (this.ShowMainForm)
                        {
                            MainForm mainForm = new MainForm();
                            mainForm.Show();
                            SplashScreenManager.CloseForm(false);
                        }
                        Program.UserPassword = textEditPassword.Text;
                        DialogResult = DialogResult.OK;
                        return;
                    }
                }
            }
            else
            {
                using(UnitOfWork unitofWork = XpoHelper.GetNewUnitOfWork())
                {
                    string message = "";
                    User validatedUser = Program.Settings.ReadOnlyUnitOfWork.FindObject<User>(new BinaryOperator("UserName", textEditUserName.Text));
                    if (LoginHelper.ValidateUser(textEditUserName.Text, textEditPassword.Text)
                        && UserHelper.UserCanLoginToCurrentStore(validatedUser, eApplicationInstance.STORE_CONTROLER,
                        Program.Settings.StoreControllerSettings.Store.Oid, out message)
                        )
                    {
                        DialogResult = DialogResult.OK;
                        username = textEditUserName.Text;
                        return;
                    }
                }
            }
            SplashScreenManager.CloseForm(false);
            XtraMessageBox.Show(Resources.Login_Failed, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            ResetFocus();
        }

        private void textEditUserName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                textEditPassword.Focus();
                textEditPassword.SelectAll();
            }
        }

        private void textEditPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                simpleButtonLogin.PerformClick();
            }
        }

        private void textEditUserName_Enter(object sender, EventArgs e)
        {
            textEditUserName.SelectAll();
        }

        private void textEditPassword_Enter(object sender, EventArgs e)
        {
            textEditPassword.SelectAll();
        }

        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(e.CloseReason == CloseReason.UserClosing && !ValidateUser)
            {
                Application.Exit();
            }
        }

        private void LoginForm_Shown(object sender, EventArgs e)
        {
            this.Focus();
        }
    }
}
