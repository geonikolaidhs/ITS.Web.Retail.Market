using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace ITS.Retail.MigrationTool.Helpers
{
    public class LogHelper
    {
        private RichTextBox logArea;

        public LogHelper(RichTextBox logRichTextBox)
        {
            logArea = logRichTextBox;
        }

        public void Message(String message){
            logArea.AppendText(message);
        }

        public void Error(String message)
        {
            logArea.SelectionStart = logArea.Text.Length;
            logArea.SelectionLength = message.Length;
            logArea.SelectionColor = Color.Red;
            logArea.AppendText(message);
            logArea.SelectionStart = logArea.Text.Length;
            logArea.ScrollToCaret();
        }

        public void Success(String message)
        {
            logArea.SelectionStart = logArea.Text.Length;
            logArea.SelectionLength = message.Length;
            logArea.SelectionColor = Color.Green;
            logArea.AppendText(message);
            logArea.SelectionStart = logArea.Text.Length;
            logArea.ScrollToCaret();
        }

        public void Warning(String message)
        {
            logArea.SelectionStart = logArea.Text.Length;
            logArea.SelectionLength = message.Length;
            logArea.SelectionColor = Color.FromArgb(255, 92, 0);
            logArea.AppendText(message);
            logArea.SelectionStart = logArea.Text.Length;
            logArea.ScrollToCaret();
        }
    }
}
