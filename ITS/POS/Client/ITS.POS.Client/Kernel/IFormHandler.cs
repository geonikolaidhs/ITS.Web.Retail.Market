using ITS.POS.Client.Forms;
using ITS.POS.Hardware.Common;
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ITS.POS.Client.Kernel
{
    public interface IFormManager : IKernelModule
    {
        DialogResult ShowForm<T>(Form currentForm, bool closeCurrentForm, bool displayAsDialog = false, bool disposeAfterShowDialog = false) where T : Form;

        IEnumerable<Control> GetAllChildControls(Control control, Type ofType = null);

        /// <summary>
        /// Creates a Form with the provided inputVariables
        /// </summary>
        /// <param name="customFields"></param>
        /// <returns></returns>
        Form CreateCustomFieldsInputForm(IEnumerable<CustomField> customFields);

        frmMessageBox CreateMessageBox(string message);

        DialogResult ShowCancelOnlyMessageBox(string message);

        DialogResult ShowFailToPrintMessageBox(string message);

        DialogResult ShowMessageBox(string message, MessageBoxButtons buttons = MessageBoxButtons.OK);

        Control GetFocusedControl();

        void HandleBackGroundImage(Form form, bool enablelowendmode);

        void ShowCancelOnlyMessageBoxWithSound(string message, List<Device> comScanners);
    }
}
