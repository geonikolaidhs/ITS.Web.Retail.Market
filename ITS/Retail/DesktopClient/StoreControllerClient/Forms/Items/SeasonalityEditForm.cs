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
    public partial class SeasonalityEditForm : XtraLocalizedForm
    {
        protected Seasonality _Seasonality;
        protected Seasonality Seasonality { get { return _Seasonality; } }

        public SeasonalityEditForm()
        {
            this._Seasonality = new Seasonality(XpoHelper.GetNewUnitOfWork());
            InitializeComponent();
        }

        private void InitializeBindings()
        {
            this.txtCode.DataBindings.Add("EditValue", this.Seasonality, "Code", true, DataSourceUpdateMode.OnPropertyChanged);
            this.txtDescription.DataBindings.Add("EditValue", this.Seasonality, "Description", true, DataSourceUpdateMode.OnPropertyChanged);

        }

        private bool ObjectCanBeSaved(out string errormessage)
        {
            errormessage = Resources.FillAllMissingFields;
            return (this.txtCode.EditValue != null &&
                    this.txtDescription.EditValue != null);
        }

        private void btnSaveSeasonality_Click(object sender, EventArgs e)
        {
            string message;
            if (ObjectCanBeSaved(out message))
            {
                try
                {
                    this.Seasonality.Owner = this.Seasonality.Session.GetObjectByKey<CompanyNew>(Program.Settings.StoreControllerSettings.Owner.Oid);
                    this.Seasonality.Save();
                    XpoHelper.CommitTransaction(this.Seasonality.Session);
                    this.DialogResult = DialogResult.OK;
                }
                catch (Exception ex)
                {
                    string errorMessage = ex.GetFullMessage();
                    this.Seasonality.Session.RollbackTransaction();
                }
            }
            else
            {
                XtraMessageBox.Show(message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnCancelSeasonality_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void SeasonalityEditForm_Load(object sender, EventArgs e)
        {
            InitializeBindings();
        }

        private void SeasonalityEditForm_Shown(object sender, EventArgs e)
        {
            this.txtCode.Focus();
        }
    }
}
