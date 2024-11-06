using DevExpress.Xpo;
using DevExpress.XtraEditors;
using ITS.Retail.Common;
using ITS.Retail.DesktopClient.StoreControllerClient.Helpers;
using ITS.Retail.Model;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;
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
    public partial class MarkUpForm : XtraLocalizedForm
    {
        private DocumentHeader DocumentHeader;

        public MarkUpForm(DocumentHeader documentHeader)
        {
            InitializeComponent();

            this.DocumentHeader = documentHeader;
            this.toggleSwitchSaveMarkUp.EditValue = true;
            this.toggleSwitchAllProducts.EditValue = false;
            this.toggleSwitchSaveMarkUp.Properties.OffText = this.toggleSwitchAllProducts.Properties.OffText =  Resources.Off;
            this.toggleSwitchSaveMarkUp.Properties.OnText = this.toggleSwitchAllProducts.Properties.OnText = Resources.On;

            this.spinEditMarkUpDif.Value = this.DocumentHeader.Owner.OwnerApplicationSettings.MarkupDefaultValueDifference;
            PrepareMarkUpGrid();
        }

        private void PrepareMarkUpGrid()
        {
            decimal sensitivity = spinEditMarkUpDif.Value;
            bool includeUnChangedPrices = (bool)toggleSwitchAllProducts.EditValue;
            gridControlMarkUpValues.DataSource = PriceCatalogHelper.GetPriceChanges(this.DocumentHeader, sensitivity, includeUnChangedPrices);
        }

        private void simpleButtonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void spinEditMarkUpDif_EditValueChanged(object sender, EventArgs e)
        {
            PrepareMarkUpGrid();
        }

        private void toggleSwitchAllProducts_EditValueChanged(object sender, EventArgs e)
        {
            PrepareMarkUpGrid();
        }

        private void gridViewMarkUpValues_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if(e.RowHandle < 0)
            {
                return;
            }

            DevExpress.XtraGrid.Views.Grid.GridView gridView = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            Reevaluate reevaluate = gridView.GetRow(e.RowHandle) as Reevaluate;
            decimal val = 0;
            bool setValue = false;
            if(e.Value != null)
            {
                setValue = decimal.TryParse(e.Value.ToString(), out val);
            }
            switch ( e.Column.FieldName )
            {
                case "MarkUp":                   
                    if (setValue)
                    {
                        reevaluate.MarkUp = val;
                    }
                    break;
                case "UnitPrice":
                    reevaluate.UnitPrice = val;
                    break;
                default:
                    break;
            }
        }

        private void simpleButtonSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {
                    List<Reevaluate> reavaluations = this.gridControlMarkUpValues.DataSource as List<Reevaluate>;
                    bool saveMarkUps = false;
                    if (toggleSwitchSaveMarkUp.EditValue != null)
                    {
                        bool.TryParse(toggleSwitchSaveMarkUp.EditValue.ToString(), out saveMarkUps);
                    }
                    DocumentHelper.SaveMarkUps(reavaluations, uow, saveMarkUps);
                    XtraMessageBox.Show(Resources.SuccesfullySaved, Resources.SuccesfullySaved, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception exception)
            {
                XtraMessageBox.Show(exception.GetFullMessage(), Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MarkUpForm_Shown(object sender, EventArgs e)
        {
            this.spinEditMarkUpDif.Focus();
        }
    }
}
