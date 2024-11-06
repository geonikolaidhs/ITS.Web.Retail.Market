using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraEditors;
using ITS.POS.Model.Transactions;
using ITS.POS.Client.ObserverPattern;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Model.Settings;
using ITS.POS.Resources;
using ITS.Retail.Platform.Enumerations;
using System.Drawing;
using System.ComponentModel;

namespace ITS.POS.Client.UserControls
{
    /// <summary>
    /// Displays the price of the current document line or payment
    /// </summary>
    [ChildsUseSameFont]
    public partial class ucLinePriceView : ucDoubleLabel, IObserverNumberDisplayer
    {
        Type[] paramsTypes = new Type[] { typeof(NumberDisplayerParams) };

        public override Type[] GetParamsTypes()
        {
            return paramsTypes;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public LabelControl Label //deprecated, only for backwards compatibility
        {
            get
            {
                return this.TitleLabel;
            }

        }

        private TextEdit dummyTextEdit;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public TextEdit PriceText //deprecated, only for backwards compatibility
        {
            get
            {
                if (dummyTextEdit == null)
                {
                    dummyTextEdit = new TextEdit(); 
                }
                return dummyTextEdit;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)] // not deleted for backwards compatibility
        public Color PriceBackColor
        {
            get
            {
                return ValueLabel.Appearance.BackColor;
            }
            set
            {
                ValueLabel.Appearance.BackColor = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)] // not deleted for backwards compatibility
        public Color PriceForeColor
        {
            get
            {
                return ValueLabel.Appearance.ForeColor;
            }
            set
            {
                ValueLabel.Appearance.ForeColor = value;
            }
        }

        //From MSDN:
        //
        //Avoid Visual Inheritance
        //The TableLayoutPanel control does not support visual inheritance in the Windows Forms Designer. 
        //A TableLayoutPanel control in a derived class appears as “locked” at design time.
        public ucLinePriceView()
        {
            InitializeComponent();
            this.ActionsToObserve.Add(eActions.PUBLISH_DOCUMENT_LINE_INFO);
            this.ActionsToObserve.Add(eActions.PUBLISH_DOCUMENT_PAYMENT_INFO);
            
            this.TitleLabel.Text = POSClientResources.LINE_TOTAL;  //see comment above
        }

        public void Update(NumberDisplayerParams parameters)
        {
			if (parameters !=null && parameters.Number !=0)
            {
                ValueLabel.Text = parameters.Number.ToString("c");
            }
            else
            {
                ValueLabel.Text = "";
            }
        }

    }
}
