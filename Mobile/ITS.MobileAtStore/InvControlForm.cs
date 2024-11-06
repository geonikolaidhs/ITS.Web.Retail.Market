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
using ITS.MobileAtStore.AuxilliaryClasses;

namespace ITS.MobileAtStore
{
    public partial class InvControlForm : Form
    {
        public InvControlForm()
        {
            InitializeComponent();
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtProduct_KeyPress_(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                string search = txtProduct.Text.Trim();
                e.Handled = true;
                if (string.IsNullOrEmpty(search))
                {
                    return;
                }
                
                try
                {
                    using (var ws = MobileAtStore.GetWebService(AppSettings.Timeout))
                    {
                        Product product = CommonUtilities.ConvertProduct(ws.GetProduct(AppSettings.Terminal.ID, true, AppSettings.Terminal.IP, search, null, null, "", ITS.MobileAtStore.WRMMobileAtStore.eDocumentType.INVENTORY, true), MobileAtStore.ItemsDL);
                        if (product.Code == "")
                        {
                            product = null;
                        }
                        string prodCode = string.Empty;
                        string barCode = string.Empty;
                        if (product == null)
                        {
                            txtDescr.Text = search;
                            prodCode = search;
                            barCode = search;
                        }
                        else
                        {
                            txtDescr.Text = product.Description;
                            prodCode = product.Code;
                            barCode = product.Barcode;
                        }
                        WRMMobileAtStore.InvLine[] lines = ws.GetInvLines(AppSettings.Terminal.ID, true, AppSettings.Terminal.IP, prodCode);
                        if (lines == null || lines.Length == 0)
                        {
                            dataGrid1.DataSource = null;
                            return;
                        }
                        else
                        {
                            DataTable dt = new DataTable();
                            dt.TableName = "InvLine";
                            dt.Columns.Add("ProdCode", typeof(string));
                            dt.Columns.Add("Qty", typeof(decimal));
                            foreach (WRMMobileAtStore.InvLine line in lines)
                            {
                                object[] o = new object[2];
                                o[0] = !string.IsNullOrEmpty(line.ProdBarcode) ? line.ProdBarcode.TrimStart('0') : (!string.IsNullOrEmpty(line.ProdCode) ? line.ProdCode.TrimStart('0') : "");
                                o[1] = line.Qty;
                                dt.Rows.Add(o);
                            }
                            dataGrid1.DataSource = dt;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageForm.Execute("Σφάλμα", "Υπήρξε κάποιο πρόβλημα\r\n" + ex.Message, MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.CRITICAL);
                }
                //TODO retrieve records
            }
        }

        private void txtDescr_GotFocus(object sender, EventArgs e)
        {
            txtProduct.Focus();
        }

        private void InvControlForm_Paint(object sender, PaintEventArgs e)
        {
            Form f = sender as Form;
            if (f.Location.X != 0 || f.Location.Y != 0)
            {
                f.Location = new Point(0, 0);
            }
        }
    }
}
