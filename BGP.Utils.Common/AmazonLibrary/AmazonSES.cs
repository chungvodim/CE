using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace BGP.Utils.Common.AmazonLibrary
{
    public class AmazonSES
    {
        private static ILog _log = log4net.LogManager.GetLogger(typeof(AmazonSES));

        /// <summary>
        /// Send email by Amazon SES
        /// </summary>
        /// <param name="to">To Address(es). Seperate by ";"</param>
        /// <param name="subject">Subject</param>
        /// <param name="body">Body</param>
        /// <param name="isHtml">Is Html body</param>
        /// <returns></returns>
        public static void SendEmail(string from, string to, string subject, string body, bool isHtml)
        {
            using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
            {
                _log.Info("Attempting to send an email through the Amazon SES SMTP interface...");
                if (string.IsNullOrEmpty(from))
                    from = AmazonHelper.AWS_FromAddress;

                mail.From = new System.Net.Mail.MailAddress(from);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = isHtml;
                mail.Priority = System.Net.Mail.MailPriority.Normal;

                foreach (string toAddr in to.Split(';'))
                {
                    if (!string.IsNullOrEmpty(toAddr))
                        mail.To.Add(toAddr);
                }

                SendEmail(mail);
            }
        }

        /// <summary>
        /// Send email by Amazon SES
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        public static void SendEmail(System.Net.Mail.MailMessage mail)
        {
            // Get Amazon SES settings from config
            string smtpUsr = AmazonHelper.AWS_SmtpUsername,
                smtpPwd = AmazonHelper.AWS_SmtpPassword,
                smtpHost = AmazonHelper.AWS_SmtpHost,
                fromDomains = AmazonHelper.AWS_FromDomains,
                fromAddress = mail.From.Address;

            int smtpPort = AmazonHelper.AWS_SmtpPort;

            if (string.IsNullOrEmpty(fromDomains))
                throw new Exception("Could not find 'AWS.FromDomains' settings.");

            // make sure that from address should be in support domains
            bool isValidFromAddress = false;
            string[] supportedDomains = fromDomains.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string domain in supportedDomains)
            {
                if (fromAddress.EndsWith(domain, StringComparison.CurrentCultureIgnoreCase))
                {
                    isValidFromAddress = true;
                    break;
                }
            }

            if (!isValidFromAddress)
            {
                throw new Exception(string.Format("FromAddress should be in domain(s): {0}", fromDomains));
            }

            // Create an SMTP client with the specified host name and port.
            using (System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(smtpHost, smtpPort))
            {
                // Create a network credential with your SMTP user name and password.
                client.Credentials = new System.Net.NetworkCredential(smtpUsr, smtpPwd);

                // Use SSL when accessing Amazon SES. The SMTP session will begin on an unencrypted connection, and then 
                // the client will issue a STARTTLS command to upgrade to an encrypted connection using SSL.
                client.EnableSsl = true;

                // Send the email. 
                try
                {
                    _log.Info("Attempting to send an email through the Amazon SES SMTP interface...");
                    client.Send(mail);
                    _log.Debug("Email sent!");
                }
                catch (Exception ex)
                {
                    _log.Error("Error message: " + ex.Message, ex);
                }
            }
        }
    }
}
