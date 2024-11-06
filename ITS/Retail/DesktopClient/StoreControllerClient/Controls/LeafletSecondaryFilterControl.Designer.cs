namespace ITS.Retail.DesktopClient.StoreControllerClient.Controls
{
    partial class LeafletSecondaryFilterControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textEdit1 = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlItemName = new DevExpress.XtraLayout.LayoutControlItem();
            this.lueIsActive = new DevExpress.XtraEditors.LookUpEdit();
            this.layoutControlItemIsActive = new DevExpress.XtraLayout.LayoutControlItem();
            this.textEdit2 = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlItemCode = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueIsActive.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemIsActive)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit2.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemCode)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.textEdit1);
            this.layoutControl1.Controls.Add(this.lueIsActive);
            this.layoutControl1.Controls.Add(this.textEdit2);
            this.layoutControl1.Size = new System.Drawing.Size(640, 54);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItemName,
            this.layoutControlItemIsActive,
            this.layoutControlItemCode});
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Size = new System.Drawing.Size(640, 54);
            // 
            // textEdit1
            // 
            this.SetBoundFieldName(this.textEdit1, "Description");
            this.SetBoundPropertyName(this.textEdit1, "EditValue");
            this.textEdit1.Location = new System.Drawing.Point(234, 12);
            this.textEdit1.Name = "textEdit1";
            this.textEdit1.Size = new System.Drawing.Size(206, 20);
            this.textEdit1.StyleController = this.layoutControl1;
            this.textEdit1.TabIndex = 9;
            // 
            // layoutControlItemName
            // 
            this.layoutControlItemName.Control = this.textEdit1;
            this.layoutControlItemName.CustomizationFormText = "@@Name";
            this.layoutControlItemName.Location = new System.Drawing.Point(159, 0);
            this.layoutControlItemName.Name = "layoutControlItemName";
            this.layoutControlItemName.Size = new System.Drawing.Size(273, 34);
            this.layoutControlItemName.Text = "@@Name";
            this.layoutControlItemName.TextLocation = DevExpress.Utils.Locations.Left;
            this.layoutControlItemName.TextSize = new System.Drawing.Size(59, 13);
            // 
            // lueIsActive
            // 
            this.SetBoundFieldName(this.lueIsActive, "IsActive");
            this.SetBoundPropertyName(this.lueIsActive, "EditValue");
            this.lueIsActive.Location = new System.Drawing.Point(507, 12);
            this.lueIsActive.Name = "lueIsActive";
            this.lueIsActive.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.lueIsActive.Properties.NullText = "";
            this.lueIsActive.Properties.PopupSizeable = false;
            this.lueIsActive.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.lueIsActive.Size = new System.Drawing.Size(121, 20);
            this.lueIsActive.StyleController = this.layoutControl1;
            this.lueIsActive.TabIndex = 13;
            this.lueIsActive.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.lueIsActive_ButtonClick);
            // 
            // layoutControlItemIsActive
            // 
            this.layoutControlItemIsActive.Control = this.lueIsActive;
            this.layoutControlItemIsActive.CustomizationFormText = "@@IsActive";
            this.layoutControlItemIsActive.Location = new System.Drawing.Point(432, 0);
            this.layoutControlItemIsActive.Name = "layoutControlItemIsActive";
            this.layoutControlItemIsActive.Size = new System.Drawing.Size(188, 34);
            this.layoutControlItemIsActive.Text = "@@IsActive";
            this.layoutControlItemIsActive.TextLocation = DevExpress.Utils.Locations.Left;
            this.layoutControlItemIsActive.TextSize = new System.Drawing.Size(59, 13);
            // 
            // textEdit2
            // 
            this.SetBoundFieldName(this.textEdit2, "Code");
            this.SetBoundPropertyName(this.textEdit2, "EditValue");
            this.textEdit2.Location = new System.Drawing.Point(75, 12);
            this.textEdit2.Name = "textEdit2";
            this.textEdit2.Size = new System.Drawing.Size(92, 20);
            this.textEdit2.StyleController = this.layoutControl1;
            this.textEdit2.TabIndex = 6;
            // 
            // layoutControlItemCode
            // 
            this.layoutControlItemCode.Control = this.textEdit2;
            this.layoutControlItemCode.CustomizationFormText = "@@Code";
            this.layoutControlItemCode.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItemCode.Name = "layoutControlItemCode";
            this.layoutControlItemCode.Size = new System.Drawing.Size(159, 34);
            this.layoutControlItemCode.Text = "@@Code";
            this.layoutControlItemCode.TextLocation = DevExpress.Utils.Locations.Left;
            this.layoutControlItemCode.TextSize = new System.Drawing.Size(59, 13);
            // 
            // PriceCatalogSecondaryFilterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "PriceCatalogSecondaryFilterControl";
            this.Size = new System.Drawing.Size(770, 54);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueIsActive.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemIsActive)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit2.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemCode)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraEditors.TextEdit textEdit1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemName;
        private DevExpress.XtraEditors.LookUpEdit lueIsActive;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemIsActive;
        private DevExpress.XtraEditors.TextEdit textEdit2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemCode;
    }
}
