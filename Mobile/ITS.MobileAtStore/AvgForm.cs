using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ITS.Common.Keyboards.Compact;
using System.Globalization;
using ITS.MobileAtStore.ObjectModel;

namespace ITS.MobileAtStore
{
    public partial class AvgForm : Form
    {
        public Product Product { get; set; }

        public AvgForm(Product product, decimal previousQuantity, DOC_TYPES docType)
            //(string descr, string code, string barcode, decimal avg, string stock, decimal orderMM, DOC_TYPES docType, string extrainfo, string prev_qty, string supplier, string basicsupplier, int basicsuppliercolor)
        {
            InitializeComponent();

            this.Product = product;
            _supplier = product.Supplier;
            _basicSupplier = product.BasicSupplier;
            _code = product.Code;
            _barcode = product.Barcode;
            _productDescription = product.Description;
            this.documentType = docType;

            todec = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            fromdec = "";
            if (todec == ",")
            {
                fromdec = ".";
            }
            else
            {
                fromdec = ",";
            }

            lblposo.Text = product.AverageMonthSales.ToString();
            _avg = product.AverageMonthSales;
            textBox1.Text = "Περιγραφή: " + _productDescription + "\r\nΚωδικός: " + MyTrim(_code) + "\r\nBarcode: " + MyTrim(_barcode);
            textBox1.Text += "\r\nAX: " + _supplier + "\r\n";//extrainfo;// +"\r\n"
            txtSupplier.Text = "ΒΠ: " + _basicSupplier;
            if(product.BasicSupplierColor == 1)
            {
                txtSupplier.ForeColor = Color.Red;
            }
            else
            {
                txtSupplier.ForeColor = Color.DarkBlue;
            }
            lblapothema.Text = product.Stock.ToString();
            lblMM.Text = product.OrderMM.ToString();
            lblextrainfo.Text = product.extrainfo;
            this.Text = "Προηγ. Ποσ.:  " + previousQuantity;
            
            button2.Focus();
        }

        private string MyTrim(string source)
        {
            if(String.IsNullOrEmpty(source))
            {
                return source;
            }
            int position=0;
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i] != '0')
                {
                    position = i;
                    break;
                }
            }

            return source.Substring(position);
        }

        string todec = "";
        string fromdec = "";
        string _productDescription = "";
        string _code = "";
        
        string _barcode = "";
        public decimal _qty = 0;
        private decimal _avg = 0;
        private DOC_TYPES documentType;

        private string _supplier, _basicSupplier;
        

        private void txtqty_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || e.KeyChar == 13 || e.KeyChar == 8 || e.KeyChar == 44 || e.KeyChar == 46)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }


            if (e.KeyChar == 13)
            {
                _qty = (string.IsNullOrEmpty(lblqty.Text) ? 0 : Convert.ToDecimal(lblqty.Text.Replace(fromdec, todec)));
                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int finalDecimalDigits = this.Product != null && this.Product.SupportsDecimalQuantities ? AppSettings.QuantityNumberOfDecimalDigits : 0;
            if (!KeyboardGateway.OpenNumeric(ref _qty, null, false, false, 0, 9999.999m, true, AppSettings.QuantityNumberOfTotalDigits, finalDecimalDigits, NumKeypad.OPERATOR.FORBID_OPERATORS, 0, _productDescription))
            {
                return;
            }
            
            if(_qty >= AppSettings.MaximumAllowedQuantity )
            {
                MessageBox.Show("Η ποσότητα υπερβαίνει τη μέγιστη επιτρεπτή","ΠΡΟΣΟΧΗ");
                return;
            }

            lblqty.Text = _qty.ToString();
            _qty = (string.IsNullOrEmpty(lblqty.Text) ? 0 : Convert.ToDecimal(lblqty.Text.Replace(fromdec, todec)));

            if (AppSettings.PerformAverageQuantityCheck[this.documentType])
            {
                if (_qty > _avg * 6)
                {
                    DialogResult r = MessageBox.Show("Η ποσότητα παραγγελίας υπερβαίνει τη μέγιστη επιτρεπτή. Να καταχωρηθεί;", "ΠΡΟΣΟΧΗ", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (r == DialogResult.Yes)
                    {
                        this.Close();
                        return;
                    }
                    else if (r == DialogResult.No)
                    {
                        lblqty.Text = "0";                        
                        return;
                    }
                }
            }
            else
            {
                this.Close();
                return;
            }
        }

        private bool IsValidQuantity(decimal quantity)
        {
            quantity = Math.Abs(quantity);
            decimal integralQuantity = decimal.Truncate(quantity);
            decimal fractionQuantity = Math.Abs(quantity - integralQuantity);
            int integralQuantityDigits = integralQuantity.ToString().Length;
            int fractionQuantityDigits = fractionQuantity.ToString().Length - 2;//0 and fraction seperator
            if (fractionQuantityDigits < 0)
            {
                fractionQuantityDigits = 0;
            }
            if (this.Product != null && this.Product.SupportsDecimalQuantities)
            {
                return fractionQuantityDigits <= AppSettings.QuantityNumberOfDecimalDigits
                     && integralQuantityDigits <= AppSettings.QuantityNumberOfIntegralDigits
                     && fractionQuantityDigits + integralQuantityDigits <= AppSettings.QuantityNumberOfTotalDigits;
            }
            else
            {
                return fractionQuantityDigits <= 0
                     && integralQuantityDigits <= AppSettings.QuantityNumberOfTotalDigits;
            }
        }

        private void AvgForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                _qty = (string.IsNullOrEmpty(lblqty.Text) ? 0 : Convert.ToDecimal(lblqty.Text.Replace(fromdec, todec)));
                

                if( !IsValidQuantity(_qty) )
                {
                    MessageBox.Show(string.Format("Μη έγκυρη ποσότητα. Το μέγιστο ανεμενόμενο πλήθος δεκαδικών ψηφίων είναι: {0} και το συνολικό πλήθος ψηφίων : {1}",
                                                  AppSettings.QuantityNumberOfDecimalDigits,
                                                  AppSettings.QuantityNumberOfTotalDigits
                                                 ),
                                                 "Λάθος Ποσότητα",
                                                 MessageBoxButtons.OK,
                                                 MessageBoxIcon.Asterisk,
                                                 MessageBoxDefaultButton.Button1
                                    );
                    return;
                }

                if (AppSettings.PerformAverageQuantityCheck[this.documentType])
                {
                    if (_qty > _avg * 6)
                    {
                        DialogResult r = MessageBox.Show("Η ποσότητα παραγγελίας υπερβαίνει τη μέγιστη επιτρεπτή. Να καταχωρηθεί;", "ΠΡΟΣΟΧΗ", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                        if (r == DialogResult.Yes)
                        {
                            this.Close();
                            return;
                        }
                        else if (r == DialogResult.No)
                        {
                            lblqty.Text = "0";
                            return;
                        }
                    }
                }
                this.Close();
                return;
            }

            if (char.IsDigit(e.KeyChar) || e.KeyChar == 13 || e.KeyChar == 8 || e.KeyChar == 44 || e.KeyChar == 46)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
                return;
            }

            if (e.KeyChar == 44 || e.KeyChar == 46)
            {
                if (lblqty.Text.IndexOf('.') > -1 || lblqty.Text.IndexOf(',') > -1)
                {
                    e.Handled = true;
                    return;
                }
                else
                {
                    e.Handled = false;
                }
            }
          

            if (e.KeyChar == 8)
            {
                if (lblqty.Text.Length > 0)
                {
                    lblqty.Text = lblqty.Text.Substring(0, lblqty.Text.Length - 1);
                }
            }
            else
            {
                //check number before current character
                decimal userQuantity = (string.IsNullOrEmpty(lblqty.Text) ? 0 : Convert.ToDecimal(lblqty.Text.Replace(fromdec, todec)));
                if (IsValidQuantity(userQuantity))
                {
                    e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                    return;
                }

                string newValue = lblqty.Text;
                if (lblqty.Text == "0" && (e.KeyChar != 44 && e.KeyChar != 46))
                {
                    newValue = e.KeyChar.ToString();
                }
                else
                {
                    newValue += e.KeyChar;
                }

                //check number after current character
                userQuantity = (string.IsNullOrEmpty(lblqty.Text) ? 0 : Convert.ToDecimal(newValue.Replace(fromdec, todec)));
                if (IsValidQuantity(userQuantity))
                {
                    lblqty.Text = newValue;
                    e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                    return;
                }
                
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _qty = (string.IsNullOrEmpty(lblqty.Text) ? 0 : Convert.ToDecimal(lblqty.Text.Replace(fromdec, todec)));
            this.Close();
        }
    }
}