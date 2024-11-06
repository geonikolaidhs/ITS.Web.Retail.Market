
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
    /// <summary>
    /// Displays the document's details
    /// </summary>
    public partial class ucDetailDataGrid : ucDataGridViewExtension
    {

        public ucDetailDataGrid()
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
                        appContext.CurrentDocumentLine = (row.IsCanceled) ? null : row;
                        return base.SetCurrentCellAddressCore(columnIndex, rowIndex, setAnchorCellAddress, validateCurrentCell, throughMouseClick);
                    }
                }

                return base.SetCurrentCellAddressCore(columnIndex, rowIndex, setAnchorCellAddress, validateCurrentCell, throughMouseClick);
            }
            catch(Exception ex)
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
            if (row != null)
            {
                if (row.IsCanceled || row.IsLinkedLine)
                {
                    int getNextRow = GridHelper.GetNextRow(previousRowIndex, nextRowIndex, this.DataSource as IEnumerable<DocumentDetail>);

                    if (getNextRow >= 0)
                    {
                        //this.CurrentCell = this.Rows[getNextRow].Cells[0];
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
                if (detail !=null && detail.IsCanceled)
                {
                    Rows[e.RowIndex].DefaultCellStyle = DefaultDeletedCellStyle;
                }
                else if (detail != null && detail.IsInvalid)
                {
                    Rows[e.RowIndex].DefaultCellStyle = DefaultInvalidCellStyle;
                }
                else
                {
                    Rows[e.RowIndex].DefaultCellStyle = DefaultCellStyle;
                }
            }
        }

        public override void Update(GridParams parameters)
        {

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
            else if(parameters.DisplayMode == null && parameters.DocumentHeader == null && parameters.Navigation == null && parameters.SelectedDocumentDetail == null && parameters.SelectedDocumentPayment == null)
            {
                //Document was closed or canceled.
                Visible = true;
            }
            else if (parameters.Navigation==null)
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
                if ((CurrentRow == null && newPosition == -1) || (CurrentRow!= null && CurrentRow.Index == newPosition && newPosition != 0))
                {
                    return;
                }
                if (parameters.SelectedDocumentDetail.CustomDescription != null)
                {
                    Rows[newPosition].Selected = true;
                    CurrentCell = Rows[newPosition].Cells[0];
                }
            }
            else if (parameters.Navigation != null && SelectedRows.Count > 0)
            {
                ////Check if all lines are deleted. In that case do NOT move
                if ((DataSource as IEnumerable<DocumentDetail>) != null && (DataSource as IEnumerable<DocumentDetail>).FirstOrDefault(detail => detail.IsCanceled == false) != null)
                {
                    switch (parameters.Navigation)
                    {
                        case eNavigation.MOVEUP:
                            if (CurrentRow.Index > 0)
                            {
                                DocumentDetail row = Rows[CurrentRow.Index - 1].DataBoundItem as DocumentDetail;
                                if (!row.IsCanceled && !row.IsLinkedLine)
                                {
                                    Rows[CurrentRow.Index - 1].Selected = true;
                                    CurrentCell = Rows[CurrentRow.Index].Cells[0];
                                }
                                else
                                {
                                    for (int i = 1; i <= CurrentRow.Index; i++)
                                    {
                                        if (!(Rows[CurrentRow.Index - i].DataBoundItem as DocumentDetail).IsCanceled && !(Rows[CurrentRow.Index - i].DataBoundItem as DocumentDetail).IsLinkedLine)
                                        {
                                            Rows[CurrentRow.Index - i].Selected = true;
                                            CurrentCell = Rows[CurrentRow.Index].Cells[0];
                                            break;
                                        }
                                    }
                                }
                            }
                            break;
                        case eNavigation.MOVEDOWN:
                            if (CurrentRow.Index >= 0 && CurrentRow.Index < (Rows.Count - 1))
                            {
                                DocumentDetail row = Rows[CurrentRow.Index + 1].DataBoundItem as DocumentDetail;
                                if (!row.IsCanceled && !row.IsLinkedLine)
                                {
                                    Rows[CurrentRow.Index + 1].Selected = true;
                                    CurrentCell = Rows[CurrentRow.Index].Cells[0];
                                }
                                else
                                {
                                    for (int i = CurrentRow.Index + 1; i < Rows.Count; i++)
                                    {
                                        if (!(Rows[i].DataBoundItem as DocumentDetail).IsCanceled && !(Rows[i].DataBoundItem as DocumentDetail).IsLinkedLine)
                                        {
                                            Rows[i].Selected = true;
                                            CurrentCell = Rows[CurrentRow.Index].Cells[0];
                                            break;
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            else if (parameters.DocumentHeader == null)
            {
                DataSource = new BindingList<DocumentDetail>();
            }
        }

    }
}
