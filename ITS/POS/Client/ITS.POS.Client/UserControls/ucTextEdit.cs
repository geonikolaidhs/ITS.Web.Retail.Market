using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using DevExpress.XtraEditors;
using System.Windows.Forms;
using ITS.POS.Client.Forms;
using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Kernel;

namespace ITS.POS.Client.UserControls
{
    public partial class ucTextEdit : TextEdit, IPoleDisplayTextInput, IScannerInput
    {

        public IPosKernel Kernel
        {
            get
            {
                Form parentForm = this.FindForm();
                if (parentForm is IPOSForm)
                {
                    return (parentForm as IPOSForm).Kernel;
                }
                else
                {
                    throw new POSException("Form must implement IPOSForm");
                }
            }
        }

        public ucTextEdit()
        {
            this.PoleDisplayName = this.Name;
        }

        private string poleDisplayName;
        public string PoleDisplayName
        {
            get
            {
                return poleDisplayName;
            }
            set
            {
                poleDisplayName = value;
            }
        }


        public void AttachTextChangedEvent(EventHandler handler)
        {
            this.TextChanged += handler;
        }

        public void DetachTextChangedEvent(EventHandler handler)
        {
            this.TextChanged -= handler;
        }

        public string GetText()
        {
            return this.Text;
        }

        public void SetText(string text)
        {
            try
            {
                this.Invoke((MethodInvoker)delegate()
                {
                    this.Text += text;
                });
            }
            catch (Exception ex)
            {
                Kernel.LogFile.Error(ex, "Error setting Text, input:" + this.Name);
            }
        }

        public void SendEnter()
        {
            try
            {
                this.Invoke((MethodInvoker)delegate()
                {
                    SendKeys.Send("~");
                });
            }
            catch (Exception ex)
            {
                Kernel.LogFile.Error(ex, "Error sending Enter, input:" + this.Name);
            }
        }

    }
}
