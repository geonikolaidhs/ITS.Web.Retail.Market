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
    public partial class ESLInvControlForm : Form
    {
        public ESLInvControlForm()
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
                    using (WRMMobileAtStore.WRMMobileAtStore ws = MobileAtStore.GetWebService(AppSettings.Timeout))
                    {
                        var wsProduct = ws.GetProduct(AppSettings.Terminal.ID, true, AppSettings.Terminal.IP, search, null, null, "",ITS.MobileAtStore.WRMMobileAtStore.eDocumentType.ESL_INV,true);
                        if (wsProduct == null)
                        {
                            MessageForm.Execute("Ενημέρωση", "Το είδος δεν βρέθηκε", MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.NOTIFY);
                            txtProduct.Focus();
                            return;
                        }
                        Product prod = CommonUtilities.ConvertProduct(wsProduct, MobileAtStore.ItemsDL);
                        if (prod.Code == "")
                            prod = null;
                        string prodCode, barCode;
                        if (prod == null)
                        {
                            txtDescr.Text = search;
                            prodCode = search;
                            barCode = search;
                        }
                        else
                        {
                            txtDescr.Text = prod.Description;
                            prodCode = prod.Code;
                            barCode = prod.Barcode;
                        }
                        WRMMobileAtStore.ESLInvLine[] lines = ws.GetESLInvLines(AppSettings.Terminal.ID, true, AppSettings.Terminal.IP, prodCode, null);
                        if (lines == null || lines.Length == 0)
                        {
                            dataGrid1.DataSource = null;
                            return;
                        }
                        else
                        {
                            DataTable dt = new DataTable();
                            dt.TableName = "ESLInvLine";
                            dt.Columns.Add("InventoryNumber", typeof(string));
                            dt.Columns.Add("ProdCode", typeof(string));
                            dt.Columns.Add("Qty", typeof(decimal));
                            foreach (ITS.MobileAtStore.WRMMobileAtStore.ESLInvLine line in lines)
                            {
                                object[] o = new object[3];
                                o[0] = line.InventoryNumber;

                                o[1] = !string.IsNullOrEmpty(line.ProdBarcode) ? line.ProdBarcode.TrimStart('0') : (!string.IsNullOrEmpty(line.ProdCode) ? line.ProdCode.TrimStart('0') : "");
                                o[2] = line.Qty;
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

        private void ESLInvControlForm_Paint(object sender, PaintEventArgs e)
        {
            Form f = sender as Form;
            if (f.Location.X != 0 || f.Location.Y != 0)
            {
                f.Location = new Point(0, 0);
            }
        }
    }
}
