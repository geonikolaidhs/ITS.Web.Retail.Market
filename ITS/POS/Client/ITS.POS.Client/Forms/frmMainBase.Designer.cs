using ITS.Retail.Platform.Enumerations;
namespace ITS.POS.Client.Forms
{
    partial class frmMainBase
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMainBase));
            this.ucInput = new ITS.POS.Client.UserControls.ucPOSInput();
            this.SuspendLayout();
            // 
            // ucInput
            // 
            this.ucInput.Location = new System.Drawing.Point(12, 12);
            this.ucInput.LookAndFeel.SkinName = "Metropolis";
            this.ucInput.LookAndFeel.UseDefaultLookAndFeel = false;
            this.ucInput.Name = "ucInput";
            this.ucInput.PoleDisplayName = null;
            this.ucInput.SelectedQty = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ucInput.Size = new System.Drawing.Size(254, 38);
            this.ucInput.TabIndex = 15;
            this.ucInput.waitSpecialCommand = false;
            // 
            // frmMainBase
            // 
            this.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(236)))), ((int)(((byte)(239)))));
            this.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(58)))), ((int)(((byte)(61)))));
            this.Appearance.Options.UseBackColor = true;
            this.Appearance.Options.UseForeColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.ControlBox = false;
            this.Controls.Add(this.ucInput);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.LookAndFeel.SkinName = "Metropolis";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "frmMainBase";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "POS Client";
            this.Activated += new System.EventHandler(this.frmMainBase_Activated);
            this.Shown += new System.EventHandler(this.frmMainBase_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmMainBase_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        
        protected UserControls.ucPOSInput ucInput;
        

    }
}