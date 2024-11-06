using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Data.Filtering;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.ObserverPattern;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Model.Transactions;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Extensions;

namespace ITS.POS.Client.UserControls
{
    /// <summary>
    /// Displays the document's payments
    /// </summary>
    public class ucPaymentDetailsGrid : ucDataGridViewExtension
    {

        public ucPaymentDetailsGrid()
        {
            this.Visible = false;
            InitializeComponent();
        }

        protected override bool SetCurrentCellAddressCore(int columnIndex, int rowIndex, bool setAnchorCellAddress, bool validateCurrentCell, bool throughMouseClick)
        {
            try
            {
                int previousRowIndex = this.CurrentRow == null ? -1 : this.CurrentRow.Index;
                int nextRowIndex = rowIndex;
                if (previousRowIndex == nextRowIndex || nextRowIndex == -1)
                {
                    return base.SetCurrentCellAddressCore(columnIndex, rowIndex, setAnchorCellAddress, validateCurrentCell, throughMouseClick);
                }

                DocumentPayment row = this.Rows[nextRowIndex].DataBoundItem as DocumentPayment;
                if (row != null)
                {
                    IAppContext appContext = Kernel.GetModule<IAppContext>();
                    appContext.CurrentDocumentPayment = row;
                    return base.SetCurrentCellAddressCore(columnIndex, rowIndex, setAnchorCellAddress, validateCurrentCell, throughMouseClick);

                }

                return base.SetCurrentCellAddressCore(columnIndex, rowIndex, setAnchorCellAddress, validateCurrentCell, throughMouseClick);
            }
            catch (Exception ex)
            {
                string errorMessage = ex.GetFullMessage();
                return false;
            }
        }

        protected override void SetSelectedRowCore(int rowIndex, bool selected)
        {
            int previousRowIndex = this.CurrentRow == null ? -1 : this.CurrentRow.Index;
            int nextRowIndex = rowIndex;
            if (previousRowIndex == nextRowIndex || nextRowIndex == -1)
            {
                base.SetSelectedRowCore(rowIndex, selected);
                return;
            }
            DocumentPayment row = this.Rows[nextRowIndex].DataBoundItem as DocumentPayment;
            if (row != null)
            {
                base.SetSelectedRowCore(rowIndex, selected);
                IAppContext appContext = Kernel.GetModule<IAppContext>();
                appContext.CurrentDocumentPayment = row;
                return;
            }

            base.SetSelectedRowCore(rowIndex, selected);
        }

        public override void Update(GridParams parameters)
        {
            if (parameters.DocumentHeader != null)
            {
                DataSource = parameters.DocumentHeader.DocumentPayments;
                Visible = (parameters.DisplayMode == eDisplayMode.PAYMENTS);
            }
            else if (parameters.SelectedDocumentPayment != null)
            {
                Visible = true;
            }
            else if (parameters.DisplayMode == eDisplayMode.PAYMENTS && parameters.DocumentHeader == null && parameters.SelectedDocumentDetail == null && parameters.SelectedDocumentPayment == null)
            {
                Visible = true;
            }
            else if (parameters.Navigation == null)
            {
                Visible = false;
            }
            if (Visible == false)
            {
                return;
            }


            if (parameters.SelectedDocumentPayment != null)
            {
                int newPosition = this.LocateByValue("Oid", parameters.SelectedDocumentPayment.Oid);
                if ((CurrentRow == null && newPosition == -1) || (CurrentRow != null && CurrentRow.Index == newPosition && newPosition != 0))
                {
                    //Same position, do nothing
                    return;
                }
                Rows[newPosition].Selected = true;
                CurrentCell = Rows[newPosition].Cells[0];
            }
            else if (parameters.Navigation != null && SelectedRows.Count > 0)
            {
                //GridView view = (grdDocumentDetails.MainView as GridView);
                switch (parameters.Navigation)
                {
                    case eNavigation.MOVEUP:
                        if (CurrentRow.Index > 0)
                        {
                            Rows[CurrentRow.Index - 1].Selected = true;
                            CurrentCell = Rows[CurrentRow.Index - 1].Cells[0];
                        }
                        break;
                    case eNavigation.MOVEDOWN:
                        if (CurrentRow.Index >= 0 && CurrentRow.Index < (Rows.Count - 1))
                        {
                            Rows[CurrentRow.Index + 1].Selected = true;
                            CurrentCell = Rows[CurrentRow.Index + 1].Cells[0];
                        }
                        break;
                }
            }
            else if (parameters.DocumentHeader == null)
            {
                DataSource = new BindingList<DocumentPayment>();
            }
        }


        private IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }


        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        }
    }
}
