using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ITS.MobileAtStore.ObjectModel;
using ITS.MobileAtStore.AuxilliaryClasses;
using System.Net;
using ITS.Common.Utilities.Compact;
using ITS.Common.Keyboards.Compact;

namespace ITS.MobileAtStore
{
    public partial class DecompositionDetailForm : Form
    {
        Document document;
        Line mainLine;

        BindingList<Line> detailLines = new BindingList<Line>();
        BindingSource detailLinesBindingSource = new BindingSource();
        public DecompositionDetailForm(Document document, Line line)
        {
            InitializeComponent();
            this.document = document;
            this.mainLine = line;
            lblMainItem.Text = this.mainLine.ProdCode + ":" + this.mainLine.Description;

            detailLinesBindingSource.DataSource = detailLines;
            detailLines.Add(mainLine);
            this.txtDescr.DataBindings.Add("Text", this.detailLinesBindingSource, "DetailDescription");
            detailLines.Clear();
            DisplayPosition();
            
        }


        private void txtProduct_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (CommonUtilities.ReplaceFNCChar(e, this.txtProduct))
            {
                return;
            }
            if ((e.KeyChar == (char)Keys.Enter || e.KeyChar == (char)Keys.Tab) && String.IsNullOrEmpty(this.txtProduct.Text) == false)
            {

                try
                {
                    Product product = CommonUtilities.GetProductFromWebService(this.txtProduct.Text.Trim(), null, EnumerationMapping.GetWebeDocumentType(this.document.Header.DocType));
                    if (product == null)
                    {
                        MessageForm.Execute("Σφάλμα", "Το είδος δεν βρέθηκε");
                        return;
                    }
                    if (string.Compare(product.Code, mainLine.ProdCode, true) == 0)
                    {
                        MessageForm.Execute("Σφάλμα", "Δεν επιτρέπεται να εισάχθει το κύριο είδος ως παραγόμενο");
                        return;
                    }
                    decimal qty = 0;
                    if (KeyboardGateway.OpenNumeric(ref qty, null, false, false, 0, 9999.999m, true,
                        AppSettings.QuantityNumberOfTotalDigits, AppSettings.QuantityNumberOfDecimalDigits,
                        NumKeypad.OPERATOR.NO_OPERATOR, 0m, product.Description))
                    {
                        Line extraLine = this.document.Add(product.Code, product.Barcode, qty, 0);
                        extraLine.Description = product.Description;
                        extraLine.LinkedLine = mainLine.Oid;
                        extraLine.Save();
                        RefreshData();
                        detailLinesBindingSource.MoveLast();
                        DisplayPosition();
                    }
                }
                catch (WebException exception)
                {
                    MessageForm.Execute("Σφάλμα", "Πρέπει να είστε online για να πραγματοποιήσετε ανάλωση");
                }
                finally
                {
                    this.txtProduct.Text = "";
                }

            }
        }

        private void RefreshData()
        {
            List<Line> linesToShow = this.document.Header.Details.FindAll(x => x.LinkedLine == this.mainLine.Oid);
            detailLines.Clear();
            linesToShow.ForEach(x => detailLines.Add(x));
            txtProduct.Focus();

        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void btnDelete_Click(object sender, EventArgs e)
        {
            Line currentLine = detailLinesBindingSource.Current as Line;
            if (currentLine !=null && currentLine.LinkedLine == mainLine.Oid && 
                MessageForm.Execute("Επιβεβαίωση διαγραφής", "Θέλετε να διαγράψετε την γραμμή;", MessageForm.DialogTypes.YESNO) == DialogResult.Yes)
            {
             
                currentLine.Delete();
                RefreshData();
            }
            txtProduct.Focus();
            DisplayPosition();
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            try
            {
                detailLinesBindingSource.MovePrevious();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                txtProduct.Focus();
                DisplayPosition();
            }
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            try
            {
                detailLinesBindingSource.MoveNext();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                txtProduct.Focus();
                DisplayPosition();
            }
        }

        private void DisplayPosition()
        {
            this.lblPosition.Text = this.detailLinesBindingSource.Position +
                 1 + "/" + this.detailLinesBindingSource.Count;
        }

        private void txtDescr_GotFocus(object sender, EventArgs e)
        {
            txtProduct.Focus();
        }
    }
}