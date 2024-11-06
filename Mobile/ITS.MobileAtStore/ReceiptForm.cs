using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenNETCF.Windows.Forms;
using ITS.MobileAtStore.ObjectModel;
using ITS.Common.Keyboards.Compact;
using ITS.Common.Utilities.Compact;
using ITS.Common.Utilities.EAN128BarcodeNS;
//using ITS.MobileAtStore.AuxilliaryClasses;
using ITS.MobileAtStore.Helpers;
using ITS.MobileAtStore.AuxilliaryClasses;

namespace ITS.MobileAtStore
{
    public partial class ReceiptForm : Form
    {
        #region DataMembers
        private Document _document;
        #endregion

        #region Constructors
        public ReceiptForm(Document document)
        {
            InitializeComponent();
            _document = document;
            txtProduct.Focus();
            RefreshUI();
        }
        #endregion

        #region Event Handlers
        private void txtDocNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                txtProduct.Focus();
            }
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            this._document.Header.Save();
            this.Close();
        }

        private void txtProduct_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (CommonUtilities.ReplaceFNCChar(e, txtProduct))
                return;
            if (e.KeyChar == (char)Keys.Enter)
            {
                ProcessCode();
                RefreshUI();
                txtProduct.Text = "";
                e.Handled = true;
            }
        }

        private void txtRecords_GotFocus(object sender, EventArgs e)
        {
            txtProduct.Focus();
        }
        #endregion

        #region Methods
        private void SetUIElements(Product prod, string scannedCode)
        {
            string description;
            decimal reqQty = 0;
            decimal scannedQty = 0;
            decimal restQty = 0;
            decimal restInvQty = 0;

            if (prod!=null &&  !string.IsNullOrEmpty(prod.Code))
            {
                description = prod.ToString();
                reqQty = prod.RequiredQuantity;
                scannedQty = _document.GetTotalQuantityScannedForCode(prod.Code);
                restInvQty = prod.RestInvQty;
            }
            else
            {
                description = CommonUtilities.TrimStartFromZeros(scannedCode);
                scannedQty = _document.GetTotalQuantityScannedForCode(scannedCode);
            }
            restQty = reqQty - scannedQty;

            txtDescr.Text = description;
            lblReqQtyValue.Text = reqQty.ToString(ObjectModel.Common.CultureInfo);
            lblScannedQtyValue.Text = scannedQty.ToString(ObjectModel.Common.CultureInfo);
            lblRestInvQty.Text = restInvQty.ToString(ObjectModel.Common.CultureInfo);
            //lblRestQtyValue.Text = restQty.ToString(Common.CultureInfo);
        }

        private void ClearUIElements()
        {
            txtDescr.Text = "";
            lblReqQtyValue.Text = "0";
            lblScannedQtyValue.Text = "0";
            lblRestInvQty.Text = "0";
            //lblRestQtyValue.Text = "0";
            lblExtraInfo.Text = "";
        }

        private void ProcessCode()
        {
            string searchString = txtProduct.Text.Trim();
            decimal inputQty = 0;
            decimal lastQty = 0;
            decimal startupQty = 0;
            if (string.IsNullOrEmpty(searchString))
                return;
            EAN128Barcode b128 = new EAN128Barcode(searchString, AppSettings.B128Settings);
            searchString = CommonBusinessLogic.GetEAN128ProductCode(DOC_TYPES.RECEPTION, b128, searchString, txtProduct);
            //startupQty = CommonBusinessLogic.GetEAN128Quantity(DOC_TYPES.RECEPTION, b128, startupQty);Take the qty from EAN128 if possible
            Line ln = null;
            ClearUIElements();

            WRMMobileAtStore.Product webServiceProduct = null;

            try
            {
                using (var ws = MobileAtStore.GetWebService(AppSettings.Timeout))
                {
                    webServiceProduct = ws.GetReceiptProduct(AppSettings.Terminal.ID, true, AppSettings.Terminal.IP, searchString, _document.Header.CustomerCode, "", "",ITS.MobileAtStore.WRMMobileAtStore.eDocumentType.RECEPTION,true);
                    Product product = CommonUtilities.ConvertProduct(webServiceProduct, MobileAtStore.ItemsDL);
                    decimal parsedQty = product!=null && product.BarcodeParsingResult == ITS.MobileAtStore.WRMMobileAtStore.BarcodeParsingResult.ITEM_CODE_QUANTITY ? product.Quantity : 0;

                    if (product==null || string.IsNullOrEmpty(product.Code)) 
                    {
                        //Product was not found.
                        ln = _document.Exists(searchString);
                        //If the line has already been entered, or the user wants to enter it then let him enter quantity.
                        if (ln != null || MessageForm.Execute("Ερώτηση", "Το προϊόν δεν βρέθηκε\r\nΘα θέλατε να προστεθεί?", MessageForm.DialogTypes.YESNO, MessageForm.MessageTypes.QUESTION) == DialogResult.Yes)
                        {
                            lastQty = inputQty = ln == null ? 0 : ln.Qty1;
                            SetUIElements(product, searchString);

                            int finalDecimalDigits = product != null && product.SupportsDecimalQuantities ? AppSettings.QuantityNumberOfDecimalDigits : 0;
                            if (KeyboardGateway.OpenNumeric(ref inputQty, null, false, false, 0, 9999.999m, true, AppSettings.QuantityNumberOfTotalDigits, finalDecimalDigits, NumKeypad.OPERATOR.FORBID_OPERATORS, startupQty))
                            {
                                if (inputQty >= AppSettings.MaximumAllowedQuantity)
                                {
                                    MessageBox.Show("Η ποσότητα υπερβαίνει τη μέγιστη επιτρεπτή", "ΠΡΟΣΟΧΗ");
                                    return;
                                }

                                if (ln != null)
                                {
                                    //If he chooses to add then we add with the last quintity, otherwise we just leave it overwrite the old one.
                                    AddOrReplaceResult addOrReplaceResult = AddOrReplace.Execute();
                                    this.Show(); //force the re-show of the original form..
                                    if (addOrReplaceResult == AddOrReplaceResult.ADD)
                                    {
                                        inputQty = lastQty + inputQty;
                                }
                                }
                                ln = _document.AddOrUpdate(searchString, string.Empty, inputQty, 0);
                            }
                        }
                    }
                    else if (parsedQty == 0)
                    {
                        //Product found.
                        //Take the barcode or code depending on what was scanned with priority to barcode
                        ln = _document.Exists(product);
                        lastQty = inputQty = ln == null ? 0 : ln.Qty1;
                        SetUIElements(product, searchString);



                        //If required quantity is 0 then it means that the product was not found in the Receipt at all, or it was found with 0 quantity.
                        if (product.RequiredQuantity == 0)
                        {
                            string extraMessage = string.Empty;
                            //Form the similar extracodes message
                            if (product.ProductQuantities != null && product.ProductQuantities.Count > 0)
                            {
                                extraMessage = "Προϊόντα με ίδιο μητρικό κωδικό σε παραγγελία\r\nΚωδικός, Ποσότητα\r\n";
                                foreach (ProductQuantity pq in product.ProductQuantities)
                                {
                                    extraMessage += String.Format("{0}, {1}\r\n", CommonUtilities.TrimStartFromZeros(pq.Code), pq.Quantity.ToString(ObjectModel.Common.CultureInfo));
                                }
                            }
                            int finalDecimalDigits = product != null && product.SupportsDecimalQuantities ? AppSettings.QuantityNumberOfDecimalDigits : 0;
                            if (KeyboardGateway.OpenNumeric(ref inputQty, null, false, false, 0, 9999.999m, true, AppSettings.QuantityNumberOfTotalDigits, finalDecimalDigits, NumKeypad.OPERATOR.FORBID_OPERATORS, startupQty, product.Description))
                            {
                                if (inputQty >= AppSettings.MaximumAllowedQuantity)
                                {
                                    MessageBox.Show("Η ποσότητα υπερβαίνει τη μέγιστη επιτρεπτή", "ΠΡΟΣΟΧΗ");
                                    return;
                                }

                                if (ln != null)
                                {
                                    //If he chooses to add then we add with the last quintity, otherwise we just leave it overwrite the old one.
                                    AddOrReplaceResult addOrReplaceResult = AddOrReplace.Execute();
                                    this.Show(); //force the re-show of the original form..
                                    if (addOrReplaceResult == AddOrReplaceResult.ADD)
                                    {
                                        inputQty = lastQty + inputQty;
                                    }
                                }

                                ln = _document.AddOrUpdate(product.Code, product.Barcode, inputQty, 0);
                            }

                        }
                        else
                        {
                            //Else the product was found in the receipt
                            //Take the required quantity
                            //decimal requiredQuantity = prod.RequiredQuantity;
                            //The existing stored quantity
                            //decimal existingQuantity = _document.GetTotalQuantityScannedForCode(prod.Code);
                            decimal lineQty = lastQty = ln == null ? 0 : ln.Qty1;

                            int finalDecimalDigits = product != null && product.SupportsDecimalQuantities ? AppSettings.QuantityNumberOfDecimalDigits : 0;
                            if (KeyboardGateway.OpenNumeric(ref lineQty, null, false, false, 0, 9999.999m, true, AppSettings.QuantityNumberOfTotalDigits, finalDecimalDigits, NumKeypad.OPERATOR.FORBID_OPERATORS, startupQty, product.Description))
                            {
                                if (lineQty >= AppSettings.MaximumAllowedQuantity)
                                {
                                    MessageBox.Show("Η ποσότητα υπερβαίνει τη μέγιστη επιτρεπτή", "ΠΡΟΣΟΧΗ");
                                    return;
                                }
                                if (ln != null)
                                {
                                    //If he chooses to add then we add with the last quintity, otherwise we just leave it overwrite the old one.
                                    AddOrReplaceResult addOrReplaceResult = AddOrReplace.Execute();
                                    this.Show(); //force the re-show of the original form..
                                    if (addOrReplaceResult == AddOrReplaceResult.ADD)
                                        lineQty = lastQty + lineQty;
                                }

                                ln = _document.AddOrUpdate(product.Code, product.Barcode, lineQty, 0);
                            }
                        }

                        CommonUtilities.SetExtraInfoText(product, lblExtraInfo);
                    }
                    else
                    {
                        _document.Add(product, 0);
                    }

                    SetUIElements(product, searchString);
                }
            }
            catch (Exception ex)
            {
                MessageForm.Execute("Σφάλμα", "Υπήρξε κάποιο πρόβλημα\r\n" + ex.Message, MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.CRITICAL);
            }
        }

        private void RefreshUI()
        {
            txtRecords.Text = _document.Header.CountLines().ToString();
        }
        #endregion

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (!AppSettings.ConnectedToWebService)
            {
                MessageForm.Execute("Πληροφορία", "Πρέπει να είστε Online για να πραγματοποιηθεί η εξαγωγή.");
                return;
            }

            if (this._document.Header.CountLines() <= 0)
            {
                MessageForm.Execute("Πληροφορία", "Έισάγετε τουλάχιστον μία εγγραφή και ξαναπροσπαθήστε");
                return;
            }

            try
            {
                btnExport.Enabled = false;

                ////if (!ValidateDoc())
                ////{
                ////    return;
                ////}
                this._document.Header.Save();
                if (ExportHelper.ExportDocuments(_document.Header.DocType, false))//this.ForcedOfflineMode))
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageForm.Execute("Σφάλμα", "Υπήρξε κάποιο πρόβλημα κατά την Online εξαγωγή\r\n" + ex.Message, MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.CRITICAL);
            }
            finally
            {
                btnExport.Enabled = true;
            }
        }

        private void txtRecords_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void txtDescr_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void txtDescr_GotFocus(object sender, EventArgs e)
        {
            txtProduct.Focus();
        }

        private void ReceiptForm_Paint(object sender, PaintEventArgs e)
        {
            Form f = sender as Form;
            if (f.Location.X != 0 || f.Location.Y != 0)
            {
                f.Location = new Point(0, 0);
            }
        }
    }
}
