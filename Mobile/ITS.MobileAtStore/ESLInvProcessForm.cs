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
using ITS.MobileAtStore.AuxilliaryClasses;

namespace ITS.MobileAtStore
{
    public partial class ESLInvProcessForm : Form
    {
        public enum Modes
        {
            PREPARE_STORE = 0,
            PROCESS_INV = 1
        }

        public Modes mode;

        public ESLInvProcessForm(Modes mode)
        {
            InitializeComponent();
            this.mode = mode;
            if (mode == Modes.PREPARE_STORE)
                this.Text = "Προετοιμασία κατασ. για απογραφή ESL";
            else
                this.Text = "Επεξεργασία απογραφής ESL";

            txtInvNumber.Focus();
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtProduct_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (CommonUtilities.ReplaceFNCChar(e, txtProduct))
                return;
            if (e.KeyChar == (char)Keys.Enter)
            {                
                try
                {
                    string invNumber = txtInvNumber.Text.Trim();
                    string search = txtProduct.Text.Trim();
                    EAN128Barcode b128 = new EAN128Barcode(search, AppSettings.B128Settings);
                    search = CommonBusinessLogic.GetEAN128ProductCode(DOC_TYPES.ESL_INV, b128, search, txtProduct);
                    e.Handled = true;
                    if (string.IsNullOrEmpty(invNumber))
                    {
                        MessageForm.Execute("Ενημέρωση", "Καταχωρήστε αριθμό απογραφής για να συνεχίσετε", MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.NOTIFY);
                        txtProduct.Focus();
                        return;
                    }
                    if (string.IsNullOrEmpty(search))
                    {
                        return;
                    }
                    using (WRMMobileAtStore.WRMMobileAtStore ws = MobileAtStore.GetWebService(AppSettings.Timeout))
                    {
                        WRMMobileAtStore.Product webServiceProduct = ws.GetProduct(AppSettings.Terminal.ID, true, AppSettings.Terminal.IP, search, "", "", "",ITS.MobileAtStore.WRMMobileAtStore.eDocumentType.ESL_INV,true);
                        if (webServiceProduct == null)
                        {
                            MessageForm.Execute("Ενημέρωση", "Το είδος δεν βρέθηκε", MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.NOTIFY);
                            txtProduct.Focus();
                            return;
                        }
                        //GetReceiptProduct(search, null);
                        Product product = CommonUtilities.ConvertProduct(webServiceProduct, MobileAtStore.ItemsDL);
                        if (product.Code == "")
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

                        WRMMobileAtStore.ESLInvLine[] lines = ws.GetESLInvLines(AppSettings.Terminal.ID, true, AppSettings.Terminal.IP, prodCode, invNumber);
                        WRMMobileAtStore.ESLInvLine line;

                        bool addQty = false;
                        if (lines == null || lines.Length == 0)
                        {
                            line = new ITS.MobileAtStore.WRMMobileAtStore.ESLInvLine();
                            line.Descr = prodDescr;
                            line.InventoryNumber = invNumber;
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
                        //startupQty = CommonBusinessLogic.GetEAN128Quantity(DOC_TYPES.ESL_INV, b128, startupQty); Take the qty from EAN128 if possible
                        if (mode == Modes.PROCESS_INV)
                        {
                            int finalDecimalDigits = product != null && product.SupportsDecimalQuantities ? AppSettings.QuantityNumberOfDecimalDigits : 0;
                            if (!KeyboardGateway.OpenNumeric(ref qty, null, false, false, 0, 9999.999m, true, AppSettings.QuantityNumberOfTotalDigits, finalDecimalDigits, NumKeypad.OPERATOR.FORBID_OPERATORS, startupQty, prodDescr))
                            {
                                return;
                            }
                            if (qty > 100)
                            {
                                MessageForm.Execute("Ενημέρωση", "Έχετε καταχωρήσει ποσότητα μεγαλύτερη του 100", MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.NOTIFY);
                        }
                        }

                        if (line.Oid != Guid.Empty.ToString() && mode == Modes.PROCESS_INV && line.Qty != 0)
                        {
                            addQty = AddOrReplace.Execute() == AddOrReplaceResult.ADD ? true : false;
                        }

                        if (!(mode == Modes.PREPARE_STORE && line.Oid != Guid.Empty.ToString()))
                        {
                            // ie if we are in prepare store mode and the line existed don't upload it
                            line = ws.UploadESLInvLine(AppSettings.Terminal.ID, true, AppSettings.Terminal.IP, line, qty, true, addQty, true);                            
                        }
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
                            {
                                txtDescr.Text = product.ToString();
                            }
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

        private void txtInvNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                if (string.IsNullOrEmpty(txtInvNumber.Text.Trim()))
                {
                    MessageForm.Execute("Ενημέρωση", "Καταχωρήστε αριθμό απογραφής για να συνεχίσετε", MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.NOTIFY);
                    return;
                }
                else
                    txtProduct.Focus();
            }
        }

        private void txtDescr_GotFocus(object sender, EventArgs e)
        {
            txtProduct.Focus();
        }

        private void ESLInvProcessForm_Paint(object sender, PaintEventArgs e)
        {
            Form f = sender as Form;
            if (f.Location.X != 0 || f.Location.Y != 0)
            {
                f.Location = new Point(0, 0);
            }
        }
    }
}
