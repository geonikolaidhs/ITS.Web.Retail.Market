﻿namespace ITS.OfficeManager.Loader
{
    partial class SettingsForm
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
            this.btnInitialise = new System.Windows.Forms.Button();
            this.txtbxServerUrl = new System.Windows.Forms.TextBox();
            this.lblStoreControllerURL = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnInitialise
            // 
            this.btnInitialise.Location = new System.Drawing.Point(205, 64);
            this.btnInitialise.Name = "btnInitialise";
            this.btnInitialise.Size = new System.Drawing.Size(129, 26);
            this.btnInitialise.TabIndex = 10;
            this.btnInitialise.Text = "Initilialise";
            this.btnInitialise.UseVisualStyleBackColor = true;
            this.btnInitialise.Click += new System.EventHandler(this.btnInitialise_Click);
            // 
            // txtbxServerUrl
            // 
            this.txtbxServerUrl.Location = new System.Drawing.Point(89, 30);
            this.txtbxServerUrl.Name = "txtbxServerUrl";
            this.txtbxServerUrl.Size = new System.Drawing.Size(245, 20);
            this.txtbxServerUrl.TabIndex = 8;
            this.txtbxServerUrl.Text = "http://";
            // 
            // lblStoreControllerURL
            // 
            this.lblStoreControllerURL.AutoSize = true;
            this.lblStoreControllerURL.Location = new System.Drawing.Point(17, 33);
            this.lblStoreControllerURL.Name = "lblStoreControllerURL";
            this.lblStoreControllerURL.Size = new System.Drawing.Size(66, 13);
            this.lblStoreControllerURL.TabIndex = 6;
            this.lblStoreControllerURL.Text = "Server URL:";
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(355, 136);
            this.ControlBox = false;
            this.Controls.Add(this.btnInitialise);
            this.Controls.Add(this.txtbxServerUrl);
            this.Controls.Add(this.lblStoreControllerURL);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnInitialise;
        private System.Windows.Forms.TextBox txtbxServerUrl;
        private System.Windows.Forms.Label lblStoreControllerURL;
    }
}

//this.ClientSize = new System.Drawing.Size(374, 122);