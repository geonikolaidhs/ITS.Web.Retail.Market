using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using ITS.Retail.Common;
using ITS.Retail.DesktopClient.StoreControllerClient.Helpers;
using ITS.Retail.Model;
using ITS.Retail.Model.SupportingClasses;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms
{
    public partial class TransformDocumentsForm : XtraLocalizedForm
    {
        private DocumentHeader InitialDocumentHeader { get; set; }
        private List<TransformationRule> TransformationRules { get; set; }
        private DocumentType _TransformationDocumentType;
       
        public DocumentType TransformationDocumentType
        {
            get
            {
                return _TransformationDocumentType;
            }
            set
            {
                _TransformationDocumentType = value;

                TransformationDocumentSeries = null;
                lookUpEditTransformationDocumentSeries.Properties.DataSource = StoreHelper.StoreSeriesForDocumentType(InitialDocumentHeader.Store, _TransformationDocumentType, eModule.STORECONTROLLER);
            }
        }
        public DocumentSeries TransformationDocumentSeries { get; set; }

        public TransformDocumentsForm( DocumentHeader initialDocumentHeader ,List<TransformationRule> transformationRules)
        {
            InitializeComponent();
            TransformationRules = transformationRules;
            InitialDocumentHeader = initialDocumentHeader;
            lookUpEditTransformationDocumentTypes.Properties.DataSource = transformationRules.Select(rule => rule.DerrivedType);
            lookUpEditTransformationDocumentTypes.Properties.Columns.Clear();
            lookUpEditTransformationDocumentTypes.Properties.ValueMember = "This";
            lookUpEditTransformationDocumentTypes.Properties.DisplayMember = "Description";
            lookUpEditTransformationDocumentTypes.Properties.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));
            lookUpEditTransformationDocumentTypes.DataBindings.Add("EditValue", this, "TransformationDocumentType", true, DataSourceUpdateMode.OnPropertyChanged);


            lookUpEditTransformationDocumentSeries.Properties.Columns.Clear();
            lookUpEditTransformationDocumentSeries.Properties.ValueMember = "This";
            lookUpEditTransformationDocumentSeries.Properties.DisplayMember = "Description";
            lookUpEditTransformationDocumentSeries.Properties.Columns.Add(new LookUpColumnInfo("Description", Resources.Description));
            lookUpEditTransformationDocumentSeries.DataBindings.Add("EditValue", this, "TransformationDocumentSeries", true, DataSourceUpdateMode.OnPropertyChanged);

            LocaliseApplication();
        }

        private void LocaliseApplication()
        {
            this.Text = Resources.Transform;
            layoutControlItemTransformationDocumentType.Text = Resources.DocumentType;
            layoutControlItemTransformationDocumentSeries.Text = Resources.DocumentSeries;
            simpleButtonOK.Text = Resources.Continue;
            simpleButtonCancel.Text = Resources.Cancel;
        }

        private void simpleButtonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void simpleButtonOK_Click(object sender, EventArgs e)
        {
            if (TransformationDocumentType == null || TransformationDocumentSeries == null)
            {
                XtraMessageBox.Show(Resources.FillAllMissingFields, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void TransformDocumentsForm_Shown(object sender, EventArgs e)
        {
            lookUpEditTransformationDocumentTypes.Focus();
        }
    }
}
