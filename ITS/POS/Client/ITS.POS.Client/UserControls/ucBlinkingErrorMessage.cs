using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ITS.POS.Model.Settings;
using System.Threading;
using ITS.POS.Client.ObserverPattern;
using ITS.POS.Client.ObserverPattern.ObserverParameters;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Helpers;

namespace ITS.POS.Client.UserControls
{
    /// <summary>
    /// Displays non disruptive blinking error messages
    /// </summary>
    [ChildsUseSameFont]
    public partial class ucBlinkingErrorMessage : ucObserver, IObserverBlinkingErrorMessenger
    {
        Type[] paramsTypes = new Type[] { typeof(BlinkingErrorMessengerParams) };

        public override Type[] GetParamsTypes()
        {
            return paramsTypes;
        }

        private System.Windows.Forms.Timer timerBlinkingError;
        private IContainer components;
        private StatusStrip statusStripBlinking;
        private ToolStripStatusLabel toolStripStatusLabelBlinking;
        private bool Blink;

        public ucBlinkingErrorMessage()
        {
            InitializeComponent();
            this.ActionsToObserve.Add(eActions.SHOW_BLINKING_ERROR);
            this.Blink = false;
            timerBlinkingError.Start();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timerBlinkingError = new System.Windows.Forms.Timer(this.components);
            this.statusStripBlinking = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelBlinking = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStripBlinking.SuspendLayout();
            this.SuspendLayout();
            // 
            // timerBlinkingError
            // 
            this.timerBlinkingError.Tick += new System.EventHandler(this.timerBlinkingError_Tick);
            // 
            // statusStripBlinking
            // 
            this.statusStripBlinking.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusStripBlinking.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelBlinking});
            this.statusStripBlinking.Location = new System.Drawing.Point(0, 0);
            this.statusStripBlinking.Name = "statusStripBlinking";
            this.statusStripBlinking.Size = new System.Drawing.Size(33, 27);
            this.statusStripBlinking.TabIndex = 0;
            this.statusStripBlinking.Text = "statusStrip1";
            // 
            // toolStripStatusLabelBlinking
            // 
            this.toolStripStatusLabelBlinking.Image = global::ITS.POS.Client.Properties.Resources.offline_dot;
            this.toolStripStatusLabelBlinking.Name = "toolStripStatusLabelBlinking";
            this.toolStripStatusLabelBlinking.Size = new System.Drawing.Size(16, 22);
            // 
            // ucBlinkingErrorMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.statusStripBlinking);
            this.Name = "ucBlinkingErrorMessage";
            this.Size = new System.Drawing.Size(33, 27);
            this.Load += new System.EventHandler(this.ucBlinkingErrorMessage_Load);
            this.statusStripBlinking.ResumeLayout(false);
            this.statusStripBlinking.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        public void Update(BlinkingErrorMessengerParams parameters)
        {
            try
            {
                this.Blink = false;
                if (parameters != null && parameters.Blink)
                {
                    UtilsHelper.HardwareBeep();
                    this.Blink = parameters.Blink;
                }
            }
            catch (System.Exception ex)
            {
                Kernel.LogFile.Info(ex, "ucBlinkingErrorMessage:Update,Exception catched");
            }

        }

        private void ucBlinkingErrorMessage_Load(object sender, EventArgs e)
        {
        }

        private void timerBlinkingError_Tick(object sender, EventArgs e)
        {
            this.toolStripStatusLabelBlinking.Visible = this.Blink ? !this.toolStripStatusLabelBlinking.Visible : false;
        }
    }
}
