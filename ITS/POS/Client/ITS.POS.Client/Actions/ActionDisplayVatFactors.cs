using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Kernel;
using ITS.POS.Hardware;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Displays the current VAT Factors of the application and the fiscal printer's (if it exists)
    /// </summary>
    public class ActionDisplayVatFactors : Action
    {
        public ActionDisplayVatFactors(IPosKernel kernel)
            : base(kernel)
        {

        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.UNKNOWN | eMachineStatus.SALE | eMachineStatus.PAUSE | eMachineStatus.OPENDOCUMENT_PAYMENT | eMachineStatus.OPENDOCUMENT | eMachineStatus.DAYSTARTED | eMachineStatus.CLOSED; }
        }

        public override bool RequiresParameters
        {
            get { return false; }
        }

        public override eActions ActionCode
        {
            get { return eActions.DISPLAY_VAT_FACTORS; }
        }

        protected override void ExecuteCore(ActionParams parameters, bool dontCheckPermissions)
        {
            IVatFactorService vatFactorService = Kernel.GetModule<IVatFactorService>();
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            IConfigurationManager configurationManager = Kernel.GetModule<IConfigurationManager>();
            IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();
            IFormManager formManager = Kernel.GetModule<IFormManager>();

            string message = Resources.POSClientResources.APPLICATION_VAT_FACTORS + Environment.NewLine;
            Store currentStore = sessionManager.GetObjectByKey<Store>(configurationManager.CurrentStoreOid);
            Guid vatLevel = Guid.Empty;
            if (currentStore != null)
            {
                Address address = sessionManager.GetObjectByKey<Address>(currentStore.Address);
                if (address != null)
                {
                    vatLevel = address.VatLevel;
                }
                else
                {
                    throw new POSException(POSClientResources.STORE_HAS_NO_ADDRESS);
                }
            }
            else
            {
                throw new POSException(POSClientResources.CURRENT_STORE_NOT_FOUND);
            }

            if (vatLevel == Guid.Empty)
            {
                throw new POSException(POSClientResources.STORE_HAS_NO_VAT_LEVEL);
            }


            foreach (eMinistryVatCategoryCode ministryCode in Enum.GetValues(typeof(eMinistryVatCategoryCode)).Cast<eMinistryVatCategoryCode>().Where(code=>code != eMinistryVatCategoryCode.NONE))
            {
                VatFactor factor = vatFactorService.GetApplicationVatFactor(ministryCode, vatLevel);
                message += ministryCode + ":  " + (factor == null ? " - " : factor.Factor.ToString()) + " \\ ";
            }

            message = message.TrimEnd(' ','\\');

            if (configurationManager.FiscalMethod == eFiscalMethod.ADHME)
            {
                message += Environment.NewLine +POSClientResources.FISCAL_PRINTER_VAT_FACTORS + ":" + Environment.NewLine;
                FiscalPrinter primaryPrinter = deviceManager.GetPrimaryDevice<FiscalPrinter>();
                if (primaryPrinter == null)
                {
                    throw new POSException(POSClientResources.NO_PRIMARY_PRINTER_FOUND);
                }
                double fiscalVatA, fiscalVatB, fiscalVatC, fiscalVatD, fiscalVatE;
                primaryPrinter.ReadVatRates(out fiscalVatA, out  fiscalVatB, out fiscalVatC, out fiscalVatD, out fiscalVatE);

                message += eMinistryVatCategoryCode.A + ":  " + fiscalVatA + " \\ ";
                message += eMinistryVatCategoryCode.B + ":  " + fiscalVatB + " \\ ";
                message += eMinistryVatCategoryCode.C + ":  " + fiscalVatC + " \\ ";
                message += eMinistryVatCategoryCode.D + ":  " + fiscalVatD + " \\ ";
                message += eMinistryVatCategoryCode.E + ":  " + fiscalVatE;
            }

            formManager.ShowCancelOnlyMessageBox(message);
        }

    }
}
