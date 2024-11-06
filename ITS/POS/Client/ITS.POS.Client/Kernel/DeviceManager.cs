using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Forms;
using ITS.POS.Client.UserControls;
using ITS.POS.Hardware;
using ITS.POS.Hardware.Common;
using ITS.POS.Hardware.Micrelec.Fiscal;
using ITS.POS.Hardware.RBS.Fiscal;
using ITS.POS.Hardware.Wincor.Fiscal;
using ITS.POS.Model.Settings;
using ITS.POS.Model.Transactions;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace ITS.POS.Client.Kernel
{
    /// <summary>
    /// Loads and provides access to the application's devices.
    /// </summary>
    public class DeviceManager : IDeviceManager
    {
        public List<Device> Devices { get; set; }

        /// <summary>
        /// Loads the devices from the configuration file.
        /// </summary>
        /// <param name="devicesXml"></param>
        /// <param name="logger"></param>
        /// <param name="posID"></param>
        public void LoadDevices(string devicesXml, Logger logger, int posID)
        {
            this.Devices = new List<Device>();
            keyStatus = eKeyStatus.UNKNOWN;

            if (File.Exists(devicesXml))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(devicesXml);
                XmlNode devicesNode = null;
                try
                {
                    devicesNode = xmlDoc.GetElementsByTagName("Devices")[0];
                }
                catch (Exception ex)
                {
                    logger.Info(ex, "GlobalContext:InitializeDevices,Exception catched");
                    throw new Exception(String.Format(POSClientResources.FILE_0_IS_INVALID, devicesXml));
                }

                foreach (XmlNode deviceNodeAbs in devicesNode.ChildNodes)
                {
                    if (deviceNodeAbs.NodeType == XmlNodeType.Comment)
                    {
                        continue;
                    }
                    Device device = null;
                    XmlElement deviceNode = deviceNodeAbs as XmlElement;
                    try
                    {
                        string contype = deviceNode.GetElementsByTagName("ConnectionType")[0].InnerText.ToUpper();
                        string name = deviceNode.GetElementsByTagName("Name")[0].InnerText;
                        string deviceType = deviceNode.GetAttribute("DeviceType");
                        //for phiscal printers 
                        int lineChars = 0;
                        int commandChars = 0;

                        Int32.TryParse(deviceNode.GetElementsByTagName("LineChars")[0].InnerText, out lineChars);
                        Int32.TryParse(deviceNode.GetElementsByTagName("CommandChars")[0].InnerText, out commandChars);

                        if (!String.IsNullOrEmpty(deviceType)) //Loads specific device dll
                        {
                            Type type = ITS.POS.Hardware.DynamicModules.GetType(deviceType);
                            if (type != null && type.IsSubclassOf(typeof(Device)))
                            {
                                device = System.Activator.CreateInstance(type, (ConnectionType)Enum.Parse(typeof(ConnectionType), contype), name) as Device;
                            }
                        }

                        if (device == null || String.IsNullOrEmpty(deviceType)) //if load failed or not required, load generic dll
                        {

                            switch (deviceNode.Name)
                            {
                                case "Scanner":
                                    device = new Scanner((ConnectionType)Enum.Parse(typeof(ConnectionType), contype), name);
                                    break;
                                case "Printer":
                                    device = new Printer((ConnectionType)Enum.Parse(typeof(ConnectionType), contype), name);
                                    break;
                                case "Drawer":
                                    device = new Drawer((ConnectionType)Enum.Parse(typeof(ConnectionType), contype), name);
                                    (device as Drawer).OnStatusChange += Drawer_OnStatusChange;
                                    break;
                                case "DataSignESD":
                                    device = new DataSignESD((ConnectionType)Enum.Parse(typeof(ConnectionType), contype), name);
                                    break;
                                case "AlgoboxNetESD":
                                    device = new AlgoboxNetESD((ConnectionType)Enum.Parse(typeof(ConnectionType), contype), name);
                                    break;
                                case "DiSign":
                                    device = new DiSign((ConnectionType)Enum.Parse(typeof(ConnectionType), contype), name);
                                    break;
                                case "PoleDisplay":
                                    device = new PoleDisplay((ConnectionType)Enum.Parse(typeof(ConnectionType), contype), name);
                                    break;
                                case "KeyLock":
                                    device = new KeyLock((ConnectionType)Enum.Parse(typeof(ConnectionType), contype), name);
                                    (device as KeyLock).StatusUpdateEvent += KeyLock_OnStatusChange;
                                    break;
                                case "MagneticStripReader":
                                    device = new MagneticStripReader((ConnectionType)Enum.Parse(typeof(ConnectionType), contype), name);
                                    (device as MagneticStripReader).ReadEvent += MagneticStripReader_OnRead;
                                    break;
                                case "MicrelecFiscalPrinter":
                                    device = new MicrelecFiscalPrinter((ConnectionType)Enum.Parse(typeof(ConnectionType), contype), name, posID, lineChars, commandChars);
                                    break;
                                case "RBSElioFiscalPrinter":
                                    device = new RBSElioFiscalPrinter((ConnectionType)Enum.Parse(typeof(ConnectionType), contype), name, posID, lineChars, commandChars);
                                    break;
                                case "WincorFiscalPrinter":
                                    device = new WincorFiscalPrinter((ConnectionType)Enum.Parse(typeof(ConnectionType), contype), name, posID, lineChars, commandChars);
                                    break;
                                case "Scale":
                                    device = new Scale((ConnectionType)Enum.Parse(typeof(ConnectionType), contype), name);
                                    break;
                                case "Edps":
                                    device = new EdpsPaymentCreditDevice((ConnectionType)Enum.Parse(typeof(ConnectionType), contype), name);
                                    break;
                                case "RemotePrint":
                                    device = new RemotePrint((ConnectionType)Enum.Parse(typeof(ConnectionType), contype), name);
                                    break;
                                case "Cardlink":
                                    device = new CardlinkPaymentCreditDevice((ConnectionType)Enum.Parse(typeof(ConnectionType), contype), name);
                                    break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Info(ex, "GlobalContext:InitilizeDevices,Exception catched");
                        throw new Exception(POSClientResources.DEVICE_INIT_FAILED + ":" + POSClientResources.NO_OR_INVALID_CONNECTION);
                    }
                    try
                    {
                        device.SetLogger(logger);
                        ReadDeviceSettings(device, deviceNode);
                    }
                    catch (Exception e)
                    {
                        logger.Info(e, "GlobalContext:InitilizeDevices,Exception catched");
                        throw new Exception(POSClientResources.DEVICE_INIT_FAILED + ":" + e.GetFullMessage());
                    }
                    Devices.Add(device);
                }
            }
            foreach (Device device in Devices)
            {
                try
                {
                    device.AfterLoad(Devices);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Device " + device.GetType().Name + " '" + device.DeviceName + "' After Load Error");
                }
            }

        }


        /// <summary>
        /// Event Handler for all the magnetic strips' "OnRead" event.
        /// 
        /// TODO: Bind this data to payment methods and/or payment method input fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MagneticStripReader_OnRead(object sender, MsrReadEventArgs e)
        {
            string errorString = "Invalid Track Data";

            string cardHolder = "";
            try
            {
                cardHolder = e.Track1Data.Split('^')[1].Trim();
            }
            catch
            {
                cardHolder = errorString;
            }

            string cardNumber = "";
            try
            {
                cardNumber = e.Track2Data.Split('=')[0];
            }
            catch
            {
                cardNumber = errorString;
            }

            if (cardNumber != errorString)
            {
                FormCollection fc = Application.OpenForms;
                foreach (Form openForm in fc)
                {
                    if (openForm.GetType() == typeof(frmCustomFieldsInput))
                    {
                        SendKeys.SendWait(cardNumber);
                        break;
                    }
                }
            }
            else
            {
                throw new POSException(errorString + " Track2 Data: " + e.Track2Data);
            }
        }

        /// <summary>
        /// Gets the primary device instance of the given device type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetPrimaryDevice<T>() where T : Device
        {
            Type parameterType = typeof(T);
            T device = Devices.Where(g => (g.GetType().Equals(parameterType) || g.GetType().IsSubclassOf(parameterType)) && g.Settings.IsPrimary).FirstOrDefault() as T;
            if (device == null) //No primary device of this type found
            {
                IEnumerable<Device> filteredDevices = Devices.Where(g => (g.GetType().Equals(parameterType) || g.GetType().IsSubclassOf(parameterType)));
                //If only one device of that type exists, get that one
                if (filteredDevices.Count() == 1)
                {
                    device = filteredDevices.FirstOrDefault() as T;
                }
            }
            return device;
        }


        public T GetDeviceByName<T>(String name) where T : Device
        {
            Type parameterType = typeof(T);
            T device = Devices.Where(g => (g.GetType().Equals(parameterType) || g.GetType().IsSubclassOf(parameterType)) && g.DeviceName == name).FirstOrDefault() as T;
            if (device == null) //No device of this type and name found
            {
                IEnumerable<Device> filteredDevices = Devices.Where(g => (g.GetType().Equals(parameterType) || g.GetType().IsSubclassOf(parameterType)));
                //If only one device of that type exists, get that one
                if (filteredDevices.Count() == 1)
                {
                    device = filteredDevices.FirstOrDefault() as T;
                }
            }

            return device;
        }



        /// <summary>
        /// Gets the EAFDSS device defined.
        /// 
        /// TODO Make an Abstract EAFDSS Class.
        /// </summary>
        /// <param name="fiscalDevice"></param>
        /// <returns></returns>
        public Device GetEAFDSSDevice(eFiscalDevice fiscalDevice)
        {
            switch (fiscalDevice)
            {
                case eFiscalDevice.DATASIGN:
                    List<Device> datasigns = this.Devices.Where(x => x.GetType() == typeof(DataSignESD)).ToList();
                    DataSignESD defaultDataSign = this.GetPrimaryDevice<DataSignESD>() ?? (datasigns.FirstOrDefault() as DataSignESD);
                    if (datasigns.Count == 0 || defaultDataSign == null)
                    {
                        throw new POSException(POSClientResources.NO_ELECTRONIC_SIGNATURE_DEVICE_FOUND);
                    }
                    return defaultDataSign;
                case eFiscalDevice.ALGOBOX_NET:
                    List<Device> algoboxes = this.Devices.Where(x => x.GetType() == typeof(AlgoboxNetESD)).ToList();
                    AlgoboxNetESD defaultAlgobox = this.GetPrimaryDevice<AlgoboxNetESD>() ?? (algoboxes.FirstOrDefault() as AlgoboxNetESD);
                    if (algoboxes.Count == 0 || defaultAlgobox == null)
                    {
                        throw new POSException(POSClientResources.NO_ELECTRONIC_SIGNATURE_DEVICE_FOUND);
                    }
                    return defaultAlgobox;
                case eFiscalDevice.DISIGN:
                    List<Device> disigns = this.Devices.Where(x => x.GetType() == typeof(DiSign)).ToList();
                    DiSign defaultDisign = this.GetPrimaryDevice<DiSign>() ?? (disigns.FirstOrDefault() as DiSign);
                    if (defaultDisign == null)
                    {
                        throw new POSException(POSClientResources.NO_ELECTRONIC_SIGNATURE_DEVICE_FOUND);
                    }
                    return defaultDisign;
            }
            throw new POSException(POSClientResources.INVALID_FISCAL_DEVICE);
        }

        /// <summary>
        /// Gets a List of EAFDSS devices defined order by priority.
        /// 
        /// TODO Make an Abstract EAFDSS Class.
        /// </summary>
        /// <param name="fiscalDevice"></param>
        /// <returns></returns>
        public List<Device> GetEAFDSSDevicesByPriority(eFiscalDevice fiscalDevice)
        {
            Type type = null;
            switch (fiscalDevice)
            {
                case eFiscalDevice.DATASIGN:
                    type = typeof(DataSignESD);
                    break;
                case eFiscalDevice.ALGOBOX_NET:
                    type = typeof(AlgoboxNetESD);
                    break;
                case eFiscalDevice.DISIGN:
                    type = typeof(DiSign);
                    break;
                default:
                    throw new POSException(POSClientResources.INVALID_FISCAL_DEVICE);
            }
            return this.Devices.Where(x => x.GetType() == type).OrderBy(x => x.FailureCount).ThenBy(x => x.Priority).ToList();
        }

        private void ReadDeviceSettings(Device device, XmlElement deviceSettings)
        {
            foreach (XmlNode property in deviceSettings.ChildNodes)
            {
                if (property.NodeType == XmlNodeType.Comment)
                    continue;
                switch (property.Name)
                {
                    case "LogicalDeviceName":
                        device.Settings.OPOS.LogicalDeviceName = property.InnerText;
                        break;
                    case "PortName":
                        device.Settings.COM.PortName = property.InnerText;
                        break;
                    case "BaudRate":
                        device.Settings.COM.BaudRate = int.Parse(property.InnerText);
                        break;
                    case "Parity":
                        device.Settings.COM.Parity = (Parity)Enum.Parse(typeof(Parity), property.InnerText);
                        break;
                    case "DataBits":
                        device.Settings.COM.DataBits = int.Parse(property.InnerText);
                        break;
                    case "StopBits":
                        device.Settings.COM.StopBits = (StopBits)Enum.Parse(typeof(StopBits), property.InnerText);
                        break;
                    case "Handshake":
                        device.Settings.COM.Handshake = (Handshake)Enum.Parse(typeof(Handshake), property.InnerText);
                        break;
                    case "WriteTimeOut":
                        device.Settings.COM.WriteTimeOut = int.Parse(property.InnerText);
                        break;
                    case "PrinterStation":
                        device.Settings.OPOS.PrinterSettings.PrinterStation = property.InnerText;
                        break;
                    case "LogoLocation":
                        device.Settings.OPOS.PrinterSettings.LogoLocation = property.InnerText;
                        break;
                    case "LogoText":
                        device.Settings.OPOS.PrinterSettings.LogoText = property.InnerText;
                        break;
                    case "IsPrimary":
                        device.Settings.IsPrimary = bool.Parse(property.InnerText);
                        break;
                    case "LineChars":
                        device.Settings.LineChars = int.Parse(property.InnerText);
                        break;
                    case "IPAddress":
                        device.Settings.Ethernet.IPAddress = property.InnerText;
                        break;
                    case "Port":
                        device.Settings.Ethernet.Port = int.Parse(property.InnerText);
                        break;
                    case "NumberOfLines":
                        device.Settings.NumberOfLines = int.Parse(property.InnerText);
                        break;
                    case "CharacterSet":
                        device.Settings.CharacterSet = int.Parse(property.InnerText);
                        break;
                    case "NewLine":
                        device.Settings.NewLine = String.IsNullOrEmpty(property.InnerText) ? device.Settings.NewLine : property.InnerText.Replace("\\r", "\r").Replace("\\n", "\n");
                        break;
                    case "OpenCommandString":
                        device.Settings.Indirect.OpenCommandString = property.InnerText;
                        break;
                    case "ParentDeviceName":
                        device.Settings.Indirect.ParentDeviceName = property.InnerText;
                        break;
                    case "KeyPosition0CommandString":
                        device.Settings.Indirect.KeyPosition0CommandString = property.InnerText;
                        break;
                    case "KeyPosition1CommandString":
                        device.Settings.Indirect.KeyPosition1CommandString = property.InnerText;
                        break;
                    case "KeyPosition2CommandString":
                        device.Settings.Indirect.KeyPosition2CommandString = property.InnerText;
                        break;
                    case "KeyPosition3CommandString":
                        device.Settings.Indirect.KeyPosition3CommandString = property.InnerText;
                        break;
                    case "KeyPosition4CommandString":
                        device.Settings.Indirect.KeyPosition4CommandString = property.InnerText;
                        break;
                    case "CommunicationType":
                        device.Settings.COM.ScaleSettings.CommunicationType = (ScaleCommunicationType)Enum.Parse(typeof(ScaleCommunicationType), property.InnerText);
                        break;
                    case "ScaleReadPattern":
                        device.Settings.COM.ScaleSettings.ScaleReadPattern = property.InnerText;
                        break;
                    case "Priority":
                        device.Priority = int.Parse(property.InnerText);
                        break;
                    case "ConvertCharset":
                        device.Settings.ConvertCharset = bool.Parse(property.InnerText);
                        break;
                    case "ConvertCharsetFrom":
                        device.Settings.ConvertCharsetFrom = int.Parse(property.InnerText);
                        break;
                    case "ConvertCharsetTo":
                        device.Settings.ConvertCharsetTo = int.Parse(property.InnerText);
                        break;
                }
            }
        }

        /// <summary>
        /// Returns true if demo mode is turned off or if the demo mode is on and the appropriate devices are emulators
        /// </summary>
        /// <param name="globalContextDemoMode"></param>
        /// <param name="fiscalmethod"></param>
        /// <param name="fiscaldevice"></param>
        /// <returns></returns>
        public bool HasDemoModeBeenSetupCorrectly(bool globalContextDemoMode, eFiscalMethod fiscalmethod, eFiscalDevice fiscaldevice)
        {
            if (!globalContextDemoMode)
            {
                return true;
            }

            if (fiscalmethod == eFiscalMethod.EAFDSS)
            {
                Device device = GetEAFDSSDevice(fiscaldevice);
                Printer printer = GetPrimaryDevice<Printer>();
                if (printer != null && device != null && printer.ConType == ConnectionType.EMULATED && device.ConType == ConnectionType.EMULATED)
                {
                    return true;
                }
            }

            return false;
        }

        private bool isDrawerOpen;

        /// <summary>
        /// Returns true if any drawer device is open.
        /// </summary>
        public bool IsDrawerOpen
        {
            get
            {
                return isDrawerOpen;
            }
        }


        /// <summary>
        /// Event handler for status changes of drawer devices
        /// </summary>
        /// <param name="status"></param>
        /// <param name="sender"></param>
        public void Drawer_OnStatusChange(DrawerStatus status, Drawer sender)
        {

            if (status == DrawerStatus.OPEN)
            {
                isDrawerOpen = true;
            }
            else if (status == DrawerStatus.CLOSED || status == DrawerStatus.UNKNOWN)
            {
                isDrawerOpen = false;
            }
        }

        public event OnKeyStatusChangeEventHandler OnKeyStatusChange;

        private eKeyStatus keyStatus;
        /// <summary>
        /// Returns the key status of the application.
        /// </summary>
        public eKeyStatus KeyStatus
        {
            get
            {
                return keyStatus;
            }
        }

        /// <summary>
        /// Event handler for status changes of keylock devices.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void KeyLock_OnStatusChange(object sender, StatusUpdateEventArgs e)
        {
            keyStatus = (eKeyStatus)e.Status;
            if (OnKeyStatusChange != null)
            {
                OnKeyStatusChange(keyStatus);
            }
        }

        /// <summary>
        /// Gets the customer's pole display.
        /// </summary>
        /// <returns></returns>
        public PoleDisplay GetCustomerPoleDisplay()
        {
            return this.GetPrimaryDevice<PoleDisplay>();
        }

        /// <summary>
        /// Gets the cashier's pole display.
        /// </summary>
        /// <returns></returns>
        public PoleDisplay GetCashierPoleDisplay()
        {
            var poleDisplays = this.Devices.Where(g => (g.GetType().Equals(typeof(PoleDisplay)) || g.GetType().IsSubclassOf(typeof(PoleDisplay))));
            if (poleDisplays.Count() <= 1)
            {
                return null;
            }

            return poleDisplays.FirstOrDefault(g => g.Settings.IsPrimary == false) as PoleDisplay;
        }

        /// <summary>
        /// For the given document detail, returns the appropriate text to display at the pole display.
        /// </summary>
        /// <param name="detail"></param>
        /// <param name="includeLineNumber"></param>
        /// <returns></returns>
        public string[] GetDocumentDetailPoleDisplayLines(DocumentDetail detail, bool includeLineNumber)
        {
            string firstLine = detail.CustomDescription == null ? "" : detail.CustomDescription.ToUpperGR();
            string secondLine = detail.Qty + " X " + String.Format("{0:C}", detail.FinalUnitPriceWithVat);
            if (includeLineNumber)
            {
                int lineVisibleNumber = detail.DocumentHeader.DocumentDetails.Where(x => x.IsCanceled == false && x.IsLinkedLine == false).ToList().IndexOf(detail) + 1;
                firstLine = lineVisibleNumber + "." + firstLine;
            }

            return new String[] { firstLine, secondLine };
        }

        /// <summary>
        /// For the given document payment, returns the appropriate text to display at the pole display
        /// </summary>
        /// <param name="header"></param>
        /// <param name="payment"></param>
        /// <param name="poleDisplay"></param>
        /// <param name="includeLineNumber"></param>
        /// <returns></returns>
        public string[] GetPaymentPoleDisplayLines(DocumentHeader header, DocumentPayment payment, PoleDisplay poleDisplay, bool includeLineNumber)
        {
            if (header == null)
            {
                throw new ArgumentNullException("header");
            }

            List<string> lines = new List<string>();
            if (poleDisplay != null)
            {
                decimal remainingAmount = header.GrossTotal - header.DocumentPayments.Sum(x => x.Amount);
                string firstLine = "";

                if (remainingAmount > 0)
                {
                    firstLine = POSClientResources.REMAINING_AMOUNT.ToUpperGR() + ": " + String.Format("{0:C}", remainingAmount);
                }
                else
                {
                    firstLine = POSClientResources.PAYOFF.ToUpperGR();
                }

                lines.Add(firstLine);
                if (payment != null)
                {
                    string description = String.IsNullOrWhiteSpace(payment.PaymentMethodDescription) ? "" : payment.PaymentMethodDescription.ToUpperGR();
                    if (includeLineNumber)
                    {
                        int visibleLineNumber = header.DocumentPayments.IndexOf(payment) + 1;
                        description = visibleLineNumber + "." + description;
                    }

                    string afterDescriptionText = ": " + String.Format("{0:C}", payment.Amount);
                    int maxLength = poleDisplay.Settings.LineChars <= 0 ? 20 : poleDisplay.Settings.LineChars;
                    if ((maxLength - afterDescriptionText.Length) > 0)
                    {
                        description = description.Limit(maxLength - afterDescriptionText.Length);
                    }
                    else
                    {
                        description = "";
                    }

                    string secondLine = description + afterDescriptionText;
                    lines.Add(secondLine);
                }
            }
            return lines.ToArray();
        }

        /// <summary>
        /// NOT YET FULLY IMPLEMENTED
        /// 
        /// Used for implementing a no-vga installation (poledisplay as a user display).
        /// Common handler for all the pole display input containers. Determines what to show at the cashier's pole display
        /// </summary>
        /// <param name="container"></param>
        /// <param name="previousInput"></param>
        /// <param name="currentInput"></param>
        /// <param name="poleDisplay"></param>
        public void HandleFocusedPoleDisplayInputChanged(IPoleDisplayInputContainer container, IPoleDisplayInput previousInput, IPoleDisplayInput currentInput, PoleDisplay poleDisplay)
        {
            if (poleDisplay != null && currentInput != null)
            {
                int lines = poleDisplay.Settings.NumberOfLines;
                int inputDataLine = lines - 1; //Last line, where the input data will be displayed
                int inputNameLine = lines - 2;
                int titleLine = 0;

                if (lines > 0 && inputNameLine <= lines)
                {
                    if (container.ShowTitle && inputDataLine != titleLine && inputNameLine != titleLine)
                    {
                        poleDisplay.ClearRow(titleLine);
                        poleDisplay.DisplayLine(container.Title, titleLine, 0, true);
                    }

                    if (container.ShowInputName && inputDataLine != inputNameLine)
                    {
                        poleDisplay.ClearRow(inputNameLine);
                        poleDisplay.DisplayLine(currentInput.PoleDisplayName, inputNameLine, 0, true);
                    }

                    if (previousInput is IPoleDisplayTextInput)
                    {
                        (previousInput as IPoleDisplayTextInput).DetachTextChangedEvent(PoleDisplayHelper_TextInputTextChanged(previousInput as IPoleDisplayTextInput, poleDisplay, inputDataLine));
                    }
                    else if (previousInput is IPoleDisplayLookupInput)
                    {
                        (previousInput as IPoleDisplayLookupInput).DetachOnValueChangedEvent(PoleDisplayHelper_LookupInputValueChanged(previousInput as IPoleDisplayLookupInput, poleDisplay, inputDataLine));
                    }

                    if (currentInput is IPoleDisplayTextInput)
                    {
                        (currentInput as IPoleDisplayTextInput).AttachTextChangedEvent(PoleDisplayHelper_TextInputTextChanged(currentInput as IPoleDisplayTextInput, poleDisplay, inputDataLine));
                    }
                    else if (currentInput is IPoleDisplayLookupInput)
                    {
                        (currentInput as IPoleDisplayLookupInput).AttachOnValueChangedEvent(PoleDisplayHelper_LookupInputValueChanged(currentInput as IPoleDisplayLookupInput, poleDisplay, inputDataLine));
                    }
                }
            }
        }

        private EventHandler PoleDisplayHelper_TextInputTextChanged(IPoleDisplayTextInput sender, PoleDisplay poleDisplay, int row)
        {
            return (object internalSender, EventArgs e) =>
            {
                poleDisplay.ClearRow(row);
                poleDisplay.DisplayLine(sender.GetText(), row, 0, true);
            };
        }

        private EventHandler PoleDisplayHelper_LookupInputValueChanged(IPoleDisplayLookupInput sender, PoleDisplay poleDisplay, int row)
        {
            return (object internalSender, EventArgs e) =>
            {
                poleDisplay.ClearRow(row);
                poleDisplay.DisplayLine(sender.GetText(), row, 0, true);
            };
        }

        /// <summary>
        /// Gets the application version that will displayed at the user. 
        /// If a fiscal printer is used the version contains two parts (FiscalVersion (2 digits) - Application Version (2 digits))
        /// </summary>
        /// <param name="fiscalMethod"></param>
        /// <returns></returns>
        public string GetVisibleVersion(eFiscalMethod fiscalMethod)
        {
            string visibleVersion = null;
            if (fiscalMethod == eFiscalMethod.ADHME)
            {
                FiscalPrinter printer = this.GetPrimaryDevice<FiscalPrinter>();
                if (printer != null)
                {
                    visibleVersion = printer.FiscalVersion.ToString(2);
                }
            }

            if (visibleVersion == null)
            {
                visibleVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(2);
            }
            else
            {
                visibleVersion += " - " + Assembly.GetExecutingAssembly().GetName().Version.ToString(2);
            }

            return visibleVersion;

        }



    }
}
