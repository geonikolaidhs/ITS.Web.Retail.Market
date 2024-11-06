namespace ITS.Retail.DesktopClient.StoreControllerClient.Forms
{
    partial class TransformDocumentsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TransformDocumentsForm));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.simpleButtonCancel = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButtonOK = new DevExpress.XtraEditors.SimpleButton();
            this.lookUpEditTransformationDocumentSeries = new DevExpress.XtraEditors.LookUpEdit();
            this.lookUpEditTransformationDocumentTypes = new DevExpress.XtraEditors.LookUpEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItemTransformationDocumentType = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItemTransformationDocumentSeries = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lookUpEditTransformationDocumentSeries.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lookUpEditTransformationDocumentTypes.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemTransformationDocumentType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemTransformationDocumentSeries)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.simpleButtonCancel);
            this.layoutControl1.Controls.Add(this.simpleButtonOK);
            this.layoutControl1.Controls.Add(this.lookUpEditTransformationDocumentSeries);
            this.layoutControl1.Controls.Add(this.lookUpEditTransformationDocumentTypes);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(510, 428, 828, 777);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(341, 102);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // simpleButtonCancel
            // 
            this.simpleButtonCancel.Image = global::ITS.Retail.DesktopClient.StoreControllerClient.Properties.Resources.Cancel_32;
            this.simpleButtonCancel.Location = new System.Drawing.Point(182, 52);
            this.simpleButtonCancel.Name = "simpleButtonCancel";
            this.simpleButtonCancel.Size = new System.Drawing.Size(147, 38);
            this.simpleButtonCancel.StyleController = this.layoutControl1;
            this.simpleButtonCancel.TabIndex = 7;
            this.simpleButtonCancel.Text = "Cancel";
            this.simpleButtonCancel.Click += new System.EventHandler(this.simpleButtonCancel_Click);
            // 
            // simpleButtonOK
            // 
            this.simpleButtonOK.Image = global::ITS.Retail.DesktopClient.StoreControllerClient.Properties.Resources.Ok_32;
            this.simpleButtonOK.Location = new System.Drawing.Point(12, 52);
            this.simpleButtonOK.Name = "simpleButtonOK";
            this.simpleButtonOK.Size = new System.Drawing.Size(166, 38);
            this.simpleButtonOK.StyleController = this.layoutControl1;
            this.simpleButtonOK.TabIndex = 6;
            this.simpleButtonOK.Text = "OK";
            this.simpleButtonOK.Click += new System.EventHandler(this.simpleButtonOK_Click);
            // 
            // lookUpEditTransformationDocumentSeries
            // 
            this.lookUpEditTransformationDocumentSeries.Location = new System.Drawing.Point(182, 28);
            this.lookUpEditTransformationDocumentSeries.Name = "lookUpEditTransformationDocumentSeries";
            this.lookUpEditTransformationDocumentSeries.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lookUpEditTransformationDocumentSeries.Properties.NullText = "";
            this.lookUpEditTransformationDocumentSeries.Size = new System.Drawing.Size(147, 20);
            this.lookUpEditTransformationDocumentSeries.StyleController = this.layoutControl1;
            this.lookUpEditTransformationDocumentSeries.TabIndex = 5;
            // 
            // lookUpEditTransformationDocumentTypes
            // 
            this.lookUpEditTransformationDocumentTypes.Location = new System.Drawing.Point(12, 28);
            this.lookUpEditTransformationDocumentTypes.Name = "lookUpEditTransformationDocumentTypes";
            this.lookUpEditTransformationDocumentTypes.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lookUpEditTransformationDocumentTypes.Properties.NullText = "";
            this.lookUpEditTransformationDocumentTypes.Size = new System.Drawing.Size(166, 20);
            this.lookUpEditTransformationDocumentTypes.StyleController = this.layoutControl1;
            this.lookUpEditTransformationDocumentTypes.TabIndex = 4;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItemTransformationDocumentType,
            this.layoutControlItemTransformationDocumentSeries,
            this.layoutControlItem3,
            this.layoutControlItem4});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Size = new System.Drawing.Size(341, 102);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItemTransformationDocumentType
            // 
            this.layoutControlItemTransformationDocumentType.Control = this.lookUpEditTransformationDocumentTypes;
            this.layoutControlItemTransformationDocumentType.CustomizationFormText = "layoutControlItemTransformationDocumentType";
            this.SetIsRequired(this.layoutControlItemTransformationDocumentType, false);
            this.layoutControlItemTransformationDocumentType.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItemTransformationDocumentType.Name = "layoutControlItemTransformationDocumentType";
            this.layoutControlItemTransformationDocumentType.Size = new System.Drawing.Size(170, 40);
            this.layoutControlItemTransformationDocumentType.Text = "DocumentType";
            this.layoutControlItemTransformationDocumentType.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItemTransformationDocumentType.TextSize = new System.Drawing.Size(77, 13);
            // 
            // layoutControlItemTransformationDocumentSeries
            // 
            this.layoutControlItemTransformationDocumentSeries.Control = this.lookUpEditTransformationDocumentSeries;
            this.SetIsRequired(this.layoutControlItemTransformationDocumentSeries, false);
            this.layoutControlItemTransformationDocumentSeries.Location = new System.Drawing.Point(170, 0);
            this.layoutControlItemTransformationDocumentSeries.Name = "layoutControlItemTransformationDocumentSeries";
            this.layoutControlItemTransformationDocumentSeries.Size = new System.Drawing.Size(151, 40);
            this.layoutControlItemTransformationDocumentSeries.Text = "DocumentSeries";
            this.layoutControlItemTransformationDocumentSeries.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItemTransformationDocumentSeries.TextSize = new System.Drawing.Size(77, 13);
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.simpleButtonOK;
            this.SetIsRequired(this.layoutControlItem3, false);
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 40);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(170, 42);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.simpleButtonCancel;
            this.SetIsRequired(this.layoutControlItem4, false);
            this.layoutControlItem4.Location = new System.Drawing.Point(170, 40);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(151, 42);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // TransformDocumentsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(341, 102);
            this.Controls.Add(this.layoutControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.LookAndFeel.SkinName = "Metropolis";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "TransformDocumentsForm";
            this.Text = "TransformDocumentsForm";
            this.Shown += new System.EventHandler(this.TransformDocumentsForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lookUpEditTransformationDocumentSeries.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lookUpEditTransformationDocumentTypes.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemTransformationDocumentType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemTransformationDocumentSeries)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.SimpleButton simpleButtonCancel;
        private DevExpress.XtraEditors.SimpleButton simpleButtonOK;
        private DevExpress.XtraEditors.LookUpEdit lookUpEditTransformationDocumentSeries;
        private DevExpress.XtraEditors.LookUpEdit lookUpEditTransformationDocumentTypes;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemTransformationDocumentType;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemTransformationDocumentSeries;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;

    }
}