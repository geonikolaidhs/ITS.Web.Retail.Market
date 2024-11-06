namespace ITS.Retail.DesktopClient.StoreControllerClient.Controls
{
    partial class CustomerSecondaryFilterControl
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
            this.textEditCode = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.textEditCompanyName = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.textEditTaxCode = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditCompanyName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditTaxCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.textEditTaxCode);
            this.layoutControl1.Controls.Add(this.textEditCompanyName);
            this.layoutControl1.Controls.Add(this.textEditCode);
            this.layoutControl1.Size = new System.Drawing.Size(639, 54);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3});
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Size = new System.Drawing.Size(639, 54);
            // 
            // textEditCode
            // 
            this.SetBoundFieldName(this.textEditCode, "CustomerCode");
            this.SetBoundPropertyName(this.textEditCode, "EditValue");
            this.textEditCode.Location = new System.Drawing.Point(108, 12);
            this.textEditCode.Name = "textEditCode";
            this.textEditCode.Size = new System.Drawing.Size(106, 20);
            this.textEditCode.StyleController = this.layoutControl1;
            this.textEditCode.TabIndex = 5;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.textEditCode;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(206, 34);
            this.layoutControlItem1.Text = "@@Code";
            this.layoutControlItem1.TextSize = new System.Drawing.Size(92, 13);
            // 
            // textEditCompanyName
            // 
            this.SetBoundFieldName(this.textEditCompanyName, "CustomerName");
            this.SetBoundPropertyName(this.textEditCompanyName, "EditValue");
            this.textEditCompanyName.Location = new System.Drawing.Point(314, 12);
            this.textEditCompanyName.Name = "textEditCompanyName";
            this.textEditCompanyName.Size = new System.Drawing.Size(107, 20);
            this.textEditCompanyName.StyleController = this.layoutControl1;
            this.textEditCompanyName.TabIndex = 6;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.textEditCompanyName;
            this.layoutControlItem2.Location = new System.Drawing.Point(206, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(207, 34);
            this.layoutControlItem2.Text = "@@CompanyName";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(92, 13);
            // 
            // textEditTaxCode
            // 
            this.SetBoundFieldName(this.textEditTaxCode, "TaxCode");
            this.SetBoundPropertyName(this.textEditTaxCode, "EditValue");
            this.textEditTaxCode.Location = new System.Drawing.Point(521, 12);
            this.textEditTaxCode.Name = "textEditTaxCode";
            this.textEditTaxCode.Size = new System.Drawing.Size(106, 20);
            this.textEditTaxCode.StyleController = this.layoutControl1;
            this.textEditTaxCode.TabIndex = 7;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.textEditTaxCode;
            this.layoutControlItem3.Location = new System.Drawing.Point(413, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(206, 34);
            this.layoutControlItem3.Text = "@@TaxCode";
            this.layoutControlItem3.TextSize = new System.Drawing.Size(92, 13);
            // 
            // CustomerSecondaryFilterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "CustomerSecondaryFilterControl";
            this.Size = new System.Drawing.Size(769, 54);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditCompanyName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditTaxCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.TextEdit textEditCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraEditors.TextEdit textEditTaxCode;
        private DevExpress.XtraEditors.TextEdit textEditCompanyName;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
    }
}
