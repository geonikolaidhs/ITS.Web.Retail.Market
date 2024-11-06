namespace ITS.Service.Guard
{
    partial class ServiceGuard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServiceGuard));
            this.LabelServiceName = new System.Windows.Forms.Label();
            this.LabelLastOperation = new System.Windows.Forms.Label();
            this.LabelOperation = new System.Windows.Forms.Label();
            this.ServiceStatusLabel = new System.Windows.Forms.Label();
            this.noty = new System.Windows.Forms.NotifyIcon();
            this.BtnMinimize = new System.Windows.Forms.Button();
            this.BtnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LabelServiceName
            // 
            this.LabelServiceName.AccessibleDescription = "LabelServiceName";
            this.LabelServiceName.AccessibleName = "LabelServiceName";
            this.LabelServiceName.AutoSize = true;
            this.LabelServiceName.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.LabelServiceName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.LabelServiceName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.LabelServiceName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.LabelServiceName.ForeColor = System.Drawing.SystemColors.Info;
            this.LabelServiceName.Location = new System.Drawing.Point(24, 34);
            this.LabelServiceName.MinimumSize = new System.Drawing.Size(500, 0);
            this.LabelServiceName.Name = "LabelServiceName";
            this.LabelServiceName.Size = new System.Drawing.Size(500, 19);
            this.LabelServiceName.TabIndex = 5;
            this.LabelServiceName.Text = "Service Name : ";
            // 
            // LabelLastOperation
            // 
            this.LabelLastOperation.AccessibleDescription = "LabelLastOperation";
            this.LabelLastOperation.AccessibleName = "LabelLastOperation";
            this.LabelLastOperation.AutoSize = true;
            this.LabelLastOperation.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.LabelLastOperation.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.LabelLastOperation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.LabelLastOperation.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.LabelLastOperation.ForeColor = System.Drawing.SystemColors.Info;
            this.LabelLastOperation.Location = new System.Drawing.Point(21, 156);
            this.LabelLastOperation.MinimumSize = new System.Drawing.Size(500, 0);
            this.LabelLastOperation.Name = "LabelLastOperation";
            this.LabelLastOperation.Size = new System.Drawing.Size(500, 19);
            this.LabelLastOperation.TabIndex = 10;
            this.LabelLastOperation.Text = "Last Operation : ";
            // 
            // LabelOperation
            // 
            this.LabelOperation.AccessibleDescription = "LabelOperation";
            this.LabelOperation.AccessibleName = "LabelOperation";
            this.LabelOperation.AutoSize = true;
            this.LabelOperation.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.LabelOperation.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.LabelOperation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.LabelOperation.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.LabelOperation.ForeColor = System.Drawing.SystemColors.Info;
            this.LabelOperation.Location = new System.Drawing.Point(24, 116);
            this.LabelOperation.MinimumSize = new System.Drawing.Size(500, 0);
            this.LabelOperation.Name = "LabelOperation";
            this.LabelOperation.Size = new System.Drawing.Size(500, 19);
            this.LabelOperation.TabIndex = 9;
            this.LabelOperation.Text = "Operation In Proggress :  None";
            // 
            // ServiceStatusLabel
            // 
            this.ServiceStatusLabel.AccessibleDescription = "ServiceStatusLabel";
            this.ServiceStatusLabel.AccessibleName = "ServiceStatusLabel";
            this.ServiceStatusLabel.AutoSize = true;
            this.ServiceStatusLabel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ServiceStatusLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.ServiceStatusLabel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ServiceStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.ServiceStatusLabel.ForeColor = System.Drawing.SystemColors.Info;
            this.ServiceStatusLabel.Location = new System.Drawing.Point(24, 79);
            this.ServiceStatusLabel.MinimumSize = new System.Drawing.Size(500, 0);
            this.ServiceStatusLabel.Name = "ServiceStatusLabel";
            this.ServiceStatusLabel.Size = new System.Drawing.Size(500, 19);
            this.ServiceStatusLabel.TabIndex = 8;
            this.ServiceStatusLabel.Text = "Service Status : ";
            // 
            // noty
            // 
            this.noty.Icon = ((System.Drawing.Icon)(resources.GetObject("noty.Icon")));
            this.noty.Text = "Service Guard";
            this.noty.Visible = true;
            // 
            // BtnMinimize
            // 
            this.BtnMinimize.AccessibleDescription = "BtnMinimize";
            this.BtnMinimize.AccessibleName = "BtnMinimize";
            this.BtnMinimize.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.BtnMinimize.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Bold);
            this.BtnMinimize.ForeColor = System.Drawing.SystemColors.Info;
            this.BtnMinimize.Location = new System.Drawing.Point(408, 234);
            this.BtnMinimize.Name = "BtnMinimize";
            this.BtnMinimize.Size = new System.Drawing.Size(116, 40);
            this.BtnMinimize.TabIndex = 11;
            this.BtnMinimize.Text = "Minimize";
            this.BtnMinimize.UseVisualStyleBackColor = false;
            this.BtnMinimize.Click += new System.EventHandler(this.BtnMinimize_Click);
            // 
            // BtnClose
            // 
            this.BtnClose.AccessibleDescription = "BtnClose";
            this.BtnClose.AccessibleName = "BtnClose";
            this.BtnClose.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.BtnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Bold);
            this.BtnClose.ForeColor = System.Drawing.SystemColors.Info;
            this.BtnClose.Location = new System.Drawing.Point(21, 234);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(116, 40);
            this.BtnClose.TabIndex = 12;
            this.BtnClose.Text = "Close";
            this.BtnClose.UseVisualStyleBackColor = false;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // ServiceGuard
            // 
            this.AccessibleDescription = "ServiceGuard";
            this.AccessibleName = "ServiceGuard";
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(609, 286);
            this.ControlBox = false;
            this.Controls.Add(this.BtnClose);
            this.Controls.Add(this.BtnMinimize);
            this.Controls.Add(this.LabelLastOperation);
            this.Controls.Add(this.LabelOperation);
            this.Controls.Add(this.ServiceStatusLabel);
            this.Controls.Add(this.LabelServiceName);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ServiceGuard";
            this.Text = "Service Guard";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ServiceGuard_FormClosing);
            this.Shown += new System.EventHandler(this.ServiceGuard_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LabelServiceName;
        private System.Windows.Forms.Label LabelLastOperation;
        private System.Windows.Forms.Label LabelOperation;
        private System.Windows.Forms.Label ServiceStatusLabel;
        private System.Windows.Forms.NotifyIcon noty;
        private System.Windows.Forms.Button BtnMinimize;
        private System.Windows.Forms.Button BtnClose;
    }
}

