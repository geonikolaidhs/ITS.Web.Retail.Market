namespace ITS.POS.Hardware
{
    partial class frmPoleDisplayEmulatorOutput
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPoleDisplayEmulatorOutput));
            this.Line1TextBox = new System.Windows.Forms.RichTextBox();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.Line2TextBox = new System.Windows.Forms.RichTextBox();
            this.Line3TextBox = new System.Windows.Forms.RichTextBox();
            this.Line4TextBox = new System.Windows.Forms.RichTextBox();
            this.Line5TextBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // Line1TextBox
            // 
            this.Line1TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.Line1TextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.Line1TextBox.Location = new System.Drawing.Point(12, 8);
            this.Line1TextBox.Name = "Line1TextBox";
            this.Line1TextBox.ReadOnly = true;
            this.Line1TextBox.Size = new System.Drawing.Size(382, 26);
            this.Line1TextBox.TabIndex = 0;
            this.Line1TextBox.TabStop = false;
            this.Line1TextBox.Text = "";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.btnClose.Appearance.Options.UseFont = true;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
            this.btnClose.Location = new System.Drawing.Point(291, 146);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(101, 43);
            this.btnClose.TabIndex = 1;
            this.btnClose.TabStop = false;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // Line2TextBox
            // 
            this.Line2TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.Line2TextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.Line2TextBox.Location = new System.Drawing.Point(12, 34);
            this.Line2TextBox.Name = "Line2TextBox";
            this.Line2TextBox.ReadOnly = true;
            this.Line2TextBox.Size = new System.Drawing.Size(382, 26);
            this.Line2TextBox.TabIndex = 2;
            this.Line2TextBox.TabStop = false;
            this.Line2TextBox.Text = "";
            // 
            // Line3TextBox
            // 
            this.Line3TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.Line3TextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.Line3TextBox.Location = new System.Drawing.Point(12, 60);
            this.Line3TextBox.Name = "Line3TextBox";
            this.Line3TextBox.ReadOnly = true;
            this.Line3TextBox.Size = new System.Drawing.Size(382, 26);
            this.Line3TextBox.TabIndex = 3;
            this.Line3TextBox.TabStop = false;
            this.Line3TextBox.Text = "";
            // 
            // Line4TextBox
            // 
            this.Line4TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.Line4TextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.Line4TextBox.Location = new System.Drawing.Point(12, 86);
            this.Line4TextBox.Name = "Line4TextBox";
            this.Line4TextBox.ReadOnly = true;
            this.Line4TextBox.Size = new System.Drawing.Size(382, 26);
            this.Line4TextBox.TabIndex = 4;
            this.Line4TextBox.TabStop = false;
            this.Line4TextBox.Text = "";
            // 
            // Line5TextBox
            // 
            this.Line5TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.Line5TextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.Line5TextBox.Location = new System.Drawing.Point(12, 112);
            this.Line5TextBox.Name = "Line5TextBox";
            this.Line5TextBox.ReadOnly = true;
            this.Line5TextBox.Size = new System.Drawing.Size(382, 26);
            this.Line5TextBox.TabIndex = 5;
            this.Line5TextBox.TabStop = false;
            this.Line5TextBox.Text = "";
            // 
            // frmPoleDisplayEmulatorOutput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 193);
            this.ControlBox = false;
            this.Controls.Add(this.Line5TextBox);
            this.Controls.Add(this.Line4TextBox);
            this.Controls.Add(this.Line3TextBox);
            this.Controls.Add(this.Line2TextBox);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.Line1TextBox);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPoleDisplayEmulatorOutput";
            this.Text = "Output";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmPoleDisplayEmulatorOutput_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.RichTextBox Line1TextBox;
        public DevExpress.XtraEditors.SimpleButton btnClose;
        public System.Windows.Forms.RichTextBox Line2TextBox;
        public System.Windows.Forms.RichTextBox Line3TextBox;
        public System.Windows.Forms.RichTextBox Line4TextBox;
        public System.Windows.Forms.RichTextBox Line5TextBox;
    }
}