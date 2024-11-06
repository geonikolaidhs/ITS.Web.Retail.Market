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
    public partial class MasterForm : Form
    {
        private Document _document;
        private WRMMobileAtStore.Customer _customer;


        public MasterForm(Document document)
        {
            InitializeComponent();
            NumKeypad.LastValueText = Resources.LastQtyText;
            _document = document;
            RefreshUI();
            txtTrader.Focus();
            txtTrader.SelectAll();

        }

        private bool ValidateReceiptState()
        {
            return ValidateCustomer();
        }

        private bool ValidateDoc()
        {
            return ValidateReceiptState();
        }

        private List<WRMMobileAtStore.Warehouse> Warehouses = null;


        private bool ValidateCustomer()
        {
            string search = txtTrader.Text.Trim();
            if (!_document.HasCustomer && string.IsNullOrEmpty(search))
            {
                string trader = this._document.Header.DocType == DOC_TYPES.INVOICE ? "προμηθευτή" : "πελάτη";
                MessageForm.Execute("Ενημέρωση", "Καταχωρήστε "+trader+" για να συνεχίσετε", MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.NOTIFY);
                txtTrader.Focus();
                return false;
            }
            else if (_document.HasCustomer && string.IsNullOrEmpty(search))
            {
                RefreshUI();
                return true;
            }

            bool result = true;

            try
            {
                using (var ws = MobileAtStore.GetWebService(AppSettings.Timeout))
                {
                    if (this._document.Header.DocType == DOC_TYPES.INVOICE)
                    {
                        _customer = ws.GetSupplier(AppSettings.Terminal.ID, true, AppSettings.Terminal.IP, txtTrader.Text.Trim());
                    }
                    else if (this._document.Header.DocType == DOC_TYPES.INVOICE_SALES)
                    {
                        _customer = ws.GetCustomer(AppSettings.Terminal.ID, true, AppSettings.Terminal.IP, txtTrader.Text.Trim());
                    }
                    else if (this._document.Header.DocType == DOC_TYPES.TRANSFER)
                    {
                        if (this.Warehouses == null)
                        {
                            var allStores = ws.GetStores(AppSettings.Terminal.ID, true, AppSettings.Terminal.IP);
                            this.Warehouses = new List<ITS.MobileAtStore.WRMMobileAtStore.Warehouse>(allStores);
                        }
                        var wareHouse = Warehouses.Find(x => x.CompCode == txtTrader.Text.Trim());
                        if (wareHouse != null)
                        {
                            _customer = new ITS.MobileAtStore.WRMMobileAtStore.Customer()
                            {
                                AFM = wareHouse.CompCode,
                                Code = wareHouse.CompCode,
                                Name = wareHouse.Description,
                                ErrorMessage = ""
                            };
                        }
                    }

                    //If there was an error..
                    if (_customer == null)
                    {
                        string trader = this._document.Header.DocType == DOC_TYPES.INVOICE ? "Προμηθευτής" : "Πελάτης";
                        MessageForm.Execute("Πληροφορία", "Δε βρέθηκε ο " + trader + "!", MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.CRITICAL);
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
            catch (Exception exception)
            {
                string exceptionMessage = exception.Message + "\r\n" + exception.StackTrace;
                MessageForm.Execute("Σφάλμα", "Υπήρξε κάποιο πρόβλημα στην επικοινωνία με το web service", MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.CRITICAL);
                _customer = null;
                result = false;
            }
            RefreshUI();
            if (!result)
            {
                txtTrader.Focus();
                txtTrader.SelectAll();
            }
            else
            {
                btnContinue.Focus();
            }
            return result;
        }

        private void RefreshUI()
        {
            switch (this._document.Header.DocType)
            {

                case DOC_TYPES.INVOICE:
                    lblTrader.Text = "Προμηθευτής";
                    break;
                case DOC_TYPES.INVOICE_SALES:
                    lblTrader.Text = "Πελάτης";
                    break;
                case DOC_TYPES.TRANSFER:
                    lblTrader.Text = "Κατάστημα";
                    break;
            }
            txtTrader.Text = _document.Header.CustomerCode;
            txtTraderAFM.Text = _document.Header.CustomerAFM;
            txtTraderName.Text = _document.Header.CustomerName;
        }

        private void ReleaseFormResources()
        {
            if (ValidateReceiptState())
            {
                DialogResult = DialogResult.OK;
            }
        }
        #region Event Handlers

        private void btnContinue_Click(object sender, EventArgs e)
        {
            ReleaseFormResources();
        }


        private void dtpDocDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
            }
        }

        private void txtSupplier_KeyPress(object sender, KeyPressEventArgs e)
        {

        }
        #endregion

        private void btnReturn_Click(object sender, EventArgs e)
        {
            if (_document != null)
            {
                _document = null;
            }
            this.DialogResult = DialogResult.Cancel;
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
            txtTrader.Focus();
        }

        private void txtSupplierAFM_GotFocus(object sender, EventArgs e)
        {
            txtTrader.Focus();
        }

        private void txtDocNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                ReleaseFormResources();
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

        private void txtTrader_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                if (string.IsNullOrEmpty(txtTrader.Text.Trim()))
                {
                    return;
                }
                ValidateCustomer();
            }
        }
    }
}
