using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using InTheHand.Net.Sockets;
using InTheHand.Net;
using ITS.MobileAtStore.AuxilliaryClasses;

namespace ITS.MobileAtStore
{
    public partial class ScanNPair : Form
    {

        public const string Pattern = "cmd=pair,([0-9A-Fa-f]{12})$";
        public ScanNPair()
        {
            InitializeComponent();
        }

        BluetoothDeviceInfo client = null;
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (client != null)
            {
                QueueQRPrinterSettings.SetQueueQRPrinter(client.DeviceAddress, Encoding.GetEncoding(1253)) ;
                QueueQRPrinterSettings.Save();
            }
            this.Close();
        }

        private void txtProduct_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar == (char)Keys.Enter) && String.IsNullOrEmpty(this.txtProduct.Text) == false)
            {
                client = null;
                Regex rgx = new Regex(Pattern);
                MatchCollection matches = rgx.Matches(txtProduct.Text);
                if (matches.Count == 1)
                {
                    //MessageBox.Show("String: " + txtProduct.Text + "\r\nMatch:" + matches[0].Groups[1].Value);
                    client = new BluetoothDeviceInfo(BluetoothAddress.Parse(matches[0].Groups[1].Value));
                    {
                        txtInfo.Text =  client.DeviceName;                        
                    }
                }
                else
                {
                    MessageBox.Show("Match not found");
                }
            }
        }
    }
}