using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Hardware.Common;
using OposMSR_CCO;
using System.Reflection;
using System.Diagnostics;

namespace ITS.POS.Hardware
{
    public delegate void MsrReadEventHandler(object sender, MsrReadEventArgs e);
    public delegate void MsrErrorEventHandler(object sender, MsrErrorEventArgs e);

    /// <summary>
    ///  Represents a generic msr device. 
    /// </summary>
    public class MagneticStripReader : Device
    {
        /// <summary>
        /// Used for OPOS Connection
        /// </summary>
        private OPOSMSRClass oposMsr;

        public MagneticStripReader(ConnectionType conType, string deviceName)
            : base()
        {
            ConType = conType;
            DeviceName = deviceName;
        }

        public override eDeviceCheckResult CheckDevice(out string message)
        {
            //TODO
            message = "NOT CHECKED";
            return eDeviceCheckResult.INFO;
        }
        public override void AfterLoad(List<Device> devices)
        {
            base.AfterLoad(devices);
            this.StartListening();
        }

        /// <summary>
        /// Event to handle the read data. Used by Read Devices
        /// </summary>
        public event MsrReadEventHandler ReadEvent;

        /// <summary>
        /// Event to handle errors. Used by ReadDevices
        /// </summary>
        public event MsrErrorEventHandler ErrorEvent;

        /// <summary>
        /// Used by the child classes to trigger the ReadEvent.
        /// </summary>
        /// <param name="e"></param>
        protected void OnRead(MsrReadEventArgs e)
        {
            if (ReadEvent != null)
            {
                ReadEvent(this, e);
            }
        }

        /// <summary>
        /// Used by the child classes to trigger the ErrorEvent.
        /// </summary>
        /// <param name="e"></param>
        protected void OnReadError(MsrErrorEventArgs e)
        {
            if (ErrorEvent != null)
            {
                ErrorEvent(this, e);
            }
        }

        /// <summary>
        /// Opens the device and starts listening for reads. Use the ReadEvent event to handle the read data.
        /// </summary>
        /// <returns>SUCCESS,
        /// FAILURE,
        /// INVALIDPROPERTY -- A required Connection Setting is null, empty or invalid. For OPOS: LogicalDeviceName. ,
        /// UNAUTHORIZEDACCESS -- System denied access due to I/O Error or security error.</returns>
        public virtual DeviceResult StartListening()
        {
            if (ConType == ConnectionType.OPOS)
            {
                return startListeningToOPOS();
            }

            return DeviceResult.FAILURE;
        }

        /// <summary>
        /// Closes the device and stops listening for reads.
        /// </summary>
        /// <returns>SUCCESS,
        /// FAILURE</returns>
        public virtual DeviceResult StopListening()
        {
            if (ConType == ConnectionType.OPOS)
            {
                return stopListeningToOPOS();
            }

            return DeviceResult.FAILURE;
        }

        /// <summary>
        /// OPOS Connection:
        /// Triggers the OnRead event of the parent class.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void DataEventFromOPOS(int Status)
        {
            string track1Data = oposMsr.Track1Data;
            string track2Data = oposMsr.Track2Data;
            string track3Data = oposMsr.Track3Data;
            string track4Data = oposMsr.Track4Data;

            OnRead(new MsrReadEventArgs { Track1Data = track1Data, Track2Data = track2Data, Track3Data = track3Data, Track4Data = track4Data });
            if (!oposMsr.DataEventEnabled)
            {
                oposMsr.DataEventEnabled = true; //After each event it turns to false automaticaly. If false it stops triggering.
            }
        }

        /// <summary>
        /// OPOS Connection:
        /// Triggers the OnReadError event of the parent class.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void ErrorEventFromOPOS(int ResultCode, int ResultCodeExtended, int ErrorLocus, ref int pErrorResponse)
        {
            OnReadError(new MsrErrorEventArgs { Error = "ErrorCode: " + ResultCode + " ErrorCodeExtended:" + ResultCodeExtended + " ErrorRespoce:" + ErrorLocus/* + " TimeStamp:" + e.TimeStamp.ToString()*/ });
        }

        protected virtual DeviceResult startListeningToOPOS()
        {
            try
            {
                if (String.IsNullOrEmpty(Settings.OPOS.LogicalDeviceName))
                {
                    return DeviceResult.INVALIDPROPERTY;
                }

                if (oposMsr == null) oposMsr = new OPOSMSRClass();
                handleOposResult(oposMsr.Open(Settings.OPOS.LogicalDeviceName));

                handleOposResult(oposMsr.ClaimDevice(1000));
                oposMsr.DeviceEnabled = true;
                oposMsr.DecodeData = true;
                oposMsr.DataEventEnabled = true;


                oposMsr.DataEvent += DataEventFromOPOS;
                oposMsr.ErrorEvent += ErrorEventFromOPOS;

            }
            catch (Exception ex)
            {
                if (oposMsr != null && oposMsr.Claimed)
                {
                    oposMsr.Close();
                }
                return DeviceErrorConverter.ToDeviceResult(ex);
            }


            return DeviceResult.SUCCESS;
        }

        protected virtual DeviceResult stopListeningToOPOS()
        {
            if (oposMsr != null)
            {

                try
                {
                    handleOposResult(oposMsr.Close());
                    oposMsr = null;
                }
                catch (Exception ex)
                {
                    return DeviceErrorConverter.ToDeviceResult(ex);
                }
            }

            return DeviceResult.SUCCESS;
        }



    }
}
