using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.ObserverPattern;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Client.Kernel;

namespace ITS.POS.Client.UserControls
{
    /// <summary>
    /// Displays the total quantity of the document. 
    /// Optionally it can be set to auto-hide when the application is not in payment mode
    /// </summary>
    public partial class ucTotalQuantity : ucDoubleLabel, IObserverNumberDisplayer
    {
        public ucTotalQuantity()
        {
            InitializeComponent();
            this.ActionsToObserve.Add(eActions.PUBLISH_DOCUMENT_QUANTITY);
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

        Type[] paramTypes = new Type[] { typeof(NumberDisplayerParams) };

        public override Type[] GetParamsTypes()
        {
            return paramTypes;
        }


        public void Update(NumberDisplayerParams parameters)
        {
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            NumberDisplayerParams castedParams = parameters as NumberDisplayerParams;
            if (castedParams != null)
            {
                if (String.IsNullOrWhiteSpace(castedParams.DisplayFormat))
                {
                    this.lblValue.Text = castedParams.Number.ToString("0.00");
                }
                else
                {
                    this.lblValue.Text = castedParams.Number.ToString(castedParams.DisplayFormat);
                }

            }
            else if (parameters == null)
            {
                this.lblValue.Text = "0";
            }
            this.Visible = this.AlwaysShow;


            if (appContext.GetMachineStatus() == eMachineStatus.OPENDOCUMENT_PAYMENT)
            {
                this.Visible = true;
            }
        }
    }
}
