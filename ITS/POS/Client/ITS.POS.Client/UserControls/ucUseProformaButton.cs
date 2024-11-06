using ITS.POS.Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Helpers;
using System.Windows.Forms;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;

namespace ITS.POS.Client.UserControls
{
    /// <summary>
    /// Changes the current document type to the Proforma type
    /// </summary>
    public class ucUseProformaButton : ucActionButton
    {
        public ucUseProformaButton()
        {
            _button.Text = POSClientResources.PROFORMA;//"Prorfoma";
            Action = eActions.USE_PROFORMA;
        }

        public bool ShouldSerializeAction()
        {
            //DO NOT DELETE
            return false;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Button
            // 
            this._button.Size = new System.Drawing.Size(108, 41);

            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "ucitsUseProformaButton";
            this.ResumeLayout(false);

        }
    }
}
