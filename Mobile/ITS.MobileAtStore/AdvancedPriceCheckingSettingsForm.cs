using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ITS.Common.Utilities.Compact;
using System.Collections;
using ITS.MobileAtStore.ObjectModel;

namespace ITS.MobileAtStore
{
    /// <summary>
    /// The settings form displays the already chosen settings and lets the user change and save them
    /// </summary>
    public partial class AdvancedPriceCheckingSettingsForm : Form
    {
        List<Warehouse> warehouses = null;
        List<PriceCatalogPolicy> priceCatalogPolicies = null;
        public AdvancedPriceCheckingSettingsForm()
        {
            InitializeComponent();
            try
            {
                warehouses = CommonUtilities.ConvertWarehouses(CommonUtilities.GetWarehousesFromWebService());
                priceCatalogPolicies = CommonUtilities.ConvertPriceCatalogPolicy(CommonUtilities.GetPriceCatalogPoliciesFromWebService());
                warehouses.Insert(0, new Warehouse() { CompCode = "", Description = "Επιλέξτε αποθηκευτικό χώρο" });
                priceCatalogPolicies.Insert(0, new PriceCatalogPolicy() { ID = "-1", Description = "Επιλέξτε τιμοκατάλογο" });
                cmbCompCode.DataSource = warehouses;
                cmdPriceCatalogPolicy.DataSource = priceCatalogPolicies;
                cmbCompCode.DisplayMember = "Description";
                cmbCompCode.ValueMember = "CompCode";
                cmdPriceCatalogPolicy.DisplayMember = "Description";
                cmdPriceCatalogPolicy.ValueMember = "ID";
                cmbCompCode.SelectedItem = GetSelectedWarehouse();
                cmdPriceCatalogPolicy.SelectedItem = GetSelectedPriceCatalogPolicy();
                this.Paint += new PaintEventHandler(SettingsForm.Form_Paint);
            }
            catch (Exception ex)
            {
                MessageForm.Execute("Σφάλμα", "Η τροποποίηση των ρυθμίσεων δεν είναι δυνατή αυτή την στιγμή εξαιτίας κάποιου σφάλματος\r\n" + ex.Message);
                DialogResult = DialogResult.OK;
            }
        }

        private Warehouse GetSelectedWarehouse()
        {           
            foreach (Warehouse ware in warehouses)
            {
                if (ware.CompCode == AppSettings.CompCode)
                {
                    return ware;
                }
            }
            return warehouses[0];
        }

        private PriceCatalogPolicy GetSelectedPriceCatalogPolicy()
        {            
            foreach (PriceCatalogPolicy policy in priceCatalogPolicies)
            {
                if (policy.ID == AppSettings.PriceList)
                {                    
                    return policy;
                }
            }
            return priceCatalogPolicies[0];
        }

        private void SetDataFromWebService()
        {
            throw new NotImplementedException();
        }

        #region Methods

        /// <summary>
        /// Saves the settings back to the XML
        /// </summary>
        /// <returns></returns>
        public bool SaveSettings()
        {
            try
            {

                AppSettings.CompCode = cmbCompCode.SelectedIndex == -1 ? "" : ((Warehouse)cmbCompCode.SelectedItem).CompCode;
                AppSettings.PriceList = cmdPriceCatalogPolicy.SelectedIndex == -1 ? "" : ((PriceCatalogPolicy)cmdPriceCatalogPolicy.SelectedItem).ID;
                return AppSettings.SaveSettings();
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region Event Handlers
        public static void Form_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            Form f = sender as Form;
            if (f.Location.X != 0 || f.Location.Y != 0)
            {
                f.Location = new Point(0, 0);
            }
        }
        #endregion
        /// <summary>
        /// Invokes save settings and if successful it closes the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button21_Click(object sender, EventArgs e)
        {
            if (this.SaveSettings())
            {
                this.Close();
            }
            else
            {
                MessageForm.Execute("Σφάλμα", "Υπήρξε ένα σφάλμα κατά την αποθήκευση των ρυθμίσεων", MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.CRITICAL);
            }
        }

        private void AdvancedPriceCheckingSettingsForm_Paint(object sender, PaintEventArgs e)
        {
            Form f = sender as Form;
            if (f.Location.X != 0 || f.Location.Y != 0)
            {
                f.Location = new Point(0, 0);
            }
        }
    }
}
