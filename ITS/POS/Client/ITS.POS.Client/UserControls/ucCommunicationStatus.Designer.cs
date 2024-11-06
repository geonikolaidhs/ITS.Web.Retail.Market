namespace ITS.POS.Client.UserControls
{
    partial class ucCommunicationStatus
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
            this.StatusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatusColor = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblDownloading = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblDownloadingMessage = new System.Windows.Forms.ToolStripStatusLabel();
            this.pbDownloading = new System.Windows.Forms.ToolStripProgressBar();
            this.lblUploading = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblUploadingMessage = new System.Windows.Forms.ToolStripStatusLabel();
            this.pbUploading = new System.Windows.Forms.ToolStripProgressBar();
            this.StatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // StatusStrip
            // 
            this.StatusStrip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.StatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatusColor,
            this.lblDownloading,
            this.lblDownloadingMessage,
            this.pbDownloading,
            this.lblUploading,
            this.lblUploadingMessage,
            this.pbUploading});
            this.StatusStrip.Location = new System.Drawing.Point(0, 0);
            this.StatusStrip.Name = "StatusStrip";
            this.StatusStrip.Size = new System.Drawing.Size(575, 23);
            this.StatusStrip.TabIndex = 0;
            this.StatusStrip.Text = "statusStrip1";
            // 
            // lblStatusColor
            // 
            this.lblStatusColor.ActiveLinkColor = System.Drawing.Color.Red;
            this.lblStatusColor.BackgroundImage = global::ITS.POS.Client.Properties.Resources.offline_dot;
            this.lblStatusColor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.lblStatusColor.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.lblStatusColor.Name = "lblStatusColor";
            this.lblStatusColor.Size = new System.Drawing.Size(22, 18);
            this.lblStatusColor.Text = "     ";
            this.lblStatusColor.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // lblDownloading
            // 
            this.lblDownloading.AutoSize = false;
            this.lblDownloading.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.lblDownloading.Name = "lblDownloading";
            this.lblDownloading.Overflow = System.Windows.Forms.ToolStripItemOverflow.Always;
            this.lblDownloading.Size = new System.Drawing.Size(79, 18);
            this.lblDownloading.Text = "Downloading:";
            // 
            // lblDownloadingMessage
            // 
            this.lblDownloadingMessage.AutoSize = false;
            this.lblDownloadingMessage.Name = "lblDownloadingMessage";
            this.lblDownloadingMessage.Overflow = System.Windows.Forms.ToolStripItemOverflow.Always;
            this.lblDownloadingMessage.Size = new System.Drawing.Size(144, 18);
            this.lblDownloadingMessage.Spring = true;
            this.lblDownloadingMessage.Text = "Checking...";
            // 
            // pbDownloading
            // 
            this.pbDownloading.Name = "pbDownloading";
            this.pbDownloading.Overflow = System.Windows.Forms.ToolStripItemOverflow.Always;
            this.pbDownloading.Size = new System.Drawing.Size(50, 17);
            this.pbDownloading.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // lblUploading
            // 
            this.lblUploading.AutoSize = false;
            this.lblUploading.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.lblUploading.Name = "lblUploading";
            this.lblUploading.Overflow = System.Windows.Forms.ToolStripItemOverflow.Always;
            this.lblUploading.Size = new System.Drawing.Size(66, 18);
            this.lblUploading.Text = "Uploading:";
            // 
            // lblUploadingMessage
            // 
            this.lblUploadingMessage.AutoSize = false;
            this.lblUploadingMessage.Name = "lblUploadingMessage";
            this.lblUploadingMessage.Overflow = System.Windows.Forms.ToolStripItemOverflow.Always;
            this.lblUploadingMessage.Size = new System.Drawing.Size(144, 18);
            this.lblUploadingMessage.Spring = true;
            this.lblUploadingMessage.Text = "Checking...";
            // 
            // pbUploading
            // 
            this.pbUploading.Name = "pbUploading";
            this.pbUploading.Overflow = System.Windows.Forms.ToolStripItemOverflow.Always;
            this.pbUploading.Size = new System.Drawing.Size(50, 17);
            this.pbUploading.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // ucCommunicationStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.StatusStrip);
            this.Name = "ucCommunicationStatus";
            this.Size = new System.Drawing.Size(575, 23);
            this.StatusStrip.ResumeLayout(false);
            this.StatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip StatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatusColor;
        private System.Windows.Forms.ToolStripStatusLabel lblDownloading;
        private System.Windows.Forms.ToolStripStatusLabel lblUploading;
        private System.Windows.Forms.ToolStripProgressBar pbDownloading;
        private System.Windows.Forms.ToolStripProgressBar pbUploading;
        private System.Windows.Forms.ToolStripStatusLabel lblDownloadingMessage;
        private System.Windows.Forms.ToolStripStatusLabel lblUploadingMessage;
    }
}
