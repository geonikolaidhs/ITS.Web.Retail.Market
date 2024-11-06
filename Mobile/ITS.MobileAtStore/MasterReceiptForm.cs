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

namespace ITS.MobileAtStore
{
    public partial class MasterReceiptForm : Form
    {
        private Document _document;
        private WRMMobileAtStore.Customer _customer;

        public MasterReceiptForm()
        {
            InitializeComponent();
            NumKeypad.LastValueText = Resources.LastQtyText;
            _document = new Document(MobileAtStore.TransactionsDL).Load(DOC_TYPES.RECEPTION,false);
            _document.Header.TerminalID = AppSettings.Terminal.ID;
            RefreshUI();
            txtSupplier.Focus();
            txtSupplier.SelectAll();
        }

        private bool ValidateReceiptState()
        {
            return ValidateCustomer() && ValidateDocNumber();
        }

        private bool ValidateDoc()
        {
            return ValidateReceiptState();
        }

        /// <summary>
        /// Validates the document number given and its format and returns true or false accordingly
        /// </summary>
        /// <returns></returns>
        private bool ValidateDocNumber()
        {
            bool result = true;
            int tryValue = Int32.MinValue;
            try
            {
                tryValue = Int32.Parse(txtDocNumber.Text);
            }
            catch
            {
            }

            if (txtDocNumber.Text.Trim() == "")
            {
                result = false;
                MessageForm.Execute("Λάθος", "Ο αριθμός παραλαβής δεν μπορεί να είναι κενός", MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.NOTIFY);
            }
            else
            {
                if (tryValue == Int32.MinValue || tryValue == 0)
                {
                    result = false;
                    MessageForm.Execute("Λάθος", "Ο αριθμός παραλαβής μπορεί να είναι μόνο ακέραιος και διαφορετικός του μηδενός", MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.NOTIFY);
                }
                else
                {
                    this._document.Header.DocNumber = tryValue;
                    _document.Header.Save();
                }
            }

            if (!result)
                this.txtDocNumber.Focus();

            return result;
        }

        private bool ValidateCustomer()
        {
            string search = txtSupplier.Text.Trim();
            if (!_document.HasCustomer && string.IsNullOrEmpty(search))
            {
                MessageForm.Execute("Ενημέρωση", "Καταχωρήστε προμηθευτή για να συνεχίσετε", MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.NOTIFY);
                txtSupplier.Focus();
                return false;
            }
            else
                if (_document.HasCustomer && string.IsNullOrEmpty(search))
                {
                    SetUICustomerInfo();
                    return true;
                }

            bool result = true;
            
            try
            {
                using (var ws = MobileAtStore.GetWebService(AppSettings.Timeout))
                {
                    _customer = ws.GetSupplier(AppSettings.Terminal.ID, true, AppSettings.Terminal.IP, txtSupplier.Text.Trim());

                    //If there was an error..
                    if(_customer==null)
                    {
                        MessageForm.Execute("Πληροφορία", "Δε βρέθηκε ο Προμηθευτής!", MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.CRITICAL);
                        result = false;
                    }
                    else if (_customer.ErrorMessage != string.Empty)
                    {
                        MessageForm.Execute("Πληροφορία", _customer.ErrorMessage, MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.CRITICAL);
                        result = false;
                    }
                    else
                    {
                        _document.Header.CustomerCode = _customer.Code;
                        _document.Header.CustomerAFM = _customer.AFM;
                        _document.Header.CustomerName = _customer.Name;
                        _document.Header.Save();
                    }
                }
            }
            catch(Exception exception)
            {
                string exceptionMessage = exception.Message + "\r\n" + exception.StackTrace;
                MessageForm.Execute("Σφάλμα", "Υπήρξε κάποιο πρόβλημα στην επικοινωνία με το web service", MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.CRITICAL);
                _customer = null;
                result = false;
            }
            SetUICustomerInfo();
            if (!result)
            {
                txtSupplier.Focus();
                txtSupplier.SelectAll();
            }
            return result;
        }

        private void RefreshUI()
        {
            SetUICustomerInfo();
            SetUIMiscInfo();
        }

        private void SetUICustomerInfo()
        {
            txtSupplier.Text = _document.Header.CustomerCode;
            txtSupplierAFM.Text = _document.Header.CustomerAFM;
            txtSupplierName.Text = _document.Header.CustomerName;
        }

        private void SetUIMiscInfo()
        {
            txtDocNumber.Text = _document.Header.DocNumber == 0 ? "" : _document.Header.DocNumber.ToString();
        }

        private void MoveToReceipt()
        {
            if (ValidateReceiptState())
            {
                using (ReceiptForm rf = new ReceiptForm(_document))
                {
                    DialogResult = rf.ShowDialog();
                }
            }
        }
        #region Event Handlers

        private void btnContinue_Click(object sender, EventArgs e)
        {
            MoveToReceipt();
        }


        private void dtpDocDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                txtDocNumber.Focus();
                txtDocNumber.SelectAll();
            }
        }

        private void txtSupplier_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                if (string.IsNullOrEmpty(txtSupplier.Text.Trim()))
                    return;
                if (ValidateCustomer())
                    txtDocNumber.Focus();
            }
        }
        #endregion

        private void btnReturn_Click(object sender, EventArgs e)
        {
            if (_document != null)
            {
                _document.Header.Save();
                _document.Dispose();
                _document = null;
            }
            this.Close();
        }

        private void txtSupplierName_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void txtSupplierAFM_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void txtSupplierName_GotFocus(object sender, EventArgs e)
        {
            txtSupplier.Focus();
        }

        private void txtSupplierAFM_GotFocus(object sender, EventArgs e)
        {
            txtSupplier.Focus();
        }

        private void txtDocNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                MoveToReceipt();
            }
        }

        private void MasterReceiptForm_Paint(object sender, PaintEventArgs e)
        {
            Form f = sender as Form;
            if (f.Location.X != 0 || f.Location.Y != 0)
            {
                f.Location = new Point(0, 0);
            }
        }
    }
}
