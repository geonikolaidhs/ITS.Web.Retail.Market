using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using ITS.POS.Hardware.Common;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Resources;
using ITS.POS.Client.Exceptions;
using System.IO;
using ITS.POS.Hardware.ScaleDialogProtocol;
using System.Diagnostics;
using OposScale_CCO;

namespace ITS.POS.Hardware
{
    /// <summary>
    ///  Represents a generic scale device.
    /// </summary>
    public class Scale : Device
    {
        /// <summary>
        /// Used for OPOS Connection
        /// </summary>
        private OPOSScaleClass oposScale;

        /// <summary>
        /// The serial port object.
        /// </summary>
        private SerialPort readPort;

        /// <summary>
        /// The regex object created from the provided string.
        /// </summary>
        private Regex scalePattern;

        /// <summary>
        /// Initializes a new instance of the Scale class. Currently Supported Connection Types: COM
        /// </summary>
        /// <param name="conType">The connection Type of the device</param>
        /// <param name="deviceName">The name of the device</param>
        public Scale(ConnectionType conType, string deviceName)
            : base()
        {
            this.ConType = conType;
            this.DeviceName = deviceName;
        }

        public override eDeviceCheckResult CheckDevice(out string message)
        {
            try
            {
                if (ConType == ConnectionType.COM)
                {
                    switch (this.Settings.COM.ScaleSettings.CommunicationType)
                    {
                        case ScaleCommunicationType.CONTINUOUS:
                            this.ReadWeight();
                            break;
                        case ScaleCommunicationType.DIALOG:
                            this.TryCommunicate();
                            break;
                    }
                }
                else if (ConType == ConnectionType.OPOS)
                {
                    this.CheckOposScale();
                }
                message = string.Empty;
                return eDeviceCheckResult.SUCCESS;

            }
            catch (Exception ex)
            {
                message = ex.Message;
                return eDeviceCheckResult.WARNING;
            }
        }
        /// <summary>
        /// Gets the current weight of the scale.
        /// </summary>
        /// <returns>The current weight on the scale</returns>
        public decimal ReadWeight(decimal unitprice = 0.01m, decimal tare = 0, string text = "")
        {

            if (this.ConType == ConnectionType.COM)
            {
                this.ValidateCOMSettings();
                if (Settings.COM.ScaleSettings.CommunicationType == ScaleCommunicationType.CONTINUOUS)
                {
                    return this.ReadWeightContinuousFromCOM();
                }
                else
                {
                    return this.ReadWeightDialog06FromCOM((int)Math.Round(unitprice * 100), (int)Math.Round(tare * 1000), text);
                }
            }
            else if (this.ConType == ConnectionType.OPOS)
            {
                try
                {
                    if (oposScale == null)
                    {
                        oposScale = new OPOSScaleClass();
                    }
                    oposScale.Open(this.Settings.OPOS.LogicalDeviceName);
                    if (!oposScale.Claimed)
                    {
                        OPOSErrorCode clainmResult = (OPOSErrorCode)oposScale.ClaimDevice(2000);
                        if (clainmResult != OPOSErrorCode.Success)
                        {
                            throw new Exception(Resources.POSClientResources.SCALE_COMMUNICATION_ERROR + " - " + clainmResult.ToString());
                        }
                    }
                    if (oposScale.Claimed)
                    {
                        oposScale.DeviceEnabled = true;
                        int grams = 0;
                        OPOSErrorCode result = (OPOSErrorCode)oposScale.ReadWeight(out grams, 2000);
                        if (result == OPOSErrorCode.Success)
                        {
                            return (decimal)(grams / 1000m);
                        }
                        else if (result == OPOSErrorCode.Timeout)
                        {
                            throw new Exception(Resources.POSClientResources.TIMEOUT);
                        }
                        else
                        {
                            throw new Exception(Resources.POSClientResources.GENERAL_SCALE_ERROR + " - " + result.ToString());
                        }
                    }
                    else
                    {
                        throw new Exception(Resources.POSClientResources.SCALE_COMMUNICATION_ERROR + " - Device is not Claimed");
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    ReleaseOposScale();
                }
            }
            else
            {
                throw new NotSupportedException("Scale connection type \"" + ConType + "\" not supported.");
            }
        }


        private void CheckOposScale()
        {
            if (oposScale == null)
            {
                oposScale = new OPOSScaleClass();
            }
            ReleaseOposScale();
            OPOSErrorCode openResult = (OPOSErrorCode)oposScale.Open(this.Settings.OPOS.LogicalDeviceName);
            if (openResult != OPOSErrorCode.Success)
            {
                throw new Exception(Resources.POSClientResources.SCALE_COMMUNICATION_ERROR + " - " + openResult.ToString());
            }
            if (!oposScale.Claimed)
            {
                OPOSErrorCode clainmResult = (OPOSErrorCode)oposScale.ClaimDevice(2000);
                if (clainmResult != OPOSErrorCode.Success)
                {
                    throw new Exception(Resources.POSClientResources.SCALE_COMMUNICATION_ERROR + " - " + clainmResult.ToString());
                }
            }
            if (!oposScale.Claimed)
            {
                throw new Exception(Resources.POSClientResources.SCALE_COMMUNICATION_ERROR + " - Device is not Claimed");
            }
        }


        private void ReleaseOposScale()
        {
            try
            {
                if (oposScale != null)
                {
                    oposScale.ReleaseDevice();
                }
            }
            catch (Exception ex)
            {
                this.LogError("ReleaseOposScale, " + ex.Message);
            }
        }

        /// <summary>
        /// Validates that the COM settings provided are correct. If not, an exception will be thrown.
        /// </summary>
        protected virtual void ValidateCOMSettings()
        {
            if (String.IsNullOrEmpty(this.Settings.COM.PortName) || this.Settings.COM.BaudRate <= 0 || this.Settings.COM.DataBits <= 0)
            {
                throw new ArgumentException("Invalid COM Settings");
            }
            if (this.Settings.COM.ScaleSettings.CommunicationType == ScaleCommunicationType.CONTINUOUS)
            {
                if (String.IsNullOrWhiteSpace(this.Settings.COM.ScaleSettings.ScaleReadPattern))
                {
                    throw new ArgumentException("Invalid Scale Read Pattern");
                }
                try
                {
                    this.scalePattern = new Regex(this.Settings.COM.ScaleSettings.ScaleReadPattern);
                }
                catch (ArgumentException ex)
                {
                    throw new ArgumentException("Invalid Scale Read Pattern", ex);
                }
            }
        }

        /// <summary>
        /// Gets the weight of the scale by com, in continuous mode.
        /// </summary>
        /// <returns>The current weight on the scale</returns>
        protected virtual decimal ReadWeightContinuousFromCOM()
        {
            CheckPort();
            bool correct = false;
            int counter = 0;
            decimal value;
            int trycounter = 0;
            do
            {
                try
                {
                    if (this.readPort.IsOpen)
                    {
                        this.readPort.Close();
                    }
                    this.readPort.Open();
                    do
                    {
                        if (counter >= 3)
                        {
                            throw new Exception(POSClientResources.WEIGH_ON_THE_SCALE_IS_NOT_STABLE);
                        }
                        counter++;
                        Thread.Sleep(300);
                        byte[] buffer = new byte[4096];
                        this.readPort.Read(buffer, 0, (int)buffer.Length);
                        string weight = System.Text.UnicodeEncoding.ASCII.GetString(buffer);
                        value = this.ParseWeight(weight, out correct);
                    }
                    while (correct == false);
                    this.readPort.Close();
                    return value;
                }
                catch (TimeoutException tx)
                {
                    if (trycounter > 3)
                    {
                        if (this.readPort != null && this.readPort.IsOpen)
                        {
                            this.readPort.Close();
                        }
                        throw new POSUserVisibleException("Communication error", tx);
                    }
                    trycounter++;
                }
                catch (Exception ex)
                {
                    if (trycounter > 3)
                    {
                        if (this.readPort != null && this.readPort.IsOpen)
                        {
                            this.readPort.Close();
                        }
                        throw new POSUserVisibleException(POSClientResources.WEIGH_ON_THE_SCALE_IS_NOT_STABLE, ex);
                    }
                    trycounter++;
                }
            }
            while (true);
        }

        private void CheckPort()
        {
            if (this.readPort == null)
            {
                this.readPort = new SerialPort(this.Settings.COM.PortName, this.Settings.COM.BaudRate, this.Settings.COM.Parity, this.Settings.COM.DataBits, this.Settings.COM.StopBits);
                this.readPort.ReadTimeout = 300;
            }
        }

        /// <summary>
        /// Converts a string output that was read from the scale, into weight.
        /// </summary>
        /// <param name="weight">The output of the scale</param>
        /// <param name="correct">If parsing failed this will be false</param>
        /// <returns>The parsed weight as decimal</returns>
        protected virtual decimal ParseWeight(string weight, out bool correct)
        {
            correct = false;
            var matches = this.scalePattern.Matches(weight);
            if (matches.Count == 0)
            {
                return 0m;
            }
            decimal returnValue;
            var lines = matches.Cast<Match>().Where(m => m.Groups.Count > 1).Select(m => m.Groups[1].Value);
            if (lines.Count() != matches.Count)
            {
                return 0m;
            }
            string firstWeight = lines.First();

            if (lines.Where(x => x != firstWeight).Count() > 0 || decimal.TryParse(firstWeight, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out returnValue) == false)
            {
                return 0m;
            }
            correct = true;
            return returnValue;
        }

        protected virtual decimal ReadWeightDialog06FromCOM(int unitprice, int tare, string text)
        {
            CheckPort();
            if (this.readPort.IsOpen == false)
            {
                this.readPort.Open();
            }
            IScaleRecord receivedRecord = new ScaleRecordNAK(new byte[] { DialogHelper.NAK }), recordToSend = new ScaleRecordDataRequest();

            //Initializing port

            this.readPort.Write(recordToSend.Command, 0, recordToSend.Command.Length);

            long startingTicks = DateTime.Now.Ticks, maxTicks = TimeSpan.TicksPerSecond * 10;

#if DEBUG
            if (Debugger.IsAttached)
            {
                maxTicks += TimeSpan.TicksPerMinute * 5;
            }
#endif
            RequestParameters parameters = new RequestParameters() { Value = unitprice, Tare = tare, Text = text, NAKCounter = 0 };
            do
            {
                List<byte> response = ReadResponse();
                receivedRecord = DialogHelper.GetScaleRecord(response);

                this.LogDebug("Received " + receivedRecord.GetType().Name);
                parameters.NAKCounter = HandleNAKResponse(receivedRecord, parameters.NAKCounter);
                if (receivedRecord is ScaleRecord02)
                {
                    break;
                }
                recordToSend = receivedRecord.GetAppropriateRequest(parameters);
                this.LogDebug("Sending " + recordToSend.GetType().Name);
                this.readPort.Write(recordToSend.Command, 0, recordToSend.Command.Length);
            } while (receivedRecord is ScaleRecord02 == false && DateTime.Now.Ticks - startingTicks < maxTicks);

            if (receivedRecord is ScaleRecord02)
            {
                ScaleRecord02 recRec02 = (ScaleRecord02)receivedRecord;
                int readValue = int.Parse(recRec02.WeightString);
                if (readValue <= 0)
                {
                    throw new POSUserVisibleException(POSClientResources.WEIGH_ON_THE_SCALE_IS_NOT_STABLE);
                }
                return readValue / 1000m;
            }

            throw new POSUserVisibleException(POSClientResources.ERROR);
        }

        private static int HandleNAKResponse(IScaleRecord receivedRecord, int nakCounter)
        {
            if (receivedRecord is ScaleRecordNAK)
            {
                Thread.Sleep(100);
                return nakCounter + 1;
            }
            return 0;


        }

        private static List<byte> acceptableEndingBytes = new List<byte>() { DialogHelper.ETX, DialogHelper.ACK, DialogHelper.NAK };
        private List<byte> ReadResponse()
        {
            List<byte> response = new List<byte>(8);
            int internalCount = 0;
            while (response.Count == 0 || acceptableEndingBytes.Contains(response.Last()) == false)
            {
                while (readPort.BytesToRead > 0)
                {
                    response.Add((byte)readPort.ReadByte());
                }
                Thread.Sleep(5);
                internalCount++;
                if (internalCount > 200)
                {
                    throw new POSUserVisibleException(POSClientResources.GENERAL_SCALE_ERROR + ":" + POSClientResources.TIMEOUT);
                }
            }
            return response;
        }


        private bool TryCommunicate()
        {
            CheckPort();
            if (this.readPort.IsOpen == false)
            {
                this.readPort.Open();
            }
            ScaleRecordDataRequest request = new ScaleRecordDataRequest();
            this.readPort.Write(request.Command, 0, request.Command.Length);
            List<byte> response = ReadResponse();
            return true;
        }
    }
}
