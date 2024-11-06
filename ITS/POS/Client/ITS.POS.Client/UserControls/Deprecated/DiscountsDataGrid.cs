using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Data.Filtering;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.ObserverPattern;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.POS.Model.Transactions;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Extensions;
using ITS.POS.Client.Helpers;
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Kernel;

namespace ITS.POS.Client.UserControls
{
    [Obsolete("DO NOT RENAME OR CHANGE NAMESPACE FOR BACKWARDS COMPATIBILITY!")]
    public class DiscountsDataGrid : DataGridViewExtension, IObserverDocumentDetailDisplayer
    {

        public DiscountsDataGrid()
        {
            //this.Visible = false;
            this.ActionsToObserve.Add(eActions.PUBLISH_DOCUMENT_LINE_INFO);
            InitializeComponent();
        }

        public override void Update(GridParams parameters)
        {
            if (parameters.DisplayMode != null)
            {
                Visible = (parameters.DisplayMode == eDisplayMode.DETAILS);
            }
            else if (parameters.DisplayMode == null && parameters.SelectedDocumentDetail == null && parameters.DocumentHeader == null && parameters.SelectedDocumentPayment == null)
            {
                DataSource = null;
                Visible = true;
            }
           

        }

        public void Update(DocumentDetailDisplayerParams parameters)
        {
            IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
            OwnerApplicationSettings appSettings = config.GetAppSettings();
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
            IPlatformRoundingHandler platformRoundingHandler = Kernel.GetModule<IPlatformRoundingHandler>();

            DataSource = parameters.DocumentDetail.DocumentDetailDiscounts.Where(x => x.DiscountSource != eDiscountSource.DOCUMENT).Select(x => DocumentDetailDiscountHelper.Convert(x,sessionManager,platformRoundingHandler)).ToList();
        }

        private IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }


        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        }

        public override Type[] GetParamsTypes()
        {            
            return new Type[]{typeof(GridParams),typeof(DocumentDetailDisplayerParams)};
        }
    }
}

