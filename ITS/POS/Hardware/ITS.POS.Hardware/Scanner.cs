using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using ITS.Retail.Platform.Enumerations;
using OposScanner_CCO;
using ITS.POS.Hardware.Common;
using System.Threading.Tasks;

namespace ITS.POS.Hardware
{
    public delegate void ScannerReadEventHandler(Scanner sender, ScannerReadEventArgs e);
    public delegate void ScannerErrorEventHandler(Scanner sender, ScannerErrorEventArgs e);

    /// <summary>
    /// Represents a generic scanner device.
    /// </summary>
    public class Scanner : Device
    {
        /// <summary>
        /// Used for COM Connection
        /// </summary>
        private SerialPort ReadPort;                            

        /// <summary>
        /// Used for OPOS Connection
        /// </summary>
        private OPOSScannerClass oposScanner;

        private bool IsOnPause;

        public Scanner(ConnectionType conType,string deviceName)
            : base()
        {
            ConType = conType;
            DeviceName = deviceName;
            IsOnPause = false;
        }

        public override eDeviceCheckResult CheckDevice(out string message)
        {
            if ((ReadPort !=null && ReadPort.IsOpen) || (oposScanner != null && oposScanner.Claimed))
            {
                message = "";
                return eDeviceCheckResult.SUCCESS;
            }
            message = "Device Not Found or is not ready";
            return eDeviceCheckResult.WARNING;
        }

        /// <summary>
        /// Event to handle the read data.
        /// </summary>
        public event ScannerReadEventHandler ReadEvent;


        /// <summary>
        /// Event to handle errors.
        /// </summary>
        public event ScannerErrorEventHandler ErrorEvent;


        /// <summary>
        /// Triggers the ReadEvent.
        /// </summary>
        /// <param name="e"></param>
        protected void OnRead(ScannerReadEventArgs e)
        {
            if (ReadEvent != null)
            {
                ReadEvent(this, e);
            }
        }

        /// <summary>
        /// Triggers the ErrorEvent.
        /// </summary>
        /// <param name="e"></param>
        protected void OnReadError(ScannerErrorEventArgs e)
        {
            if (ErrorEvent != null)
            {
                ErrorEvent(this, e);
            }
        }

        /// <summary>
        /// Opens the device and starts listening for scans. Use the ReadEvent event to handle the scanned data.
        /// </summary>
        /// <returns>SUCCESS,
        /// FAILURE,
        /// INVALIDPROPERTY -- A required Connection Setting is null, empty or invalid. For COM: PortName, BaudRate, Parity, DataBits, StopBits. For OPOS: LogicalDeviceName. ,
        /// UNAUTHORIZEDACCESS -- System denied access due to I/O Error or security error.</returns>
        public virtual DeviceResult StartListening()
        {
            if (ConType == ConnectionType.COM)
            {
                return startListeningToCOM();
            }
            else if (ConType == ConnectionType.OPOS)
            {
                return startListeningToOPOS();
            }

            return DeviceResult.FAILURE;
        }

        /// <summary>
        /// Closes the device and stops listening for scans.
        /// </summary>
        /// <returns>SUCCESS,
        /// FAILURE</returns>
        public virtual DeviceResult StopListening()
        {
            if (ConType == ConnectionType.COM)
            {
                return stopListeningToCOM();
            }
            else if (ConType == ConnectionType.OPOS)
            {
                return stopListeningToOPOS();
            }

            return DeviceResult.FAILURE;
        }

        /// <summary>
        /// COM Connection:
        /// Triggers the OnRead event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void DataEventFromCOM(object sender, SerialDataReceivedEventArgs e)
        {
            string data;
            try
            {
                data = ReadPort.ReadLine();
                OnRead(new ScannerReadEventArgs(data, BarCodeSymbology.Unknown));
            }
            catch(TimeoutException ex)
            {
                data = ReadPort.ReadExisting();
                ReadPort.DiscardInBuffer();
                throw new Exception("Scanner Read timeout. Please check the device's NewLine setting.\n Expected NewLine: \""+ReadPort.NewLine.Replace("\r","\\r").Replace("\n","\\n")
                                    +"\"\nData Read: \""+(data != null ? data.Replace("\r","\\r").Replace("\n","\\n") : "")+"\"" , ex);
            }
            
        }

        /// <summary>
        /// COM Connection:
        /// Triggers the OnError event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void ErrorEventFromCOM(object sender, SerialErrorReceivedEventArgs e)
        {
            OnReadError(new ScannerErrorEventArgs { Error = "EventType:" + e.EventType.ToString() + " " + e.ToString() });
        }


        /// <summary>
        /// OPOS Connection:
        /// Triggers the OnRead event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void DataEventFromOPOS(int Status)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();

            string data = oposScanner.ScanDataLabel;
            BarCodeSymbology bcSymbology = (BarCodeSymbology)oposScanner.ScanDataType;

            OnRead(new ScannerReadEventArgs(data,bcSymbology));
            if (!oposScanner.DataEventEnabled)
            {
                oposScanner.DataEventEnabled = true; //After each event it turns to false automaticaly. If false it stops triggering.
            }
        }

        /// <summary>
        /// OPOS Connection:
        /// Triggers the OnReadError event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void ErrorEventFromOPOS(int ResultCode, int ResultCodeExtended, int ErrorLocus, ref int pErrorResponse)
        {
            OnReadError(new ScannerErrorEventArgs { Error = "ErrorCode: " + ResultCode + " ErrorCodeExtended:" + ResultCodeExtended + " ErrorRespoce:" + ErrorLocus/* + " TimeStamp:" + e.TimeStamp.ToString()*/ });
        }


        protected virtual DeviceResult startListeningToCOM()
        {
            if (String.IsNullOrEmpty(Settings.COM.PortName) || Settings.COM.BaudRate <= 0 || Settings.COM.DataBits <= 0 )
            {
                return DeviceResult.INVALIDPROPERTY;
            }
            ReadPort = new SerialPort(Settings.COM.PortName, Settings.COM.BaudRate, Settings.COM.Parity, Settings.COM.DataBits, Settings.COM.StopBits);
            ReadPort.Handshake = Settings.COM.Handshake;
            ReadPort.DataReceived += DataEventFromCOM;
            ReadPort.ErrorReceived += ErrorEventFromCOM;
            ReadPort.WriteTimeout = Settings.COM.WriteTimeOut;
            ReadPort.ReadTimeout = 5000;
            if (Settings.NewLine != null)
            {
                ReadPort.NewLine = Settings.NewLine;
            }

            try
            {
                ReadPort.Open();
            }
            catch (Exception ex)
            {
                return DeviceErrorConverter.ToDeviceResult(ex);
            }

            return DeviceResult.SUCCESS;
        }

        protected virtual DeviceResult stopListeningToCOM()
        {
            if (ReadPort != null)
            {
                try
                {
                    if (ReadPort.IsOpen)
                    {
                        ReadPort.Close();
                        ReadPort = null;
                    }
                }
                catch (Exception ex)
                {
                    return DeviceErrorConverter.ToDeviceResult(ex);
                }
            }
            return DeviceResult.SUCCESS;
        }


        protected virtual DeviceResult startListeningToOPOS()
        {
            try
            {
                if (String.IsNullOrEmpty(Settings.OPOS.LogicalDeviceName))
                {
                    return DeviceResult.INVALIDPROPERTY;
                }

                if(oposScanner==null) oposScanner = new OPOSScannerClass();
                handleOposResult(oposScanner.Open(Settings.OPOS.LogicalDeviceName));
                
                handleOposResult(oposScanner.ClaimDevice(1000));
                oposScanner.DeviceEnabled = true;
                oposScanner.DecodeData = true;
                oposScanner.DataEventEnabled = true;
                

                oposScanner.DataEvent += DataEventFromOPOS;
                oposScanner.ErrorEvent += ErrorEventFromOPOS;

            }
            catch (Exception ex)
            {
                if (oposScanner != null && oposScanner.Claimed)
                {
                    oposScanner.Close();
                }
                return DeviceErrorConverter.ToDeviceResult(ex);
            }


            return DeviceResult.SUCCESS;
        }

        protected virtual DeviceResult stopListeningToOPOS()
        {
            if (oposScanner != null)
            {

                try
                {
                    handleOposResult(oposScanner.Close());
                    oposScanner = null;
                }
                catch (Exception ex)
                {
                    return DeviceErrorConverter.ToDeviceResult(ex);
                }
            }

            return DeviceResult.SUCCESS;
        }

        public virtual void Pause()
        {
            this.IsOnPause = true;
        }

        public virtual void Resume()
        {
            DiscardSerialPortData();
            this.IsOnPause = false;
        }

        public virtual bool Paused
        {
            get
            {
                return this.IsOnPause;
            }
        }

        public virtual void DiscardSerialPortData()
        {
            try
            {
                string data = ReadPort.ReadExisting();
                System.Threading.Thread.Sleep(100);
                ReadPort.DiscardInBuffer();
                System.Threading.Thread.Sleep(100);
                ReadPort.DiscardOutBuffer();
                System.Threading.Thread.Sleep(100);
            }
            catch (Exception exception)
            {

            }
        }
    }
}
