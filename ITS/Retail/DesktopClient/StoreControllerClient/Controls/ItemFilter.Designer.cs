namespace ITS.Retail.DesktopClient.StoreControllerClient.Controls
{
    partial class ItemFilter
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
            this.textEditItemSupplier = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlItemItemSupplier = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItemSeasonality = new DevExpress.XtraLayout.LayoutControlItem();
            this.lueSeasonality = new DevExpress.XtraEditors.LookUpEdit();
            this.layoutControlItemInsertedDateFilter = new DevExpress.XtraLayout.LayoutControlItem();
            this.textEditInsertedDateFilter = new DevExpress.XtraEditors.DateEdit();
            this.layoutControlItemUpdatedOn = new DevExpress.XtraLayout.LayoutControlItem();
            this.textEditUpdatedOn = new DevExpress.XtraEditors.DateEdit();
            this.layoutControlItemIsActive = new DevExpress.XtraLayout.LayoutControlItem();
            this.lueIsActive = new DevExpress.XtraEditors.LookUpEdit();
            this.layoutControlItemBuyer = new DevExpress.XtraLayout.LayoutControlItem();
            this.lueBuyer = new DevExpress.XtraEditors.LookUpEdit();
            this.chkDoesNotAllowDiscount = new DevExpress.XtraEditors.CheckEdit();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlFilter)).BeginInit();
            this.layoutControlFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupFilter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditItemSupplier.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemItemSupplier)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemSeasonality)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueSeasonality.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemInsertedDateFilter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditInsertedDateFilter.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditInsertedDateFilter.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemUpdatedOn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditUpdatedOn.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditUpdatedOn.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemIsActive)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueIsActive.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemBuyer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueBuyer.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkDoesNotAllowDiscount.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControlFilter
            // 
            this.layoutControlFilter.Controls.Add(this.chkDoesNotAllowDiscount);
            this.layoutControlFilter.Controls.Add(this.textEditItemSupplier);
            this.layoutControlFilter.Controls.Add(this.lueSeasonality);
            this.layoutControlFilter.Controls.Add(this.lueIsActive);
            this.layoutControlFilter.Controls.Add(this.lueBuyer);
            this.layoutControlFilter.Controls.Add(this.textEditInsertedDateFilter);
            this.layoutControlFilter.Controls.Add(this.textEditUpdatedOn);
            this.layoutControlFilter.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(432, 470, 914, 646);
            this.layoutControlFilter.Size = new System.Drawing.Size(871, 138);
            // 
            // layoutControlGroupFilter
            // 
            this.layoutControlGroupFilter.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.emptySpaceItem1,
            this.layoutControlItemItemSupplier,
            this.layoutControlItemSeasonality,
            this.layoutControlItemInsertedDateFilter,
            this.layoutControlItemUpdatedOn,
            this.layoutControlItemBuyer,
            this.layoutControlItemIsActive,
            this.emptySpaceItem2});
            this.layoutControlGroupFilter.Name = "Root";
            this.layoutControlGroupFilter.Size = new System.Drawing.Size(871, 138);
            // 
            // textEditItemSupplier
            // 
            this.SetBoundFieldName(this.textEditItemSupplier, "DefaultSupplier");
            this.SetBoundPropertyName(this.textEditItemSupplier, "EditValue");
            this.textEditItemSupplier.Location = new System.Drawing.Point(12, 30);
            this.textEditItemSupplier.Name = "textEditItemSupplier";
            this.textEditItemSupplier.Size = new System.Drawing.Size(276, 20);
            this.textEditItemSupplier.StyleController = this.layoutControlFilter;
            this.textEditItemSupplier.TabIndex = 7;
            // 
            // layoutControlItemItemSupplier
            // 
            this.layoutControlItemItemSupplier.Control = this.textEditItemSupplier;
            this.layoutControlItemItemSupplier.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItemItemSupplier.Name = "layoutControlItemItemSupplier";
            this.layoutControlItemItemSupplier.Size = new System.Drawing.Size(280, 42);
            this.layoutControlItemItemSupplier.Text = "@@ItemSupplier";
            this.layoutControlItemItemSupplier.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
            this.layoutControlItemItemSupplier.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItemItemSupplier.TextSize = new System.Drawing.Size(80, 13);
            this.layoutControlItemItemSupplier.TextToControlDistance = 5;
            // 
            // layoutControlItemSeasonality
            // 
            this.layoutControlItemSeasonality.Control = this.lueSeasonality;
            this.layoutControlItemSeasonality.Location = new System.Drawing.Point(0, 42);
            this.layoutControlItemSeasonality.Name = "layoutControlItemSeasonality";
            this.layoutControlItemSeasonality.Size = new System.Drawing.Size(280, 40);
            this.layoutControlItemSeasonality.Text = "@@Seasonality";
            this.layoutControlItemSeasonality.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItemSeasonality.TextSize = new System.Drawing.Size(108, 13);
            // 
            // lueSeasonality
            // 
            this.SetBoundFieldName(this.lueSeasonality, "Seasonality");
            this.SetBoundPropertyName(this.lueSeasonality, "EditValue");
            this.lueSeasonality.Location = new System.Drawing.Point(12, 70);
            this.lueSeasonality.Name = "lueSeasonality";
            this.lueSeasonality.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.lueSeasonality.Properties.NullText = "";
            this.lueSeasonality.Properties.PopupSizeable = false;
            this.lueSeasonality.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.lueSeasonality.Size = new System.Drawing.Size(276, 20);
            this.lueSeasonality.StyleController = this.layoutControlFilter;
            this.lueSeasonality.TabIndex = 8;
            this.lueSeasonality.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.lueSeasonality_ButtonClick);
            // 
            // layoutControlItemInsertedDateFilter
            // 
            this.layoutControlItemInsertedDateFilter.Control = this.textEditInsertedDateFilter;
            this.layoutControlItemInsertedDateFilter.Location = new System.Drawing.Point(280, 0);
            this.layoutControlItemInsertedDateFilter.Name = "layoutControlItemInsertedDateFilter";
            this.layoutControlItemInsertedDateFilter.Size = new System.Drawing.Size(281, 42);
            this.layoutControlItemInsertedDateFilter.Text = "@@InsertedDateFilter";
            this.layoutControlItemInsertedDateFilter.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItemInsertedDateFilter.TextSize = new System.Drawing.Size(108, 13);
            // 
            // textEditInsertedDateFilter
            // 
            this.SetBoundFieldName(this.textEditInsertedDateFilter, "CreatedOn");
            this.SetBoundPropertyName(this.textEditInsertedDateFilter, "EditValue");
            this.textEditInsertedDateFilter.EditValue = null;
            this.textEditInsertedDateFilter.Location = new System.Drawing.Point(292, 28);
            this.textEditInsertedDateFilter.Name = "textEditInsertedDateFilter";
            this.textEditInsertedDateFilter.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.textEditInsertedDateFilter.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.textEditInsertedDateFilter.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.textEditInsertedDateFilter.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.textEditInsertedDateFilter.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.textEditInsertedDateFilter.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.textEditInsertedDateFilter.Properties.Mask.EditMask = "";
            this.textEditInsertedDateFilter.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
            this.textEditInsertedDateFilter.Properties.MinValue = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.textEditInsertedDateFilter.Size = new System.Drawing.Size(277, 20);
            this.textEditInsertedDateFilter.StyleController = this.layoutControlFilter;
            this.textEditInsertedDateFilter.TabIndex = 11;
            this.textEditInsertedDateFilter.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.textEditInsertedDateFilter_ButtonClick);
            // 
            // layoutControlItemUpdatedOn
            // 
            this.layoutControlItemUpdatedOn.Control = this.textEditUpdatedOn;
            this.layoutControlItemUpdatedOn.Location = new System.Drawing.Point(280, 42);
            this.layoutControlItemUpdatedOn.Name = "layoutControlItemUpdatedOn";
            this.layoutControlItemUpdatedOn.Size = new System.Drawing.Size(281, 40);
            this.layoutControlItemUpdatedOn.Text = "@@UpdatedOn";
            this.layoutControlItemUpdatedOn.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItemUpdatedOn.TextSize = new System.Drawing.Size(108, 13);
            // 
            // textEditUpdatedOn
            // 
            this.SetBoundFieldName(this.textEditUpdatedOn, "UpdatedOn");
            this.SetBoundPropertyName(this.textEditUpdatedOn, "EditValue");
            this.textEditUpdatedOn.EditValue = null;
            this.textEditUpdatedOn.Location = new System.Drawing.Point(292, 70);
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
            this.textEditUpdatedOn.Size = new System.Drawing.Size(277, 20);
            this.textEditUpdatedOn.StyleController = this.layoutControlFilter;
            this.textEditUpdatedOn.TabIndex = 12;
            this.textEditUpdatedOn.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.textEditUpdatedOn_ButtonClick);
            // 
            // layoutControlItemIsActive
            // 
            this.layoutControlItemIsActive.Control = this.lueIsActive;
            this.layoutControlItemIsActive.Location = new System.Drawing.Point(561, 0);
            this.layoutControlItemIsActive.Name = "layoutControlItemIsActive";
            this.layoutControlItemIsActive.Size = new System.Drawing.Size(280, 42);
            this.layoutControlItemIsActive.Text = "@@IsActive";
            this.layoutControlItemIsActive.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItemIsActive.TextSize = new System.Drawing.Size(108, 13);
            // 
            // lueIsActive
            // 
            this.SetBoundFieldName(this.lueIsActive, "IsActive");
            this.SetBoundPropertyName(this.lueIsActive, "EditValue");
            this.lueIsActive.Location = new System.Drawing.Point(573, 28);
            this.lueIsActive.Name = "lueIsActive";
            this.lueIsActive.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.lueIsActive.Properties.NullText = "";
            this.lueIsActive.Properties.PopupSizeable = false;
            this.lueIsActive.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.lueIsActive.Size = new System.Drawing.Size(276, 20);
            this.lueIsActive.StyleController = this.layoutControlFilter;
            this.lueIsActive.TabIndex = 13;
            // 
            // layoutControlItemBuyer
            // 
            this.layoutControlItemBuyer.Control = this.lueBuyer;
            this.layoutControlItemBuyer.Location = new System.Drawing.Point(561, 42);
            this.layoutControlItemBuyer.Name = "layoutControlItemBuyer";
            this.layoutControlItemBuyer.Size = new System.Drawing.Size(280, 40);
            this.layoutControlItemBuyer.Text = "@@Buyer";
            this.layoutControlItemBuyer.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItemBuyer.TextSize = new System.Drawing.Size(108, 13);
            // 
            // lueBuyer
            // 
            this.SetBoundFieldName(this.lueBuyer, "Buyer");
            this.SetBoundPropertyName(this.lueBuyer, "EditValue");
            this.lueBuyer.Location = new System.Drawing.Point(573, 70);
            this.lueBuyer.Name = "lueBuyer";
            this.lueBuyer.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.lueBuyer.Properties.NullText = "";
            this.lueBuyer.Properties.PopupSizeable = false;
            this.lueBuyer.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.lueBuyer.Size = new System.Drawing.Size(276, 20);
            this.lueBuyer.StyleController = this.layoutControlFilter;
            this.lueBuyer.TabIndex = 14;
            this.lueBuyer.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.lueBuyer_ButtonClick);
            // 
            // chkDoesNotAllowDiscount
            // 
            this.SetBoundFieldName(this.chkDoesNotAllowDiscount, "DoesNotAllowDiscount");
            this.SetBoundPropertyName(this.chkDoesNotAllowDiscount, "EditValue");
            this.chkDoesNotAllowDiscount.Location = new System.Drawing.Point(12, 94);
            this.chkDoesNotAllowDiscount.Name = "chkDoesNotAllowDiscount";
            this.chkDoesNotAllowDiscount.Properties.AllowGrayed = true;
            this.chkDoesNotAllowDiscount.Properties.Caption = "@@DoesNotAllowDiscount";
            this.chkDoesNotAllowDiscount.Size = new System.Drawing.Size(837, 19);
            this.chkDoesNotAllowDiscount.StyleController = this.layoutControlFilter;
            this.chkDoesNotAllowDiscount.TabIndex = 15;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.chkDoesNotAllowDiscount;
            this.layoutControlItem1.CustomizationFormText = "@@DoesNotAllowDiscount";
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 82);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(841, 23);
            this.layoutControlItem1.Text = "@@DoesNotAllowDiscount";
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 105);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(841, 13);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(841, 0);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(10, 118);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // ItemFilter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "ItemFilter";
            this.Size = new System.Drawing.Size(871, 138);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlFilter)).EndInit();
            this.layoutControlFilter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupFilter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditItemSupplier.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemItemSupplier)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemSeasonality)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueSeasonality.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemInsertedDateFilter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditInsertedDateFilter.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditInsertedDateFilter.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemUpdatedOn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditUpdatedOn.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditUpdatedOn.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemIsActive)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueIsActive.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemBuyer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lueBuyer.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkDoesNotAllowDiscount.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraEditors.TextEdit textEditItemSupplier;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemItemSupplier;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemSeasonality;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemInsertedDateFilter;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemUpdatedOn;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemIsActive;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemBuyer;
        private DevExpress.XtraEditors.LookUpEdit lueSeasonality;
        private DevExpress.XtraEditors.LookUpEdit lueIsActive;
        private DevExpress.XtraEditors.LookUpEdit lueBuyer;
        private DevExpress.XtraEditors.DateEdit textEditInsertedDateFilter;
        private DevExpress.XtraEditors.DateEdit textEditUpdatedOn;
        private DevExpress.XtraEditors.CheckEdit chkDoesNotAllowDiscount;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
    }
}
