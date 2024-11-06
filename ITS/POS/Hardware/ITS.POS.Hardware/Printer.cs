using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.IO;
using ITS.Retail.Platform.Enumerations;
using OposPOSPrinter_CCO;
using ITS.POS.Hardware.Common;
using ITS.POS.Client.Exceptions;

namespace ITS.POS.Hardware
{
    /// <summary>
    ///  Represents a generic non-fiscal printer device. 
    /// </summary>
    public class Printer : Device, IDrawerPrinter
    {
        private SerialPort WritePort;
        private OPOSPOSPrinterClass OposPrinter;
        private FileStream lptPrinter;
        private bool TransactionOpen;
        private frmPrinterEmulatorOutput EmulatorForm;

        public Printer(ConnectionType conType, String deviceName)
            : base()
        {
            ConType = conType;
            TransactionOpen = false;
            DeviceName = deviceName;
            if (conType == ConnectionType.EMULATED)
            {
                EmulatorForm = new frmPrinterEmulatorOutput();
                EmulatorForm.Text = "Emulated Printer: " + deviceName;
            }
        }

        public override eDeviceCheckResult CheckDevice(out string message)
        {
            try
            {
                DeviceResult result = this.checkCanPrint();
                if (result == DeviceResult.SUCCESS)
                {
                    message = "";
                    return eDeviceCheckResult.SUCCESS;

                }
                message = "OFFLINE";
                return eDeviceCheckResult.FAILURE;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return eDeviceCheckResult.FAILURE;
            }
        }

        /// <summary>
        /// Function imported from the system for writing to LPT.
        /// </summary>
        /// <param name="lpFileName"></param>
        /// <param name="dwDesiredAccess"></param>
        /// <param name="dwShareMode"></param>
        /// <param name="lpSecurityAttributes"></param>
        /// <param name="dwCreationDisposition"></param>
        /// <param name="dwFlagsAndAttributes"></param>
        /// <param name="hTemplateFile"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess,
        uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition,
        uint dwFlagsAndAttributes, IntPtr hTemplateFile);


        /// <summary>
        /// Prints the lines given as parameter using the Settings.NewLine string (Default: "\n").  
        /// The appropriate settings must be set for the ConnectionType selected, before calling this function. (e.x. for ConnectionType.COM, use Settings.COM)
        /// </summary>
        /// <param name="lines">The lines to print.</param>
        /// <returns>SUCCESS,
        /// FAILURE,
        /// INVALIDPROPERTY -- A required Connection Setting is null, empty or invalid. For COM and LPT: PortName. For OPOS: LogicalDeviceName. ,
        /// UNAUTHORIZEDACCESS -- System denied access due to I/O Error or security error </returns>
        public virtual DeviceResult PrintLines(string[] lines)
        {
            DeviceResult checkResult = checkCanPrint();
            if (checkResult != DeviceResult.SUCCESS)
            {
                return checkResult;
            }

            if (ConType == ConnectionType.COM)
            {
                return printToCOM(lines, true);
            }
            else if (ConType == ConnectionType.OPOS)
            {
                return printToOpos(lines, true);
            }
            else if (ConType == ConnectionType.LPT)
            {
                return printToLPT(lines, true);
            }
            else if (ConType == ConnectionType.EMULATED)
            {
                return printToEmulator(lines, true);
            }

            return DeviceResult.CONNECTIONNOTSUPPORTED;
        }

        /// <summary>
        /// Opens and claims the device and will not release it until EndTransaction() is called. 
        /// Use this when you want to execute many actions in a row, without having to open and close the device in between.
        /// </summary>
        /// <returns>>SUCCESS,
        /// FAILURE,
        /// INVALIDPROPERTY --  A required Connection Setting is null, empty or invalid. For COM and LPT: PortName. For OPOS: LogicalDeviceName</returns>
        public virtual DeviceResult BeginTransaction()
        {
            try
            {
                switch (ConType)
                {
                    case ConnectionType.OPOS:
                        if (OposPrinter == null)
                        {
                            if (String.IsNullOrEmpty(Settings.OPOS.LogicalDeviceName))
                            {
                                return DeviceResult.INVALIDPROPERTY;
                            }
                        }
                        break;
                    case ConnectionType.COM:
                        if (WritePort == null)
                        {
                            if (String.IsNullOrEmpty(Settings.COM.PortName))
                            {
                                return DeviceResult.INVALIDPROPERTY;
                            }
                        }
                        break;
                    case ConnectionType.LPT:
                        if (lptPrinter == null)
                        {
                            if (String.IsNullOrEmpty(Settings.LPT.PortName))
                            {
                                return DeviceResult.INVALIDPROPERTY;
                            }
                        }
                        break;
                    case ConnectionType.EMULATED:
                        break;
                }
                TransactionOpen = true;
                return DeviceResult.SUCCESS;
            }
            catch (Exception ex)
            {
                return DeviceErrorConverter.ToDeviceResult(ex);
            }

        }
        /// <summary>
        /// Closes and releases the device. Use this when you are done, after calling BeginTransaction()
        /// </summary>
        public virtual void EndTransaction()
        {
            TransactionOpen = false;
            switch (ConType)
            {
                case ConnectionType.OPOS:
                    CloseOposPrinterConnection();
                    break;
                case ConnectionType.COM:
                    CloseComPrinterConnection(ref WritePort);
                    break;
                case ConnectionType.LPT:
                    CloseLPTPrinterConnection(ref lptPrinter);
                    break;
                case ConnectionType.EMULATED:
                    break;
            }

        }
        /// <summary>
        /// Prints the string given as parameter.
        /// The appropriate settings must be set for the ConnectionType selected, before calling this function. (e.x. for ConnectionType.COM, use Settings.COM).
        /// </summary>
        /// <param name="lines">The string to print.</param>
        /// <returns>SUCCESS,
        /// FAILURE,
        /// INVALIDPROPERTY --  A required Connection Setting is null, empty or invalid. For COM and LPT: PortName. For OPOS: LogicalDeviceName,
        /// UNAUTHORIZEDACCESS -- System denied access due to I/O Error or security error </returns>
        public virtual DeviceResult Print(string data)
        {
            string[] line = new String[1];
            line[0] = data;
            DeviceResult checkResult = checkCanPrint();
            if (checkResult != DeviceResult.SUCCESS)
            {
                return checkResult;
            }

            switch (ConType)
            {
                case ConnectionType.COM:
                    return printToCOM(line, false);
                case ConnectionType.OPOS:
                    return printToOpos(line, false);
                case ConnectionType.LPT:
                    return printToLPT(line, false);
                case ConnectionType.EMULATED:
                    return printToEmulator(line, false);
                case ConnectionType.OPERATING_SYSTEM_DRIVER:
                    return DeviceResult.SUCCESS;
            }


            return DeviceResult.CONNECTIONNOTSUPPORTED;
        }

        /// <summary>
        /// Makes calls to the proper "check" function , depending on connection type, 
        /// </summary>
        /// <returns></returns>
        protected DeviceResult checkCanPrint()
        {
            switch (ConType)
            {
                case ConnectionType.COM:
                    return checkCanPrintCOM();
                case ConnectionType.OPOS:
                    return checkCanPrintOPOS();
                case ConnectionType.LPT:
                    return checkCanPrintLPT();
                case ConnectionType.EMULATED:
                    return checkCanPrintEmulated();
                case ConnectionType.OPERATING_SYSTEM_DRIVER:
                    return DeviceResult.SUCCESS;
            }

            return DeviceResult.CONNECTIONNOTSUPPORTED;
        }

        /// <summary>
        /// Default implemetation checks for empty Port Name and Clear-to-Send line state.
        /// </summary>
        /// <returns></returns>
        protected virtual DeviceResult checkCanPrintCOM()
        {

            if (String.IsNullOrEmpty(Settings.COM.PortName))
            {
                return DeviceResult.INVALIDPROPERTY;
            }

            if (WritePort == null)
            {
                WritePort = OpenComPrinterConnection(Settings.COM.PortName);
                WritePort.NewLine = Settings.NewLine;
            }

            if (WritePort.CtsHolding == false)
                return DeviceResult.DEVICENOTREADY;

            return DeviceResult.SUCCESS;
        }

        /// <summary>
        /// Default implementation checks for Open cover, Out of paper and empty Logical Device Name.
        /// </summary>
        /// <returns></returns>
        protected virtual DeviceResult checkCanPrintOPOS()
        {
            if (String.IsNullOrEmpty(Settings.OPOS.LogicalDeviceName))
            {
                return DeviceResult.INVALIDPROPERTY;
            }

            OpenOposPrinterConnection(Settings.OPOS.LogicalDeviceName);
            if (OposPrinter.CoverOpen)
            {
                return DeviceResult.COVEROPEN;
            }
            else if (OposPrinter.RecEmpty)
            {
                return DeviceResult.OUTOFPAPER;
            }

            this.SupportsCutter = OposPrinter.CapRecPapercut;
            return DeviceResult.SUCCESS;
        }

        /// <summary>
        /// Default implemetation checks for empty Port Name.
        /// </summary>
        /// <returns></returns>
        protected virtual DeviceResult checkCanPrintLPT()
        {
            if (String.IsNullOrEmpty(Settings.LPT.PortName))
            {
                return DeviceResult.INVALIDPROPERTY;
            }
            return DeviceResult.SUCCESS;
        }

        /// <summary>
        /// Default implemetation checks for empty Port Name.
        /// </summary>
        /// <returns></returns>
        protected virtual DeviceResult checkCanPrintEmulated()
        {
            return DeviceResult.SUCCESS;
        }

        /// <summary>
        /// SUPPORTED ONLY BY OPOS CONNECTION.
        ///
        /// Prints a greyscale bitmap. The Settings.OPOS.LogicalDeviceName property must be set and the Settings.OPOS.OPOSPrinterSettings.PrinterStation property must be set to either Receipt or Slip.
        /// </summary>
        /// <param name="filename">Image path</param>
        /// <param name="width">Image Width. Can use PosPrinter.PrinterBitmapAsIs</param>
        /// <param name="alignment">Image Alignment. Use PosPrinter.PrinterBitmapCenter , PosPrinter.PrinterBitmapLeft or PosPrinter.PrinterBitmapRight</param>
        /// <returns>SUCCESS,
        /// FAILURE,
        /// OUTOFPAPER,
        /// COVEROPEN,
        /// INVALIDPROPERTY -- LogicalDeviceName property is null or empty,
        /// ACTIONNOTSUPPORTED -- This Device's OPOS Driver reports that it does not support this action OR the ConnectionType is not OPOS,
        /// STATIONNOTSUPPORTED -- PrinterStation is not Receipt or Slip
        /// </returns>
        public DeviceResult PrintBitmap(string filename, int width, int alignment)
        {
            if (ConType != ConnectionType.OPOS)
            {
                return DeviceResult.ACTIONNOTSUPPORTED;
            }

            if (OposPrinter != null && OposPrinter.Claimed && !TransactionOpen)
            {
                OposPrinter.Close();
                OposPrinter = null;
            }

            try
            {
                if (String.IsNullOrEmpty(Settings.OPOS.LogicalDeviceName))
                {
                    return DeviceResult.INVALIDPROPERTY;
                }
                if (OposPrinter == null)
                {
                    if (OposPrinter == null)
                        OposPrinter = new OPOSPOSPrinterClass();

                    OposPrinter.Open(Settings.OPOS.LogicalDeviceName);
                    OposPrinter.ClaimDevice(1000);
                    OposPrinter.DeviceEnabled = true;

                }

                if (!OposPrinter.CapRecBitmap) //check if can print bitmap
                {
                    if (!TransactionOpen)
                    {
                        OposPrinter.Close();
                        OposPrinter = null;
                    }
                    return DeviceResult.ACTIONNOTSUPPORTED;
                }

                if (OposPrinter.CoverOpen)  //check cover
                {
                    if (!TransactionOpen)
                    {
                        OposPrinter.Close();
                        OposPrinter = null;
                    }
                    return DeviceResult.COVEROPEN;
                }

                if (OposPrinter.RecEmpty) //check paper
                {
                    if (!TransactionOpen)
                    {
                        OposPrinter.Close();
                        OposPrinter = null;
                    }
                    return DeviceResult.OUTOFPAPER;
                }

                if (Settings.OPOS.PrinterSettings._PrinterStation != PrinterStation.Receipt && Settings.OPOS.PrinterSettings._PrinterStation != PrinterStation.Slip) //check station
                {
                    if (!TransactionOpen)
                    {
                        OposPrinter.Close();
                        OposPrinter = null;
                    }
                    return DeviceResult.STATIONNOTSUPPORTED;
                }


                OposPrinter.PrintBitmap((int)Settings.OPOS.PrinterSettings._PrinterStation, filename, width, alignment);
                if (!TransactionOpen)
                {
                    OposPrinter.Close();
                    OposPrinter = null;
                }

                return DeviceResult.SUCCESS;
            }

            catch (Exception ex)
            {
                if (!TransactionOpen)
                {
                    OposPrinter.Close();
                    OposPrinter = null;
                }
                return DeviceErrorConverter.ToDeviceResult(ex);
            }
        }
        /// <summary>
        /// Prints a barcode. The DeviceName property must be set and the PrinterStation property must be set to either Receipt or Slip.
        /// </summary>
        /// <param name="barcodeString">The barcode string to encode</param>
        /// <param name="mapMode">MapMode to use for drawing the barcode</param>
        /// <param name="barcodeSymbology">The BarcodeSymbology to encode the barcodeString into</param>
        /// <param name="height">The height of the bitmap in pixels</param>
        /// <param name="width">The width of the bitmap in pixels</param>
        /// <param name="alignment">The alignment of the barcode. Use PosPrinter.PrinterBarCodeCenter, PosPrinter.PrinterBarCodeLeft or PrinterBarCodeRight</param>
        /// <param name="barcodeTextPosition">Defines if and where the barcode text string will apear relative to the code.</param>
        /// <returns>SUCCESS,
        /// FAILURE,
        /// OUTOFPAPER,
        /// COVEROPEN,
        /// INVALIDPROPERTY -- DeviceName property is null or empty,
        /// ACTIONNOTSUPPORTED -- This Device's OPOS Driver reports that it does not support this action OR the ConnectionType is not OPOS,
        /// STATIONNOTSUPPORTED -- Station is not Receipt or Slip
        /// </returns>
        public DeviceResult PrintBarcode(string barcodeString, MapMode mapMode, BarCodeSymbology barcodeSymbology, int height, int width, int alignment, BarCodeTextPosition barcodeTextPosition)
        {
            if (OposPrinter != null && OposPrinter.Claimed && !TransactionOpen)
            {
                OposPrinter.Close();
                OposPrinter = null;
            }

            if (ConType != ConnectionType.OPOS)
            {
                return DeviceResult.ACTIONNOTSUPPORTED;
            }

            try
            {
                if (OposPrinter == null)
                {
                    if (OposPrinter == null) OposPrinter = new OPOSPOSPrinterClass();
                    OposPrinter.Open(Settings.OPOS.LogicalDeviceName);
                    OposPrinter.ClaimDevice(1000);
                    OposPrinter.DeviceEnabled = true;
                }

                if (!OposPrinter.CapRecBarCode) // check if can print barcode
                {
                    if (!TransactionOpen)
                    {
                        OposPrinter.Close();
                        OposPrinter = null;
                    }
                    return DeviceResult.ACTIONNOTSUPPORTED;
                }

                if (OposPrinter.CoverOpen) //check cover
                {
                    if (!TransactionOpen)
                    {
                        OposPrinter.Close();
                        OposPrinter = null;
                    }
                    return DeviceResult.COVEROPEN;
                }
                else if (OposPrinter.RecEmpty) //check paper
                {
                    if (!TransactionOpen)
                    {
                        OposPrinter.Close();
                        OposPrinter = null;
                    }
                    return DeviceResult.OUTOFPAPER;
                }

                if (Settings.OPOS.PrinterSettings._PrinterStation != PrinterStation.Receipt && Settings.OPOS.PrinterSettings._PrinterStation != PrinterStation.Slip) //check station
                {
                    if (!TransactionOpen)
                    {
                        OposPrinter.Close();
                        OposPrinter = null;
                    }
                    return DeviceResult.STATIONNOTSUPPORTED;
                }

                OposPrinter.PrintBarCode((int)Settings.OPOS.PrinterSettings._PrinterStation, barcodeString, (int)barcodeSymbology, height, width, alignment, (int)barcodeTextPosition);
                if (!TransactionOpen)
                {
                    OposPrinter.Close();
                    OposPrinter = null;
                }

                return DeviceResult.SUCCESS;
            }

            catch (Exception ex)
            {
                if (!TransactionOpen)
                {
                    OposPrinter.Close();
                    OposPrinter = null;
                }
                return DeviceErrorConverter.ToDeviceResult(ex);
            }
        }


        protected void OpenOposPrinterConnection(string logicalDeviceName)
        {
            if (OposPrinter == null)
            {
                OposPrinter = new OPOSPOSPrinterClass();
                try
                {
                    int result = OposPrinter.Open(logicalDeviceName);
                    int extendedResultCode = OposPrinter.ResultCodeExtended;
                    handleOposResult(result, extendedResultCode);
                }
                catch
                {
                    OposPrinter = null;
                    throw;
                }
            }
            if (!OposPrinter.Claimed)
            {
                int result = OposPrinter.ClaimDevice(2000);
                int extendedResultCode = OposPrinter.ResultCodeExtended;
                handleOposResult(result, extendedResultCode);
                OposPrinter.DeviceEnabled = true;
                OposPrinter.CharacterSet = this.Settings.CharacterSet;
            }
            else
            {
                if (OposPrinter.State != 2)
                {
                    throw new Exception(String.Format("Opos Printer State is : {0}", OposPrinter.State));
                }
            }
        }

        protected void CloseOposPrinterConnection()
        {
            if (OposPrinter != null && OposPrinter.Claimed)
            {
                // OposPrinter.DeviceEnabled = false;
                // handleOposResult(OposPrinter.ReleaseDevice());
            }
        }

        protected SerialPort OpenComPrinterConnection(string portName)
        {
            SerialPort comPrinter = new SerialPort();
            if (String.IsNullOrEmpty(portName))
            {
                return null;
            }
            comPrinter.PortName = portName;
            comPrinter.Open();
            return comPrinter;
        }

        protected void CloseComPrinterConnection(ref SerialPort printer)
        {
            if (printer != null && printer.IsOpen)
            {
                printer.Close();

            }
            printer = null;
        }

        protected FileStream OpenLPTConnection(string portName, uint genericWrite, uint openExisting)
        {
            FileStream printer = null;
            if (String.IsNullOrEmpty(portName))
            {
                return null;
            }
            IntPtr ptr = CreateFile(portName, genericWrite, 0,
            IntPtr.Zero, openExisting, 0, IntPtr.Zero);

            /* Is bad handle? INVALID_HANDLE_VALUE */
            if (ptr.ToInt32() == -1)
            {
                /* ask the framework to marshall the win32 error code to an exception */
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }
            else
            {
                printer = new FileStream(ptr, FileAccess.ReadWrite);
            }

            return printer;
        }

        protected void CloseLPTPrinterConnection(ref FileStream printer)
        {
            if (printer != null)
            {
                printer.Close();
                printer = null;
            }
        }

        protected virtual DeviceResult printToCOM(string[] lines, bool printNewLine)
        {
            try
            {
                if (WritePort == null)
                {
                    WritePort = OpenComPrinterConnection(Settings.COM.PortName);
                    WritePort.NewLine = Settings.NewLine;
                }

                WritePort.Encoding = Encoding.ASCII;
                foreach (String line in lines)
                {
                    byte[] str = Encoding.GetEncoding(Settings.CharacterSet <= 0 ? 737 : Settings.CharacterSet).GetBytes(line);
                    WritePort.Write(str, 0, str.Length);
                    if (printNewLine)
                        WritePort.Write(WritePort.NewLine);

                    DeviceResult checkResult = checkCanPrint(); // check after each line print if printer is still able to print
                    if (checkResult != DeviceResult.SUCCESS)
                    {
                        CloseComPrinterConnection(ref WritePort);
                        return checkResult;
                    }

                }

                return DeviceResult.SUCCESS;
            }
            catch (UnauthorizedAccessException)
            {
                if (WritePort != null && WritePort.IsOpen && !TransactionOpen)
                {
                    CloseComPrinterConnection(ref WritePort);
                }

                return DeviceResult.UNAUTHORIZEDACCESS;
            }
            catch (Exception ex)
            {
                if (WritePort != null && WritePort.IsOpen && !TransactionOpen)
                {
                    CloseComPrinterConnection(ref WritePort);
                }

                return DeviceErrorConverter.ToDeviceResult(ex);
            }
        }

        protected virtual DeviceResult printToOpos(string[] lines, bool printNewLine)
        {
            try
            {
                bool needsConvertion = false, needsTsompaniaConversion = false;
                OpenOposPrinterConnection(Settings.OPOS.LogicalDeviceName);

                if (!String.IsNullOrEmpty(Settings.OPOS.PrinterSettings.LogoText))
                {
                    int result = OposPrinter.SetLogo((int)Settings.OPOS.PrinterSettings._LogoLocation, Settings.OPOS.PrinterSettings.LogoText);
                    int extendedResultCode = OposPrinter.ResultCodeExtended;
                    handleOposResult(result, extendedResultCode);
                }

                //needsConvertion = (Settings.CharacterSet == 737);
                //needsTsompaniaConversion = Settings.CharacterSet == 73799;

                if (OposPrinter.CapTransaction) //if printer supports it, begin transactional print
                {
                    int result = OposPrinter.TransactionPrint((int)Settings.OPOS.PrinterSettings._PrinterStation, (int)PrinterTransactionControl.Transaction);
                    int extendedResultCode = OposPrinter.ResultCodeExtended;
                    handleOposResult(result, extendedResultCode);
                }
                foreach (string line in lines)
                {
                    String linetoSend = ConvertString(line);
                    //needsConvertion ? ConvertStringTo737(line) : (needsTsompaniaConversion ? ConvertGreekStringToGreeklish(line) : line);
                    if (printNewLine)
                    {
                        linetoSend = linetoSend + Settings.NewLine;
                    }
                    int result = OposPrinter.PrintNormal((int)Settings.OPOS.PrinterSettings._PrinterStation, linetoSend);
                    int extendedResultCode = OposPrinter.ResultCodeExtended;
                    handleOposResult(result, extendedResultCode);


                    DeviceResult checkResult = checkCanPrint(); // check after each line print if printer is still able to print
                    if (checkResult != DeviceResult.SUCCESS)
                    {
                        CloseOposPrinterConnection();
                        return checkResult;
                    }
                }

                if (OposPrinter.CapTransaction) //End Transaction print
                {
                    int result = OposPrinter.TransactionPrint((int)Settings.OPOS.PrinterSettings._PrinterStation, (int)PrinterTransactionControl.Normal);
                    int extendedResultCode = OposPrinter.ResultCodeExtended;
                    if (result != 0)
                    {
                        OposPrinter.ClearOutput();
                    }
                    handleOposResult(result, extendedResultCode);
                }


                if (!TransactionOpen)
                {
                    CloseOposPrinterConnection();
                }
                return DeviceResult.SUCCESS;
            }
            catch (Exception ex)
            {
                LogInfoException("Exception During OPOS Printing", ex);
                return DeviceErrorConverter.ToDeviceResult(ex);
            }

        }

        protected virtual DeviceResult printToLPT(string[] lines, bool printNewLine)
        {
            try
            {
                Byte[] buffer = new Byte[2048];

                if (lptPrinter == null)
                {
                    lptPrinter = OpenLPTConnection(Settings.LPT.PortName, Settings.LPT.GENERIC_WRITE, Settings.LPT.OPEN_EXISTING);
                }

                foreach (string line in lines)
                {
                    string lineToPrint = line;

                    if (printNewLine)
                    {
                        lineToPrint = lineToPrint + Settings.NewLine;
                    }

                    buffer = Encoding.GetEncoding(Settings.CharacterSet).GetBytes(line);
                    //Check to see if your printer support ASCII encoding or Unicode.
                    //If unicode is supported, use the following:
                    //buffer = System.Text.Encoding.Unicode.GetBytes(Temp);
                    lptPrinter.Write(buffer, 0, buffer.Length);

                    DeviceResult checkResult = checkCanPrint(); // check after each line print if printer is still able to print
                    if (checkResult != DeviceResult.SUCCESS)
                    {
                        CloseLPTPrinterConnection(ref lptPrinter);
                        return checkResult;
                    }
                }
                return DeviceResult.SUCCESS;
            }
            catch (Exception ex)
            {
                if (!TransactionOpen && lptPrinter != null)
                {
                    CloseLPTPrinterConnection(ref lptPrinter);
                }
                return DeviceErrorConverter.ToDeviceResult(ex);
            }
        }

        protected virtual DeviceResult printToEmulator(string[] lines, bool printNewLine)
        {
            try
            {
                if (EmulatorForm != null)
                {
                    if (EmulatorForm.Visible == false)
                    {
                        EmulatorForm.Show();
                    }

                    foreach (string line in lines)
                    {
                        string lineToPrint = line;

                        if (printNewLine)
                        {
                            lineToPrint = lineToPrint + Settings.NewLine;
                        }

                        EmulatorForm.RichTextBox.AppendText(lineToPrint);
                        EmulatorForm.RichTextBox.SelectionStart = EmulatorForm.RichTextBox.Text.Length;
                        EmulatorForm.RichTextBox.ScrollToCaret();
                    }
                }
                return DeviceResult.SUCCESS;
            }
            catch (Exception ex)
            {
                return DeviceErrorConverter.ToDeviceResult(ex);
            }
        }

        public DeviceResult OpenDrawer(string openDrawerCustomString)
        {
            return this.Print(openDrawerCustomString);
        }

        public override bool ShouldRunOnMainThread
        {
            get
            {
                if (ConType == ConnectionType.OPOS)
                {
                    return true;
                }
                return base.ShouldRunOnMainThread;
            }
        }


        public bool SupportsCutter
        {
            get; set;
        }


    }
}
