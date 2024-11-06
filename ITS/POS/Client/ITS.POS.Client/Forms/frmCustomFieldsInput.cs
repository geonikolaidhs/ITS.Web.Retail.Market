using DevExpress.XtraEditors;
using ITS.POS.Client.Kernel;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ITS.POS.Client.Forms
{
    public partial class frmCustomFieldsInput : frmInputFormBase
    {
        public frmCustomFieldsInput(IPosKernel kernel) : base(kernel)
        {
            InitializeComponent();
            TitleLabel.Text = POSClientResources.INSERT_VALUES;
        }

        private void frmCustomFieldsInput_FormClosed(object sender, FormClosedEventArgs e)
        {
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            actionManager.GetAction(eActions.HIDE_TOUCH_PAD).Execute();
        }

        public override void Update(ObserverPattern.ObserverParameters.KeyListenerParams parameters)
        {
            base.Update(parameters);

            if (parameters.NotificationType == eNotificationsTypes.ACTION && parameters.ActionCode == eActions.MOVE_UP)
            {
                LookUpEdit selectedControl = FindFocusedControl(this) as LookUpEdit;
                if (selectedControl != null)
                {
                    selectedControl.ItemIndex--;
                }
            }
            else if (parameters.NotificationType == eNotificationsTypes.ACTION && parameters.ActionCode == eActions.MOVE_DOWN)
            {
                LookUpEdit selectedControl = FindFocusedControl(this) as LookUpEdit;
                if (selectedControl != null)
                {
                    selectedControl.ItemIndex++;
                }
            }
        }

        public static Control FindFocusedControl(Control control)
        {
            ContainerControl container = control as ContainerControl;
            while (container != null)
            {
                control = container.ActiveControl;
                container = control as ContainerControl;
            }
            return control;
        }
    }
}
