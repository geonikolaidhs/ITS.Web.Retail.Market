using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace POSLoader.Helpers
{
    public class LogHelper
    {
        private RichTextBox logArea;
        private string logFile;

        public LogHelper(RichTextBox logRichTextBox, string logFile)
        {
            logArea = logRichTextBox;
            this.logFile = logFile;
        }

        public void Message(String message)
        {
            string msg = DateTime.Now.ToString() + " " + message;
            logArea.AppendText(msg+Environment.NewLine);
            using (StreamWriter log_file_stream = new StreamWriter(logFile, true))
            {
                log_file_stream.WriteLine(msg);
                Application.DoEvents();
                log_file_stream.Close();
            }
        }

        public void Error(String message)
        {
            SetUpLogger("Error: " + message, Color.Red);            
            Application.DoEvents();
        }

        public void Success(String message)
        {
            SetUpLogger(message, Color.Green);
        }

        public void Warning(String message)
        {
            SetUpLogger("Warning: " + message, Color.Orange);
        }

        private void SetUpLogger(string message, Color color)
        {
            logArea.SelectionStart = logArea.Text.Length;
            logArea.SelectionColor = color;
            logArea.SelectionLength = message.Length;
            Message(message + Environment.NewLine);
        }
    }
}
