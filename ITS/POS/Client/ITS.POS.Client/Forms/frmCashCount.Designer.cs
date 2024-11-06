namespace ITS.POS.Client.Forms
{
    partial class frmCashCount
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
            this.nvfMain = new DevExpress.XtraBars.Navigation.NavigationFrame();
            this.nvpMain = new DevExpress.XtraBars.Navigation.NavigationPage();
            this.splitContainerControl3 = new DevExpress.XtraEditors.SplitContainerControl();
            this.tileControl1 = new DevExpress.XtraEditors.TileControl();
            this.grpMesh = new DevExpress.XtraEditors.TileGroup();
            this.btnOk = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.nvpCash = new DevExpress.XtraBars.Navigation.NavigationPage();
            this.cinMain = new ITS.POS.Client.UserControls.ucCashCounterCash();
            this.nvpNonMoneyInput = new DevExpress.XtraBars.Navigation.NavigationPage();
            this.nmiMain = new ITS.POS.Client.UserControls.ucCashNonMoneyInput();
            this.dataGridViewImageColumn1 = new System.Windows.Forms.DataGridViewImageColumn();
            this.uscMain = new DevExpress.XtraEditors.XtraUserControl();
            this.btnOk2 = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel2 = new DevExpress.XtraEditors.SimpleButton();
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.tileBar1 = new DevExpress.XtraBars.Navigation.TileBar();
            this.grpAkri = new DevExpress.XtraBars.Navigation.TileBarGroup();
            this.splitContainerControl2 = new DevExpress.XtraEditors.SplitContainerControl();
            this.btnnOK = new DevExpress.XtraEditors.SimpleButton();
            this.btnnCancel = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.nvfMain)).BeginInit();
            this.nvfMain.SuspendLayout();
            this.nvpMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl3)).BeginInit();
            this.splitContainerControl3.SuspendLayout();
            this.nvpCash.SuspendLayout();
            this.nvpNonMoneyInput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            this.splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl2)).BeginInit();
            this.splitContainerControl2.SuspendLayout();
            this.SuspendLayout();
            // 
            // nvfMain
            // 
            this.nvfMain.Controls.Add(this.nvpMain);
            this.nvfMain.Controls.Add(this.nvpCash);
            this.nvfMain.Controls.Add(this.nvpNonMoneyInput);
            this.nvfMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nvfMain.Location = new System.Drawing.Point(0, 0);
            this.nvfMain.Name = "nvfMain";
            this.nvfMain.Pages.AddRange(new DevExpress.XtraBars.Navigation.NavigationPageBase[] {
            this.nvpMain,
            this.nvpCash,
            this.nvpNonMoneyInput});
            this.nvfMain.SelectedPage = this.nvpCash;
            this.nvfMain.SelectedPageIndex = 0;
            this.nvfMain.Size = new System.Drawing.Size(800, 704);
            this.nvfMain.TabIndex = 1;
            this.nvfMain.Text = "navigationFrame1";
            this.nvfMain.TransitionAnimationProperties.FrameInterval = 10;
            this.nvfMain.TransitionType = DevExpress.Utils.Animation.Transitions.Cover;
            // 
            // nvpMain
            // 
            this.nvpMain.Controls.Add(this.splitContainerControl3);
            this.nvpMain.Name = "nvpMain";
            this.nvpMain.Size = new System.Drawing.Size(800, 704);
            // 
            // splitContainerControl3
            // 
            this.splitContainerControl3.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.splitContainerControl3.Appearance.Options.UseBackColor = true;
            this.splitContainerControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerControl3.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.Panel2;
            this.splitContainerControl3.Horizontal = false;
            this.splitContainerControl3.IsSplitterFixed = true;
            this.splitContainerControl3.Location = new System.Drawing.Point(0, 0);
            this.splitContainerControl3.Name = "splitContainerControl3";
            this.splitContainerControl3.Panel1.Controls.Add(this.tileControl1);
            this.splitContainerControl3.Panel1.Text = "Panel1";
            this.splitContainerControl3.Panel2.Controls.Add(this.btnOk);
            this.splitContainerControl3.Panel2.Controls.Add(this.btnCancel);
            this.splitContainerControl3.Panel2.Text = "Panel2";
            this.splitContainerControl3.Size = new System.Drawing.Size(800, 704);
            this.splitContainerControl3.SplitterPosition = 50;
            this.splitContainerControl3.TabIndex = 86;
            this.splitContainerControl3.Text = "splitContainerControl3";
            // 
            // tileControl1
            // 
            this.tileControl1.AppearanceItem.Normal.BackColor = System.Drawing.Color.DodgerBlue;
            this.tileControl1.AppearanceItem.Normal.Options.UseBackColor = true;
            this.tileControl1.AppearanceItem.Normal.Options.UseTextOptions = true;
            this.tileControl1.AppearanceItem.Normal.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Bottom;
            this.tileControl1.BackColor = System.Drawing.Color.Transparent;
            this.tileControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tileControl1.DragSize = new System.Drawing.Size(0, 0);
            this.tileControl1.Groups.Add(this.grpMesh);
            this.tileControl1.ItemImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleRight;
            this.tileControl1.ItemSize = 60;
            this.tileControl1.Location = new System.Drawing.Point(0, 0);
            this.tileControl1.MaxId = 37;
            this.tileControl1.Name = "tileControl1";
            this.tileControl1.Size = new System.Drawing.Size(800, 642);
            this.tileControl1.TabIndex = 85;
            this.tileControl1.Text = "tileControl1";
            // 
            // grpMesh
            // 
            this.grpMesh.Name = "grpMesh";
            // 
            // btnOk
            // 
            this.btnOk.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnOk.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnOk.Appearance.Options.UseBackColor = true;
            this.btnOk.Appearance.Options.UseBorderColor = true;
            this.btnOk.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnOk.Image = global::ITS.POS.Client.Properties.Resources.correct_64;
            this.btnOk.Location = new System.Drawing.Point(612, 0);
            this.btnOk.LookAndFeel.SkinName = "Metropolis";
            this.btnOk.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(188, 50);
            this.btnOk.TabIndex = 84;
            this.btnOk.Text = "οκ";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnCancel.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnCancel.Appearance.Options.UseBackColor = true;
            this.btnCancel.Appearance.Options.UseBorderColor = true;
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnCancel.Image = global::ITS.POS.Client.Properties.Resources.Ribbon_Exit_32x32;
            this.btnCancel.Location = new System.Drawing.Point(0, 0);
            this.btnCancel.LookAndFeel.SkinName = "Metropolis";
            this.btnCancel.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(188, 50);
            this.btnCancel.TabIndex = 78;
            this.btnCancel.Text = "Ακύρωση";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // nvpCash
            // 
            this.nvpCash.Caption = "nvpCash";
            this.nvpCash.Controls.Add(this.cinMain);
            this.nvpCash.Name = "nvpCash";
            this.nvpCash.Size = new System.Drawing.Size(800, 704);
            // 
            // cinMain
            // 
            this.cinMain.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(197)))), ((int)(((byte)(231)))));
            this.cinMain.Appearance.Font = new System.Drawing.Font("Tahoma", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.cinMain.Appearance.Options.UseBackColor = true;
            this.cinMain.Appearance.Options.UseFont = true;
            this.cinMain.AutoSize = true;
            this.cinMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cinMain.IsNowVisible = false;
            this.cinMain.Location = new System.Drawing.Point(0, 0);
            this.cinMain.Name = "cinMain";
            this.cinMain.Size = new System.Drawing.Size(800, 704);
            this.cinMain.TabIndex = 0;
            this.cinMain.Return += new System.EventHandler(this.uc_Return);
            // 
            // nvpNonMoneyInput
            // 
            this.nvpNonMoneyInput.Caption = "nvpNonMoneyInput";
            this.nvpNonMoneyInput.Controls.Add(this.nmiMain);
            this.nvpNonMoneyInput.Name = "nvpNonMoneyInput";
            this.nvpNonMoneyInput.Size = new System.Drawing.Size(800, 704);
            // 
            // nmiMain
            // 
            this.nmiMain.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(197)))), ((int)(((byte)(231)))));
            this.nmiMain.Appearance.Font = new System.Drawing.Font("Tahoma", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.nmiMain.Appearance.Options.UseBackColor = true;
            this.nmiMain.Appearance.Options.UseFont = true;
            this.nmiMain.AutoSize = true;
            this.nmiMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nmiMain.IsNowVisible = false;
            this.nmiMain.Location = new System.Drawing.Point(0, 0);
            this.nmiMain.Name = "nmiMain";
            this.nmiMain.Size = new System.Drawing.Size(800, 704);
            this.nmiMain.TabIndex = 0;
            this.nmiMain.Return += new System.EventHandler(this.uc_Return);
            // 
            // dataGridViewImageColumn1
            // 
            this.dataGridViewImageColumn1.HeaderText = "Column3";
            this.dataGridViewImageColumn1.Image = global::ITS.POS.Client.Properties.Resources.minus_32;
            this.dataGridViewImageColumn1.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom;
            this.dataGridViewImageColumn1.Name = "dataGridViewImageColumn1";
            // 
            // uscMain
            // 
            this.uscMain.Location = new System.Drawing.Point(0, 0);
            this.uscMain.Name = "uscMain";
            this.uscMain.Size = new System.Drawing.Size(1024, 768);
            this.uscMain.TabIndex = 92;
            // 
            // btnOk2
            // 
            this.btnOk2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk2.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnOk2.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnOk2.Appearance.Options.UseBackColor = true;
            this.btnOk2.Appearance.Options.UseBorderColor = true;
            this.btnOk2.Image = global::ITS.POS.Client.Properties.Resources.correct_64;
            this.btnOk2.Location = new System.Drawing.Point(812, 686);
            this.btnOk2.LookAndFeel.SkinName = "Metropolis";
            this.btnOk2.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnOk2.Name = "btnOk2";
            this.btnOk2.Size = new System.Drawing.Size(200, 60);
            this.btnOk2.TabIndex = 93;
            this.btnOk2.Text = "Ακύρωση";
            this.btnOk2.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel2
            // 
            this.btnCancel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel2.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnCancel2.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnCancel2.Appearance.Options.UseBackColor = true;
            this.btnCancel2.Appearance.Options.UseBorderColor = true;
            this.btnCancel2.Image = global::ITS.POS.Client.Properties.Resources.Ribbon_Exit_32x32;
            this.btnCancel2.Location = new System.Drawing.Point(12, 686);
            this.btnCancel2.LookAndFeel.SkinName = "Metropolis";
            this.btnCancel2.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnCancel2.Name = "btnCancel2";
            this.btnCancel2.Size = new System.Drawing.Size(194, 60);
            this.btnCancel2.TabIndex = 86;
            this.btnCancel2.Text = "Ακύρωση";
            this.btnCancel2.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // splitContainerControl1
            // 
            this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerControl1.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.Panel2;
            this.splitContainerControl1.IsSplitterFixed = true;
            this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
            this.splitContainerControl1.Name = "splitContainerControl1";
            this.splitContainerControl1.Panel1.Controls.Add(this.tileBar1);
            this.splitContainerControl1.Panel1.Text = "Panel1";
            this.splitContainerControl1.Panel2.Controls.Add(this.splitContainerControl2);
            this.splitContainerControl1.Panel2.MinSize = 800;
            this.splitContainerControl1.Panel2.Text = "Panel2";
            this.splitContainerControl1.Size = new System.Drawing.Size(1024, 768);
            this.splitContainerControl1.SplitterPosition = 800;
            this.splitContainerControl1.TabIndex = 94;
            this.splitContainerControl1.Text = "splitContainerControl1";
            // 
            // tileBar1
            // 
            this.tileBar1.AllowDrag = false;
            this.tileBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tileBar1.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
            this.tileBar1.Groups.Add(this.grpAkri);
            this.tileBar1.Location = new System.Drawing.Point(0, 0);
            this.tileBar1.MaxId = 20;
            this.tileBar1.Name = "tileBar1";
            this.tileBar1.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.tileBar1.ScrollMode = DevExpress.XtraEditors.TileControlScrollMode.ScrollButtons;
            this.tileBar1.Size = new System.Drawing.Size(212, 768);
            this.tileBar1.TabIndex = 86;
            this.tileBar1.Text = "tileBar1";
            // 
            // grpAkri
            // 
            this.grpAkri.Name = "grpAkri";
            // 
            // splitContainerControl2
            // 
            this.splitContainerControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerControl2.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.Panel2;
            this.splitContainerControl2.Horizontal = false;
            this.splitContainerControl2.IsSplitterFixed = true;
            this.splitContainerControl2.Location = new System.Drawing.Point(0, 0);
            this.splitContainerControl2.Name = "splitContainerControl2";
            this.splitContainerControl2.Panel1.Controls.Add(this.nvfMain);
            this.splitContainerControl2.Panel1.MinSize = 600;
            this.splitContainerControl2.Panel1.Text = "Panel1";
            this.splitContainerControl2.Panel2.Controls.Add(this.btnnOK);
            this.splitContainerControl2.Panel2.Controls.Add(this.btnnCancel);
            this.splitContainerControl2.Panel2.Padding = new System.Windows.Forms.Padding(0, 0, 2, 2);
            this.splitContainerControl2.Panel2.Text = "Panel2";
            this.splitContainerControl2.Size = new System.Drawing.Size(800, 768);
            this.splitContainerControl2.SplitterPosition = 52;
            this.splitContainerControl2.TabIndex = 2;
            this.splitContainerControl2.Text = "splitContainerControl2";
            // 
            // btnnOK
            // 
            this.btnnOK.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnnOK.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnnOK.Appearance.Options.UseBackColor = true;
            this.btnnOK.Appearance.Options.UseBorderColor = true;
            this.btnnOK.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnnOK.Image = global::ITS.POS.Client.Properties.Resources.correct_64;
            this.btnnOK.Location = new System.Drawing.Point(612, 0);
            this.btnnOK.LookAndFeel.SkinName = "Metropolis";
            this.btnnOK.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnnOK.Name = "btnnOK";
            this.btnnOK.Size = new System.Drawing.Size(188, 50);
            this.btnnOK.TabIndex = 86;
            this.btnnOK.Text = "οκ";
            this.btnnOK.Click += new System.EventHandler(this.btnnOK_Click);
            // 
            // btnnCancel
            // 
            this.btnnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.btnnCancel.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnnCancel.Appearance.Options.UseBackColor = true;
            this.btnnCancel.Appearance.Options.UseBorderColor = true;
            this.btnnCancel.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnnCancel.Image = global::ITS.POS.Client.Properties.Resources.Ribbon_Exit_32x32;
            this.btnnCancel.Location = new System.Drawing.Point(0, 0);
            this.btnnCancel.LookAndFeel.SkinName = "Metropolis";
            this.btnnCancel.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnnCancel.Name = "btnnCancel";
            this.btnnCancel.Size = new System.Drawing.Size(188, 50);
            this.btnnCancel.TabIndex = 85;
            this.btnnCancel.Text = "Ακύρωση";
            this.btnnCancel.Click += new System.EventHandler(this.btnnCancel_Click);
            // 
            // frmCashCount
            // 
            this.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(164)))), ((int)(((byte)(197)))), ((int)(((byte)(231)))));
            this.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(58)))), ((int)(((byte)(61)))));
            this.Appearance.Options.UseBackColor = true;
            this.Appearance.Options.UseForeColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1024, 768);
            this.Controls.Add(this.splitContainerControl1);
            this.Controls.Add(this.btnCancel2);
            this.Controls.Add(this.btnOk2);
            this.Controls.Add(this.uscMain);
            this.LookAndFeel.SkinName = "Metropolis";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "frmCashCount";
            this.Text = "frmCancelNotIncludedItems";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmCashCount_KeyDownz);
            ((System.ComponentModel.ISupportInitialize)(this.nvfMain)).EndInit();
            this.nvfMain.ResumeLayout(false);
            this.nvpMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl3)).EndInit();
            this.splitContainerControl3.ResumeLayout(false);
            this.nvpCash.ResumeLayout(false);
            this.nvpCash.PerformLayout();
            this.nvpNonMoneyInput.ResumeLayout(false);
            this.nvpNonMoneyInput.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl2)).EndInit();
            this.splitContainerControl2.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        #endregion

        public DevExpress.XtraBars.Navigation.NavigationFrame nvfMain;
        public DevExpress.XtraBars.Navigation.NavigationPage nvpMain;
        public DevExpress.XtraBars.Navigation.NavigationPage nvpCash;
        public System.Windows.Forms.DataGridViewImageColumn dataGridViewImageColumn1;
        public UserControls.ucCashCounterCash cinMain;
        public DevExpress.XtraEditors.XtraUserControl uscMain;
        public DevExpress.XtraEditors.SimpleButton btnOk2;
        public DevExpress.XtraEditors.SimpleButton btnCancel2;
        public DevExpress.XtraBars.Navigation.NavigationPage nvpNonMoneyInput;
        public UserControls.ucCashNonMoneyInput nmiMain;
        public DevExpress.XtraEditors.SimpleButton btnCancel;
        public DevExpress.XtraEditors.SimpleButton btnOk;
        public DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        public DevExpress.XtraBars.Navigation.TileBar tileBar1;
        public DevExpress.XtraBars.Navigation.TileBarGroup grpAkri;
        public DevExpress.XtraEditors.SplitContainerControl splitContainerControl2;
        public DevExpress.XtraEditors.SplitContainerControl splitContainerControl3;
        public DevExpress.XtraEditors.TileControl tileControl1;
        public DevExpress.XtraEditors.TileGroup grpMesh;
        public DevExpress.XtraEditors.SimpleButton btnnOK;
        public DevExpress.XtraEditors.SimpleButton btnnCancel;
    }
}