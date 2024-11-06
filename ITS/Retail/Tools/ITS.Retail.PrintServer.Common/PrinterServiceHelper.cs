using ITS.Common.Communication;
using ITS.Retail.Model;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ITS.Retail.PrintServer.Common
{
    public static class PrinterServiceHelper
    {
        private const int TIMEOUT = 120000;


        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);


        /// <summary>
        /// Test the availabality of remote printer service
        /// </summary>
        /// <param name="printerService">The remote printer service to be tested</param>
        /// <returns></returns>
        public static PrintServerGetPrintersResponse TestRemotePrinterServerConnection(POSDevice printerService)
        {
            EthernetDeviceSettings ethernetDeviceSettings = printerService.DeviceSettings as EthernetDeviceSettings;
            Logger logger = GetLogger();
            MessageClient client = new MessageClient(ethernetDeviceSettings.IPAddress, ethernetDeviceSettings.Port, logger);
            PrintServerGetPrintersRequest request = new PrintServerGetPrintersRequest();
            PrintServerGetPrintersResponse response = client.SendMessageAndWaitResponse<PrintServerGetPrintersResponse>(request, TIMEOUT);
            return response;
        }

        /// <summary>
        /// Sends a request to print a DocumentHeader
        /// </summary>
        /// <param name="printerService">The remote printer service</param>
        /// <param name="userOid">The Oid of the User that requested printing the DocumentHeader</param>
        /// <param name="documentHeaderOid">The Oid of the DocumentHeader to be printed</param>
        /// <param name="posID">The ID of the POS if the request is made by the POS</param>
        /// <param name="printerNickName">The NickName of the printer at the remote printer service</param>
        /// <returns></returns>
        public static PrintServerPrintDocumentResponse PrintDocument(POSDevice printerService, Guid userOid, Guid documentHeaderOid, int posID, string printerNickName = "")
        {
            EthernetDeviceSettings ethernetDeviceSettings = printerService.DeviceSettings as EthernetDeviceSettings;
            Logger logger = GetLogger();
            MessageClient client = new MessageClient(ethernetDeviceSettings.IPAddress, ethernetDeviceSettings.Port, logger);
            PrintServerPrintDocumentRequest request = new PrintServerPrintDocumentRequest()
            {
                DocumentID = documentHeaderOid,
                PosID = posID,
                PrinterNickName = printerNickName,
                UserID = userOid
            };
            PrintServerPrintDocumentResponse response = client.SendMessageAndWaitResponse<PrintServerPrintDocumentResponse>(request, TIMEOUT);
            return response;
        }

        public static PrintServerPrintLabelResponse PrintLabel(POSDevice printerService, string labelText, int encoding, string printerNickName = "")
        {
            EthernetDeviceSettings ethernetDeviceSettings = printerService.DeviceSettings as EthernetDeviceSettings;
            Logger logger = GetLogger();
            MessageClient client = new MessageClient(ethernetDeviceSettings.IPAddress, ethernetDeviceSettings.Port, logger);
            PrintServerPrintLabelRequest request = new PrintServerPrintLabelRequest()
            {
                LabelText = labelText,
                PrinterNickName = printerNickName,
                Encoding = encoding == 0 ? 737 : encoding// see also ToolsHelper.GetLabelsStringToPrint()
            };
            PrintServerPrintLabelResponse response = client.SendMessageAndWaitResponse<PrintServerPrintLabelResponse>(request, TIMEOUT);
            return response;
        }

        private static Logger GetLogger()
        {
            return NLog.LogManager.GetLogger("ITS.Retail");//TODO Remove this and update License Library
        }



        // SendBytesToPrinter()
        // When the function is given a printer name and an unmanaged array
        // of bytes, the function sends those bytes to the print queue.
        // Returns true on success, false on failure.
        public static bool SendBytesToPrinter(string szPrinterName, IntPtr pBytes, Int32 dwCount)
        {
            Int32 dwError = 0, dwWritten = 0;
            IntPtr hPrinter = new IntPtr(0);
            DOCINFOA di = new DOCINFOA();
            bool bSuccess = false; // Assume failure unless you specifically succeed.

            di.pDocName = "Labels";
            di.pDataType = "RAW";

            // Open the printer.
            if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
            {
                // Start a document.
                if (StartDocPrinter(hPrinter, 1, di))
                {
                    // Start a page.
                    if (StartPagePrinter(hPrinter))
                    {
                        // Write your bytes.
                        bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                        EndPagePrinter(hPrinter);
                    }
                    EndDocPrinter(hPrinter);
                }
                ClosePrinter(hPrinter);
            }
            // If you did not succeed, GetLastError may give more information
            // about why not.
            if (bSuccess == false)
            {
                dwError = Marshal.GetLastWin32Error();
            }
            return bSuccess;
        }


        public static bool SendByteArrayToPrinter(string printerName, byte[] arrayData)
        {
            try
            {
                bool bSuccess = false;

                IntPtr pUnmanagedBytes = new IntPtr(0);
                int nLength = arrayData.Length;

                // Allocate some unmanaged memory for those bytes.
                pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength);
                // Copy the managed byte array into the unmanaged array.
                Marshal.Copy(arrayData, 0, pUnmanagedBytes, nLength);
                // Send the unmanaged bytes to the printer.
                bSuccess = SendBytesToPrinter(printerName, pUnmanagedBytes, nLength);
                // Free the unmanaged memory that you allocated earlier.
                Marshal.FreeCoTaskMem(pUnmanagedBytes);
                return bSuccess;

            }
            catch (Exception ex)
            {
                //Program.Logger.Info(ex, "Error in printing");
                return false;
            }

        }

        public static bool SendFileToPrinter(string printerName, string fileName)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(fileName, FileMode.Open);

                // Create a BinaryReader on the file.
                BinaryReader br = new BinaryReader(fs, Encoding.GetEncoding(1253));
                // Dim an array of bytes big enough to hold the file's contents.
                Byte[] bytes = new Byte[fs.Length];
                bool bSuccess = false;
                // Your unmanaged pointer.
                IntPtr pUnmanagedBytes = new IntPtr(0);
                int nLength;
                nLength = Convert.ToInt32(fs.Length);
                //fs.Close();
                // Read the contents of the file into the array.

                bytes = br.ReadBytes(nLength);
                //bytes = Encoding.GetEncoding(1253).GetBytes(aaa);

                // Allocate some unmanaged memory for those bytes.
                pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength);
                // Copy the managed byte array into the unmanaged array.
                Marshal.Copy(bytes, 0, pUnmanagedBytes, nLength);
                // Send the unmanaged bytes to the printer.
                bSuccess = SendBytesToPrinter(printerName, pUnmanagedBytes, nLength);
                // Free the unmanaged memory that you allocated earlier.
                Marshal.FreeCoTaskMem(pUnmanagedBytes);
                return bSuccess;

            }
            catch (Exception ex)
            {
                string errorMessage = ex.GetFullMessage();
                return false;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
        }
        public static bool SendStringToPrinter(string printerName, string text)
        {
            IntPtr pBytes;
            Int32 dwCount;
            // How many characters are in the string?
            dwCount = text.Length;
            // Assume that the printer is expecting ANSI text, and then convert
            // the string to ANSI text.
            pBytes = Marshal.StringToCoTaskMemAnsi(text);
            // Send the converted ANSI string to the printer.
            SendBytesToPrinter(printerName, pBytes, dwCount);
            Marshal.FreeCoTaskMem(pBytes);
            return true;
        }
    }
}
