using ITS.POS.Client.Kernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ITS.POS.Client.Forms
{
    public partial class frmAsyncMessageBox : frmMessageBox
    {
        IActionProgress action;
        public frmAsyncMessageBox(IPosKernel Kernel, IActionProgress action)
            :base(Kernel)
        {
            InitializeComponent();
            action.ProgressStarted += action_ProgressStarted;
            action.ProgressChanged += action_ProgressChanged;
            action.ProgressCompleted += action_ProgressCompleted;
            this.action = action;
        }
        

        void action_ProgressCompleted(object sender, string messageAppend, object result)
        {
            this.Invoke(new Action(() =>
            {
                this.progressBarControl1.Position = this.progressBarControl1.Properties.Maximum;
                this.Message += messageAppend;
                this.pnlActions.Enabled = true;
            }));
            action.ProgressStarted -= action_ProgressStarted;
            action.ProgressChanged -= action_ProgressChanged;
            action.ProgressCompleted -= action_ProgressCompleted;
        }

        void action_ProgressChanged(object sender, int currentvalue, string messageAppend)
        {
            this.Invoke(new Action(() =>
            {
                this.progressBarControl1.Position = currentvalue;
                this.Message += messageAppend;
            }));
        }

        void action_ProgressStarted(object sender, int minimum, int maximum, string initialMessage)
        {
            this.Invoke(new Action(() =>
            {
                this.Message = initialMessage;
                this.progressBarControl1.Properties.Minimum = minimum;
                this.progressBarControl1.Properties.Maximum = maximum;
                this.pnlActions.Enabled = false;
            }));
        }

        



       

    }
}
