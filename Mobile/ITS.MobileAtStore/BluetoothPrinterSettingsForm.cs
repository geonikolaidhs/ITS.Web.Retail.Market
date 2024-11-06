using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ITS.MobileAtStore.AuxilliaryClasses;
using ITS.Common.Utilities.Compact;
using InTheHand.Net;

namespace ITS.MobileAtStore
{
    public partial class BluetoothPrinterSettingsForm : Form
    {
        private List<ScannedBluetoothDevice> _ScannedDevices { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (_ScannedDevices == null || _ScannedDevices.Count == 0)
            {
                MessageForm.Execute("Σφάλμα", "Δε βρέθηκαν διαθέσιμες συσκευές Bluetooth", MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.NOTIFY);
                this.Close();
                return;
            }

            this.comboBoxBluetoothDevice.DataSource = _ScannedDevices;            

            this.comboBoxEncoding.DataSource = new List<Encoding>()
            {
                Encoding.ASCII,
                Encoding.BigEndianUnicode,
                Encoding.Unicode,
                Encoding.UTF7,
                Encoding.UTF8,
                Encoding.GetEncoding(1253)
            };

            if (QueueQRPrinterSettings.QueueQRPrinter != null)
            {
                if (QueueQRPrinterSettings.QueueQRPrinter.BluetoothAddress != null)
                {
                    this.comboBoxBluetoothDevice.SelectedValue = QueueQRPrinterSettings.QueueQRPrinter.BluetoothAddress;
                }
                if (QueueQRPrinterSettings.QueueQRPrinter.Encoding != null)
                {
                    this.comboBoxEncoding.SelectedValue = QueueQRPrinterSettings.QueueQRPrinter.Encoding;
                }
            }
        }

        public BluetoothPrinterSettingsForm(List<ScannedBluetoothDevice> scannedDevices)
        {
            InitializeComponent();
            this._ScannedDevices = scannedDevices;
        }

        private void buttonΟΚ_Click(object sender, EventArgs e)
        {
            try
            {
                ScannedBluetoothDevice scannedBluetoothDevice = (ScannedBluetoothDevice)this.comboBoxBluetoothDevice.SelectedItem;
                Encoding encoding = (Encoding)this.comboBoxEncoding.SelectedItem;
                QueueQRPrinterSettings.SetQueueQRPrinter(scannedBluetoothDevice.Address,encoding);
                QueueQRPrinterSettings.Save();
                this.Close();
            }
            catch (Exception exception)
            {
                MessageForm.Execute("Σφάλμα", exception.Message, MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.NOTIFY);
                this.Close();
                return;
            }
        }
    }
}