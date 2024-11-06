using System;
using ITS.MobileAtStore.AuxilliaryClasses;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ITS.Common.Utilities.Compact;
using ITS.MobileAtStore.Helpers;

namespace ITS.MobileAtStore
{
    /// <summary>
    /// The settings form displays the already chosen settings and lets the user change and save them
    /// </summary>
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            InitializeSettings();
            this.Paint += new PaintEventHandler(SettingsForm.Form_Paint);
            this.txtTerminalID.DataBindings.Add(new Binding("Text", AppSettings.Terminal,"ID"));
        }

        #region Methods
        /// <summary>
        /// Initializes and retrieves initial values for the settings
        /// </summary>
        public void InitializeSettings()
        {
            if (AppSettings.OperationMode == AppSettings.OPERATION_MODE.BATCH)
                cmbOperationMode.SelectedItem = "Batch";
            else
                cmbOperationMode.SelectedItem = "Online";

            cmbOperationMode.SelectedValue = AppSettings.OperationMode.ToString();
            txtWebServiceURL.Text = AppSettings.ServerIP;
        }

        /// <summary>
        /// Saves the settings back to the XML
        /// </summary>
        /// <returns></returns>
        public bool SaveSettings()
        {
            AppSettings.OperationMode = cmbOperationMode.SelectedItem.ToString() == "Batch" ? AppSettings.OPERATION_MODE.BATCH : AppSettings.OPERATION_MODE.ONLINE;
            AppSettings.ServerIP = txtWebServiceURL.Text;

            return AppSettings.SaveSettings();
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
                MessageForm.Execute("Επιτυχία", "Η αποθήκευση των ρυθμίσεων ήταν επιτυχής", MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.NOTIFY);
                this.Close();
            }
            else
            {
                MessageForm.Execute("Σφάλμα", "Υπήρξε ένα σφάλμα κατά την αποθήκευση των ρυθμίσεων", MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.CRITICAL);
            }
        }

        private void SettingsForm_Paint(object sender, PaintEventArgs e)
        {
            Form f = sender as Form;
            if (f.Location.X != 0 || f.Location.Y != 0)
            {
                f.Location = new Point(0, 0);
            }
        }

        private void buttonPairBluetoothPrinter_Click(object sender, EventArgs e)
        {
            List<ScannedBluetoothDevice> scannedDevices = BluetoothPrinterHelper.SearchBluetoothDevices();
            using (BluetoothPrinterSettingsForm bluetoothPrinterSettingsForm = new BluetoothPrinterSettingsForm(scannedDevices))
            {
                bluetoothPrinterSettingsForm.ShowDialog();
            }
        }

        private void btnScanNPair_Click(object sender, EventArgs e)
        {
            using (ScanNPair frm = new ScanNPair())
            {
                frm.ShowDialog();
            }
        }
    }
}
