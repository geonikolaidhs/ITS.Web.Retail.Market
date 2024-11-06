namespace ITS.MobileAtStore
{
    partial class PriceAtStoreEnterCodeControl
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
            this.upperLogo = new System.Windows.Forms.PictureBox();
            this.bottomLogo = new System.Windows.Forms.PictureBox();
            this.SuspendLayout();
            // 
            // upperLogo
            // 
            this.upperLogo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.upperLogo.Location = new System.Drawing.Point(0, 0);
            this.upperLogo.Name = "upperLogo";
            this.upperLogo.Size = new System.Drawing.Size(320, 75);
            // 
            // bottomLogo
            // 
            this.bottomLogo.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.bottomLogo.Location = new System.Drawing.Point(111, 267);
            this.bottomLogo.Name = "bottomLogo";
            this.bottomLogo.Size = new System.Drawing.Size(118, 26);
            // 
            // PriceAtStoreEnterCodeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.bottomLogo);
            this.Controls.Add(this.upperLogo);
            this.Name = "PriceAtStoreEnterCodeControl";
            this.Size = new System.Drawing.Size(320, 293);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.PriceAtStoreEnterCodeControl_KeyPress);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox upperLogo;
        private System.Windows.Forms.PictureBox bottomLogo;
    }
}
