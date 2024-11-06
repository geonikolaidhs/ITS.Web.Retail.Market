using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Windows.Forms;
namespace ITS.POS.Client.Forms
{
    partial class frmFindItemByDesc
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFindItemByDesc));
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.lblTitle = new DevExpress.XtraEditors.LabelControl();
            this.grdFoundItems = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.blItems = new System.Windows.Forms.BindingSource(this.components);
            this.edtItemDescription = new ITS.POS.Client.UserControls.ucTouchFriendlyInput();
            this.edtPrice = new DevExpress.XtraEditors.TextEdit();
            this.lblPrice = new DevExpress.XtraEditors.LabelControl();
            this.baseActionButton10 = new ITS.POS.Client.UserControls.ucActionButton();
            this.baseActionButton8 = new ITS.POS.Client.UserControls.ucActionButton();
            ((System.ComponentModel.ISupportInitialize)(this.grdFoundItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.blItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtItemDescription.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtPrice.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnCancel.Appearance.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnCancel.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.btnCancel.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.btnCancel.Appearance.Options.UseBackColor = true;
            this.btnCancel.Appearance.Options.UseBorderColor = true;
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Image = global::ITS.POS.Client.Properties.Resources.Ribbon_Exit_32x32;
            this.btnCancel.Location = new System.Drawing.Point(605, 252);
            this.btnCancel.LookAndFeel.SkinName = "Metropolis";
            this.btnCancel.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(114, 59);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.TabStop = false;
            this.btnCancel.Text = "Ακύρωση";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnOK.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnOK.Appearance.Options.UseBackColor = true;
            this.btnOK.Appearance.Options.UseBorderColor = true;
            this.btnOK.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Image = global::ITS.POS.Client.Properties.Resources.cashbox_32;
            this.btnOK.Location = new System.Drawing.Point(605, 338);
            this.btnOK.LookAndFeel.SkinName = "Metropolis";
            this.btnOK.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(114, 61);
            this.btnOK.TabIndex = 9;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnSearch.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnSearch.Appearance.Options.UseBackColor = true;
            this.btnSearch.Appearance.Options.UseBorderColor = true;
            this.btnSearch.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
            this.btnSearch.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
            this.btnSearch.Location = new System.Drawing.Point(391, 12);
            this.btnSearch.LookAndFeel.SkinName = "Metropolis";
            this.btnSearch.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(114, 55);
            this.btnSearch.TabIndex = 10;
            this.btnSearch.Text = "Αναζήτηση";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.lblTitle.Location = new System.Drawing.Point(22, 12);
            this.lblTitle.LookAndFeel.SkinName = "Metropolis";
            this.lblTitle.LookAndFeel.UseDefaultLookAndFeel = false;
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(234, 19);
            this.lblTitle.TabIndex = 21;
            this.lblTitle.Text = "Αναζήτηση Είδους με Περιγραφή";
            // 
            // grdFoundItems
            // 
            this.grdFoundItems.EmbeddedNavigator.Appearance.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.grdFoundItems.EmbeddedNavigator.Buttons.Append.Enabled = false;
            this.grdFoundItems.EmbeddedNavigator.Buttons.Append.Visible = false;
            this.grdFoundItems.EmbeddedNavigator.Buttons.CancelEdit.Enabled = false;
            this.grdFoundItems.EmbeddedNavigator.Buttons.CancelEdit.Visible = false;
            this.grdFoundItems.EmbeddedNavigator.Buttons.Edit.Enabled = false;
            this.grdFoundItems.EmbeddedNavigator.Buttons.Edit.Visible = false;
            this.grdFoundItems.EmbeddedNavigator.Buttons.EndEdit.Enabled = false;
            this.grdFoundItems.EmbeddedNavigator.Buttons.EndEdit.Visible = false;
            this.grdFoundItems.EmbeddedNavigator.Buttons.First.Enabled = false;
            this.grdFoundItems.EmbeddedNavigator.Buttons.First.Visible = false;
            this.grdFoundItems.EmbeddedNavigator.Buttons.Last.Enabled = false;
            this.grdFoundItems.EmbeddedNavigator.Buttons.Last.Visible = false;
            this.grdFoundItems.EmbeddedNavigator.Buttons.NextPage.Enabled = false;
            this.grdFoundItems.EmbeddedNavigator.Buttons.NextPage.Visible = false;
            this.grdFoundItems.EmbeddedNavigator.Buttons.PrevPage.Enabled = false;
            this.grdFoundItems.EmbeddedNavigator.Buttons.PrevPage.Visible = false;
            this.grdFoundItems.EmbeddedNavigator.Buttons.Remove.Enabled = false;
            this.grdFoundItems.EmbeddedNavigator.Buttons.Remove.Visible = false;
            this.grdFoundItems.EmbeddedNavigator.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
            this.grdFoundItems.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.grdFoundItems.Location = new System.Drawing.Point(22, 88);
            this.grdFoundItems.LookAndFeel.SkinName = "Metropolis";
            this.grdFoundItems.LookAndFeel.UseDefaultLookAndFeel = false;
            this.grdFoundItems.MainView = this.gridView1;
            this.grdFoundItems.Name = "grdFoundItems";
            this.grdFoundItems.Size = new System.Drawing.Size(483, 311);
            this.grdFoundItems.TabIndex = 22;
            this.grdFoundItems.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            this.grdFoundItems.KeyDown += new System.Windows.Forms.KeyEventHandler(this.grdFoundItems_KeyDown);
            // 
            // gridView1
            // 
            this.gridView1.ActiveFilterEnabled = false;
            this.gridView1.Appearance.HeaderPanel.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.gridView1.Appearance.HeaderPanel.FontStyleDelta = System.Drawing.FontStyle.Bold;
            this.gridView1.Appearance.HeaderPanel.Options.UseFont = true;
            this.gridView1.Appearance.Row.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.gridView1.Appearance.Row.Options.UseFont = true;
            this.gridView1.Appearance.RowSeparator.BackColor = System.Drawing.Color.Gray;
            this.gridView1.Appearance.RowSeparator.Options.UseBackColor = true;
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3});
            this.gridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;
            this.gridView1.GridControl = this.grdFoundItems;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
            this.gridView1.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.False;
            this.gridView1.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.False;
            this.gridView1.OptionsBehavior.Editable = false;
            this.gridView1.OptionsBehavior.ReadOnly = true;
            this.gridView1.OptionsBehavior.SmartVertScrollBar = false;
            this.gridView1.OptionsCustomization.AllowColumnMoving = false;
            this.gridView1.OptionsCustomization.AllowFilter = false;
            this.gridView1.OptionsCustomization.AllowGroup = false;
            this.gridView1.OptionsCustomization.AllowQuickHideColumns = false;
            this.gridView1.OptionsCustomization.AllowSort = false;
            this.gridView1.OptionsDetail.EnableMasterViewMode = false;
            this.gridView1.OptionsFilter.AllowFilterEditor = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.True;
            this.gridView1.RowHeight = 30;
            this.gridView1.RowClick += new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.gridView1_RowClick);
            this.gridView1.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gridView1_FocusedRowChanged);
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "Barcode";
            this.gridColumn1.FieldName = "Code";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowEdit = false;
            this.gridColumn1.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn1.OptionsColumn.AllowMove = false;
            this.gridColumn1.OptionsColumn.FixedWidth = true;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "Περιγραφή";
            this.gridColumn2.FieldName = "Name";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.AllowEdit = false;
            this.gridColumn2.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn2.OptionsColumn.AllowMove = false;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "Τιμή";
            this.gridColumn3.FieldName = "Τιμή";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsColumn.AllowEdit = false;
            this.gridColumn3.OptionsColumn.AllowMove = false;
            this.gridColumn3.OptionsColumn.AllowShowHide = false;
            this.gridColumn3.UnboundType = DevExpress.Data.UnboundColumnType.Decimal;
            // 
            // edtItemDescription
            // 
            this.edtItemDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.edtItemDescription.AutoHideTouchPad = false;
            this.edtItemDescription.Location = new System.Drawing.Point(22, 41);
            this.edtItemDescription.Name = "edtItemDescription";
            this.edtItemDescription.PoleDisplayName = "";
            this.edtItemDescription.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.edtItemDescription.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.edtItemDescription.Properties.Appearance.Options.UseBackColor = true;
            this.edtItemDescription.Properties.Appearance.Options.UseFont = true;
            this.edtItemDescription.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.edtItemDescription.Properties.AppearanceFocused.Options.UseBackColor = true;
            this.edtItemDescription.Properties.Mask.EditMask = ".?{150}";
            this.edtItemDescription.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
            this.edtItemDescription.Properties.Mask.ShowPlaceHolders = false;
            this.edtItemDescription.Size = new System.Drawing.Size(343, 26);
            this.edtItemDescription.TabIndex = 0;
            this.edtItemDescription.KeyDown += new System.Windows.Forms.KeyEventHandler(this.edtItemDescription_KeyDown);
            this.edtItemDescription.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.edtItemDescription_PreviewKeyDown);
            // 
            // edtPrice
            // 
            this.edtPrice.EditValue = "";
            this.edtPrice.Location = new System.Drawing.Point(534, 117);
            this.edtPrice.Name = "edtPrice";
            this.edtPrice.Properties.AllowFocused = false;
            this.edtPrice.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold);
            this.edtPrice.Properties.Appearance.Options.UseFont = true;
            this.edtPrice.Properties.Appearance.Options.UseTextOptions = true;
            this.edtPrice.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.edtPrice.Properties.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.edtPrice.Properties.DisplayFormat.FormatString = "c2";
            this.edtPrice.Properties.EditFormat.FormatString = "c2";
            this.edtPrice.Properties.ReadOnly = true;
            this.edtPrice.Size = new System.Drawing.Size(140, 30);
            this.edtPrice.TabIndex = 74;
            this.edtPrice.TabStop = false;
            // 
            // lblPrice
            // 
            this.lblPrice.Appearance.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold);
            this.lblPrice.Location = new System.Drawing.Point(534, 88);
            this.lblPrice.LookAndFeel.SkinName = "Metropolis";
            this.lblPrice.LookAndFeel.UseDefaultLookAndFeel = false;
            this.lblPrice.Name = "lblPrice";
            this.lblPrice.Size = new System.Drawing.Size(48, 23);
            this.lblPrice.TabIndex = 73;
            this.lblPrice.Text = "Τιμή ";
            // 
            // baseActionButton10
            // 
            this.baseActionButton10.Action = ITS.Retail.Platform.Enumerations.eActions.MOVE_UP;
            this.baseActionButton10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.baseActionButton10.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.baseActionButton10.Appearance.Options.UseBackColor = true;
            // 
            // 
            // 
            this.baseActionButton10.Button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.baseActionButton10.Button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.baseActionButton10.Button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.baseActionButton10.Button.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.baseActionButton10.Button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.baseActionButton10.Button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(219)))), ((int)(((byte)(219)))));
            this.baseActionButton10.Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.baseActionButton10.Button.Image = ((System.Drawing.Image)(resources.GetObject("baseActionButton10.Button.Image")));
            this.baseActionButton10.Button.Location = new System.Drawing.Point(0, 0);
            this.baseActionButton10.Button.Name = "Button";
            this.baseActionButton10.Button.Size = new System.Drawing.Size(61, 59);
            this.baseActionButton10.Button.TabIndex = 0;
            this.baseActionButton10.Button.Text = " ";
            this.baseActionButton10.Button.UseVisualStyleBackColor = false;
            this.baseActionButton10.CheckPermissions = true;
            this.baseActionButton10.Location = new System.Drawing.Point(511, 252);
            this.baseActionButton10.LookAndFeel.SkinName = "Metropolis";
            this.baseActionButton10.LookAndFeel.UseDefaultLookAndFeel = false;
            this.baseActionButton10.Name = "baseActionButton10";
            this.baseActionButton10.Size = new System.Drawing.Size(61, 59);
            this.baseActionButton10.TabIndex = 66;
            // 
            // baseActionButton8
            // 
            this.baseActionButton8.Action = ITS.Retail.Platform.Enumerations.eActions.MOVE_DOWN;
            this.baseActionButton8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.baseActionButton8.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.baseActionButton8.Appearance.Options.UseBackColor = true;
            // 
            // 
            // 
            this.baseActionButton8.Button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.baseActionButton8.Button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.baseActionButton8.Button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.baseActionButton8.Button.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.baseActionButton8.Button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.baseActionButton8.Button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(219)))), ((int)(((byte)(219)))));
            this.baseActionButton8.Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.baseActionButton8.Button.Image = ((System.Drawing.Image)(resources.GetObject("baseActionButton8.Button.Image")));
            this.baseActionButton8.Button.Location = new System.Drawing.Point(0, 0);
            this.baseActionButton8.Button.Name = "Button";
            this.baseActionButton8.Button.Size = new System.Drawing.Size(61, 61);
            this.baseActionButton8.Button.TabIndex = 0;
            this.baseActionButton8.Button.Text = " ";
            this.baseActionButton8.Button.UseVisualStyleBackColor = false;
            this.baseActionButton8.CheckPermissions = true;
            this.baseActionButton8.Location = new System.Drawing.Point(511, 338);
            this.baseActionButton8.LookAndFeel.SkinName = "Metropolis";
            this.baseActionButton8.LookAndFeel.UseDefaultLookAndFeel = false;
            this.baseActionButton8.Name = "baseActionButton8";
            this.baseActionButton8.Size = new System.Drawing.Size(61, 61);
            this.baseActionButton8.TabIndex = 72;
            // 
            // frmFindItemByDesc
            // 
            this.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(197)))), ((int)(((byte)(231)))));
            this.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(58)))), ((int)(((byte)(61)))));
            this.Appearance.Options.UseBackColor = true;
            this.Appearance.Options.UseForeColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(742, 411);
            this.Controls.Add(this.baseActionButton10);
            this.Controls.Add(this.baseActionButton8);
            this.Controls.Add(this.lblPrice);
            this.Controls.Add(this.edtPrice);
            this.Controls.Add(this.edtItemDescription);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.grdFoundItems);
            this.LookAndFeel.SkinName = "Metropolis";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "frmFindItemByDesc";
            this.Text = "frmFindItemByDesc";
            ((System.ComponentModel.ISupportInitialize)(this.grdFoundItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.blItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtItemDescription.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.edtPrice.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void edtItemDescription_EditValueChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        public DevExpress.XtraEditors.SimpleButton btnOK;
        public DevExpress.XtraEditors.SimpleButton btnSearch;
        private DevExpress.XtraEditors.LabelControl lblTitle;
        private System.Windows.Forms.BindingSource blItems;
        private UserControls.ucTouchFriendlyInput edtItemDescription;
        private DevExpress.XtraGrid.GridControl grdFoundItems;
        private GridView gridView1;
        public DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        public DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        public DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraEditors.TextEdit edtPrice;
        private DevExpress.XtraEditors.LabelControl lblPrice;
        private UserControls.ucActionButton baseActionButton10;
        private UserControls.ucActionButton baseActionButton8;
    }
}