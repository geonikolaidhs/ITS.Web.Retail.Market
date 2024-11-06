using System;
using System.Collections.Generic;
using System.Text;
using InTheHand.Net.Sockets;
using InTheHand.Net;
using ITS.MobileAtStore.AuxilliaryClasses;
using System.IO;
using ITS.MobileAtStore.ObjectModel;
using System.Net.Sockets;
using System.Windows.Forms;
using ITS.Common.Utilities.Compact;
using System.Threading;

namespace ITS.MobileAtStore.Helpers
{
    public static class BluetoothPrinterHelper
    {
        public static List<ScannedBluetoothDevice> SearchBluetoothDevices()
        {
            List<ScannedBluetoothDevice> scannedDevices = new List<ScannedBluetoothDevice>();
            try
            {
                BluetoothClient client = new BluetoothClient();
                BluetoothDeviceInfo[] devices = client.DiscoverDevices(10, false, true, true);
                foreach (BluetoothDeviceInfo device in devices)
                {
                    scannedDevices.Add(new ScannedBluetoothDevice()
                                     {
                                         Name = device.DeviceName,
                                         Address = device.DeviceAddress
                                     }
                                   );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return scannedDevices;
        }

        public static bool Print(string data, out string printResultMessage)
        {
            if (QueueQRPrinterSettings.QueueQRPrinter == null || string.IsNullOrEmpty(QueueQRPrinterSettings.QueueQRPrinter.Address.Trim()))
            {
                if (DialogResult.No == MessageForm.Execute("Ερώτηση", "Δεν υπάρχει συνδεδεμένος εκτυπωτής. Θέλετε να αντιστοιχίσετε έναν τώρα;", MessageForm.DialogTypes.YESNO, MessageForm.MessageTypes.QUESTION))
                {
                    printResultMessage = "Δεν υπάρχει συνδεδεμένος εκτυπωτής";
                    return false;
                }
                using (SettingsForm frm = new SettingsForm())
                {
                    frm.ShowDialog();
                }
                if (QueueQRPrinterSettings.QueueQRPrinter == null || string.IsNullOrEmpty(QueueQRPrinterSettings.QueueQRPrinter.Address.Trim()))
                {
                    printResultMessage = "Δεν υπάρχει συνδεδεμένος εκτυπωτής";
                    return false;
                }
            }
            printResultMessage = string.Empty;
            BluetoothClient bt_client=null;
            try
            {
                bt_client = new BluetoothClient();
                if (bt_client.Connected == false)
                {
                    bt_client.Connect(new BluetoothEndPoint(QueueQRPrinterSettings.QueueQRPrinter.BluetoothAddress, QueueQRPrinterSettings.Service));
                }
                if (bt_client.Connected == false)
                {
                    printResultMessage = "Αδυναμία σύνδεσης με τον εκτυπώτή\r\nΠιθανότατα είναι κλειστός, ή βρίσκεται εκτός εμβέλειας ή δεν έχει ενεργοποιημένη την bluetooth επικοινωνία";
                    return false;
                }
    
                StreamWriter printer_port = new StreamWriter(bt_client.GetStream(), QueueQRPrinterSettings.QueueQRPrinter.Encoding);
                Encoding printerEncoding = printer_port.Encoding;
                printer_port.Write(data);
                printer_port.Flush();
                printer_port.Close();
                bt_client.Close();
                bt_client.Dispose();
                return true;
            }
            catch (SocketException exc)
            {
                printResultMessage = "Πρόβλημα στην επικοινωνία με τον εκτυπώτή\r\n" + exc.Message;
                return false;
            }
            catch (Exception exception)
            {
                printResultMessage = exception.Message + "\r\n" + exception.StackTrace;
                return false;
            }
            finally
            {
                if (bt_client != null)
                {
                    if (bt_client.Connected)
                    {
                        bt_client.Close();
                    }
                    bt_client.Dispose();
                }
            }
        }

    }
}
