using ITS.Retail.ResourcesLib;

namespace ITS.Retail.DesktopClient.StoreControllerClient.Controls
{
    partial class LabelItemChangeFilter
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
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            this.textEditFromCode = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlItemFromCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.textEditToCode = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlItemToCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.textEditDescription = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlItemDescription = new DevExpress.XtraLayout.LayoutControlItem();
            this.checkEditWithValueChange = new DevExpress.XtraEditors.CheckEdit();
            this.layoutControlItemWithValueChange = new DevExpress.XtraLayout.LayoutControlItem();
            this.checkEditWithValueFilter = new DevExpress.XtraEditors.CheckEdit();
            this.lcITimeValue = new DevExpress.XtraLayout.LayoutControlItem();
            this.textEditBarcode = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlItemBarcode = new DevExpress.XtraLayout.LayoutControlItem();
            this.lcIItemCategory = new DevExpress.XtraLayout.LayoutControlItem();
            this.TreeListItemCategory = new DevExpress.XtraEditors.TreeListLookUpEdit();
            this.TreeListItemCategoryRepository = new DevExpress.XtraTreeList.TreeList();
            this.TreeListColCode = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.TreeListColDescription = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.checkEditWithLeaflet = new DevExpress.XtraEditors.CheckEdit();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlFilter)).BeginInit();
            this.layoutControlFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupFilter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditFromCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemFromCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditToCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemToCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditDescription.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemDescription)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditWithValueChange.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemWithValueChange)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditWithValueFilter.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcITimeValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditBarcode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemBarcode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcIItemCategory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TreeListItemCategory.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TreeListItemCategoryRepository)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditWithLeaflet.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControlFilter
            // 
            this.layoutControlFilter.Controls.Add(this.checkEditWithLeaflet);
            this.layoutControlFilter.Controls.Add(this.checkEditWithValueFilter);
            this.layoutControlFilter.Controls.Add(this.checkEditWithValueChange);
            this.layoutControlFilter.Controls.Add(this.textEditDescription);
            this.layoutControlFilter.Controls.Add(this.textEditToCode);
            this.layoutControlFilter.Controls.Add(this.textEditFromCode);
            this.layoutControlFilter.Controls.Add(this.textEditBarcode);
            this.layoutControlFilter.Controls.Add(this.TreeListItemCategory);
            this.layoutControlFilter.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(764, 199, 752, 607);
            this.layoutControlFilter.Size = new System.Drawing.Size(645, 105);
            // 
            // layoutControlGroupFilter
            // 
            this.layoutControlGroupFilter.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItemDescription,
            this.layoutControlItemFromCode,
            this.layoutControlItemToCode,
            this.layoutControlItemBarcode,
            this.lcIItemCategory,
            this.emptySpaceItem1,
            this.layoutControlItem1,
            this.lcITimeValue,
            this.layoutControlItemWithValueChange});
            this.layoutControlGroupFilter.Name = "Root";
            this.layoutControlGroupFilter.Size = new System.Drawing.Size(645, 105);
            // 
            // textEditFromCode
            // 
            this.SetBoundFieldName(this.textEditFromCode, "FromCode");
            this.SetBoundPropertyName(this.textEditFromCode, "EditValue");
            this.textEditFromCode.Location = new System.Drawing.Point(12, 70);
            this.textEditFromCode.Name = "textEditFromCode";
            this.textEditFromCode.Size = new System.Drawing.Size(102, 20);
            this.textEditFromCode.StyleController = this.layoutControlFilter;
            this.textEditFromCode.TabIndex = 10;
            // 
            // layoutControlItemFromCode
            // 
            this.layoutControlItemFromCode.Control = this.textEditFromCode;
            this.layoutControlItemFromCode.Location = new System.Drawing.Point(0, 42);
            this.layoutControlItemFromCode.Name = "layoutControlItemFromCode";
            this.layoutControlItemFromCode.Size = new System.Drawing.Size(106, 43);
            this.layoutControlItemFromCode.Text = "@@FromCode";
            this.layoutControlItemFromCode.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItemFromCode.TextSize = new System.Drawing.Size(73, 13);
            // 
            // textEditToCode
            // 
            this.SetBoundFieldName(this.textEditToCode, "ToCode");
            this.SetBoundPropertyName(this.textEditToCode, "EditValue");
            this.textEditToCode.Location = new System.Drawing.Point(118, 70);
            this.textEditToCode.Name = "textEditToCode";
            this.textEditToCode.Size = new System.Drawing.Size(87, 20);
            this.textEditToCode.StyleController = this.layoutControlFilter;
            this.textEditToCode.TabIndex = 11;
            // 
            // layoutControlItemToCode
            // 
            this.layoutControlItemToCode.Control = this.textEditToCode;
            this.layoutControlItemToCode.Location = new System.Drawing.Point(106, 42);
            this.layoutControlItemToCode.Name = "layoutControlItemToCode";
            this.layoutControlItemToCode.Size = new System.Drawing.Size(91, 43);
            this.layoutControlItemToCode.Text = "@@ToCode";
            this.layoutControlItemToCode.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItemToCode.TextSize = new System.Drawing.Size(73, 13);
            // 
            // textEditDescription
            // 
            this.SetBoundFieldName(this.textEditDescription, "Description");
            this.SetBoundPropertyName(this.textEditDescription, "EditValue");
            this.textEditDescription.Location = new System.Drawing.Point(209, 70);
            this.textEditDescription.Name = "textEditDescription";
            this.textEditDescription.Size = new System.Drawing.Size(88, 20);
            this.textEditDescription.StyleController = this.layoutControlFilter;
            this.textEditDescription.TabIndex = 12;
            // 
            // layoutControlItemDescription
            // 
            this.layoutControlItemDescription.Control = this.textEditDescription;
            this.layoutControlItemDescription.Location = new System.Drawing.Point(197, 42);
            this.layoutControlItemDescription.Name = "layoutControlItemDescription";
            this.layoutControlItemDescription.Size = new System.Drawing.Size(92, 43);
            this.layoutControlItemDescription.Text = "@@Description";
            this.layoutControlItemDescription.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItemDescription.TextSize = new System.Drawing.Size(73, 13);
            // 
            // checkEditWithValueChange
            // 
            this.SetBoundFieldName(this.checkEditWithValueChange, "WithValueChangeOnly");
            this.SetBoundPropertyName(this.checkEditWithValueChange, "EditValue");
            this.checkEditWithValueChange.EditValue = true;
            this.checkEditWithValueChange.Location = new System.Drawing.Point(412, 58);
            this.checkEditWithValueChange.Name = "checkEditWithValueChange";
            this.checkEditWithValueChange.Properties.Caption = "@@WithValueChange";
            this.checkEditWithValueChange.Size = new System.Drawing.Size(221, 19);
            this.checkEditWithValueChange.StyleController = this.layoutControlFilter;
            this.checkEditWithValueChange.TabIndex = 13;
            // 
            // layoutControlItemWithValueChange
            // 
            this.layoutControlItemWithValueChange.Control = this.checkEditWithValueChange;
            this.layoutControlItemWithValueChange.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.layoutControlItemWithValueChange.ImageAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.layoutControlItemWithValueChange.Location = new System.Drawing.Point(400, 46);
            this.layoutControlItemWithValueChange.Name = "layoutControlItemWithValueChange";
            this.layoutControlItemWithValueChange.Size = new System.Drawing.Size(225, 23);
            this.layoutControlItemWithValueChange.TextLocation = DevExpress.Utils.Locations.Bottom;
            this.layoutControlItemWithValueChange.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItemWithValueChange.TextVisible = false;
            this.layoutControlItemWithValueChange.TrimClientAreaToControl = false;
            // 
            // checkEditWithValueFilter
            // 
            this.SetBoundFieldName(this.checkEditWithValueFilter, "WithTimeValueFilter");
            this.SetBoundPropertyName(this.checkEditWithValueFilter, "EditValue");
            this.checkEditWithValueFilter.EditValue = true;
            this.checkEditWithValueFilter.Location = new System.Drawing.Point(412, 12);
            this.checkEditWithValueFilter.Name = "checkEditWithValueFilter";
            this.checkEditWithValueFilter.Properties.Caption = "@@TimeValue";
            this.checkEditWithValueFilter.Size = new System.Drawing.Size(221, 19);
            this.checkEditWithValueFilter.StyleController = this.layoutControlFilter;
            this.checkEditWithValueFilter.TabIndex = 18;
            // 
            // lcITimeValue
            // 
            this.lcITimeValue.Control = this.checkEditWithValueFilter;
            this.lcITimeValue.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.lcITimeValue.Location = new System.Drawing.Point(400, 0);
            this.lcITimeValue.Name = "lcITimeValue";
            this.lcITimeValue.Size = new System.Drawing.Size(225, 23);
            this.lcITimeValue.TextLocation = DevExpress.Utils.Locations.Bottom;
            this.lcITimeValue.TextSize = new System.Drawing.Size(0, 0);
            this.lcITimeValue.TextVisible = false;
            this.lcITimeValue.TrimClientAreaToControl = false;
            // 
            // textEditBarcode
            // 
            this.SetBoundFieldName(this.textEditBarcode, "Barcode");
            this.SetBoundPropertyName(this.textEditBarcode, "EditValue");
            this.textEditBarcode.Location = new System.Drawing.Point(301, 70);
            this.textEditBarcode.Name = "textEditBarcode";
            this.textEditBarcode.Size = new System.Drawing.Size(107, 20);
            this.textEditBarcode.StyleController = this.layoutControlFilter;
            this.textEditBarcode.TabIndex = 10;
            // 
            // layoutControlItemBarcode
            // 
            this.layoutControlItemBarcode.Control = this.textEditBarcode;
            this.layoutControlItemBarcode.CustomizationFormText = "@@Barcode";
            this.layoutControlItemBarcode.Location = new System.Drawing.Point(289, 42);
            this.layoutControlItemBarcode.Name = "layoutControlItemBarcode";
            this.layoutControlItemBarcode.Size = new System.Drawing.Size(111, 43);
            this.layoutControlItemBarcode.Text = "@@Barcode";
            this.layoutControlItemBarcode.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItemBarcode.TextSize = new System.Drawing.Size(73, 13);
            // 
            // lcIItemCategory
            // 
            this.lcIItemCategory.Control = this.TreeListItemCategory;
            this.lcIItemCategory.Location = new System.Drawing.Point(0, 0);
            this.lcIItemCategory.Name = "lcIItemCategory";
            this.lcIItemCategory.Size = new System.Drawing.Size(400, 42);
            this.lcIItemCategory.Text = "@@Category";
            this.lcIItemCategory.TextLocation = DevExpress.Utils.Locations.Top;
            this.lcIItemCategory.TextSize = new System.Drawing.Size(73, 13);
            // 
            // TreeListItemCategory
            // 
            this.SetBoundFieldName(this.TreeListItemCategory, "ItemCategory");
            this.SetBoundPropertyName(this.TreeListItemCategory, "EditValue");
            this.TreeListItemCategory.EditValue = "";
            this.TreeListItemCategory.Location = new System.Drawing.Point(12, 28);
            this.TreeListItemCategory.Name = "TreeListItemCategory";
            this.TreeListItemCategory.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.Default, global::ITS.Retail.DesktopClient.StoreControllerClient.Properties.Resources.Delete_Sign_16, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, "", null, null, true)});
            this.TreeListItemCategory.Properties.DisplayMember = "Description";
            this.TreeListItemCategory.Properties.NullText = "";
            this.TreeListItemCategory.Properties.TreeList = this.TreeListItemCategoryRepository;
            this.TreeListItemCategory.Properties.ValueMember = "Oid";
            this.TreeListItemCategory.Size = new System.Drawing.Size(396, 22);
            this.TreeListItemCategory.StyleController = this.layoutControlFilter;
            this.TreeListItemCategory.TabIndex = 19;
            this.TreeListItemCategory.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.TreeListItemCategory_ButtonClick);
            // 
            // TreeListItemCategoryRepository
            // 
            this.TreeListItemCategoryRepository.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.TreeListColCode,
            this.TreeListColDescription});
            this.TreeListItemCategoryRepository.KeyFieldName = "Oid";
            this.TreeListItemCategoryRepository.Location = new System.Drawing.Point(0, 0);
            this.TreeListItemCategoryRepository.Name = "TreeListItemCategoryRepository";
            this.TreeListItemCategoryRepository.OptionsBehavior.EnableFiltering = true;
            this.TreeListItemCategoryRepository.OptionsView.ShowIndentAsRowStyle = true;
            this.TreeListItemCategoryRepository.ParentFieldName = "ParentOid";
            this.TreeListItemCategoryRepository.Size = new System.Drawing.Size(400, 200);
            this.TreeListItemCategoryRepository.TabIndex = 0;
            // 
            // TreeListColCode
            // 
            this.TreeListColCode.Caption = "@@Code";
            this.TreeListColCode.FieldName = "Code";
            this.TreeListColCode.Name = "TreeListColCode";
            this.TreeListColCode.Visible = true;
            this.TreeListColCode.VisibleIndex = 0;
            // 
            // TreeListColDescription
            // 
            this.TreeListColDescription.Caption = "@@Description";
            this.TreeListColDescription.FieldName = "Description";
            this.TreeListColDescription.Name = "TreeListColDescription";
            this.TreeListColDescription.Visible = true;
            this.TreeListColDescription.VisibleIndex = 1;
            // 
            // checkEditWithLeaflet
            // 
            this.SetBoundFieldName(this.checkEditWithLeaflet, "");
            this.SetBoundPropertyName(this.checkEditWithLeaflet, "EditValue");
            this.checkEditWithLeaflet.EditValue = true;
            this.checkEditWithLeaflet.Location = new System.Drawing.Point(412, 35);
            this.checkEditWithLeaflet.Name = "checkEditWithLeaflet";
            this.checkEditWithLeaflet.Properties.Caption = "@@Leaflets";
            this.checkEditWithLeaflet.Size = new System.Drawing.Size(221, 19);
            this.checkEditWithLeaflet.StyleController = this.layoutControlFilter;
            this.checkEditWithLeaflet.TabIndex = 20;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.checkEditWithLeaflet;
            this.layoutControlItem1.Location = new System.Drawing.Point(400, 23);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(225, 23);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(400, 69);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(225, 16);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // LabelItemChangeFilter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Name = "LabelItemChangeFilter";
            this.Size = new System.Drawing.Size(645, 105);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlFilter)).EndInit();
            this.layoutControlFilter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupFilter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditFromCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemFromCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditToCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemToCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditDescription.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemDescription)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditWithValueChange.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemWithValueChange)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditWithValueFilter.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcITimeValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditBarcode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemBarcode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lcIItemCategory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TreeListItemCategory.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TreeListItemCategoryRepository)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditWithLeaflet.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraEditors.TextEdit textEditFromCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemFromCode;
        private DevExpress.XtraEditors.TextEdit textEditDescription;
        private DevExpress.XtraEditors.TextEdit textEditToCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemToCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemDescription;
        private DevExpress.XtraEditors.CheckEdit checkEditWithValueChange;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemWithValueChange;
        private DevExpress.XtraEditors.CheckEdit checkEditWithValueFilter;
        private DevExpress.XtraLayout.LayoutControlItem lcITimeValue;
        private DevExpress.XtraEditors.TextEdit textEditBarcode;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemBarcode;
        private DevExpress.XtraLayout.LayoutControlItem lcIItemCategory;
        private DevExpress.XtraEditors.TreeListLookUpEdit TreeListItemCategory;
        private DevExpress.XtraTreeList.TreeList TreeListItemCategoryRepository;
        private DevExpress.XtraTreeList.Columns.TreeListColumn TreeListColCode;
        private DevExpress.XtraTreeList.Columns.TreeListColumn TreeListColDescription;
        private DevExpress.XtraEditors.CheckEdit checkEditWithLeaflet;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
    }
}
