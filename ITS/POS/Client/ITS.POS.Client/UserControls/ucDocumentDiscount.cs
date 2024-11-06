using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.POS.Client.ObserverPattern;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Kernel;

namespace ITS.POS.Client.UserControls
{
    /// <summary>
    /// Displays the document's discount amount. 
    /// Optionally it can be set to auto-hide when the application is not in payment mode
    /// </summary>
    public partial class ucDocumentDiscount : ucDoubleLabel, IObserverNumberDisplayer
    {
        public ucDocumentDiscount()
        {
            InitializeComponent();
            ActionsToObserve.Add(eActions.PUBLISH_DOCUMENT_INFO);
        }

        private bool _AlwaysShow;
        /// <summary>
        /// If set to true, the control will be always visible, else it will auto-hide when not in payment mode
        /// </summary>
        public bool AlwaysShow
        {
            get
            {
                return _AlwaysShow;
            }
            set
            {
                _AlwaysShow = value;
            }
        }

        Type[] paramsTypes = new Type[] { typeof(NumberDisplayerParams) };

        public override Type[] GetParamsTypes()
        {
            return paramsTypes;
        }

        public void Update(ObserverPattern.ObserverParameters.NumberDisplayerParams parameters)
        {
            if (Kernel != null)
            {
                IAppContext appContext = Kernel.GetModule<IAppContext>();
                if (appContext.CurrentDocument != null)
                {
                    decimal discount = appContext.CurrentDocument.DocumentDiscountAmount;
                    this.lblValue.Text = discount > 0 ? String.Format("{0:C}", discount) : "-";
                }
                else if (appContext.GetMachineStatus() == eMachineStatus.OPENDOCUMENT || appContext.GetMachineStatus() == eMachineStatus.SALE)
                {
                    this.lblValue.Text = "-";
                }

                if (appContext.GetMachineStatus() == eMachineStatus.OPENDOCUMENT_PAYMENT)
                {
                    this.Visible = true;
                }
                else
                {
                    this.Visible = this.AlwaysShow;
                }
            }
        }

        private void ucDocumentDiscount_Load(object sender, EventArgs e)
        {
            if (!this.DesignMode)
            {
                this.Update(new NumberDisplayerParams(0, null));
            }
        }
    }
}
