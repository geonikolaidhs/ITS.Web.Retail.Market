using System;
using System.Collections.Generic;
using System.Text;
using InTheHand.Net.Bluetooth;
using InTheHand.Net;
using System.IO;
using System.Xml.Serialization;
using ITS.Common.Utilities.Compact;
using System.Xml;

namespace ITS.MobileAtStore.AuxilliaryClasses
{
    public static class QueueQRPrinterSettings
    {
        private static string QUEUE_QR_PRINTER_SETTINGS_FILE
        {
            get
            {
                return OpenNETCF.Windows.Forms.Application2.StartupPath + "//QueueQRPrinterSettings.xml";
            }
        }

        public static Guid Service
        {
            get
            {
                return BluetoothService.SerialPort;
            }
        }
        private static QueueQRPrinter _QueueQRPrinter { get; set; }
        public static QueueQRPrinter QueueQRPrinter
        {
            get
            {
                return _QueueQRPrinter;
            }
        }

        public static void SetQueueQRPrinter(BluetoothAddress bluetoothAddress, Encoding encoding)
        {
            if (_QueueQRPrinter != null)
            {
                _QueueQRPrinter = null;
            }
            _QueueQRPrinter = new QueueQRPrinter(bluetoothAddress, encoding);
        }

        public static void Save()
        {
            FileStream file = null;
            try
            {
                XmlSerializer xmlSerialiser = new System.Xml.Serialization.XmlSerializer(typeof(QueueQRPrinter));

                file = System.IO.File.Create(QUEUE_QR_PRINTER_SETTINGS_FILE);
                xmlSerialiser.Serialize(file, QueueQRPrinterSettings.QueueQRPrinter);
            }
            catch (Exception exception)
            {
                MessageForm.Execute("Σφάλμα", exception.Message, MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.NOTIFY);
            }
            finally
            {
                try
                {
                    if (file != null)
                    {
                        file.Close();
                    }
                }
                catch (Exception exc)
                {
                    string exceptionError = exc.Message;
                }
            }
        }

        public static void Load()
        {
            FileStream fileStream = null;
            try
            {
                XmlSerializer xmlSerialiser = new System.Xml.Serialization.XmlSerializer(typeof(QueueQRPrinter));
                fileStream = new FileStream(QUEUE_QR_PRINTER_SETTINGS_FILE, FileMode.Open);
                XmlReader xmlReader = XmlReader.Create(fileStream);
                QueueQRPrinter queryQRPrinter = xmlSerialiser.Deserialize(xmlReader) as QueueQRPrinter;
                QueueQRPrinterSettings.SetQueueQRPrinter(queryQRPrinter.BluetoothAddress, queryQRPrinter.Encoding);
            }
            catch (Exception exception)
            {
                string exceptionError = exception.Message;
                //MessageForm.Execute("Σφάλμα", exception.Message, MessageForm.DialogTypes.MESSAGE, MessageForm.MessageTypes.NOTIFY);
            }
            finally
            {
                try
                {
                    if (fileStream != null)
                    {
                        fileStream.Close();
                    }
                }
                catch (Exception exc)
                {
                    string exceptionError = exc.Message;
                }
            }
        }
    }
}
