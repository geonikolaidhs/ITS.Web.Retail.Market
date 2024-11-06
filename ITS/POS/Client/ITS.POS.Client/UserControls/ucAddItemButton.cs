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

namespace ITS.POS.Client.UserControls
{
    /// <summary>
    /// An action button for adding a specific item to the current document. 
    /// The UI designer sets the Barcode that will be added. 
    /// </summary>
    public partial class ucAddItemButton : ucButton
    {
        /// <summary>
        /// The Barcode of the item to add to the current document
        /// </summary>
        public string Barcode { get; set; }

        public int Order { get; set; }

        [Browsable(false)] //do not delete for backwards compatibility
        [Obsolete]
        public double Quantity { get; set; }


        public ucAddItemButton()
        {
            _button.Text = POSClientResources.ADD_ITEM;
            Quantity = 1;
            InitializeComponent();
        }

        public ucAddItemButton(int order)
        {
            Order = order;
            _button.Text = POSClientResources.ADD_ITEM;
            Quantity = 1;
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
                string itemCode = String.IsNullOrWhiteSpace(this.Barcode) ? (this.FindMainForm()).InputText : this.Barcode;
                actionManager.GetAction(eActions.ADD_ITEM).Execute(new ActionAddItemParams(itemCode, this.FindMainForm().SelectedQty));
            }
            catch (Exception ex)
            {
                Kernel.LogFile.Info(ex, "AddItemButton:Button_Click,Exception catched");
                actionManager.GetAction(eActions.SHOW_ERROR).Execute(new ActionShowErrorParams(ex.GetFullMessage()));
            }
            finally
            {
                this.FindMainForm().SelectedQty = 1;
                this.FindMainForm().ResetInputText();
            }
        }
    }
}
