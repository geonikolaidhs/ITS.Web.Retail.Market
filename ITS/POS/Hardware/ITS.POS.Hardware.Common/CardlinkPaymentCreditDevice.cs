using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Hardware.Common;
using ITS.POS.Client.Exceptions;
using ITS.POS.Model.Transactions;

namespace ITS.POS.Hardware.Common
{
    public class CardlinkPaymentCreditDevice : Device
    {
        public CardlinkPaymentCreditDevice(ConnectionType conType, string deviceName)
            : base()
        {
            this.ConType = conType;
            this.DeviceName = deviceName;
        }

        public CardlinkPaymentCreditDevice()

        {
        }

        public bool PortInitialized { get; private set; }
        public override eDeviceCheckResult CheckDevice(out string message)
        {
            PortInitialized = true;
            //there is no cardlink check device system
            message = "OK";
            return eDeviceCheckResult.SUCCESS;

        }

        public override void AfterLoad(List<Device> devices)
        {
            base.AfterLoad(devices);
        }


    }
}
