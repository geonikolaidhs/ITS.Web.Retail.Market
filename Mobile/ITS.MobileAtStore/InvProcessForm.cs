using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ITS.MobileAtStore.ObjectModel;
using ITS.Common.Utilities.Compact;
using ITS.Common.Keyboards.Compact;
using ITS.MobileAtStore.Properties;
using ITS.Common.Utilities.EAN128BarcodeNS;
using ITS.MobileAtStore.Helpers;
using ITS.MobileAtStore.AuxilliaryClasses;

namespace ITS.MobileAtStore
{
    public partial class InvProcessForm : Form
    {
        public InvProcessForm()
        {
            InitializeComponent();
            this.Text = "Επεξεργασία απογραφής";
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtProduct_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (CommonUtilities.ReplaceFNCChar(e, txtProduct)){
                return;
            }
            if (e.KeyChar == (char)Keys.Enter)
            {                
                try
                {
                    string search = txtProduct.Text.Trim();
                    EAN128Barcode b128 = new EAN128Barcode(search, AppSettings.B128Settings);
                    search = CommonBusinessLogic.GetEAN128ProductCode(DOC_TYPES.INVENTORY, b128, search, txtProduct);
                    e.Handled = true;
                    if (string.IsNullOrEmpty(search))
                    {
                        return;
                    }

                    using (var ws = MobileAtStore.GetWebService(AppSettings.Timeout))
                    {
                        ITS.MobileAtStore.WRMMobileAtStore.Product webServiceProduct = ws.GetProduct(AppSettings.Terminal.ID, true, AppSettings.Terminal.IP, search, "", "", "",ITS.MobileAtStore.WRMMobileAtStore.eDocumentType.INVENTORY,true);
                        if (webServiceProduct == null)
                        {
                            MessageForm.Execute("Ενημέρωση", "Το προϊόν δεν βρέθηκε", MessageForm.DialogTypes.MESSAGE);
                            return;
                        }
                        //ws.GetReceiptProduct(search, null);
                        Product product = CommonUtilities.ConvertProduct(webServiceProduct, MobileAtStore.ItemsDL);
                        if (product == null || product.Code == "")
                        {
                            product = null;
                        }
                        string prodCode = string.Empty;
                        string barCode = string.Empty;
                        string prodDescr = string.Empty;
                        if (product == null)
                        {
                            prodCode = search;
                            barCode = search;
                        }
                        else
                        {
                            prodCode = product.Code;
                            barCode = product.Barcode;
                            prodDescr = product.Description;
                        }

                        WRMMobileAtStore.InvLine[] lines = ws.GetInvLines(AppSettings.Terminal.ID, true, AppSettings.Terminal.IP, prodCode);
                        WRMMobileAtStore.InvLine line;

                        bool addQty = false;
                        if (lines == null || lines.Length == 0)
                        {
                            line = new WRMMobileAtStore.InvLine();
                            line.Descr = prodDescr;
                            line.ProdCode = prodCode;
                            line.ProdBarcode = barCode;
                        }
                        else
                        {
                            line = lines[0];
                            line.ProdCode = prodCode;
                            line.ProdBarcode = barCode;
                        }

                        if (line.Oid == Guid.Empty.ToString() && product == null && MessageForm.Execute("Ερώτηση", "Το είδος δεν βρέθηκε, θέλετε να προστεθεί?", MessageForm.DialogTypes.YESNO) == DialogResult.No)
                        {
                            return;
                        }

                        decimal qty = line.Qty;
                        decimal startupQty = 0;

                        int finalDecimalDigits = product != null && product.SupportsDecimalQuantities ? AppSettings.QuantityNumberOfDecimalDigits : 0;
                        if (!KeyboardGateway.OpenNumeric(ref qty, null, false, false, 0, 9999.999m, true, AppSettings.QuantityNumberOfTotalDigits, finalDecimalDigits, NumKeypad.OPERATOR.FORBID_OPERATORS, startupQty, prodDescr))
                        {
                            return;
                        }
                        if (qty > 100)
                        {
                            DialogResult result = MessageBox.Show("Έχετε εισάγει ποσότητα μεγαλύτερη του 100. Να καταχωρηθεί;", "ΠΡΟΣΟΧΗ", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                            if (result == DialogResult.No)
                                return;
                            //MessageForm.Execute("Ενημέρωση", "Έχετε καταχωρήσει ποσότητα μεγαλύτερη του 100", MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.NOTIFY);
                        }                        
                        if (line.Oid != Guid.Empty.ToString() && line.Qty != 0)
                            addQty = AddOrReplace.Execute() == AddOrReplaceResult.ADD ? true : false;
                        line = ws.UploadInvLine(AppSettings.Terminal.ID, true, AppSettings.Terminal.IP, line, qty, true, addQty, true, "");
                        if (line == null)
                        {
                            MessageForm.Execute("Σφάλμα", "Η αποθήκευση της γραμμής απογραφής ήταν αποτυχής", MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.CRITICAL);
                            lblQtyValue.Text = "0";
                            txtDescr.Text = "";
                        }
                        else
                        {
                            lblQtyValue.Text = line.Qty.ToString(ObjectModel.Common.CultureInfo);
                            if (product != null)
                                txtDescr.Text = product.ToString();
                            else
                            {
                                txtDescr.Text = search;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageForm.Execute("Σφάλμα", "Υπήρξε κάποιο πρόβλημα\r\n" + ex.Message, MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.CRITICAL);
                }
                finally
                {
                    txtProduct.Text = "";
                }
            }
        }

        private void txtDescr_GotFocus(object sender, EventArgs e)
        {
            txtProduct.Focus();
        }

        private void InvProcessForm_Paint(object sender, PaintEventArgs e)
        {
            Form f = sender as Form;
            if (f.Location.X != 0 || f.Location.Y != 0)
            {
                f.Location = new Point(0, 0);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            string errorMessage = string.Empty;
            if (Main.ExportInventory(out errorMessage))
            {
                MessageForm.Execute("Επιτυχία", "H εξαγωγή ήταν επιτυχής!", MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.NOTIFY);
                this.Close();
                return;
            }
            else
            {
                MessageForm.Execute("Αποτυχία", errorMessage, MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.NOTIFY);
            }
        }
     
    }
}
