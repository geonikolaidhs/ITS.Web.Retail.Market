using System;
using System.Linq;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Hardware.Common;
using OposLineDisplay_CCO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Reflection;

namespace ITS.POS.Hardware
{
    /// <summary>
    ///  Represents a generic poledisplay device. 
    /// </summary>
    public class PoleDisplay : Device
    {
        private frmPoleDisplayEmulatorOutput EmulatorForm;
        OPOSLineDisplayClass oposLineDisplay = null;
        bool oposIsOpened = false;

        string[] _EmptyLines;


        public PoleDisplay(ConnectionType conType, string deviceName)
            : base()
        {
            ConType = conType;
            DeviceName = deviceName;

            if (conType == ConnectionType.EMULATED)
            {
                EmulatorForm = new frmPoleDisplayEmulatorOutput();
                EmulatorForm.Text = "Emulated PoleDisplay: " + deviceName;
            }
        }

        public override void AfterLoad(List<Device> devices)
        {
            base.AfterLoad(devices);
            string emptyLine = new string(' ', this.Settings.LineChars);
            _EmptyLines = new string[this.Settings.NumberOfLines];
            for (int i = 0; i < this.Settings.NumberOfLines; i++)
            {
                _EmptyLines[i] = emptyLine;
            }
        }

        public override eDeviceCheckResult CheckDevice(out string message)
        {
            /*try
            {
                FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
                string[] display = new string[] { versionInfo.ProductName, versionInfo.LegalCopyright };
                if (this.Display(display, false, true, true) == DeviceResult.SUCCESS)
                {
                    message = "";
                    return eDeviceCheckResult.SUCCESS;
                }
                message = "Not Connected";
                return eDeviceCheckResult.WARNING;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return eDeviceCheckResult.WARNING;
            }*/
            message = "NOT CHECKED";
            return eDeviceCheckResult.INFO;
        }

        protected virtual void SelectCodePageCOM(int codePage, SerialPort sp)
        {
            throw new NotSupportedException();
        }

        protected virtual void ClearDisplayOPOS()
        {
            oposLineDisplay.ClearText();
        }

        protected virtual DeviceResult ClearDisplayCOM()
        {
            return DisplayCOM(_EmptyLines);
            //return DeviceResult.ACTIONNOTSUPPORTED;
        }

        protected virtual DeviceResult ClearDisplayEmulated()
        {
            if (EmulatorForm != null)
            {
                if (EmulatorForm.Visible == false)
                {
                    EmulatorForm.Show();
                    Application.DoEvents();
                }
                EmulatorForm.Invoke((MethodInvoker)delegate ()
                {
                    EmulatorForm.Line1TextBox.Text = "";
                    EmulatorForm.Line2TextBox.Text = "";
                    EmulatorForm.Line3TextBox.Text = "";
                    EmulatorForm.Line4TextBox.Text = "";
                    EmulatorForm.Line5TextBox.Text = "";
                });
            }
            return DeviceResult.SUCCESS;
        }

        /// <summary>
        /// WARNING: Default implementation only supports textMode = DisplayTextMode.Normal
        /// </summary>
        /// <param name="text"></param>
        /// <param name="textMode"></param>
        /// <returns></returns>
        protected virtual DeviceResult DisplayCOM(String[] text, DisplayTextMode textMode = DisplayTextMode.Normal)
        {
            if (textMode != DisplayTextMode.Normal)
            {
                return DeviceResult.ACTIONNOTSUPPORTED;
            }

            DeviceResult valueToReturn = DeviceResult.SUCCESS;

            SerialPort serialPort = new SerialPort();
            if (Settings.LineChars == 0)
            {
                Settings.LineChars = 15;
            }
            if (Settings.NumberOfLines == 0)
            {
                Settings.NumberOfLines = 1;
            }

            serialPort.PortName = Settings.COM.PortName;
            serialPort.Parity = Settings.COM.Parity;
            serialPort.NewLine = Settings.NewLine;
            serialPort.BaudRate = Settings.COM.BaudRate;
            try
            {
                serialPort.Open();
                try
                {
                    SelectCodePageCOM(Settings.CharacterSet, serialPort);
                }
                catch (Exception ex)
                {
                    valueToReturn = DeviceErrorConverter.ToDeviceResult(ex);//DeviceResult.CHARACTERSETNOTSUPPORTED;
                }
                foreach (String txt in text)
                {
                    byte[] toShow = Encoding.GetEncoding(Settings.CharacterSet).GetBytes(limitString(txt));
                    serialPort.Write(toShow, 0, toShow.Length);
                }
                serialPort.Close();
                return valueToReturn;
            }
            catch (Exception ex)
            {
                return DeviceErrorConverter.ToDeviceResult(ex);//DeviceResult.FAILURE;
            }

        }

        protected virtual DeviceResult DisplayEmulated(String[] text, DisplayTextMode textMode = DisplayTextMode.Normal)
        {
            DeviceResult valueToReturn = DeviceResult.SUCCESS;
            try
            {
                if (EmulatorForm != null)
                {
                    if (EmulatorForm.Visible == false)
                    {
                        EmulatorForm.Show();
                        Application.DoEvents();
                    }

                    string line1 = "";
                    string line2 = "";
                    string line3 = "";
                    string line4 = "";
                    string line5 = "";
                    try
                    {
                        line1 = text[0];
                        line2 = text[1];
                        line3 = text[2];
                        line4 = text[3];
                        line5 = text[4];
                    }
                    catch
                    {
                    }
                    EmulatorForm.Invoke((MethodInvoker)delegate ()
                    {
                        EmulatorForm.Line1TextBox.Text = line1;
                        EmulatorForm.Line2TextBox.Text = line2;
                        EmulatorForm.Line3TextBox.Text = line3;
                        EmulatorForm.Line4TextBox.Text = line4;
                        EmulatorForm.Line5TextBox.Text = line5;
                    });
                }
                return valueToReturn;
            }
            catch (Exception ex)
            {
                return DeviceErrorConverter.ToDeviceResult(ex);//DeviceResult.FAILURE;
            }

        }

        protected String limitString(String text1)
        {
            String text = (String.IsNullOrEmpty(text1)) ? "" : text1;

            if (text.Length < this.Settings.LineChars)
            {
                return text.PadRight(this.Settings.LineChars);
            }
            return text.Substring(0, this.Settings.LineChars);
        }

        protected void CloseOPOSPoleDisplay()
        {
            /* if (oposLineDisplay != null && oposLineDisplay.Claimed)
             {
                 oposLineDisplay.ReleaseDevice();
                 oposLineDisplay.Close();
             }*/
            if (oposLineDisplay != null && oposLineDisplay.Claimed)
            {
                oposLineDisplay.ReleaseDevice();
            }
            if (oposLineDisplay != null && oposIsOpened)
            {
                oposLineDisplay.Close();
            }
            oposLineDisplay = null;
            oposIsOpened = false;
        }

        protected void OpenOPOSPoleDisplay()
        {
            if (oposLineDisplay == null)
            {
                oposLineDisplay = new OPOSLineDisplayClass();
                try
                {
                    int result = oposLineDisplay.Open(Settings.OPOS.LogicalDeviceName);
                    int extendedResultCode = oposLineDisplay.ResultCodeExtended;
                    handleOposResult(result, extendedResultCode);
                    oposIsOpened = true;
                }
                catch (Exception ex)
                {
                    this.LogInfoException("Error Opening Pole Display", ex);
                    CloseOPOSPoleDisplay();
                    throw;
                }
            }

            try
            {
                if (!oposLineDisplay.Claimed)
                {
                    int result = oposLineDisplay.ClaimDevice(2000);
                    int extendedResultCode = oposLineDisplay.ResultCodeExtended;
                    handleOposResult(result, extendedResultCode);
                    oposLineDisplay.DeviceEnabled = true;
                    this.Settings.NumberOfLines = oposLineDisplay.DeviceRows;
                    this.Settings.LineChars = oposLineDisplay.DeviceColumns;
                    oposLineDisplay.CharacterSet = this.Settings.CharacterSet;
                }
            }
            catch (Exception ex)
            {
                this.LogInfoException("Error Claiming Pole Display", ex);
                CloseOPOSPoleDisplay();
                throw;
            }

        }

        protected void DisplayOPOS(String text, DisplayTextMode textMode = DisplayTextMode.Normal)
        {
            string textConv = this.ConvertString(text);
            handleOposResult(oposLineDisplay.DisplayText(textConv, (int)textMode));
        }

        protected void ClearTextOPOS()
        {
            handleOposResult(oposLineDisplay.ClearText());
        }

        protected List<String> oldValues = new List<string>();

        private object lockObject = new object();

        /// <summary>
        /// Shows the given text to the pole display.
        /// </summary>
        /// <param name="textToShow"></param>
        /// <param name="async"></param>
        /// <param name="fromTheTop"></param>
        /// <param name="clearFirst"></param>
        /// <returns></returns>
        public DeviceResult Display(String[] textToShow, bool async, bool fromTheTop, bool clearFirst)
        {
            if (oldValues.Count == textToShow.Length &&
                oldValues.Where(g => textToShow.Contains(g)).Count() == textToShow.Length)
            {
                return DeviceResult.SUCCESS;
            }
            oldValues.Clear();
            oldValues.AddRange(textToShow);
            Task<DeviceResult> task = new Task<DeviceResult>(() =>
            {
                Exception throwedException = null;
                lock (lockObject)
                {
                    try
                    {
                        if (ConType == ConnectionType.OPOS)
                        {
                            /// Ôodo 
                            /// When poledisplay is closed, this function inserts a delay of 2 seconds
                            OpenOPOSPoleDisplay();
                        }

                        if (ConnectionType.OPOS == ConType)
                        {
                            if (clearFirst)
                            {
                                ClearDisplayOPOS();
                            }
                            if (fromTheTop)
                            {
                                oposLineDisplay.CursorColumn = 0;
                                oposLineDisplay.CursorRow = 0;
                            }
                        }

                        for (int i = 0; i < textToShow.Length && i < this.Settings.NumberOfLines; i++)
                        {
                            switch (ConType)
                            {
                                case ConnectionType.OPOS:
                                    OpenOPOSPoleDisplay();
                                    DisplayOPOS(limitString(textToShow[i]));
                                    Thread.Sleep(100);
                                    break;
                                case ConnectionType.COM:
                                    if (clearFirst)
                                    {
                                        ClearDisplayCOM();
                                    }
                                    return DisplayCOM(textToShow);
                                case ConnectionType.EMULATED:
                                    if (clearFirst)
                                    {
                                        ClearDisplayEmulated();
                                    }
                                    return DisplayEmulated(textToShow);
                                default:
                                    throw new NotSupportedException();
                            }
                        }
                        return DeviceResult.SUCCESS;
                    }
                    catch(Exception ex)
                    {
                        throwedException = ex;
                    }
                }
                if(throwedException != null)
                {
                    throw throwedException;
                }
                throw new Exception("Locking exception");
            });

            task.ContinueWith(t =>
            {
                var aggException = t.Exception.Flatten();
                foreach (var exception in aggException.InnerExceptions)
                {
                    LogErrorException("PoleDisplay Error at Display", exception);
                }
                switch (ConType)
                {
                    case ConnectionType.OPOS:
                        CloseOPOSPoleDisplay();
                        break;
                }
               
            }, TaskContinuationOptions.OnlyOnFaulted);

            if (async && ConType != ConnectionType.EMULATED)
            {
                task.Start();
                return DeviceResult.SUCCESS;
            }
            else
            {
                task.RunSynchronously();
                return task.Result;
            }
        }

        protected void ClearRowOpos(int rowNumber)
        {
            int lineChars = this.Settings.LineChars;
            string text = "".PadLeft(lineChars, ' ');
            handleOposResult(oposLineDisplay.DisplayTextAt(rowNumber, 0, text, (int)DisplayTextMode.Normal));
        }

        protected void ClearRowEmulated(int rowNumber)
        {
            if (EmulatorForm != null)
            {
                if (EmulatorForm.Visible == false)
                {
                    EmulatorForm.Show();
                    Application.DoEvents();
                }
                EmulatorForm.Invoke((MethodInvoker)delegate ()
                {
                    if (rowNumber == 0)
                    {
                        EmulatorForm.Line1TextBox.Text = "";
                    }
                    else if (rowNumber == 1)
                    {
                        EmulatorForm.Line2TextBox.Text = "";
                    }
                    else if (rowNumber == 2)
                    {
                        EmulatorForm.Line3TextBox.Text = "";
                    }
                    else if (rowNumber == 3)
                    {
                        EmulatorForm.Line4TextBox.Text = "";
                    }
                    else if (rowNumber == 4)
                    {
                        EmulatorForm.Line5TextBox.Text = "";
                    }
                });
            }
        }

        /// <summary>
        /// Clears the given row of the pole display.
        /// </summary>
        /// <param name="rowNumber"></param>
        /// <returns></returns>
        public DeviceResult ClearRow(int rowNumber)
        {
            switch (ConType)
            {
                case ConnectionType.OPOS:
                    OpenOPOSPoleDisplay();
                    ClearRowOpos(rowNumber);
                    break;
                case ConnectionType.COM:
                    ////TODO
                    throw new NotSupportedException();
                case ConnectionType.EMULATED:
                    ClearRowEmulated(rowNumber);
                    break;
                default:
                    throw new NotSupportedException();
            }

            return DeviceResult.SUCCESS;
        }

        protected void DisplayLineOpos(String textToShow, int row, int column)
        {
            int maxLength = this.Settings.LineChars - column;
            handleOposResult(oposLineDisplay.DisplayTextAt(row, column, textToShow, (int)DisplayTextMode.Normal));
        }

        protected void DisplayLineEmulated(String textToShow, int row, int column)
        {
            System.Windows.Forms.RichTextBox textBox = null;
            if (row == 0)
            {
                textBox = EmulatorForm.Line1TextBox;
            }
            else if (row == 1)
            {
                textBox = EmulatorForm.Line2TextBox;
            }
            else if (row == 2)
            {
                textBox = EmulatorForm.Line3TextBox;
            }
            else if (row == 3)
            {
                textBox = EmulatorForm.Line4TextBox;
            }
            else if (row == 4)
            {
                textBox = EmulatorForm.Line5TextBox;
            }

            if (textBox != null)
            {
                textBox.Invoke((MethodInvoker)delegate ()
                {
                    var line = textBox.Text;
                    var stringBuilder = new StringBuilder(line);
                    int lineRemainingLength = (line.Length - column);
                    stringBuilder.Remove(column, (lineRemainingLength > textToShow.Length) ? textToShow.Length : lineRemainingLength);
                    stringBuilder.Insert(column, (lineRemainingLength > textToShow.Length) ? textToShow : textToShow.Substring(0, lineRemainingLength));
                    textBox.Text = stringBuilder.ToString();
                });
            }
        }

        /// <summary>
        /// Displays the given line to the given X (column) and Y (row) of the pole display.
        /// </summary>
        /// <param name="textToShow"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="async"></param>
        /// <returns></returns>
        public DeviceResult DisplayLine(String textToShow, int row, int column, bool async)
        {
            if (ConType == ConnectionType.OPOS)
            {
                OpenOPOSPoleDisplay();
            }

            Task<DeviceResult> task = new Task<DeviceResult>(() =>
            {
                switch (ConType)
                {
                    case ConnectionType.OPOS:
                        DisplayLineOpos(textToShow, row, column);
                        break;
                    case ConnectionType.COM:
                        throw new NotImplementedException();
                    case ConnectionType.EMULATED:
                        DisplayLineEmulated(textToShow, row, column);
                        break;
                    default:
                        throw new NotSupportedException();
                }
                return DeviceResult.SUCCESS;
            });

            task.ContinueWith(t =>
            {
                var aggException = t.Exception.Flatten();
                foreach (var exception in aggException.InnerExceptions)
                {
                    LogErrorException("PoleDisplay Error at DisplayLine", exception);
                }
                switch (ConType)
                {
                    case ConnectionType.OPOS:
                        CloseOPOSPoleDisplay();
                        break;
                }
            }, TaskContinuationOptions.OnlyOnFaulted);

            if (async)
            {
                task.Start();
                return DeviceResult.SUCCESS;
            }
            else
            {
                task.RunSynchronously();
                return task.Result;
            }

        }

        /// <summary>
        /// Displays the given text with a blinking effect. Supported ONLY for OPOS Connection type.
        /// </summary>
        /// <param name="textToShow"></param>
        /// <returns></returns>
        public DeviceResult Blink(String[] textToShow, bool async, bool fromTheTop, bool clearFirst)
        {
            if (ConType == ConnectionType.OPOS) //clear opos
            {
                OpenOPOSPoleDisplay();
            }

            Task<DeviceResult> task = new Task<DeviceResult>(() =>
            {
                if (ConnectionType.OPOS == ConType)
                {
                    if (clearFirst)
                    {
                        ClearDisplayOPOS();
                    }
                    if (fromTheTop)
                    {
                        oposLineDisplay.CursorRow = 0;
                        oposLineDisplay.CursorColumn = 0;
                    }
                }

                for (int i = 0; i < textToShow.Length && i < this.Settings.NumberOfLines; i++)
                {
                    switch (ConType)
                    {

                        case ConnectionType.OPOS:
                            OpenOPOSPoleDisplay();
                            DisplayOPOS(limitString(textToShow[i]), DisplayTextMode.Blink);
                            break;
                        //case ConnectionType.LPT:
                        //  return printToLPT(line, false);
                        case ConnectionType.COM:
                            if (clearFirst)
                            {
                                ClearDisplayCOM();
                            }
                            return DisplayCOM(textToShow, DisplayTextMode.Blink);
                        case ConnectionType.EMULATED:
                            if (clearFirst)
                            {
                                ClearDisplayEmulated();
                            }

                            DisplayEmulated(textToShow, DisplayTextMode.Blink);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }

                return DeviceResult.SUCCESS;
            });

            task.ContinueWith(t =>
            {
                var aggException = t.Exception.Flatten();
                foreach (var exception in aggException.InnerExceptions)
                {
                    LogErrorException("PoleDisplay Error at Blink", exception);
                }
                switch (ConType)
                {
                    case ConnectionType.OPOS:
                        CloseOPOSPoleDisplay();
                        break;
                }
            }, TaskContinuationOptions.OnlyOnFaulted);
            if (async)
            {
                task.Start();
                return DeviceResult.SUCCESS;
            }
            else
            {
                task.RunSynchronously();
                return task.Result;
            }
        }

        /// <summary>
        /// Clears the display.
        /// </summary>
        /// <returns></returns>
        public DeviceResult ClearDisplay()
        {
            switch (ConType)
            {

                case ConnectionType.OPOS:
                    OpenOPOSPoleDisplay();
                    ClearTextOPOS();
                    return DeviceResult.SUCCESS;
                case ConnectionType.COM:
                    return ClearDisplayCOM();
                case ConnectionType.EMULATED:
                    return ClearDisplayEmulated();
                default:
                    throw new NotImplementedException();
            }
        }


    }
}
