namespace ITS.POS.Client.UserControls
{
    partial class ucSimpleDocumentDetailGrid
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
            this.grdDocumentDetails = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.grdDocumentDetails)).BeginInit();
            this.SuspendLayout();
            // 
            // grdDocumentDetails
            // 
            this.grdDocumentDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdDocumentDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdDocumentDetails.Location = new System.Drawing.Point(0, 0);
            this.grdDocumentDetails.MultiSelect = false;
            this.grdDocumentDetails.Name = "grdDocumentDetails";
            this.grdDocumentDetails.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdDocumentDetails.Size = new System.Drawing.Size(536, 318);
            this.grdDocumentDetails.TabIndex = 0;
            // 
            // SimpleDocumentDetailGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grdDocumentDetails);
            this.Name = "SimpleDocumentDetailGrid";
            this.Size = new System.Drawing.Size(536, 318);
            ((System.ComponentModel.ISupportInitialize)(this.grdDocumentDetails)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView grdDocumentDetails;




    }
}
