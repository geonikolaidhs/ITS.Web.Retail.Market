using ITS.POS.Client.UserControls;
using ITS.POS.Hardware;
using ITS.POS.Hardware.Common;
using ITS.POS.Model.Transactions;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Kernel
{
    public delegate void OnKeyStatusChangeEventHandler(eKeyStatus newStatus);

    public interface IDeviceManager : IKernelModule
    {
        T GetPrimaryDevice<T>() where T : Device;
        List<Device> Devices { get; set; }
        bool IsDrawerOpen { get; }
        eKeyStatus KeyStatus { get; }
        bool HasDemoModeBeenSetupCorrectly(bool globalContextDemoMode, eFiscalMethod fiscalmethod, eFiscalDevice fiscaldevice);
        event OnKeyStatusChangeEventHandler OnKeyStatusChange;
        Device GetEAFDSSDevice(eFiscalDevice fiscalDevice);
        PoleDisplay GetCustomerPoleDisplay();
        PoleDisplay GetCashierPoleDisplay();
        string[] GetDocumentDetailPoleDisplayLines(DocumentDetail detail, bool includeLineNumber);
        string[] GetPaymentPoleDisplayLines(DocumentHeader header, DocumentPayment payment, PoleDisplay poleDisplay, bool includeLineNumber);
        void HandleFocusedPoleDisplayInputChanged(IPoleDisplayInputContainer container, IPoleDisplayInput previousInput, IPoleDisplayInput currentInput, PoleDisplay poleDisplay);
        string GetVisibleVersion(eFiscalMethod fiscalMethod);
        List<Device> GetEAFDSSDevicesByPriority(eFiscalDevice fiscalDevice);
        T GetDeviceByName<T>(String name) where T : Device;
    }
}
