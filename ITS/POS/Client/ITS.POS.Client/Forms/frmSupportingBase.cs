using DevExpress.XtraEditors;
using ITS.POS.Client.Helpers;
using ITS.POS.Client.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.POS.Client.ObserverPattern;
using ITS.POS.Client.Kernel;

namespace ITS.POS.Client.Forms
{
    public partial class frmSupportingBase : XtraForm, IObserverContainer, IPOSForm
    {
        private bool _subscriptionsAreInitialized;
        public bool SubscriptionsAreInitialized
        {
            get
            {
                return _subscriptionsAreInitialized;
            }
        }
        public frmSupportingBase()
        {
            InitializeComponent();
        }



        public void IntitializeSubscriptions()
        {

            if (!SubscriptionsAreInitialized)
            {
                IFormManager formHandler = Kernel.GetModule<IFormManager>();
                IEnumerable<Control> controls = formHandler.GetAllChildControls(this);
                foreach (Control control in controls)
                {
                    if (typeof(IObserver).IsAssignableFrom(control.GetType()))
                    {
                        ((IObserver)control).InitializeActionSubscriptions();                        
                    }
                }

                _subscriptionsAreInitialized = true;
            }
        }

        public void DropSubscriptions()
        {
            if (SubscriptionsAreInitialized)
            {
                IFormManager formHandler = Kernel.GetModule<IFormManager>();
                IEnumerable<Control> controls = formHandler.GetAllChildControls(this);
                foreach (Control control in controls)
                {
                    if (control is ucObserver)
                    {
                        (control as ucObserver).DropActionSubscriptions();
                    }
                }

                _subscriptionsAreInitialized = false;
            }
        }

        public IPosKernel Kernel { get; set; }

        public void Initialize(IPosKernel kernel)
        {
            this.Kernel = kernel;
        }
    }
}
