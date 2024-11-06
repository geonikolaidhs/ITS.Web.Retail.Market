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
    [Obsolete("DO NOT RENAME OR CHANGE NAMESPACE FOR BACKWARDS COMPATIBILITY!")]
    public partial class TotalQuantity : DoubleLabel, IObserverNumberDisplayer
    {
        public TotalQuantity()
        {
            InitializeComponent();
            this.ActionsToObserve.Add(eActions.PUBLISH_DOCUMENT_QUANTITY);
        }

        private bool _AlwaysShow;
        public bool AlwaysShow
        {
            get
            {
                return _AlwaysShow;
            }
            set
            {
                _AlwaysShow = value;
                //if (!this.DesignMode)
                //{
                //    this.Visible = value;
                //}
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
