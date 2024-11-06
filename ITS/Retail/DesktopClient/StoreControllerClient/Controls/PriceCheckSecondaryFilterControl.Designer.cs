namespace ITS.Retail.DesktopClient.StoreControllerClient.Controls
{
    partial class PriceCheckSecondaryFilterControl
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
            this.lueCustomer = new DevExpress.XtraEditors.SearchLookUpEdit();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.layoutControlItemCustomer = new DevExpress.XtraLayout.LayoutControlItem();
            this.lueItemBarcode = new DevExpress.XtraEditors.SearchLookUpEdit();
            this.gridView2 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.layoutControlItemItemBarcode = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueCustomer.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemCustomer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueItemBarcode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemItemBarcode)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.lueCustomer);
            this.layoutControl1.Controls.Add(this.lueItemBarcode);
            this.layoutControl1.Size = new System.Drawing.Size(640, 54);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItemCustomer,
            this.layoutControlItemItemBarcode});
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Size = new System.Drawing.Size(640, 54);
            // 
            // lueCustomer
            // 
            this.SetBoundFieldName(this.lueCustomer, "Customer");
            this.SetBoundPropertyName(this.lueCustomer, "EditValue");
            this.lueCustomer.Location = new System.Drawing.Point(82, 12);
            this.lueCustomer.Name = "lueCustomer";
            this.lueCustomer.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lueCustomer.Properties.View = this.gridView1;
            this.lueCustomer.Size = new System.Drawing.Size(235, 20);
            this.lueCustomer.StyleController = this.layoutControl1;
            this.lueCustomer.TabIndex = 4;
            // 
            // gridView1
            // 
            this.gridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            // 
            // layoutControlItemCustomer
            // 
            this.layoutControlItemCustomer.Control = this.lueCustomer;
            this.layoutControlItemCustomer.CustomizationFormText = "@@Customer";
            this.layoutControlItemCustomer.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItemCustomer.Name = "layoutControlItemCustomer";
            this.layoutControlItemCustomer.Size = new System.Drawing.Size(309, 34);
            this.layoutControlItemCustomer.Text = "@@Customer";
            this.layoutControlItemCustomer.TextSize = new System.Drawing.Size(66, 13);
            // 
            // lueItemBarcode
            // 
            this.SetBoundFieldName(this.lueItemBarcode, "ItemBarcode");
            this.SetBoundPropertyName(this.lueItemBarcode, "EditValue");
            this.lueItemBarcode.Location = new System.Drawing.Point(391, 12);
            this.lueItemBarcode.Name = "lueItemBarcode";
            this.lueItemBarcode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lueItemBarcode.Properties.View = this.gridView2;
            this.lueItemBarcode.Size = new System.Drawing.Size(237, 20);
            this.lueItemBarcode.StyleController = this.layoutControl1;
            this.lueItemBarcode.TabIndex = 5;
            // 
            // gridView2
            // 
            this.gridView2.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridView2.Name = "gridView2";
            this.gridView2.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView2.OptionsView.ShowGroupPanel = false;
            // 
            // layoutControlItemItemBarcode
            // 
            this.layoutControlItemItemBarcode.Control = this.lueItemBarcode;
            this.layoutControlItemItemBarcode.CustomizationFormText = "@@Barcode";
            this.layoutControlItemItemBarcode.Location = new System.Drawing.Point(309, 0);
            this.layoutControlItemItemBarcode.Name = "layoutControlItemItemBarcode";
            this.layoutControlItemItemBarcode.Size = new System.Drawing.Size(311, 34);
            this.layoutControlItemItemBarcode.Text = "@@Barcode";
            this.layoutControlItemItemBarcode.TextSize = new System.Drawing.Size(66, 13);
            // 
            // PriceCheckSecondaryFilterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "PriceCheckSecondaryFilterControl";
            this.Size = new System.Drawing.Size(770, 54);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueCustomer.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemCustomer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueItemBarcode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemItemBarcode)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SearchLookUpEdit lueCustomer;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemCustomer;
        private DevExpress.XtraEditors.SearchLookUpEdit lueItemBarcode;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemItemBarcode;
    }
}
