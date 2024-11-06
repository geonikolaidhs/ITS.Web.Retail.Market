using ITS.POS.Client.Actions.Permission;
using ITS.POS.Client.Forms;
using ITS.POS.Client.Helpers;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.Receipt;
using ITS.POS.Client.UserControls;
using ITS.POS.Hardware;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
using ITS.POS.Model.Transactions;
using ITS.Retail.Platform.Enumerations;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Kernel;

namespace ITS.POS.Client.Kernel
{

    public interface IAppContext : IKernelModule
    {
        DocumentHeader CurrentDocument { get; set; }
        DocumentDetail CurrentDocumentLine { get; set; }
        DocumentPayment CurrentDocumentPayment { get; set; }
        DailyTotals CurrentDailyTotals { get; set; }
        UserDailyTotals CurrentUserDailyTotals { get; set; }
        IScannerInput CurrentFocusedScannerInput { get; set; }
        frmSplashScreen SplashForm { get; set; }
        DateTime FiscalDate { get; set; }
        frmTouchPad TouchPadPopup { get; set; }
        Customer CurrentCustomer { get; set; }
        User CurrentUser { get; set; }
        frmMainBase MainForm { get; set; }
        frmSupportingBase SupportingForm { get; set; }
        List<DocumentHeader> DocumentsOnHold { get; set; }
        eMachineStatus GetMachineStatus();
        void SetMachineStatus(eMachineStatus newStatus, bool showMessage = true, int messageDelay = -1);

        EventHandler ScannerInputOnEnterHandler(IAppContext appContext);
        EventHandler ScannerInputOnLeaveHandler(IAppContext appContext);

    }

}

