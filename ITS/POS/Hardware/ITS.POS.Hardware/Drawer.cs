using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Hardware.Common;
using OposCashDrawer_CCO;

namespace ITS.POS.Hardware
{
    /// <summary>
    /// Represents a generic drawer device.
    /// </summary>
    public class Drawer : Device
    {

        private IDrawerPrinter _printer;
        public IDrawerPrinter Printer
        {
            get
            {
                return _printer;
            }
        }

        private OPOSCashDrawerClass OposDrawer;

        public Drawer(ConnectionType conType,string deviceName)
        {
            this.ConType = conType;
            this.DeviceName = deviceName;
        }

        public override void AfterLoad(List<Device> devices)
        {
            base.AfterLoad(devices);
            if (this.ConType == ConnectionType.INDIRECT)
            {
                this._printer = devices.Find(x => x.DeviceName == this.Settings.Indirect.ParentDeviceName) as IDrawerPrinter;
            }
        }
        public override eDeviceCheckResult CheckDevice(out string message)
        {
            message = "NOT CHECKED";
            return eDeviceCheckResult.INFO;
        }

        /// <summary>
        /// Opens the drawer
        /// </summary>
        /// <returns></returns>
        public DeviceResult OpenDrawer()
        {
            switch (this.ConType)
            {
                case ConnectionType.INDIRECT:
                    return openDrawerIndirect();
                case ConnectionType.OPOS:
                    return openDrawerOPOS();
            }

            return DeviceResult.CONNECTIONNOTSUPPORTED;
        }

        protected virtual DeviceResult openDrawerIndirect()
        {
            if (this.Printer != null)
            {
                return this.Printer.OpenDrawer(this.Settings.Indirect.OpenCommandString);
            }
            else
            {
                return DeviceResult.FAILURE;
            }
        }

        protected virtual DeviceResult openDrawerOPOS()
        {
            try
            {
                if (OposDrawer == null || OposDrawer.Claimed == false)
                {
                    OpenOposDrawerConnection(Settings.OPOS.LogicalDeviceName);
                }
                handleOposResult(OposDrawer.OpenDrawer());
                return DeviceResult.SUCCESS;

            }
            catch (Exception ex)
            {
                if (OposDrawer != null)
                {
                    CloseOposDrawerConnection(ref OposDrawer);
                }
                return DeviceErrorConverter.ToDeviceResult(ex);
            }
        }

        /// <summary>
        /// Returs the drawer's status
        /// </summary>
        /// <returns></returns>
        public DrawerStatus GetDrawerStatus()
        {
            switch (this.ConType)
            {
                case ConnectionType.INDIRECT:
                    return getDrawerStatusIndirect();
                case ConnectionType.OPOS:
                    return getDrawerStatusOPOS();
            }

            return DrawerStatus.UNKNOWN;
        }

        /// <summary>
        /// WARNING: Default implementation returns always UNKNOWN
        /// </summary>
        /// <returns></returns>
        protected virtual DrawerStatus getDrawerStatusIndirect()
        {
            return DrawerStatus.UNKNOWN;
        }

        protected virtual DrawerStatus getDrawerStatusOPOS()
        {
            try
            {
                if (OposDrawer == null || OposDrawer.Claimed == false)
                {
                    OpenOposDrawerConnection(Settings.OPOS.LogicalDeviceName);
                }
                if (OposDrawer.DrawerOpened)
                {
                    return DrawerStatus.OPEN;
                }
                else
                {
                    return DrawerStatus.CLOSED;
                }

            }
            catch (Exception)
            {
                if (OposDrawer != null)
                {
                    CloseOposDrawerConnection(ref OposDrawer);
                }
                return DrawerStatus.UNKNOWN;
            }
        }

        protected OPOSCashDrawerClass OpenOposDrawerConnection(string logicalDeviceName)
        {
            if(String.IsNullOrWhiteSpace(logicalDeviceName))
            {
                throw new Exception("Empty Logical Device Name");
            }
            OposDrawer = new OPOSCashDrawerClass();
            handleOposResult(OposDrawer.Open(logicalDeviceName));
            handleOposResult(OposDrawer.ClaimDevice(1000));
            OposDrawer.DeviceEnabled = true;
            OposDrawer.StatusUpdateEvent += oposDrawer_StatusUpdateEvent;

            return OposDrawer;
        }

        public delegate void OnStatucChangeEventHandler(DrawerStatus status,Drawer sender);

        /// <summary>
        /// Event handler that triggers when the drawer's status changes
        /// </summary>
        public event OnStatucChangeEventHandler OnStatusChange;

        protected virtual void oposDrawer_StatusUpdateEvent(int Data)
        {
            const int dataWhenClosed = 0;
            const int dataWhenOpen = 1;
            DrawerStatus reportedStatus = DrawerStatus.UNKNOWN;
            if(Data == dataWhenClosed)
            {
                reportedStatus = DrawerStatus.CLOSED;
            }
            else if(Data == dataWhenOpen)
            {
                reportedStatus = DrawerStatus.OPEN;
            }

            DrawerStatus actualStatus = getDrawerStatusOPOS();

            if(OnStatusChange != null && reportedStatus == actualStatus)
            {
                OnStatusChange(reportedStatus,this);
            }
        }

        protected void CloseOposDrawerConnection(ref OPOSCashDrawerClass drawer)
        {
            if (drawer != null && drawer.Claimed)
            {
                handleOposResult(drawer.Close());
                drawer = null;
            }
        }

    }
}
