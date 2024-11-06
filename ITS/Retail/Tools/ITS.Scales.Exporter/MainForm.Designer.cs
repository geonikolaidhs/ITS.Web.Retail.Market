namespace ITS.Scales.Exporter
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.richTextBoxLog = new System.Windows.Forms.RichTextBox();
            this.labelLog = new System.Windows.Forms.Label();
            this.labelServiceURL = new System.Windows.Forms.Label();
            this.labelRepeatTimeInMinutes = new System.Windows.Forms.Label();
            this.textBoxServiceURL = new System.Windows.Forms.TextBox();
            this.numericUpDownRepeatTimeInMinutes = new System.Windows.Forms.NumericUpDown();
            this.buttonSaveSettings = new System.Windows.Forms.Button();
            this.buttonStartStop = new System.Windows.Forms.Button();
            this.buttonClearLog = new System.Windows.Forms.Button();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.labelWebServiceCallTimeOut = new System.Windows.Forms.Label();
            this.numericUpDownwebServiceCallTimeOut = new System.Windows.Forms.NumericUpDown();
            this.checkBoxAutoRun = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRepeatTimeInMinutes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownwebServiceCallTimeOut)).BeginInit();
            this.SuspendLayout();
            // 
            // richTextBoxLog
            // 
            this.richTextBoxLog.Location = new System.Drawing.Point(13, 143);
            this.richTextBoxLog.Name = "richTextBoxLog";
            this.richTextBoxLog.ReadOnly = true;
            this.richTextBoxLog.Size = new System.Drawing.Size(651, 274);
            this.richTextBoxLog.TabIndex = 0;
            this.richTextBoxLog.Text = "";
            // 
            // labelLog
            // 
            this.labelLog.AutoSize = true;
            this.labelLog.Location = new System.Drawing.Point(10, 127);
            this.labelLog.Name = "labelLog";
            this.labelLog.Size = new System.Drawing.Size(31, 13);
            this.labelLog.TabIndex = 1;
            this.labelLog.Text = "Log :";
            // 
            // labelServiceURL
            // 
            this.labelServiceURL.AutoSize = true;
            this.labelServiceURL.Location = new System.Drawing.Point(13, 13);
            this.labelServiceURL.Name = "labelServiceURL";
            this.labelServiceURL.Size = new System.Drawing.Size(74, 13);
            this.labelServiceURL.TabIndex = 2;
            this.labelServiceURL.Text = "Service URL :";
            // 
            // labelRepeatTimeInMinutes
            // 
            this.labelRepeatTimeInMinutes.AutoSize = true;
            this.labelRepeatTimeInMinutes.Location = new System.Drawing.Point(10, 44);
            this.labelRepeatTimeInMinutes.Name = "labelRepeatTimeInMinutes";
            this.labelRepeatTimeInMinutes.Size = new System.Drawing.Size(119, 13);
            this.labelRepeatTimeInMinutes.TabIndex = 3;
            this.labelRepeatTimeInMinutes.Text = "Repeat Time (minutes) :";
            // 
            // textBoxServiceURL
            // 
            this.textBoxServiceURL.Location = new System.Drawing.Point(94, 13);
            this.textBoxServiceURL.Name = "textBoxServiceURL";
            this.textBoxServiceURL.Size = new System.Drawing.Size(468, 20);
            this.textBoxServiceURL.TabIndex = 4;
            // 
            // numericUpDownRepeatTimeInMinutes
            // 
            this.numericUpDownRepeatTimeInMinutes.Location = new System.Drawing.Point(135, 42);
            this.numericUpDownRepeatTimeInMinutes.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDownRepeatTimeInMinutes.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownRepeatTimeInMinutes.Name = "numericUpDownRepeatTimeInMinutes";
            this.numericUpDownRepeatTimeInMinutes.Size = new System.Drawing.Size(120, 20);
            this.numericUpDownRepeatTimeInMinutes.TabIndex = 5;
            this.numericUpDownRepeatTimeInMinutes.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // buttonSaveSettings
            // 
            this.buttonSaveSettings.Location = new System.Drawing.Point(16, 86);
            this.buttonSaveSettings.Name = "buttonSaveSettings";
            this.buttonSaveSettings.Size = new System.Drawing.Size(212, 34);
            this.buttonSaveSettings.TabIndex = 6;
            this.buttonSaveSettings.Text = "Save settings";
            this.buttonSaveSettings.UseVisualStyleBackColor = true;
            this.buttonSaveSettings.Click += new System.EventHandler(this.buttonSaveSettings_Click);
            // 
            // buttonStartStop
            // 
            this.buttonStartStop.Location = new System.Drawing.Point(249, 86);
            this.buttonStartStop.Name = "buttonStartStop";
            this.buttonStartStop.Size = new System.Drawing.Size(194, 34);
            this.buttonStartStop.TabIndex = 7;
            this.buttonStartStop.Text = "Start/Stop";
            this.buttonStartStop.UseVisualStyleBackColor = true;
            this.buttonStartStop.Click += new System.EventHandler(this.buttonStartStop_Click);
            // 
            // buttonClearLog
            // 
            this.buttonClearLog.Location = new System.Drawing.Point(472, 86);
            this.buttonClearLog.Name = "buttonClearLog";
            this.buttonClearLog.Size = new System.Drawing.Size(192, 34);
            this.buttonClearLog.TabIndex = 8;
            this.buttonClearLog.Text = "Clear Log";
            this.buttonClearLog.UseVisualStyleBackColor = true;
            this.buttonClearLog.Click += new System.EventHandler(this.buttonClearLog_Click);
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon.BalloonTipText = "ITS Scale Exporter";
            this.notifyIcon.BalloonTipTitle = "ITS Scale Exporter";
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "ITS Scale Exporter";
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDoubleClick);
            // 
            // labelWebServiceCallTimeOut
            // 
            this.labelWebServiceCallTimeOut.AutoSize = true;
            this.labelWebServiceCallTimeOut.Location = new System.Drawing.Point(282, 44);
            this.labelWebServiceCallTimeOut.Name = "labelWebServiceCallTimeOut";
            this.labelWebServiceCallTimeOut.Size = new System.Drawing.Size(172, 13);
            this.labelWebServiceCallTimeOut.TabIndex = 9;
            this.labelWebServiceCallTimeOut.Text = "Web Service Call Time Out (secs) :";
            // 
            // numericUpDownwebServiceCallTimeOut
            // 
            this.numericUpDownwebServiceCallTimeOut.Location = new System.Drawing.Point(472, 42);
            this.numericUpDownwebServiceCallTimeOut.Maximum = new decimal(new int[] {
            900000,
            0,
            0,
            0});
            this.numericUpDownwebServiceCallTimeOut.Name = "numericUpDownwebServiceCallTimeOut";
            this.numericUpDownwebServiceCallTimeOut.Size = new System.Drawing.Size(120, 20);
            this.numericUpDownwebServiceCallTimeOut.TabIndex = 10;
            this.numericUpDownwebServiceCallTimeOut.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // checkBoxAutoRun
            // 
            this.checkBoxAutoRun.AutoSize = true;
            this.checkBoxAutoRun.Location = new System.Drawing.Point(584, 15);
            this.checkBoxAutoRun.Name = "checkBoxAutoRun";
            this.checkBoxAutoRun.Size = new System.Drawing.Size(68, 17);
            this.checkBoxAutoRun.TabIndex = 11;
            this.checkBoxAutoRun.Text = "AutoRun";
            this.checkBoxAutoRun.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(676, 429);
            this.Controls.Add(this.checkBoxAutoRun);
            this.Controls.Add(this.numericUpDownwebServiceCallTimeOut);
            this.Controls.Add(this.labelWebServiceCallTimeOut);
            this.Controls.Add(this.buttonClearLog);
            this.Controls.Add(this.buttonStartStop);
            this.Controls.Add(this.buttonSaveSettings);
            this.Controls.Add(this.numericUpDownRepeatTimeInMinutes);
            this.Controls.Add(this.textBoxServiceURL);
            this.Controls.Add(this.labelRepeatTimeInMinutes);
            this.Controls.Add(this.labelServiceURL);
            this.Controls.Add(this.labelLog);
            this.Controls.Add(this.richTextBoxLog);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "ITS Scales Exporter";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRepeatTimeInMinutes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownwebServiceCallTimeOut)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBoxLog;
        private System.Windows.Forms.Label labelLog;
        private System.Windows.Forms.Label labelServiceURL;
        private System.Windows.Forms.Label labelRepeatTimeInMinutes;
        private System.Windows.Forms.TextBox textBoxServiceURL;
        private System.Windows.Forms.NumericUpDown numericUpDownRepeatTimeInMinutes;
        private System.Windows.Forms.Button buttonSaveSettings;
        private System.Windows.Forms.Button buttonStartStop;
        private System.Windows.Forms.Button buttonClearLog;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.Label labelWebServiceCallTimeOut;
        private System.Windows.Forms.NumericUpDown numericUpDownwebServiceCallTimeOut;
        private System.Windows.Forms.CheckBox checkBoxAutoRun;
    }
}

