using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
using ITS.POS.Client.Helpers;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Resources;
using ITS.POS.Client.Exceptions;
using ITS.Retail.Platform.Enumerations;
using System.Media;
using ITS.POS.Client.Forms;
using ITS.POS.Hardware;

namespace ITS.POS.Client.UserControls
{
    [Obsolete("DO NOT RENAME OR CHANGE NAMESPACE FOR BACKWARDS COMPATIBILITY!")]
    public partial class AddWeightedItemButton : ucitsButton
    {
        public string Barcode { get; set; }

        public AddWeightedItemButton()
        {
            _button.Text = POSClientResources.ADD_ITEM;//"Add Item";                        
            InitializeComponent();
        }

        private void ucAddItemCustom_MouseClick(object sender, MouseEventArgs e)
        {
            Button_Click(sender, e);
        }

        private void Button_Click(object sender, EventArgs e)
        {
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            try
            {
                eMachineStatus status = appContext.GetMachineStatus();
                if (status != eMachineStatus.SALE && status != eMachineStatus.OPENDOCUMENT)
                {
                    return;
                }
                string itemCode = String.IsNullOrWhiteSpace(this.Barcode) ? (this.FindForm() as frmMainBase).InputText : this.Barcode;
                actionManager.GetAction(eActions.ADD_ITEM_WEIGHTED).Execute(new ActionAddWeightedItemParams(itemCode));
                (this.FindForm() as frmMainBase).ResetInputText();
            }
            catch (Exception ex)
            {
                Kernel.LogFile.InfoException("AddWeightedItemButton:Button_Click,Exception catched", ex);
                actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(ex.GetFullMessage()));
            }
        }
    }
}
