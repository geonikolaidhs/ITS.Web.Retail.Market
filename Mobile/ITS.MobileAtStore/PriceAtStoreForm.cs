using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ITS.Common.Utilities.Compact;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using ITS.Common.Keyboards.Compact;
using ITS.MobileAtStore.WRMMobileAtStore;
using ITS.MobileAtStore.AuxilliaryClasses;

namespace ITS.MobileAtStore
{

    /// <summary>
    /// The settings form displays the already chosen settings and lets the user change and save them
    /// </summary>
    public partial class PriceAtStoreForm : Form
    {
        //FML
        Timer focusChecker = new Timer();
        Product _currentProd = null;
        public PriceAtStoreForm()
        {
            InitializeComponent();
            CustomUIConfig();
            ClearUI();
            this.Paint += new PaintEventHandler(SettingsForm.Form_Paint);
            WindowState = FormWindowState.Maximized;
            txtInput.Focus();
            focusChecker.Tick += new EventHandler(focusChecker_Tick);
            focusChecker.Interval = 3000;
            focusChecker.Enabled = true;
        }

        void focusChecker_Tick(object sender, EventArgs e)
        {
            if (lbOffers.Focused)
                txtInput.Focus();
        }

        private void CustomUIConfig()
        {
            lbOffers.ItemHeight = 37;
            lbOffers.DoubleClick += new EventHandler(lbOffers_DoubleClick);
        }

        void lbOffers_DoubleClick(object sender, EventArgs e)
        {
            if (lbOffers.SelectedIndex != -1 && lbOffers.Items != null && lbOffers.Items.Count > lbOffers.SelectedIndex)
            {
                MessageForm.Execute("Προσφορά", lbOffers.Items[lbOffers.SelectedIndex].ToString());
            }
        }



        private List<string> FormatOffers(Offer[] offers)
        {
            if (offers == null || offers.Length <= 0)
                return null;
            lbOffers.BeginUpdate();
            lbOffers.Items.Clear();
            List<string> ds = new List<string>();
            foreach (Offer offer in offers)
            {
                lbOffers.Items.Add(
                (offer.ValidForMembers ? "Για μέλη" : "Για όλους") +
                " - " + offer.DescriptionProcessed);
            }
            lbOffers.Visible = true;
            lbOffers.EndUpdate();
            return ds;
        }

        public static void Form_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            Form f = sender as Form;
            if (f.Location.X != 0 || f.Location.Y != 0)
            {
                f.Location = new Point(0, 0);
            }
        }


        public void ClearUI()
        {
            lblDescription.Text = "";
            lblPrice.Text = "";
            lblUnitPrice.Text = "";
            lblPoints.Text = "";
            lbOffers.Items.Clear();
            lbOffers.DataSource = null;
            lbOffers.Visible = false;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                SearchProduct();
            }
            else
            {
                if (e.KeyChar == (char)Keys.Escape)
                {
                    this.Close();
                }
            }
        }

        private void SearchProduct()
        {
            //MobileWebService service = null;
            string search = txtInput.Text.Trim();
            if (string.IsNullOrEmpty(search))
                return;
            try
            {
                ClearUI();
                _currentProd = null;
                using (var service = MobileAtStore.GetWebService(AppSettings.Timeout))
                {
                    Product prod = service.GetProduct(AppSettings.Terminal.ID, true, AppSettings.Terminal.IP, search, "", AppSettings.CompCode, "",eDocumentType.PRICE_CHECK,true);//TOCHECK
                    Offer[] offers;
                    if (prod != null)
                    {
                        offers = service.GetOffers(AppSettings.Terminal.ID, true, AppSettings.Terminal.IP, prod.Code, AppSettings.CompCode);
                        lblDescription.Text = prod.Description;
                        lblPrice.Text = prod.Price.ToString("0.00€");
                        lblUnitPrice.Text = prod.MeasurementUnitText + " " + prod.PricePerMeasurementUnit.ToString("0.00€");
                        lblPoints.Text = "Πόντοι: " + prod.Points;
                        FormatOffers(offers);
                        _currentProd = prod;
                    }
                    else
                    {
                        lblDescription.Text = "Το είδος δεν βρέθηκε";
                    }
                    txtInput.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageForm.Execute("Σφάλμα", "Υπήρξε κάποιο πρόβλημα κατά την ανάκτηση πληροφοριών είδους\r\n" + ex.Message);
            }
        }

        private void linkLabel1_Click(object sender, EventArgs e)
        {
            KeyboardGateway.OpenAlpha(txtInput, txtInput.Text, false);
            SearchProduct();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PriceAtStoreForm_Paint(object sender, PaintEventArgs e)
        {
            Form f = sender as Form;
            if (f.Location.X != 0 || f.Location.Y != 0)
            {
                f.Location = new Point(0, 0);
            }
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            if (_currentProd == null)
                return;
            try
            {
                CommonUtilities.WaitingCursor();
                using (var service = MobileAtStore.GetWebService(AppSettings.Timeout))
                {
                    bool result, resultSpecified;
                    service.ProductCheckAdd(AppSettings.Terminal.ID, true, AppSettings.Terminal.IP, _currentProd.Code, AppSettings.CompCode, out result, out resultSpecified);
                    if (resultSpecified && result)
                        MessageForm.Execute("Πληροφορία", "Το είδος έχει προστεθεί στην λίστα ελέγχου");
                    else
                        MessageForm.Execute("Σφάλμα", "Υπήρξε ένα σφάλμα κατά την προσθήκη του είδους στην λίστα ελέγχου");
                }
            }
            catch (Exception ex)
            {
                MessageForm.Execute("Σφάλμα", "Υπήρξε ένα σφάλμα κατά την προσθήκη του είδους στην λίστα ελέγχου\r\n" + ex.Message);
            }
            finally
            {
                CommonUtilities.NormalCursor();
            }
            txtInput.Focus();
        }

        private void btnUncheck_Click(object sender, EventArgs e)
        {
            if (_currentProd == null)
                return;
            try
            {
                CommonUtilities.WaitingCursor();
                using (var service = MobileAtStore.GetWebService(AppSettings.Timeout))
                {
                    bool result, resultSetted;
                    service.ProductCheckRemove(AppSettings.Terminal.ID, true, AppSettings.Terminal.IP, _currentProd.Code, AppSettings.CompCode, out result, out resultSetted);
                    if (result && resultSetted)
                        MessageForm.Execute("Πληροφορία", "Το είδος έχει αφαιρεθεί από την λίστα ελέγχου");
                    else
                        MessageForm.Execute("Σφάλμα", "Υπήρξε ένα σφάλμα κατά την αφαίρεση του είδους από την λίστα ελέγχου");
                }
            }
            catch (Exception ex)
            {
                MessageForm.Execute("Σφάλμα", "Υπήρξε ένα σφάλμα κατά την αφαίρεση του είδους από την λίστα ελέγχου\r\n" + ex.Message);
            }
            finally
            {
                CommonUtilities.NormalCursor();
            }
            txtInput.Focus();
        }
    }
}
