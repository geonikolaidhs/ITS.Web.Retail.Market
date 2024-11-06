using System;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Collections.Generic;
using ITS.POS.Model.Settings;
using System.Web.UI.WebControls;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Actions;
using ITS.POS.Client.Exceptions;
using System.Drawing;
using ITS.POS.Client.Helpers;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Resources;

namespace ITS.POS.Client.UserControls
{
    [Obsolete("DO NOT RENAME OR CHANGE NAMESPACE FOR BACKWARDS COMPATIBILITY!")]
    public partial class BaseActionButton: ucitsButton
    {
        public BaseActionButton()
        {
            CheckPermissions = true;
            InitializeComponent();
            this._button.Click += new System.EventHandler(OnButtonClick);
        }

        private eActions _Action;
        public eActions Action
        {
            get
            {
                return _Action;
            }
            set
            {
                _Action = value;
            }
        }

        public bool CheckPermissions { get; set; }

        protected virtual void OnButtonClick(object sender,EventArgs e)
        {
            if(this._Action != eActions.NONE)
            {
                IFormManager formManager = Kernel.GetModule<IFormManager>();
                IActionManager actionManager = Kernel.GetModule<IActionManager>();

                IActionExecutor executor = Kernel.GetModule<IActionExecutor>();
                try
                {
                    executor.ExecuteAction(this._Action, "",!CheckPermissions);
                }
                catch (InvalidMachineStatusException )
                {
                    actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.ACTION_FORBIDDEN));
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null && ex.InnerException.GetType() == typeof(InvalidMachineStatusException))
                    {
                        actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(POSClientResources.ACTION_FORBIDDEN));
                    }
                    else
                    {
                        formManager.ShowMessageBox(ex.GetFullMessage());
                        //MessageBox.Show("An error has occured." + Environment.NewLine + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Kernel.LogFile.ErrorException("Exception during custom button execution", ex);
                    }
                }
            }
        }

    }
}
