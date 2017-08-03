using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace BGP.Utils.Common
{
    public static class ErrorHelper
    {
        public static readonly ILog Logger = LogManager.GetLogger("BELGIUM-LOGS");
        public static EmailSpamChecker _EmailSpamChecker = new EmailSpamChecker();

        #region Public Methods

        /// <summary>
        /// Add Error to log file
        /// </summary>
        /// <param name="description"></param>
        /// <param name="exp"></param>
        public static void LogAndSendEmail(string description, Exception exp)
        {
            if (!_EmailSpamChecker.IsSpamMessage(description, DateTime.UtcNow) && !Configuration.disableErrorEmail)
            {
                SendErrorEmail(description, exp);
                //Logger.Info("Send Email to Devs");
            }
            Logger.Debug(exp.Message, exp);
        }

        public static void LogDetailFromException(Exception error)
        {
            Logger.ErrorFormat("Detail Exception: {0}", GetDetailFromException(error));
        }


        public static string GetDetailFromException(Exception error)
        {
            StringBuilder sb = new StringBuilder();

            // sb.AppendLine(error.ToString());

            DbEntityValidationException dbValidationEx;
            if ((dbValidationEx = error as DbEntityValidationException) != null)
            {
                sb.AppendLine("==========================");
                foreach (var validationErrors in dbValidationEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        sb.AppendFormat("Property: {0} Error: {1} \n", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }

            sb.AppendLine("==========================");
            sb.AppendLine(FlattenException(error));

            return sb.ToString();
        }

        public static string FlattenException(Exception exception)
        {
            var stringBuilder = new StringBuilder();

            while (exception != null)
            {
                stringBuilder.AppendLine(exception.Message);
                stringBuilder.AppendLine(exception.StackTrace);

                exception = exception.InnerException;
            }

            return stringBuilder.ToString();
        }

        public static void SendErrorEmail(string strDescription, Exception exp)
        {
            try
            {
                HttpContext current = HttpContext.Current;
                // bool hasUserInfo = false;

                if (current == null || current.Session == null)
                    return;

                string sUserFullName = current.User.Identity.Name;
                string sUserEmail = current.User.Identity.Name;
                
                System.Text.StringBuilder strMessage = new System.Text.StringBuilder();
                strMessage.Append(@"<style type=""text/css"">");
                strMessage.Append(@"<!--");
                strMessage.Append(@".basix {");
                strMessage.Append(@"font-family: Verdana, Arial, Helvetica, sans-serif");
                strMessage.Append(@"font-size: 12px");
                strMessage.Append(@"}");
                strMessage.Append(@".header1 {");
                strMessage.Append(@"font-family: Verdana, Arial, Helvetica, sans-serif");
                strMessage.Append(@"font-size: 12px");
                strMessage.Append(@"font-weight: bold");
                strMessage.Append(@"color: #000099");
                strMessage.Append(@"}");
                strMessage.Append(@".tlbbkground1 {");
                strMessage.Append(@"background-color: #000099");
                strMessage.Append(@"}");
                strMessage.Append(@"-->");
                strMessage.Append(@"</style>");

                strMessage.Append(@"<table width=""85%"" border=""0"" align=""center"" cellpadding=""5"" cellspacing=""1"" class=""tlbbkground1"">");
                strMessage.Append(@"<tr bgcolor=""#eeeeee"">");
                strMessage.Append(@"<td colspan=""2"" class=""header1"">Page Error</td>");
                strMessage.Append(@"</tr>");
                strMessage.Append(@"<tr>");
                strMessage.Append(@"<td width=""100"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>IP Address</td>");
                strMessage.Append(@"<td bgcolor=""#FFFFFF"" class=""basix"">" + current.Request.ServerVariables["REMOTE_ADDR"] + "</td>");
                strMessage.Append(@"</tr>");
                strMessage.Append(@"<tr>");
                strMessage.Append(@"<td width=""100"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>User Agent</td>");
                strMessage.Append(@"<td bgcolor=""#FFFFFF"" class=""basix"">" + current.Request.ServerVariables["HTTP_USER_AGENT"] + "</td>");
                strMessage.Append(@"</tr>");
                strMessage.Append(@"<tr>");
                strMessage.Append(@"<td width=""100"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>Page</td>");
                strMessage.Append(@"<td bgcolor=""#FFFFFF"" class=""basix"">" + current.Request.Url.AbsoluteUri + "</td>");
                strMessage.Append(@"</tr>");
                strMessage.Append(@"<tr>");
                strMessage.Append(@"<td width=""100"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>Time</td>");
                strMessage.Append(@"<td bgcolor=""#FFFFFF"" class=""basix"">" + System.DateTime.Now.ToUniversalTime() + " PST</td>");
                strMessage.Append(@"</tr>");
                strMessage.Append(@"<tr>");
                strMessage.Append(@"<td width=""100"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>User</td>");
                strMessage.Append(@"<td bgcolor=""#FFFFFF"" class=""basix"">" + sUserFullName + " </td>");
                strMessage.Append(@"</tr>");
                strMessage.Append(@"<tr>");
                strMessage.Append(@"<td width=""100"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>Login</td>");
                strMessage.Append(@"<td bgcolor=""#FFFFFF"" class=""basix"">" + sUserEmail + " </td>");
                strMessage.Append(@"</tr>");
                strMessage.Append(@"<tr>");
                strMessage.Append(@"<td width=""100"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>Details</td>");
                strMessage.Append(@"<td bgcolor=""#FFFFFF"" class=""basix""><pre>" + HttpUtility.HtmlEncode(GetDetailFromException(exp)) + "<br/>" + HttpUtility.HtmlEncode(strDescription) + "</pre></td>");
                strMessage.Append(@"</tr>");
                strMessage.Append(@"</table>");

                System.Net.Mail.MailMessage oMail = new System.Net.Mail.MailMessage(Configuration.errFromEmail, Configuration.errToEmail, exp.Message, strMessage.ToString());
                oMail.IsBodyHtml = true;

                EmailEngine.SendMailToDeveloper(oMail);
            }
            catch
            {
            }
        }
        #endregion
    }

    public class EmailSpamChecker
    {
        private static List<KeyValuePair<string, DateTime>> _Emails { get; set; }
        public static readonly object _LockObject = new object();

        public bool IsSpamMessage(string description, DateTime createdDate)
        {
            if (Monitor.TryEnter(_LockObject, new TimeSpan(0, 0, 10)))
            {
                try
                {
                    _Emails = _Emails.OrderByDescending(x => x.Value).ToList();

                    bool isSpam = true;

                    var recentSimilarEmails = _Emails.Where(x => x.Key == description).OrderByDescending(x => x.Value);

                    if (recentSimilarEmails == null || !recentSimilarEmails.Any())
                    {
                        isSpam = false;
                    }
                    else
                    {
                        var latestSimilarEmail = recentSimilarEmails.First();
                        if ((latestSimilarEmail.Value - DateTime.UtcNow).TotalMinutes > Configuration.spamDuration)
                        {
                            isSpam = false;
                        }
                    }

                    if (!isSpam)
                    {
                        _Emails.Add(new KeyValuePair<string, DateTime>(description, createdDate));
                        if (_Emails.Count > Configuration.maxStoredErrorDescriptions)
                        {
                            _Emails.RemoveAt(_Emails.Count - 1);
                        }
                    }

                    return isSpam;
                }
                finally
                {
                    Monitor.Exit(_LockObject);
                }
            }
            else
            {
                // failed to get lock: throw exceptions, log messages, get angry etc.
                throw new System.TimeoutException("failed to acquire the lock.");
            }
        }

        public EmailSpamChecker()
        {
            if(_Emails == null)
            {
                _Emails = new List<KeyValuePair<string, DateTime>>();
            }
        }
    }
}
