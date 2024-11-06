namespace ITS.POS.Client.UserControls
{
    partial class ucAddWeightedItemButton
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
            this.SuspendLayout();
            // 
            // Button
            // 
            this._button.Click += new System.EventHandler(this.Button_Click);
            // 
            // AddItemButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._button);
            this.Name = "AddItemButton";
            this.Size = new System.Drawing.Size(112, 48);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ucAddItemCustom_MouseClick);
            this.ResumeLayout(false);

        }

        #endregion







    }
}
