
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.POS.Client.ObserverPattern;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Model.Transactions;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using DevExpress.Data.Filtering;
using ITS.POS.Client.Helpers;
using ITS.POS.Client.Extensions;

namespace ITS.POS.Client.UserControls
{
    [Obsolete("DO NOT RENAME OR CHANGE NAMESPACE FOR BACKWARDS COMPATIBILITY!")]
    public partial class DetailDataGrid : DataGridViewExtension//DataGridView, IObserver, IObserverGrid
    {

        public DetailDataGrid()
        {
            InitializeComponent();
            this.RowPrePaint += DetailDataGrid_RowPrePaint;
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

                DocumentDetail row = this.Rows[nextRowIndex].DataBoundItem as DocumentDetail;
                if (row != null)
                {
                    if (row.IsCanceled || row.IsLinkedLine)
                    {
                        int getNextRow = GridHelper.GetNextRow(previousRowIndex, nextRowIndex, this.DataSource as IEnumerable<DocumentDetail>);

                        if (getNextRow >= 0)
                        {
                            return base.SetCurrentCellAddressCore(columnIndex, getNextRow, setAnchorCellAddress, validateCurrentCell, throughMouseClick);
                        }
                    }
                    else
                    {
                        IAppContext appContext = Kernel.GetModule<IAppContext>();
                        IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
                        appContext.CurrentDocumentLine = (row.IsCanceled) ? null : row;
                        actionManager.GetAction(eActions.PUBLISH_DOCUMENT_LINE_INFO).Execute(new ActionPublishDocumentLineInfoParams(appContext.CurrentDocumentLine, false, false));
                        if (appContext.CurrentDocumentLine != null) //Update Pole display
                        {
                            string[] lines = new String[] { appContext.CurrentDocumentLine.CustomDescription.ToUpperGR(), appContext.CurrentDocumentLine.Qty + " X " + String.Format("{0:C}", appContext.CurrentDocumentLine.FinalUnitPriceWithVatBeforeDocumentDiscount)/*detail.FinalUnitPriceWithVat + " €" */};
                            actionManager.GetAction(eActions.CUSTOMER_POLE_DISPLAY_MESSAGE).Execute(new ActionCustomerPoleDisplayMessageParams(lines));
                        }
                        return base.SetCurrentCellAddressCore(columnIndex, rowIndex, setAnchorCellAddress, validateCurrentCell, throughMouseClick);
                    }
                }

                return base.SetCurrentCellAddressCore(columnIndex, rowIndex, setAnchorCellAddress, validateCurrentCell, throughMouseClick);
            }
            catch (Exception ex)
            {
                string errorMessage = ex.GetFullMessage();
                //gargara
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
            DocumentDetail row = this.Rows[nextRowIndex].DataBoundItem as DocumentDetail;
            //gridViewDocumentDetails.GetRow(e.FocusedRowHandle) as DocumentDetail;
            if (row != null)
            {
                if (row.IsCanceled || row.IsLinkedLine)
                {
                    int getNextRow = GridHelper.GetNextRow(previousRowIndex, nextRowIndex, this.DataSource as IEnumerable<DocumentDetail>);

                    if (getNextRow >= 0)
                    {
                        this.CurrentCell = this.Rows[getNextRow].Cells[0];
                        base.SetSelectedRowCore(getNextRow, selected);
                        return;
                    }
                }
                else
                {

                    base.SetSelectedRowCore(rowIndex, selected);
                    IAppContext appContext = Kernel.GetModule<IAppContext>();
                    appContext.CurrentDocumentLine = (row.IsCanceled) ? null : row;
                    return;
                }
            }

            base.SetSelectedRowCore(rowIndex, selected);
        }

        void DetailDataGrid_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (this.HideDeletedLines)
                return;
            if (e.RowIndex >= 0)
            {
                DocumentDetail detail = Rows[e.RowIndex].DataBoundItem as DocumentDetail;
                if (detail != null && detail.IsCanceled)
                {
                    Rows[e.RowIndex].DefaultCellStyle = DefaultDeletedCellStyle;
                }
            }
        }

        public override void Update(GridParams parameters)
        {
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            if (parameters.DocumentHeader != null)
            {
                if (HideDeletedLines)
                {
                    parameters.DocumentHeader.DocumentDetails.Filter = new BinaryOperator("IsCanceled", false);
                }
                else
                {
                    parameters.DocumentHeader.DocumentDetails.Filter = null;
                }
                DataSource = parameters.DocumentHeader.DocumentDetails;
                Visible = (parameters.DisplayMode == eDisplayMode.DETAILS);
            }
            else if (parameters.SelectedDocumentDetail != null)
            {
                Visible = true;
            }
            else if (parameters.DisplayMode == eDisplayMode.DETAILS && parameters.DocumentHeader == null && parameters.SelectedDocumentDetail == null && parameters.SelectedDocumentPayment == null)
            {
                Visible = true;
            }
            else if (parameters.DisplayMode == null && parameters.DocumentHeader == null && parameters.Navigation == null && parameters.SelectedDocumentDetail == null && parameters.SelectedDocumentPayment == null)
            {
                //Document was closed or canceled.
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

            if (parameters.SelectedDocumentDetail != null)
            {

                int newPosition = this.LocateByValue("Oid", parameters.SelectedDocumentDetail.Oid);
                if ((CurrentRow == null && newPosition == -1) || (CurrentRow != null && CurrentRow.Index == newPosition && newPosition != 0))
                {
                    return;
                }
                if (parameters.SelectedDocumentDetail.CustomDescription != null)
                {
                    string[] lines = new String[] { parameters.SelectedDocumentDetail.CustomDescription.ToUpperGR(), parameters.SelectedDocumentDetail.Qty + " X " + String.Format("{0:C}", parameters.SelectedDocumentDetail.FinalUnitPriceWithVatBeforeDocumentDiscount) };
                    IAppContext appContext = Kernel.GetModule<IAppContext>();
                    actionManager.GetAction(eActions.CUSTOMER_POLE_DISPLAY_MESSAGE).Execute(new ActionCustomerPoleDisplayMessageParams(lines));
                    Rows[newPosition].Selected = true;
                    CurrentCell = Rows[newPosition].Cells[0];
                }
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
                            DocumentDetail detail = SelectedRows[0].DataBoundItem as DocumentDetail;
                            if (detail != null) //Update Pole display
                            {
                                string[] lines = new String[] { detail.CustomDescription.ToUpperGR(), detail.Qty + " X " + String.Format("{0:C}", detail.FinalUnitPriceWithVatBeforeDocumentDiscount)/*detail.FinalUnitPriceWithVat + " €" */};

                                actionManager.GetAction(eActions.CUSTOMER_POLE_DISPLAY_MESSAGE).Execute(new ActionCustomerPoleDisplayMessageParams(lines));
                            }
                        }
                        break;
                    case eNavigation.MOVEDOWN:
                        if (CurrentRow.Index >= 0 && CurrentRow.Index < (Rows.Count - 1))
                        {
                            Rows[CurrentRow.Index + 1].Selected = true;
                            CurrentCell = Rows[CurrentRow.Index + 1].Cells[0];
                            DocumentDetail detail = SelectedRows[0].DataBoundItem as DocumentDetail;
                            if (detail != null) //Update Pole display
                            {
                                string[] lines = new String[] { detail.CustomDescription.ToUpperGR(), detail.Qty + " X " + String.Format("{0:C}", detail.FinalUnitPriceWithVatBeforeDocumentDiscount)/*detail.FinalUnitPriceWithVat + " €" */};

                                actionManager.GetAction(eActions.CUSTOMER_POLE_DISPLAY_MESSAGE).Execute(new ActionCustomerPoleDisplayMessageParams(lines));
                            }
                        }
                        break;
                }
            }
            else if (parameters.DocumentHeader == null)
            {
                DataSource = new BindingList<DocumentDetail>();
            }
        }

    }
}
