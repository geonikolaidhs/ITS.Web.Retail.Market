namespace ITS.Retail.DesktopClient.StoreControllerClient.Controls
{
    partial class SupplierFilter
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
            this.layoutControlItemCreatedOn = new DevExpress.XtraLayout.LayoutControlItem();
            this.textEditCreatedOn = new DevExpress.XtraEditors.DateEdit();
            this.layoutControlItemUpdatedOn = new DevExpress.XtraLayout.LayoutControlItem();
            this.textEditUpdatedOn = new DevExpress.XtraEditors.DateEdit();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlFilter)).BeginInit();
            this.layoutControlFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupFilter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemCreatedOn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditCreatedOn.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditCreatedOn.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemUpdatedOn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditUpdatedOn.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditUpdatedOn.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControlFilter
            // 
            this.layoutControlFilter.Controls.Add(this.textEditCreatedOn);
            this.layoutControlFilter.Controls.Add(this.textEditUpdatedOn);
            this.layoutControlFilter.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(398, 475, 749, 631);
            this.layoutControlFilter.Size = new System.Drawing.Size(678, 145);
            // 
            // layoutControlGroupFilter
            // 
            this.layoutControlGroupFilter.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItemCreatedOn,
            this.layoutControlItemUpdatedOn,
            this.emptySpaceItem1});
            this.layoutControlGroupFilter.Name = "Root";
            this.layoutControlGroupFilter.Size = new System.Drawing.Size(678, 145);
            // 
            // layoutControlItemCreatedOn
            // 
            this.layoutControlItemCreatedOn.Control = this.textEditCreatedOn;
            this.layoutControlItemCreatedOn.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItemCreatedOn.Name = "layoutControlItemCreatedOn";
            this.layoutControlItemCreatedOn.Size = new System.Drawing.Size(658, 40);
            this.layoutControlItemCreatedOn.Text = "@@NewSuppliersFrom";
            this.layoutControlItemCreatedOn.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItemCreatedOn.TextSize = new System.Drawing.Size(108, 13);
            // 
            // textEditCreatedOn
            // 
            this.SetBoundFieldName(this.textEditCreatedOn, "CreatedOn");
            this.SetBoundPropertyName(this.textEditCreatedOn, "EditValue");
            this.textEditCreatedOn.EditValue = null;
            this.textEditCreatedOn.Location = new System.Drawing.Point(12, 28);
            this.textEditCreatedOn.Name = "textEditCreatedOn";
            this.textEditCreatedOn.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.textEditCreatedOn.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.textEditCreatedOn.Properties.DisplayFormat.FormatString = "";
            this.textEditCreatedOn.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.textEditCreatedOn.Properties.EditFormat.FormatString = "";
            this.textEditCreatedOn.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.textEditCreatedOn.Properties.Mask.EditMask = "";
            this.textEditCreatedOn.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
            this.textEditCreatedOn.Properties.MinValue = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.textEditCreatedOn.Size = new System.Drawing.Size(654, 20);
            this.textEditCreatedOn.StyleController = this.layoutControlFilter;
            this.textEditCreatedOn.TabIndex = 9;
            this.textEditCreatedOn.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.textEditCreatedOn_ButtonPressed);
            // 
            // layoutControlItemUpdatedOn
            // 
            this.layoutControlItemUpdatedOn.Control = this.textEditUpdatedOn;
            this.layoutControlItemUpdatedOn.Location = new System.Drawing.Point(0, 40);
            this.layoutControlItemUpdatedOn.Name = "layoutControlItemUpdatedOn";
            this.layoutControlItemUpdatedOn.Size = new System.Drawing.Size(658, 40);
            this.layoutControlItemUpdatedOn.Text = "@@UpdatedOn";
            this.layoutControlItemUpdatedOn.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItemUpdatedOn.TextSize = new System.Drawing.Size(108, 13);
            // 
            // textEditUpdatedOn
            // 
            this.SetBoundFieldName(this.textEditUpdatedOn, "UpdatedOn");
            this.SetBoundPropertyName(this.textEditUpdatedOn, "EditValue");
            this.textEditUpdatedOn.EditValue = null;
            this.textEditUpdatedOn.Location = new System.Drawing.Point(12, 68);
            this.textEditUpdatedOn.Name = "textEditUpdatedOn";
            this.textEditUpdatedOn.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.textEditUpdatedOn.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.textEditUpdatedOn.Properties.DisplayFormat.FormatString = "";
            this.textEditUpdatedOn.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.textEditUpdatedOn.Properties.EditFormat.FormatString = "";
            this.textEditUpdatedOn.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.textEditUpdatedOn.Properties.Mask.EditMask = "";
            this.textEditUpdatedOn.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
            this.textEditUpdatedOn.Properties.MinValue = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.textEditUpdatedOn.Size = new System.Drawing.Size(654, 20);
            this.textEditUpdatedOn.StyleController = this.layoutControlFilter;
            this.textEditUpdatedOn.TabIndex = 10;
            this.textEditUpdatedOn.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.textEditUpdatedOn_ButtonClick);
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 80);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(658, 45);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // SupplierFilter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "SupplierFilter";
            this.Size = new System.Drawing.Size(678, 145);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlFilter)).EndInit();
            this.layoutControlFilter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupFilter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemCreatedOn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditCreatedOn.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditCreatedOn.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemUpdatedOn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditUpdatedOn.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditUpdatedOn.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemCreatedOn;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemUpdatedOn;
        private DevExpress.XtraEditors.DateEdit textEditCreatedOn;
        private DevExpress.XtraEditors.DateEdit textEditUpdatedOn;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
    }
}
