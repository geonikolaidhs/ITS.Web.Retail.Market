using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ITS.Service.Guard
{
    public static class Extensions
    {

        private static Form _mainForm { get; set; } = Program.GetMainForm();
        public static void Invoke<TControlType>(this TControlType control, Action<TControlType> del)
            where TControlType : Control
        {
            if (control.InvokeRequired)
                control.Invoke(new Action(() => del(control)));
            else
                del(control);
        }

        private static void Invoke(Form form, String controlName, Action<Control> del)

        {

            Control[] controls = form.Controls.Find(controlName, true);
            if (controls.Length > 0)
            {
                if (controls[0].InvokeRequired)
                    controls[0].Invoke(new Action(() => del(controls[0])));
                else
                    del(controls[0]);

            }

        }


        public static void UpdateUI(String controlName, String text, String operationText = "")
        {
            if (_mainForm != null)
            {
                Extensions.Invoke(_mainForm, controlName, t => t.Text = text);

                if (!String.IsNullOrEmpty(operationText))
                {
                    Extensions.Invoke(_mainForm, "LabelLastOperation", t => t.Text = "Last Operation :" + operationText + " " + DateTime.Now.ToString());
                }
            }

        }
    }
}
