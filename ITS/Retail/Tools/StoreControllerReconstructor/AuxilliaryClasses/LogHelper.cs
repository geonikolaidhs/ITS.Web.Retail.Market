using System;
using System.IO;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Media;

namespace StoreControllerReconstructor.AuxilliaryClasses
{
    public class LogHelper
    {
        private System.Windows.Controls.RichTextBox logArea;
        private string logFile;

        public LogHelper(System.Windows.Controls.RichTextBox logRichTextBox, string logFile)
        {
            logArea = logRichTextBox;
            this.logFile = logFile;
        }

        public void Message(String message)
        {
            string msg = DateTime.Now.ToString() + " " + message;
            logArea.AppendText(msg + Environment.NewLine);
            using (StreamWriter log_file_stream = new StreamWriter(logFile, true))
            {
                log_file_stream.WriteLine(msg);
                Application.DoEvents();
                log_file_stream.Close();
            }
        }

        public void Error(String message)
        {
            SetUpLogger("Error: " + message, Brushes.Red);
            Application.DoEvents();
        }

        public void Success(String message)
        {
            SetUpLogger(message, Brushes.Green);
        }

        public void Warning(String message)
        {
            SetUpLogger("Warning: " + message, Brushes.Orange);
        }

        private void SetUpLogger(string message, Brush brush)
        {
            //logArea.SelectionStart = logArea.Text.Length;
            //logArea.SelectionColor = color;
            //logArea.SelectionLength = message.Length;
            string finalMessage = message + Environment.NewLine;
            logArea.CaretBrush = brush;
            logArea.BeginChange();
            TextPointer forward = logArea.CaretPosition.GetPositionAtOffset(0, LogicalDirection.Forward);
            forward.InsertTextInRun(finalMessage);
            logArea.CaretPosition = forward;
            logArea.EndChange();
            Message(finalMessage);
        }
    }
}
