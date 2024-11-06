using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Retail.Mobile_Model;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using Common;
using ITS.Common.Keyboards.Compact;

namespace Retail.Mobile
{
    public partial class LinesForm : Form
    {
        private Document _doc;
        private MainForm caller;
        public LinesForm(Document doc, MainForm cl)
        {
            InitializeComponent();
            this._doc = doc;
            UpdateData();
            caller = cl;
        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        public void UpdateData()
        {
            DataTable DT = new DataTable();
            DT.Columns.Add(Resources.Resources.Code);
            DT.Columns.Add(Resources.Resources.Barcode);
            DT.Columns.Add(Resources.Resources.Quantity);
            DT.Columns.Add(Resources.Resources.Value);
            DT.Columns.Add(Resources.Resources.NetValue);
            DT.Columns.Add(Resources.Resources.Description);
            DT.Columns.Add(Resources.Resources.PackingQty);
           // DT.Columns.Add("ID");

            for (int i = 0; i < _doc.Header.DocLines.Count; i++)
            {
                DT.Rows.Add(_doc.Header.DocLines[i].ItemCode.TrimStart(" 0".ToCharArray()), (_doc.Header.DocLines[i].Barcode == null) ? "" : _doc.Header.DocLines[i].Barcode.TrimStart(" 0".ToCharArray()), 
                    string.Format(Common.CultureInfo, "{0,10:###,##0.000}", _doc.Header.DocLines[i].Qty), 
                    //_doc.Header.DocLines[i].Qty.ToString("{0,10:#,##0.##}",Common.CultureInfo),
                    string.Format(Common.CultureInfo, "{0,10:###,##0.00}", _doc.Header.DocLines[i].ItemPrice),
                    string.Format(Common.CultureInfo, "{0,10:###,##0.00}", _doc.Header.DocLines[i].NetTotal),
                    _doc.Header.DocLines[i].ItemName,
                    string.Format(Common.CultureInfo, "{0,10:###,##0.00}", _doc.Header.DocLines[i].PackingQty)
             //      , _doc.Header.DocLines[i].Oid
                );
            }
                        
            dataGrid1.DataSource = DT;
            dataGrid1_CurrentCellChanged(null, null);
            

            SizeColumnsToContent(dataGrid1, -1);
        }

        public void SizeColumnsToContent(DataGrid dataGrid, int nRowsToScan)
        {
            // Create graphics object for measuring widths.
            Graphics Graphics = dataGrid.CreateGraphics();

            // Define new table style.
            DataGridTableStyle tableStyle = new DataGridTableStyle();

            try
            {
                DataTable dataTable = (DataTable)dataGrid.DataSource;

                if (-1 == nRowsToScan)
                {
                    nRowsToScan = dataTable.Rows.Count;
                }
                else
                {
                    // Can only scan rows if they exist.
                    nRowsToScan = System.Math.Min(nRowsToScan,
                        dataTable.Rows.Count);
                }

                // Clear any existing table styles.
                dataGrid.TableStyles.Clear();

                // Use mapping name that is defined in the data source.
                tableStyle.MappingName = dataTable.TableName;

                // Now create the column styles within the table style.
                DataGridTextBoxColumn columnStyle;
                int iWidth;

                for (int iCurrCol = 0; iCurrCol < dataTable.Columns.Count;
                                                                iCurrCol++)
                {
                    DataColumn dataColumn = dataTable.Columns[iCurrCol];
                    columnStyle = new DataGridTextBoxColumn();

                    //columnStyle.TextBox.Enabled = true;
                    columnStyle.HeaderText = dataColumn.ColumnName;
                    columnStyle.MappingName = dataColumn.ColumnName;

                    // Set width to header text width.
                    iWidth = (int)(Graphics.MeasureString(columnStyle.HeaderText,
                        dataGrid.Font).Width);

                    // Change width, if data width is wider than header text width.
                    // Check the width of the data in the first X rows.
                    DataRow dataRow;
                    for (int iRow = 0; iRow < nRowsToScan; iRow++)
                    {
                        dataRow = dataTable.Rows[iRow];

                        if (null != dataRow[dataColumn.ColumnName])
                        {
                            int iColWidth = (int)(Graphics.MeasureString(dataRow.
                                ItemArray[iCurrCol].ToString(),
                                dataGrid.Font).Width);
                            iWidth = (int)System.Math.Max(iWidth, iColWidth);
                        }
                    }

                    if(dataTable.Columns[iCurrCol].ColumnMapping==MappingType.Hidden)
                        columnStyle.Width = 0;
                    else
                        columnStyle.Width = iWidth + 14;

                    // Add the new column style to the table style.
                    tableStyle.GridColumnStyles.Add(columnStyle);
                }
                // Add the new table style to the data grid.
                dataGrid.TableStyles.Add(tableStyle);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, AppSettings.Title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                Graphics.Dispose();
            }
        }

        private void dataGrid1_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dataGrid1.CurrentRowIndex >= 0)
            {
                dataGrid1.Select(dataGrid1.CurrentRowIndex);
            }
            btnDeleteLine.Enabled = btnUpdateQty.Enabled = (dataGrid1.CurrentRowIndex >= 0);
            
            //MessageBox.Show("change");
            String text="";
            int selectedRow = dataGrid1.CurrentRowIndex;
            DataTable dt =  dataGrid1.DataSource as DataTable;
            if (selectedRow >= 0)
            {
                text = dt.Rows[selectedRow].ItemArray[4].ToString();
                txt_item_details.Text = this._doc.Header.DocLines[selectedRow].ItemName;
                txtEditQty.Text = this._doc.Header.DocLines[selectedRow].Qty.ToString();
                btnDeleteLine.Enabled = btnUpdateQty.Enabled = this._doc.Header.DocLines[selectedRow].RefDocLine == null;
            }
        }

        private void btnUpdateQty_Click(object sender, EventArgs e)
        {
            int selectedRow = dataGrid1.CurrentRowIndex;
            if (selectedRow < 0)
                return;
            DocLine editLine = this._doc.Header.DocLines[selectedRow];
            if (editLine.RefDocLine != null)
            {
                MessageBox.Show(Resources.Resources.RecordIsLinkedtoItem+":" + editLine.RefDocLine.ItemName +  "\r\n.", AppSettings.Title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                return;
            }

            Decimal dcm;
            
            try
            {
                dcm = Decimal.Parse(txtEditQty.Text);
                if (sender == btnUpdateQty)
                {                    
                    KeyboardGateway.OpenNumeric(ref dcm, null, false, false, 0, 9999.999m, true, 7, 3, NumKeypad.OPERATOR.FORBID_OPERATORS, dcm, Resources.Resources.NewQty);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Resources.Resources.InvalidQty, AppSettings.Title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                dataGrid1.Focus();
                return;
            }
            
            if (dcm == 0)
            {
                if (MessageBox.Show(Resources.Resources.WouldYouLikeToDeleteRecord, AppSettings.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    foreach(DocLine dc in editLine.RefLines)
                    {
                        this._doc.Header.DocLines.Remove(dc);
                    }
                    editLine.Session.Delete(editLine.RefLines);
                    this._doc.Header.DocLines.Remove(editLine);
                }
                
            }
            else if (dcm > AppSettings.limit || dcm < -AppSettings.limit)
            {
                MessageBox.Show(Resources.Resources.InvalidQty, AppSettings.Title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                dataGrid1.Focus();
                return;
            }
            else if ((Decimal)editLine.Qty - dcm != 0)
            {
                if (MessageBox.Show(Resources.Resources.WouldYouLikeToEditRecord, AppSettings.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    dataGrid1.Focus();
                    return;
                }
                if (AppSettings.OperationMode == AppSettings.OPERATION_MODE.BATCH)
                {
                    editLine.Qty = (double)dcm;
                    editLine.EditOffline = true;
                    if (editLine.RefLines!=null)
                    {
                        foreach (DocLine dc in editLine.RefLines)
                        {
                            dc.Qty = (double)dcm;                            
                        }
                    }
                }
                else
                {
                    int timeout = 10000;
                    using (RetailService.RetailService getLine = caller.GetService(timeout))
                    {
                        String barcodeToSend = this._doc.Header.DocLines[selectedRow].Barcode;
                        Settings set = XpoDefault.Session.FindObject<Settings>(CriteriaOperator.Parse("Oid>0", ""));

                        RetailService.TransferedDocumentDetail[] lineDetails = getLine.GetDocumentDetail(set.UserID, barcodeToSend, (double)dcm);
                        //NewXmlParser doclinexml = new NewXmlParser(lineDetails);
                        if (lineDetails.Length == 1 && lineDetails[0].Barcode == null)
                        {
                            MessageBox.Show(Resources.Resources.Error+":\r\n" + lineDetails[0].ItemName, AppSettings.Title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                        }
                        else
                        {

                            try
                            {
                                /*foreach (DocLine dc in editLine.RefLines)
                                {
                                    this._doc.Header.DocLines.Remove(dc);
                                }
                                editLine.Session.Delete(editLine.RefDocLine);*/
                                
                                int i = 0;
                                foreach (RetailService.TransferedDocumentDetail tline in lineDetails)
                                {
                                    DocLine currentLine = null;
                                    if(i==0)
                                    {
                                        currentLine = editLine;
                                    }
                                    else
                                    {
                                        foreach (DocLine dc in editLine.RefLines)
                                        {
                                            if (dc.ItemCode == tline.ItemCode)
                                                currentLine = dc;
                                        }
                                    }
                                    if (currentLine == null)
                                        currentLine = new DocLine(XpoDefault.Session);
                                    currentLine.Barcode = tline.Barcode;
                                    currentLine.FinalUnitPrice = tline.FinalUnitPrice;
                                    currentLine.FirstDiscount = tline.FirstDiscount;
                                    currentLine.GrossTotal = tline.GrossTotal;
                                    currentLine.ItemCode = tline.ItemCode;
                                    currentLine.ItemName = tline.ItemName;
                                    currentLine.ItemOid = tline.ItemOid.ToString();
                                    currentLine.ItemPrice = tline.UnitPrice;
                                    currentLine.NetTotal = tline.NetTotal;
                                    currentLine.NetTotalAfterDiscount = tline.NetTotalAfterDiscount;
                                    currentLine.Qty = tline.Qty;
                                    currentLine.SecondDiscount = tline.SecondDiscount;
                                    currentLine.TotalDiscount = tline.TotalDiscount;
                                    currentLine.TotalVatAmount = tline.TotalVatAmount;
                                    currentLine.UnitPriceAfterDiscount = tline.UnitPriceAfterDiscount;
                                    currentLine.VatAmount = tline.VatAmount;
                                    currentLine.VatFactor = tline.VatFactor;
                                    currentLine.EditOffline = false;
                                    if (i > 0)
                                    {
                                        editLine.RefLines.Add(currentLine);
                                        currentLine.RefDocLine = editLine;
                                    }
                                    _doc.Header.DocLines.Add(currentLine);
                                    i++;
                                }      
                            }
                            catch (Exception exept)
                            {
                                int rr = 0;
                            }
                        }
                    }
                }
            }
            this._doc.Header.Save();
            this._doc.Header.UpdateDocumentHeader();
            caller.UpdateDocumentHeaderDisplay();
            this.UpdateData();
            dataGrid1.Focus();
            return;
        }



        private void dataGrid1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                txtEditQty.Focus();
            }
        }

        private void txtEditQty_GotFocus(object sender, EventArgs e)
        {
            //txtEditQty.SelectAll();\
            dataGrid1.Focus();
            decimal qty;
            try
            {
                qty = decimal.Parse(txtEditQty.Text);
            }
            catch (Exception ex)
            {
                qty = 0;
            }

            KeyboardGateway.OpenNumeric(ref qty, null, false, false, 0, 9999.999m, true, 7, 3, NumKeypad.OPERATOR.FORBID_OPERATORS, qty, Resources.Resources.Quantity);
            txtEditQty.Text = ""+qty;
            btnUpdateQty_Click(null, null);

        }

        private void txtEditQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                btnUpdateQty_Click(null, null);
            }
        }

        private void LinesForm_Load(object sender, EventArgs e)
        {
            this.Text = Resources.Resources.Lines;
            btnDeleteLine.Text = Resources.Resources.Delete;
            btnUpdateQty.Text = Resources.Resources.Quantity;
            btnclose.Text = Resources.Resources._Return;

            if((dataGrid1.DataSource as DataTable).Rows.Count>0)
                dataGrid1.Select((dataGrid1.CurrentRowIndex >= 0) ? dataGrid1.CurrentRowIndex : 0);
            dataGrid1_CurrentCellChanged(null, null);
        }

        private void btnDeleteLine_Click(object sender, EventArgs e)
        {
            if (dataGrid1.CurrentRowIndex >= 0)
            {
                txtEditQty.Text = "0";
                btnUpdateQty_Click(sender, e);
            }
        }

        private void txtEditQty_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtEditQty.Text.Length > 9)
                {
                    txtEditQty.Text = txtEditQty.Text.Substring(0,9);
                    txtEditQty.SelectionStart = 9;
                    txtEditQty.SelectionLength = 0;
                }
            }
            catch (Exception ec)
            {
                
            }
        }

    }
}