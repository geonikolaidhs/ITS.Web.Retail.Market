using System.Windows.Forms;
namespace ITS.POS.Client.UserControls
{
    partial class ucButton
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
            this._button = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Button
            // 
            this._button.Dock = System.Windows.Forms.DockStyle.Fill;
            this._button.Location = new System.Drawing.Point(0, 0);
            this._button.Name = "Button";
            this._button.Size = new System.Drawing.Size(141, 40);
            this._button.TabIndex = 0;
            this._button.Text = "Button Text";
            // 
            // ucitsButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._button);
            this.Name = "ucitsButton";
            this.Size = new System.Drawing.Size(141, 40);
            this.ResumeLayout(false);

        }

        #endregion

        protected Button _button;

    }
}
