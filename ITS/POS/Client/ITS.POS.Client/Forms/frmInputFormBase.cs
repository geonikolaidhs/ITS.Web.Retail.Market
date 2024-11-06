// -----------------------------------------------------------------------
// <copyright file="frmInputFormBase.cs" company="Hewlett-Packard Company">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace ITS.POS.Client.Forms
{
    using System;
    using System.Collections.Generic;
    using ITS.POS.Client.UserControls;
    using System.Windows.Forms;
    using ITS.POS.Client.Kernel;
    using ObserverPattern.ObserverParameters;

    public class frmInputFormBase : frmDialogBase, IPoleDisplayInputContainer
    {
        protected frmInputFormBase()
        {
        }

        public frmInputFormBase(IPosKernel kernel)
            : base(kernel)
        {
            FocusedPoleDisplayInputChanged += OnFocusedPoleDisplayInputChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (Kernel != null)
            {
                IAppContext appContext = Kernel.GetModule<IAppContext>();
                IFormManager formManager = Kernel.GetModule<IFormManager>();
                appContext.CurrentFocusedScannerInput = null;
                IEnumerable<Control> scannerInputs = formManager.GetAllChildControls(this, typeof(IScannerInput));
                foreach (Control scannerInput in scannerInputs)
                {
                    scannerInput.Enter += appContext.ScannerInputOnEnterHandler(appContext);
                    scannerInput.Leave += appContext.ScannerInputOnLeaveHandler(appContext);
                }
            }
        }


        public bool ShowTitle
        {
            get { return true; }
        }

        public bool ShowInputName
        {
            get { return true; }
        }

        public string Title
        {
            get { return this.Text; }
        }

        public event FocusedPoleDisplayInputChangedEventHandler FocusedPoleDisplayInputChanged;

        IPoleDisplayInput previousPoleDisplayControl = null;
        /// <summary>
        /// This method is called whenever the ActiveControl property changes
        /// </summary>
        protected override void UpdateDefaultButton()
        {
            base.UpdateDefaultButton();
            if (this.ActiveControl != null)
            {
                IPoleDisplayInput actualPoleDisplayControl = this.ActiveControl is IPoleDisplayInput ? this.ActiveControl as IPoleDisplayInput : this.ActiveControl.Parent as IPoleDisplayInput;
                if (actualPoleDisplayControl != null && actualPoleDisplayControl != previousPoleDisplayControl)
                {
                    FocusedPoleDisplayInputChanged(this, new FocusedPoleDisplayInputChangedArgs(previousPoleDisplayControl, actualPoleDisplayControl));
                    previousPoleDisplayControl = actualPoleDisplayControl;
                }
            }
        }

        public void OnFocusedPoleDisplayInputChanged(object sender, FocusedPoleDisplayInputChangedArgs args)
        {
            IDeviceManager deviceManager = Kernel.GetModule<IDeviceManager>();
            deviceManager.HandleFocusedPoleDisplayInputChanged(this as IPoleDisplayInputContainer, args.PreviousControlWithFocus, args.CurrentControlWithFocus, deviceManager.GetCashierPoleDisplay());
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmInputFormBase));
            this.SuspendLayout();
            // 
            // frmInputFormBase
            // 
            this.Appearance.BackColor = System.Drawing.Color.FromArgb(164, 197, 231);
            this.Appearance.ForeColor = System.Drawing.Color.FromArgb(55, 58, 61);
            this.Appearance.Options.UseBackColor = true;
            this.Appearance.Options.UseForeColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.BackgroundImageStore = null;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.LookAndFeel.SkinName = "Metropolis";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "frmInputFormBase";
            this.ResumeLayout(false);

        }
        public virtual void Update(ItemGridParams parameters)
        {
        }
    }
}
