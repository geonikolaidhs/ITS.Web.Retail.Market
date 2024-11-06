namespace ITS.POS.Client.UserControls
{
    partial class ucLookUpEditFilterControl
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.dropDown = new DevExpress.XtraEditors.LookUpEdit();
            this.controlLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dropDown.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.tableLayoutPanel1.Controls.Add(this.dropDown, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.controlLabel, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Font = new System.Drawing.Font("Tahoma", 9.25F);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(500, 40);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // dropDown
            // 
            this.dropDown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dropDown.Location = new System.Drawing.Point(128, 3);
            this.dropDown.Name = "dropDown";
            this.dropDown.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.dropDown.Properties.Appearance.Options.UseFont = true;
            this.dropDown.Properties.AppearanceDisabled.Font = new System.Drawing.Font("Tahoma", 11F);
            this.dropDown.Properties.AppearanceDisabled.Options.UseFont = true;
            this.dropDown.Properties.AppearanceDropDown.Font = new System.Drawing.Font("Tahoma", 11F);
            this.dropDown.Properties.AppearanceDropDown.Options.UseFont = true;
            this.dropDown.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dropDown.Properties.NullText = "";
            this.dropDown.Properties.PopupSizeable = false;
            this.dropDown.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.dropDown.Size = new System.Drawing.Size(369, 26);
            this.dropDown.TabIndex = 14;
            // 
            // controlLabel
            // 
            this.controlLabel.AutoSize = true;
            this.controlLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controlLabel.Location = new System.Drawing.Point(3, 0);
            this.controlLabel.Name = "controlLabel";
            this.controlLabel.Size = new System.Drawing.Size(119, 40);
            this.controlLabel.TabIndex = 2;
            this.controlLabel.Text = "label1";
            this.controlLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ucLookUpEditFilterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ucLookUpEditFilterControl";
            this.Size = new System.Drawing.Size(500, 40);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dropDown.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label controlLabel;
        private DevExpress.XtraEditors.LookUpEdit dropDown;
    }
}
