using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;

namespace ITS.Retail.WebClient.Helpers
{

    public static class MailHelper
    {
        /// <summary>
        /// Sends an mail message
        /// </summary>
        /// <param name="from">Sender address</param>
        /// <param name="to">Recepient address</param>
        /// <param name="bcc">Bcc recepient</param>
        /// <param name="cc">Cc recepient</param>
        /// <param name="subject">Subject of mail message</param>
        /// <param name="body">Body of mail message</param>
        public static void SendMailMessage(string from, List<string> to, string subject, string body, string host, string bcc = null, string cc = null, string username = null, string password = null, string domain = null, bool enableSSL = false, string port = "25")
        {
            // Instantiate a new instance of MailMessage
            using (MailMessage mMailMessage = new MailMessage())
            {
                // Set the sender address of the mail message
                mMailMessage.From = new MailAddress(from);
                // Set the recepient address of the mail message
                foreach (string mail_receiver in to)
                {
                    mMailMessage.To.Add(new MailAddress(mail_receiver));
                }
                // Check if the bcc value is null or an empty string
                if (!string.IsNullOrEmpty(bcc))
                    // Set the Bcc address of the mail message
                    mMailMessage.Bcc.Add(new MailAddress(bcc));
                // Check if the cc value is null or an empty value
                if (!string.IsNullOrEmpty(cc))
                    // Set the CC address of the mail message
                    mMailMessage.CC.Add(new MailAddress(cc));
                // Set the subject of the mail message
                mMailMessage.Subject = subject;
                // Set the body of the mail message
                mMailMessage.Body = PreMailer.Net.PreMailer.MoveCssInline(body).Html;
                // Set the format of the mail message body as HTML
                mMailMessage.IsBodyHtml = true;
                // Set the priority of the mail message to normal
                mMailMessage.Priority = MailPriority.Normal;
                // Instantiate a new instance of SmtpClient
                using (SmtpClient mSmtpClient = new SmtpClient(host))
                {
                    int parsedPort;
                    if (int.TryParse(port, out parsedPort))
                    {
                        mSmtpClient.Port = parsedPort;
                    }

                    mSmtpClient.EnableSsl = enableSSL;

                    if (!String.IsNullOrWhiteSpace(username) || !String.IsNullOrWhiteSpace(password))
                    {
                        if (String.IsNullOrWhiteSpace(domain))
                        {
                            mSmtpClient.Credentials = new NetworkCredential(username, password);
                        }
                        else
                        {
                            mSmtpClient.Credentials = new NetworkCredential(username, password, domain);
                        }
                    }
                    // Send the mail message
                    mSmtpClient.Send(mMailMessage);
                }
            }
        }
    }
}