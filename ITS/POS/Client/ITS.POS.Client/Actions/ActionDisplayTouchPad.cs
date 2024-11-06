using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Forms;
using ITS.POS.Client.UserControls;
using ITS.POS.Model.Settings;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Shows the TouchPad. For internal use (not directly invoked by the user)
    /// </summary>
    public class ActionDisplayTouchPad : Action
    {
        public ActionDisplayTouchPad(IPosKernel kernel) : base(kernel)
        {

        }

        public override eActions ActionCode
        {
            get { return eActions.DISPLAY_TOUCH_PAD; }
        }

        public override bool RequiresParameters
        {
            get { return true; }
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.UNKNOWN | eMachineStatus.SALE | eMachineStatus.PAUSE | eMachineStatus.OPENDOCUMENT_PAYMENT | eMachineStatus.OPENDOCUMENT | eMachineStatus.DAYSTARTED | eMachineStatus.CLOSED; }
        }

        protected override void ExecuteCore(ActionParameters.ActionParams parameters, bool dontCheckPermissions = false)
        {
            ucTouchFriendlyInput input = (parameters as ActionDisplayTouchPadParams).Input as ucTouchFriendlyInput;
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            if (appContext.TouchPadPopup.Visible && appContext.TouchPadPopup.ucTouchPad.TouchFriendlyInput != input)
            {
                appContext.TouchPadPopup.Hide();
                appContext.TouchPadPopup.Dispose();
                appContext.TouchPadPopup = new frmTouchPad();
            }

            appContext.TouchPadPopup.Show();


            if (appContext.TouchPadPopup.ucTouchPad.TouchFriendlyInput != input) //input was changed, re-adjust position
            {
                AdjustTouchPadPosition(input);
            }
        }

        protected override eKeyStatus DefaultKeyStatusRequirement
        {
            get
            {
                return eKeyStatus.UNKNOWN;
            }
        }

        private void AdjustTouchPadPosition(ucTouchFriendlyInput input)
        {
            Form parentForm = input.FindForm();
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            appContext.TouchPadPopup.ucTouchPad.TouchFriendlyInput = input;

            Rectangle workingArea = Screen.FromControl(parentForm).Bounds;//Screen.PrimaryScreen.Bounds;
            if ((workingArea.Width - parentForm.Width) >= 2 * appContext.TouchPadPopup.Width)
            {//Form and Keyboard fit to screen. Form in center and Keyboard to the right
                appContext.TouchPadPopup.Left = parentForm.Right;
                appContext.TouchPadPopup.Top = parentForm.Top;
            }
            else if (workingArea.Width >= parentForm.Width + appContext.TouchPadPopup.Width)
            {//Form and Keyboard Fit to Screen if moved to the left side while keeping it in the center
                int widthDiff = workingArea.Width - parentForm.Width - appContext.TouchPadPopup.Width;
                parentForm.Left = widthDiff / 2;
                appContext.TouchPadPopup.Left = parentForm.Right;
                appContext.TouchPadPopup.Top = parentForm.Top;
            }
            else
            {//Form and Keyboard DO NOT FIT TO SCREEN. Therefore we try to move them to the left
                parentForm.Left = 0;
                appContext.TouchPadPopup.Left = Screen.FromControl(parentForm).WorkingArea.Right - appContext.TouchPadPopup.Width;
                appContext.TouchPadPopup.Top = parentForm.Top;
            }
            Application.DoEvents();
        }
    }
}
