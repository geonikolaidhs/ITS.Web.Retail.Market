using System;
using System.Collections.Generic;
using ITS.POS.Model.Transactions;
using DevExpress.XtraEditors;
using ITS.POS.Client.ObserverPattern;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Client.UserControls;
using ITS.POS.Model.Settings;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using System.Drawing;
using System.ComponentModel;
using ITS.POS.Client.Kernel;

namespace ITS.POS.Client.UserControls
{
    /// <summary>
    /// Displays the gross total of the document. 
    /// Optionally it can be set to auto-hide when the application is not in payment mode
    /// </summary>
    [ChildsUseSameFont]
    public partial class ucTotalPriceView : ucDoubleLabel, IObserverNumberDisplayer
    {
        Type[] paramsTypes = new Type[] { typeof(NumberDisplayerParams) };

        public override Type[] GetParamsTypes()
        {
            return paramsTypes;
        }

        private bool _AlwaysShow;
        /// <summary>
        /// If set to true, the control will be always visible, else it will auto-hide when not in payment mode
        /// </summary>
        public bool AlwaysShow {
            get
            {
                return _AlwaysShow;
            }
            set
            {
                _AlwaysShow = value;
            }
        }

       

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)] // not deleted for backwards compatibility
        public Color PriceBackColor
        {
            get
            {
                return this.lblValue.Appearance.BackColor;
            }
            set
            {
                this.lblValue.Appearance.BackColor = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)] // not deleted for backwards compatibility
        public Color PriceForeColor
        {
            get
            {
                return this.lblValue.Appearance.ForeColor;
            }
            set
            {
                this.lblValue.Appearance.ForeColor = value;
            }
        }

        public ucTotalPriceView()
        {
            InitializeComponent();
            this.ActionsToObserve.Add(eActions.PUBLISH_DOCUMENT_INFO);
            //this.lyctrlitmTotal.Text = "         "+POSClientResources.TOTAL;
            //this.Visible = this.AlwaysShow;
            this.lblTitle.Text = POSClientResources.TOTAL;
        }

        
        public void Update(NumberDisplayerParams parameters)
        {
            IAppContext appContext = Kernel.GetModule<IAppContext>();
            if ((parameters as NumberDisplayerParams) != null)
            {
                this.lblValue.Text = (parameters as NumberDisplayerParams).Number.ToString("c");
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
