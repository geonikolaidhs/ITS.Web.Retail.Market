namespace ITS.Retail.DesktopClient.StoreControllerClient.Controls
{
    partial class CustomerFilter
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
            this.textEditCardID = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlItemCardID = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItemNewCustomersFrom = new DevExpress.XtraLayout.LayoutControlItem();
            this.textEditNewCustomersFrom = new DevExpress.XtraEditors.DateEdit();
            this.layoutControlItemUpdatedOn = new DevExpress.XtraLayout.LayoutControlItem();
            this.textEditUpdatedOn = new DevExpress.XtraEditors.DateEdit();
            this.layoutControlItemIsActive = new DevExpress.XtraLayout.LayoutControlItem();
            this.lueIsActive = new DevExpress.XtraEditors.LookUpEdit();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlFilter)).BeginInit();
            this.layoutControlFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupFilter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditCardID.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemCardID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemNewCustomersFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditNewCustomersFrom.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditNewCustomersFrom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemUpdatedOn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditUpdatedOn.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditUpdatedOn.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemIsActive)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueIsActive.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControlFilter
            // 
            this.layoutControlFilter.Controls.Add(this.textEditCardID);
            this.layoutControlFilter.Controls.Add(this.textEditUpdatedOn);
            this.layoutControlFilter.Controls.Add(this.lueIsActive);
            this.layoutControlFilter.Controls.Add(this.textEditNewCustomersFrom);
            this.layoutControlFilter.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(640, 390, 868, 350);
            this.layoutControlFilter.Size = new System.Drawing.Size(1024, 140);
            // 
            // layoutControlGroupFilter
            // 
            this.layoutControlGroupFilter.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItemIsActive,
            this.layoutControlItemCardID,
            this.layoutControlItemNewCustomersFrom,
            this.layoutControlItemUpdatedOn,
            this.emptySpaceItem1});
            this.layoutControlGroupFilter.Name = "Root";
            this.layoutControlGroupFilter.Size = new System.Drawing.Size(1024, 140);
            // 
            // textEditCardID
            // 
            this.SetBoundFieldName(this.textEditCardID, "CardID");
            this.SetBoundPropertyName(this.textEditCardID, "EditValue");
            this.textEditCardID.Location = new System.Drawing.Point(12, 28);
            this.textEditCardID.Name = "textEditCardID";
            this.textEditCardID.Size = new System.Drawing.Size(366, 20);
            this.textEditCardID.StyleController = this.layoutControlFilter;
            this.textEditCardID.TabIndex = 7;
            // 
            // layoutControlItemCardID
            // 
            this.layoutControlItemCardID.Control = this.textEditCardID;
            this.layoutControlItemCardID.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItemCardID.Name = "layoutControlItemCardID";
            this.layoutControlItemCardID.Size = new System.Drawing.Size(370, 40);
            this.layoutControlItemCardID.Text = "@@CardID";
            this.layoutControlItemCardID.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItemCardID.TextSize = new System.Drawing.Size(116, 13);
            // 
            // layoutControlItemNewCustomersFrom
            // 
            this.layoutControlItemNewCustomersFrom.Control = this.textEditNewCustomersFrom;
            this.layoutControlItemNewCustomersFrom.Location = new System.Drawing.Point(370, 0);
            this.layoutControlItemNewCustomersFrom.Name = "layoutControlItemNewCustomersFrom";
            this.layoutControlItemNewCustomersFrom.Size = new System.Drawing.Size(634, 40);
            this.layoutControlItemNewCustomersFrom.Text = "@@NewCustomersFrom";
            this.layoutControlItemNewCustomersFrom.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItemNewCustomersFrom.TextSize = new System.Drawing.Size(116, 13);
            // 
            // textEditNewCustomersFrom
            // 
            this.SetBoundFieldName(this.textEditNewCustomersFrom, "CreatedOn");
            this.SetBoundPropertyName(this.textEditNewCustomersFrom, "EditValue");
            this.textEditNewCustomersFrom.EditValue = null;
            this.textEditNewCustomersFrom.Location = new System.Drawing.Point(382, 28);
            this.textEditNewCustomersFrom.Name = "textEditNewCustomersFrom";
            this.textEditNewCustomersFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.textEditNewCustomersFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.textEditNewCustomersFrom.Properties.DisplayFormat.FormatString = "";
            this.textEditNewCustomersFrom.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.textEditNewCustomersFrom.Properties.EditFormat.FormatString = "";
            this.textEditNewCustomersFrom.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.textEditNewCustomersFrom.Properties.Mask.EditMask = "";
            this.textEditNewCustomersFrom.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
            this.textEditNewCustomersFrom.Properties.MinValue = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.textEditNewCustomersFrom.Size = new System.Drawing.Size(630, 20);
            this.textEditNewCustomersFrom.StyleController = this.layoutControlFilter;
            this.textEditNewCustomersFrom.TabIndex = 10;
            this.textEditNewCustomersFrom.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.textEditNewCustomersFrom_ButtonClick);
            // 
            // layoutControlItemUpdatedOn
            // 
            this.layoutControlItemUpdatedOn.Control = this.textEditUpdatedOn;
            this.layoutControlItemUpdatedOn.Location = new System.Drawing.Point(0, 40);
            this.layoutControlItemUpdatedOn.Name = "layoutControlItemUpdatedOn";
            this.layoutControlItemUpdatedOn.Size = new System.Drawing.Size(370, 40);
            this.layoutControlItemUpdatedOn.Text = "@@UpdatedOn";
            this.layoutControlItemUpdatedOn.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItemUpdatedOn.TextSize = new System.Drawing.Size(116, 13);
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
            this.textEditUpdatedOn.Size = new System.Drawing.Size(366, 20);
            this.textEditUpdatedOn.StyleController = this.layoutControlFilter;
            this.textEditUpdatedOn.TabIndex = 11;
            this.textEditUpdatedOn.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.textEditUpdatedOn_ButtonClick);
            // 
            // layoutControlItemIsActive
            // 
            this.layoutControlItemIsActive.Control = this.lueIsActive;
            this.layoutControlItemIsActive.Location = new System.Drawing.Point(370, 40);
            this.layoutControlItemIsActive.Name = "layoutControlItemIsActive";
            this.layoutControlItemIsActive.Size = new System.Drawing.Size(634, 40);
            this.layoutControlItemIsActive.Text = "@@IsActive";
            this.layoutControlItemIsActive.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItemIsActive.TextSize = new System.Drawing.Size(116, 13);
            // 
            // lueIsActive
            // 
            this.SetBoundFieldName(this.lueIsActive, "IsActive");
            this.SetBoundPropertyName(this.lueIsActive, "EditValue");
            this.lueIsActive.Location = new System.Drawing.Point(382, 68);
            this.lueIsActive.Name = "lueIsActive";
            this.lueIsActive.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lueIsActive.Properties.NullText = "";
            this.lueIsActive.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.lueIsActive.Size = new System.Drawing.Size(630, 20);
            this.lueIsActive.StyleController = this.layoutControlFilter;
            this.lueIsActive.TabIndex = 12;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 80);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(1004, 40);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // CustomerFilter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "CustomerFilter";
            this.Size = new System.Drawing.Size(1024, 140);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlFilter)).EndInit();
            this.layoutControlFilter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupFilter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditCardID.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemCardID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemNewCustomersFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditNewCustomersFrom.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditNewCustomersFrom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemUpdatedOn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditUpdatedOn.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditUpdatedOn.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemIsActive)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueIsActive.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraEditors.TextEdit textEditCardID;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemCardID;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemNewCustomersFrom;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemUpdatedOn;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemIsActive;
        private DevExpress.XtraEditors.DateEdit textEditUpdatedOn;
        private DevExpress.XtraEditors.LookUpEdit lueIsActive;
        private DevExpress.XtraEditors.DateEdit textEditNewCustomersFrom;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
    }
}
