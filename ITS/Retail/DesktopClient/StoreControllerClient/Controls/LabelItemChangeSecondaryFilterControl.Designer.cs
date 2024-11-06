namespace ITS.Retail.DesktopClient.StoreControllerClient.Controls
{
    partial class LabelItemChangeSecondaryFilterControl
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
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.textEditFromDate = new DevExpress.XtraEditors.DateEdit();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.textEditToDate = new DevExpress.XtraEditors.DateEdit();
            this.textEditFromDateTime = new DevExpress.XtraEditors.TimeEdit();
            this.layoutControlItemFromDateTime = new DevExpress.XtraLayout.LayoutControlItem();
            this.textEditToDateTime = new DevExpress.XtraEditors.TimeEdit();
            this.layoutControlItemToDateTime = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditFromDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditFromDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditToDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditToDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditFromDateTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemFromDateTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditToDateTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemToDateTime)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.textEditToDate);
            this.layoutControl1.Controls.Add(this.textEditFromDate);
            this.layoutControl1.Controls.Add(this.textEditFromDateTime);
            this.layoutControl1.Controls.Add(this.textEditToDateTime);
            this.layoutControl1.Size = new System.Drawing.Size(640, 54);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItemToDateTime,
            this.layoutControlItem2,
            this.layoutControlItemFromDateTime,
            this.layoutControlItem3});
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Size = new System.Drawing.Size(640, 54);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.textEditFromDate;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(155, 34);
            this.layoutControlItem2.Text = "@@FromDate";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(67, 13);
            // 
            // textEditFromDate
            // 
            this.SetBoundFieldName(this.textEditFromDate, "FromDate");
            this.SetBoundPropertyName(this.textEditFromDate, "EditValue");
            this.textEditFromDate.EditValue = null;
            this.textEditFromDate.Location = new System.Drawing.Point(83, 12);
            this.textEditFromDate.Name = "textEditFromDate";
            this.textEditFromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.textEditFromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.textEditFromDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.textEditFromDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.textEditFromDate.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.textEditFromDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.textEditFromDate.Properties.Mask.EditMask = "";
            this.textEditFromDate.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
            this.textEditFromDate.Size = new System.Drawing.Size(80, 20);
            this.textEditFromDate.StyleController = this.layoutControl1;
            this.textEditFromDate.TabIndex = 6;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.textEditToDate;
            this.layoutControlItem3.Location = new System.Drawing.Point(310, 0);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(155, 34);
            this.layoutControlItem3.Text = "@@ToDate";
            this.layoutControlItem3.TextSize = new System.Drawing.Size(67, 13);
            // 
            // textEditToDate
            // 
            this.SetBoundFieldName(this.textEditToDate, "ToDate");
            this.SetBoundPropertyName(this.textEditToDate, "EditValue");
            this.textEditToDate.EditValue = null;
            this.textEditToDate.Location = new System.Drawing.Point(393, 12);
            this.textEditToDate.Name = "textEditToDate";
            this.textEditToDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.textEditToDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.textEditToDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.textEditToDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.textEditToDate.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.textEditToDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.textEditToDate.Properties.Mask.EditMask = "";
            this.textEditToDate.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
            this.textEditToDate.Size = new System.Drawing.Size(80, 20);
            this.textEditToDate.StyleController = this.layoutControl1;
            this.textEditToDate.TabIndex = 7;
            // 
            // textEditFromDateTime
            // 
            this.SetBoundFieldName(this.textEditFromDateTime, "FromDateTime");
            this.SetBoundPropertyName(this.textEditFromDateTime, "EditValue");
            this.textEditFromDateTime.EditValue = null;
            this.textEditFromDateTime.Location = new System.Drawing.Point(238, 12);
            this.textEditFromDateTime.Name = "textEditFromDateTime";
            this.textEditFromDateTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.textEditFromDateTime.Properties.DisplayFormat.FormatString = "HH:mm";
            this.textEditFromDateTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.textEditFromDateTime.Properties.EditFormat.FormatString = "HH:mm";
            this.textEditFromDateTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.textEditFromDateTime.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
            this.textEditFromDateTime.Properties.Mask.EditMask = "HH:mm";
            this.textEditFromDateTime.Size = new System.Drawing.Size(80, 20);
            this.textEditFromDateTime.StyleController = this.layoutControl1;
            this.textEditFromDateTime.TabIndex = 8;
            // 
            // layoutControlItemFromDateTime
            // 
            this.layoutControlItemFromDateTime.Control = this.textEditFromDateTime;
            this.layoutControlItemFromDateTime.CustomizationFormText = "@@From";
            this.layoutControlItemFromDateTime.Location = new System.Drawing.Point(155, 0);
            this.layoutControlItemFromDateTime.Name = "layoutControlItemFromDateTime";
            this.layoutControlItemFromDateTime.Size = new System.Drawing.Size(155, 34);
            this.layoutControlItemFromDateTime.Text = "@@From";
            this.layoutControlItemFromDateTime.TextLocation = DevExpress.Utils.Locations.Left;
            this.layoutControlItemFromDateTime.TextSize = new System.Drawing.Size(67, 13);
            // 
            // textEditToDateTime
            // 
            this.SetBoundFieldName(this.textEditToDateTime, "ToDateTime");
            this.SetBoundPropertyName(this.textEditToDateTime, "EditValue");
            this.textEditToDateTime.EditValue = null;
            this.textEditToDateTime.Location = new System.Drawing.Point(548, 12);
            this.textEditToDateTime.Name = "textEditToDateTime";
            this.textEditToDateTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.textEditToDateTime.Properties.DisplayFormat.FormatString = "HH:mm";
            this.textEditToDateTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.textEditToDateTime.Properties.EditFormat.FormatString = "HH:mm";
            this.textEditToDateTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.textEditToDateTime.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
            this.textEditToDateTime.Properties.Mask.EditMask = "HH:mm";
            this.textEditToDateTime.Size = new System.Drawing.Size(80, 20);
            this.textEditToDateTime.StyleController = this.layoutControl1;
            this.textEditToDateTime.TabIndex = 9;
            // 
            // layoutControlItemToDateTime
            // 
            this.layoutControlItemToDateTime.Control = this.textEditToDateTime;
            this.layoutControlItemToDateTime.CustomizationFormText = "@@To";
            this.layoutControlItemToDateTime.Location = new System.Drawing.Point(465, 0);
            this.layoutControlItemToDateTime.Name = "layoutControlItemToDateTime";
            this.layoutControlItemToDateTime.Size = new System.Drawing.Size(155, 34);
            this.layoutControlItemToDateTime.Text = "@@To";
            this.layoutControlItemToDateTime.TextLocation = DevExpress.Utils.Locations.Left;
            this.layoutControlItemToDateTime.TextSize = new System.Drawing.Size(67, 13);
            // 
            // LabelFilterSecondaryFilterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "LabelFilterSecondaryFilterControl";
            this.Size = new System.Drawing.Size(770, 54);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditFromDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditFromDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditToDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditToDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditFromDateTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemFromDateTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEditToDateTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemToDateTime)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraEditors.DateEdit textEditToDate;
        private DevExpress.XtraEditors.DateEdit textEditFromDate;
        private DevExpress.XtraEditors.TimeEdit textEditFromDateTime;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemFromDateTime;
        private DevExpress.XtraEditors.TimeEdit textEditToDateTime;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItemToDateTime;
    }
}
