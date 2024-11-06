namespace ITS.POS.Client.Forms
{
    partial class frmTouchPad
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTouchPad));
            this.ucTouchPad = new ITS.POS.Client.UserControls.ucTouchPad();
            this.SuspendLayout();
            // 
            // ucTouchPad
            // 
            this.ucTouchPad.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucTouchPad.Location = new System.Drawing.Point(0, 0);
            this.ucTouchPad.LookAndFeel.SkinName = "Metropolis";
            this.ucTouchPad.LookAndFeel.UseDefaultLookAndFeel = false;
            this.ucTouchPad.Name = "ucTouchPad";
            this.ucTouchPad.Size = new System.Drawing.Size(258, 228);
            this.ucTouchPad.TabIndex = 0;
            this.ucTouchPad.TouchFriendlyInput = null;
            // 
            // frmTouchPad
            // 
            this.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(34)))), ((int)(((byte)(34)))));
            this.Appearance.Options.UseForeColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(258, 228);
            this.ControlBox = false;
            this.Controls.Add(this.ucTouchPad);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.LookAndFeel.SkinName = "Metropolis";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "frmTouchPad";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Touch Pad";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        public UserControls.ucTouchPad ucTouchPad;


    }
}