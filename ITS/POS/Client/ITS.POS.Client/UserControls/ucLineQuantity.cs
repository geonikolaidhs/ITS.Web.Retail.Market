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
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Enumerations;
using DevExpress.XtraEditors;

namespace ITS.POS.Client.UserControls
{
    /// <summary>
    /// Displays the currently selected quantity that will be used for the next document line
    /// </summary>
    [ChildsUseSameFont]
    public partial class ucLineQuantity : ucObserver, IObserverNumberDisplayer
    {
        Type[] paramsTypes = new Type[] { typeof(NumberDisplayerParams) };

        public override Type[] GetParamsTypes()
        {
            return paramsTypes;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public LabelControl Label
        {
            get
            {
                return this.labelControl1;
            }

        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public TextEdit QuantityText
        {
            get
            {
                return this.tbQty;
            }
        }

        public ucLineQuantity()
        {
            InitializeComponent();
            this.ActionsToObserve.Add(eActions.PUBLISH_LINE_QUANTITY_INFO);
        }

        public void Update(NumberDisplayerParams parameters)
        {
            if (parameters != null)
            {
                this.tbQty.Text = parameters.Number.ToString();
            }
        }

        public void SetQuantity(decimal qty)
        {
            this.tbQty.Text = qty.ToString();
        }

        private void ucLineQuantity_Load(object sender, EventArgs e)
        {

        }

    }
}
