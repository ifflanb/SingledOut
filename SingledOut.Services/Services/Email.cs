using System.Configuration;
using System.Net;
using System.Net.Mail;
using SingledOut.Services.Interfaces;

namespace SingledOut.Services.Services
{
    public class Email : IEmail
    {
        /// <summary>
        /// Sends an Email.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="fromName"></param>
        /// <param name="to"></param>
        /// <param name="cc"></param>
        /// <param name="bcc"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="isHtml"></param>
        public void SendEmail(string from, string fromName, string to, string cc, string bcc, string subject, string body, bool isHtml)
        {
            var smtpServer = ConfigurationManager.AppSettings["SmtpServer"];
            var smtpPort = ConfigurationManager.AppSettings["SmtpPort"];
            var smtpUsername = ConfigurationManager.AppSettings["SmtpUsername"];
            var smtpPassword = ConfigurationManager.AppSettings["SmtpPassword"];

            var mailClient = new SmtpClient(smtpServer)
            {
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                Port = int.Parse(smtpPort)
            };

            var message = new MailMessage();
            message.From = !string.IsNullOrEmpty(fromName) ? new MailAddress(@from, fromName) : new MailAddress(@from);

            message.To.Add(new MailAddress(to));

            if (!string.IsNullOrEmpty(cc))
            {
                message.CC.Add(cc);
            }

            if (!string.IsNullOrEmpty(bcc))
            {
                message.Bcc.Add(bcc);
            }

            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = isHtml;
            
            mailClient.Send(message);
        }
    }
}
