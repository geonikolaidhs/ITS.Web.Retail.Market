using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Model.Settings;
using ITS.POS.Client.Helpers;
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Client.UserControls
{
    /// <summary>
    /// Appends the Characters string property to the main form's input text. 
    /// Used for adding special characters or any kind of text that must be written directly to the input.
    /// </summary>
    public partial class ucCustomKeyPressButton : ucButton
    {
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]  //do not delete for backwards compatibility
        public Keys KeyData {get; set;}

        /// <summary>
        /// The text to append
        /// </summary>
        public string Characters { get; set; }


        public ucCustomKeyPressButton()
        {
            InitializeComponent();
            this._button.Click += OnButtonClick;
        }

        protected void OnButtonClick(object sender, EventArgs e)
        {
            IAppContext AppContext = Kernel.GetModule<IAppContext>();
            AppContext.MainForm.AppendInputText(Characters);
        }
    }
}
