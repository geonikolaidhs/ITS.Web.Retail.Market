using DevExpress.XtraEditors;
using ITS.Retail.Common;
using ITS.Retail.DesktopClient.StoreControllerClient.Helpers;
using ITS.Retail.Model;
using ITS.Retail.ResourcesLib;
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
    public partial class BuyerEditForm : XtraLocalizedForm
    {
        protected Buyer _Buyer;
        protected Buyer Buyer { get { return _Buyer; } }

        public BuyerEditForm()
        {
            this._Buyer = new Buyer(XpoHelper.GetNewUnitOfWork());
            InitializeComponent();
        }

        private void InitializeBindings()
        {
            this.txtCode.DataBindings.Add("EditValue", this.Buyer, "Code", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDescription.DataBindings.Add("EditValue", this.Buyer, "Description", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtReferenceCode.DataBindings.Add("EditValue", this.Buyer, "ReferenceCode", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private bool ObjectCanBeSaved(out string errormessage)
        {
            errormessage = Resources.FillAllMissingFields;
            return (this.txtCode.EditValue != null &&
                    this.txtReferenceCode.EditValue != null &&
                    this.txtDescription.EditValue != null);
        }

        private void BuyerEditForm_Load(object sender, EventArgs e)
        {
            InitializeBindings();
        }

        private void btnSaveBuyer_Click(object sender, EventArgs e)
        {
            string message;
            if (ObjectCanBeSaved(out message))
            {
                try
                {
                    this.Buyer.Owner = this.Buyer.Session.GetObjectByKey<CompanyNew>(Program.Settings.StoreControllerSettings.Owner.Oid);
                    this.Buyer.Save();
                    XpoHelper.CommitTransaction(this.Buyer.Session);
                    this.DialogResult = DialogResult.OK;
                }
                catch (Exception ex)
                {
                    string errorMessage = ex.GetFullMessage();
                    this.Buyer.Session.RollbackTransaction();
                }
            }
            else
            {
                XtraMessageBox.Show(message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            try
            {
                this.Buyer.Owner = Program.Settings.StoreControllerSettings.Owner;
                this.Buyer.Save();
                XpoHelper.CommitTransaction(this.Buyer.Session);
            }
            catch (Exception ex)
            {
                string errorMessage = ex.GetFullMessage();
                this.Buyer.Session.RollbackTransaction();
            }
        }

        private void btnCancelBuyer_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void BuyerEditForm_Shown(object sender, EventArgs e)
        {
            this.txtCode.Focus();
        }
    }
}
