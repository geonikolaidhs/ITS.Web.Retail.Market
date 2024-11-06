using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Hardware.Common;
using System.Reflection;

namespace ITS.POS.Client.Actions
{
    public class ActionResetEAFDSSDevicesOrder : Action
    {
        public ActionResetEAFDSSDevicesOrder(IPosKernel kernel) : base(kernel)
        {
        }

        public override eActions ActionCode
        {
            get
            {
                return eActions.RESET_EAFDSS_DEVICES_ORDER;
            }
        }

        public override bool RequiresParameters
        {
            get
            {
                return false;
            }
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get
            {
                return eMachineStatus.CLOSED | eMachineStatus.DAYSTARTED;
            }
        }

        protected override void ExecuteCore(ActionParams parameters, bool dontCheckPermissions)
        {
            IConfigurationManager configurationManager = Kernel.GetModule<IConfigurationManager>();
            IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();
            var type = typeof(eFiscalDevice);
            var name = Enum.GetName(type, configurationManager.FiscalDevice);
            FiscalMethodAttribute fiscalMethodAttribute = type.GetField(name).GetCustomAttributes(true).OfType<FiscalMethodAttribute>().FirstOrDefault();
            /*FiscalMethodAttribute fiscalMethodAttribute = configurationManager.FiscalDevice
                                                          .GetType()
                                                          .GetTypeInfo(typeof(eFiscalDevice))
                                                          .GetDeclaredField(configurationManager.FiscalDevice.ToString())
                                                          .GetCustomAttribute(typeof(FiscalMethodAttribute), true).FirstOrDefault() as FiscalMethodAttribute;
             */
            if (fiscalMethodAttribute == null || fiscalMethodAttribute.FiscalMethod != eFiscalMethod.EAFDSS)
            {
                return;
            }
           
            List<Device> devices = deviceManager.GetEAFDSSDevicesByPriority(configurationManager.FiscalDevice);
            if ( devices != null && devices.Count > 0 )
            {
                devices.ForEach(device => {
                    device.FailureCount = 0;
                });
            }
        }
    }
}
