using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Exceptions;
using ITS.POS.Resources;
using ITS.POS.Hardware.Common;
using OposKeylock_CCO;

namespace ITS.POS.Hardware
{

    public delegate void StatusUpdateHandler(object sender, StatusUpdateEventArgs e);

    /// <summary>
    ///  Represents a generic key lock device. 
    /// </summary>
    public class KeyLock : Device
    {
        private OPOSKeylockClass OposKeyLock;
        private bool isIndirectListening;
        private Queue<Keys> inputBuffer;
        private int minBufferSize;
        private int maxBufferSize;
        public eKeyStatus KeyStatus { get; private set; }

        public KeyLock(ConnectionType conType, string deviceName)
            : base()
        {
            ConType = conType;
            DeviceName = deviceName;
            isIndirectListening = false;
            if (ConType == ConnectionType.INDIRECT)
            {
                inputBuffer = new Queue<Keys>();
            }
        }

        public override eDeviceCheckResult CheckDevice(out string message)
        {
            message = "NOT CHECKED";
            return eDeviceCheckResult.INFO;
        }

        public override void AfterLoad(List<Device> devices)
        {
            base.AfterLoad(devices);
            if (ConType == ConnectionType.INDIRECT)
            {
                getBufferSize(out minBufferSize, out maxBufferSize);
            }

            DeviceResult result = this.StartListening();
            if (result != DeviceResult.SUCCESS)
            {
                throw new POSDeviceResultException(result, this.DeviceName);
            }
        }

        /// <summary>
        /// Common event trigger for all connection types
        /// </summary>
        /// <param name="e"></param>
        protected void OnStatusUpdate(StatusUpdateEventArgs e)
        {
            this.KeyStatus = (eKeyStatus)e.Status;
            if (StatusUpdateEvent != null)
            {
                StatusUpdateEvent(this, e);
            }
        }

        /// <summary>
        /// Event to handle the status update data.
        /// </summary>
        public event StatusUpdateHandler StatusUpdateEvent;

        /// <summary>
        /// Opens the device and starts listening for status updates. Use the StatusUpdate event to handle the data.
        /// </summary>
        /// <returns>SUCCESS,
        /// FAILURE,
        /// INVALIDPROPERTY -- A required Connection Setting is null, empty or invalid. For COM: PortName, BaudRate, Parity, DataBits, StopBits. For OPOS: LogicalDeviceName. ,
        /// UNAUTHORIZEDACCESS -- System denied access due to I/O Error or security error.</returns>
        public DeviceResult StartListening()
        {
            if (ConType == ConnectionType.OPOS)
            {
                return startListeningOPOS();
            }
            else if (ConType == ConnectionType.INDIRECT)
            {
                return startListeningIndirect();
            }

            return DeviceResult.CONNECTIONNOTSUPPORTED;
        }

        /// <summary>
        /// Closes the device and stops listening status updates.
        /// </summary>
        /// <returns>SUCCESS,
        /// FAILURE</returns>
        public DeviceResult StopListening()
        {
            if (ConType == ConnectionType.OPOS)
            {
                return stopListeningOPOS();
            }
            else if (ConType == ConnectionType.INDIRECT)
            {
                return stopListeningIndirect();
            }

            return DeviceResult.CONNECTIONNOTSUPPORTED;
        }

        /// <summary>
        /// Returns true if the given KeyData is part of the keylock command chaing and must be supressed.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool IndirectConnectionHandleKey(KeyEventArgs e)
        {
            if (isIndirectListening)
            {
                if (inputBuffer.Count < maxBufferSize) //add character
                {
                    inputBuffer.Enqueue(e.KeyData);
                }
                else if (inputBuffer.Count == maxBufferSize) //remove and add
                {
                    inputBuffer.Dequeue();
                    inputBuffer.Enqueue(e.KeyData);
                }

                bool mustPreventKeyPress = isKeyPartOfTheCommandChain(inputBuffer.ToArray(), e.KeyData);

                if (inputBuffer.Count >= minBufferSize && inputBuffer.Count <= maxBufferSize) //buffer is in valid scope, check for command
                {
                    int result = checkForCommandString(inputBuffer.ToArray());
                    if (result != -1)
                    {
                        StatusUpdateFromIndirect(result);
                        if (minBufferSize == maxBufferSize || inputBuffer.Count == maxBufferSize)
                        {
                            for (int i = 0; i < maxBufferSize; i++)
                            {
                                inputBuffer.Dequeue();
                            }
                        }
                        return true;
                    }
                }

                if (mustPreventKeyPress)
                {
                    return true;
                }

            }
            return false;
        }


        private Keys[] ParceCommandString(string commandString)
        {
            string[] fields = commandString.Split(',');
            Keys[] keys = new Keys[fields.Count()];
            for (int i = 0; i < fields.Count(); i++)
            {
                int key;
                if (int.TryParse(fields[i], out key))
                {
                    keys[i] = (Keys)key;
                }
            }
            return keys;
        }

        private bool isKeyPartOfTheCommandChain(Keys[] buffer, Keys currentKey)
        {
            if (buffer.Length > 1)
            {
                Keys previousKey = buffer[buffer.Length - 2];
                Keys? previousOfPreviousKey = null;
                if (buffer.Length > 2 && minBufferSize > 2)
                {
                    previousOfPreviousKey = buffer[buffer.Length - 3];
                }

                List<Keys[]> keyPositionCommands = new List<Keys[]>(5);
                keyPositionCommands.Add(ParceCommandString(this.Settings.Indirect.KeyPosition0CommandString));
                keyPositionCommands.Add(ParceCommandString(this.Settings.Indirect.KeyPosition1CommandString));
                keyPositionCommands.Add(ParceCommandString(this.Settings.Indirect.KeyPosition2CommandString));
                keyPositionCommands.Add(ParceCommandString(this.Settings.Indirect.KeyPosition3CommandString));
                keyPositionCommands.Add(ParceCommandString(this.Settings.Indirect.KeyPosition4CommandString));


                for (int i = 0; i < 5; i++)
                {
                    int currentKeyIndexAtCommand = keyPositionCommands[i].ToList().IndexOf(currentKey);
                    int previousKeyIndexAtCommand = keyPositionCommands[i].ToList().IndexOf(previousKey);
                    int previousOfPreviousKeyIndexAtCommand = previousOfPreviousKey.HasValue ? keyPositionCommands[i].ToList().IndexOf(previousOfPreviousKey.Value) : -1;
                    if (buffer.Length > 2 && minBufferSize > 2 && ////check 3 last keys in a row
                        currentKeyIndexAtCommand != -1 &&
                        previousKeyIndexAtCommand != -1 &&
                        previousOfPreviousKeyIndexAtCommand != -1 &&
                        previousKeyIndexAtCommand == (currentKeyIndexAtCommand - 1) &&
                        previousOfPreviousKeyIndexAtCommand == (currentKeyIndexAtCommand - 2))
                    {
                        return true;
                    }
                    else if (previousOfPreviousKey == null && ////else check 2 keys in a row
                        currentKeyIndexAtCommand != -1 &&
                        previousKeyIndexAtCommand != -1 &&
                        previousKeyIndexAtCommand == (currentKeyIndexAtCommand - 1))
                    {
                        return true;
                    }

                }
            }

            return false;
        }

        private int checkForCommandString(Keys[] buffer)
        {
            if (!String.IsNullOrWhiteSpace(this.Settings.Indirect.KeyPosition0CommandString) && Enumerable.SequenceEqual(buffer, ParceCommandString(this.Settings.Indirect.KeyPosition0CommandString)))
            {
                return 0;
            }
            else if (!String.IsNullOrWhiteSpace(this.Settings.Indirect.KeyPosition1CommandString) && Enumerable.SequenceEqual(buffer, ParceCommandString(this.Settings.Indirect.KeyPosition1CommandString)))
            {
                return 1;
            }
            else if (!String.IsNullOrWhiteSpace(this.Settings.Indirect.KeyPosition2CommandString) && Enumerable.SequenceEqual(buffer, ParceCommandString(this.Settings.Indirect.KeyPosition2CommandString)))
            {
                return 2;
            }
            else if (!String.IsNullOrWhiteSpace(this.Settings.Indirect.KeyPosition3CommandString) && Enumerable.SequenceEqual(buffer, ParceCommandString(this.Settings.Indirect.KeyPosition3CommandString)))
            {
                return 3;
            }
            else if (!String.IsNullOrWhiteSpace(this.Settings.Indirect.KeyPosition4CommandString) && Enumerable.SequenceEqual(buffer, ParceCommandString(this.Settings.Indirect.KeyPosition4CommandString)))
            {
                return 4;
            }

            return -1;
        }

        private DeviceResult startListeningIndirect()
        {
            isIndirectListening = true;
            return DeviceResult.SUCCESS;
        }

        private void getBufferSize(out int min, out int max)
        {
            List<int> lengths = new List<int>();
            if (!String.IsNullOrWhiteSpace(this.Settings.Indirect.KeyPosition0CommandString))
            {
                lengths.Add(this.Settings.Indirect.KeyPosition0CommandString.Split(',').Count());
            }
            if (!String.IsNullOrWhiteSpace(this.Settings.Indirect.KeyPosition1CommandString))
            {
                lengths.Add(this.Settings.Indirect.KeyPosition1CommandString.Split(',').Count());
            }
            if (!String.IsNullOrWhiteSpace(this.Settings.Indirect.KeyPosition2CommandString))
            {
                lengths.Add(this.Settings.Indirect.KeyPosition2CommandString.Split(',').Count());
            }
            if (!String.IsNullOrWhiteSpace(this.Settings.Indirect.KeyPosition3CommandString))
            {
                lengths.Add(this.Settings.Indirect.KeyPosition3CommandString.Split(',').Count());
            }
            if (!String.IsNullOrWhiteSpace(this.Settings.Indirect.KeyPosition4CommandString))
            {
                lengths.Add(this.Settings.Indirect.KeyPosition4CommandString.Split(',').Count());
            }
            lengths.Sort();
            min = lengths.First();
            max = lengths.Last();
        }

        private void StatusUpdateFromIndirect(int Data)
        {
            OnStatusUpdate(new StatusUpdateEventArgs { Status = Data });
        }

        private DeviceResult stopListeningIndirect()
        {
            isIndirectListening = false;
            inputBuffer = new Queue<Keys>();
            return DeviceResult.SUCCESS;
        }

        protected virtual DeviceResult startListeningOPOS()
        {
            try
            {
                if (String.IsNullOrEmpty(Settings.OPOS.LogicalDeviceName))
                {
                    return DeviceResult.INVALIDPROPERTY;
                }

                if (OposKeyLock == null) OposKeyLock = new OPOSKeylockClass();
                handleOposResult(OposKeyLock.Open(Settings.OPOS.LogicalDeviceName));
                //OposKeyLock.ClaimDevice(1000); Cannot claim a Key lock

                OposKeyLock.DeviceEnabled = true;
                OposKeyLock.StatusUpdateEvent += StatusUpdateFromOPOS;

            }
            catch (Exception ex)
            {
                if (OposKeyLock != null && OposKeyLock.Claimed)
                {
                    OposKeyLock.Close();
                }
                return DeviceErrorConverter.ToDeviceResult(ex);
            }


            return DeviceResult.SUCCESS;
        }

        /// <summary>
        /// This function will be called when the StatusUpdateEvent is triggered from the device, and it will trigger in return this class's StatusUpdate event
        /// </summary>
        /// <param name="Data"></param>
        private void StatusUpdateFromOPOS(int Data)
        {
            OnStatusUpdate(new StatusUpdateEventArgs { Status = Data });
        }

        protected virtual DeviceResult stopListeningOPOS()
        {
            if (OposKeyLock != null)
            {

                try
                {
                    OposKeyLock.Close();
                    OposKeyLock = null;
                }
                catch (Exception ex)
                {
                    return DeviceErrorConverter.ToDeviceResult(ex);//DeviceResult.FAILURE;
                }
            }

            return DeviceResult.SUCCESS;
        }
    }
}
