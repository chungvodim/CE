using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGP.Utils.Common
{
    public static class Configuration
    {
        public static string errFromEmail = ConfigurationManager.AppSettings["errFromEmail"];
        public static string errToEmail = ConfigurationManager.AppSettings["errToEmail"];
        public static string smtpSendFrom = ConfigurationManager.AppSettings["smtpSendFrom"];
        public static string smtpHost = ConfigurationManager.AppSettings["AWS.smtpHost"];
        public static int smtpPort = Convert.ToInt32(ConfigurationManager.AppSettings["AWS.smtpPort"]);
        public static string smtpUsername = ConfigurationManager.AppSettings["AWS.smtpUsername"];
        public static string smtpPassword = ConfigurationManager.AppSettings["AWS.smtpPassword"];
        public static bool enableSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["enableSSL"]);
        public static bool isTestMode = Convert.ToBoolean(ConfigurationManager.AppSettings["isTestMode"]);
        public static string testerEmail = ConfigurationManager.AppSettings["testerEmail"];
        public static string supportEmail = ConfigurationManager.AppSettings["supportEmail"];
        public static short maxFileCount = Convert.ToInt16(ConfigurationManager.AppSettings["maxFileCount"]);
        public static string EMAIL_IMAGE_ATTATCHMENT_URL = ConfigurationManager.AppSettings["EMAIL_IMAGE_ATTATCHMENT_URL"];
        public static short balanceRefreshFrequency = Convert.ToInt16(ConfigurationManager.AppSettings["balanceRefreshFrequency"]);
        public static bool turnOnValidateAntiForgery = Convert.ToBoolean(ConfigurationManager.AppSettings["turnOnValidateAntiForgery"]);
        public static bool enableGoogleTagManager = Convert.ToBoolean(ConfigurationManager.AppSettings["enableGoogleTagManager"]);
        public static string email2dehandsSupport = ConfigurationManager.AppSettings["2dehandsSupport"];
        public static string emaild2ememainSupport = ConfigurationManager.AppSettings["2ememainSupport"];
        public static string adminCompanyID = ConfigurationManager.AppSettings["adminCompanyID"] ?? "1";
        public static string validImageExtension = ConfigurationManager.AppSettings["validImageExtension"] ?? "jpeg,jpg,gif,png";
        public const short expirationMinutes = 24 * 60;
        public static bool searchOnly1Page = Convert.ToBoolean(ConfigurationManager.AppSettings["searchOnly1Page"]);
        public static bool disableErrorEmail = Convert.ToBoolean(ConfigurationManager.AppSettings["disableErrorEmail"]);
        public static bool disableInlineTranslation = Convert.ToBoolean(ConfigurationManager.AppSettings["disableInlineTranslation"]);
        public static bool useS3TempFolder = Convert.ToBoolean(ConfigurationManager.AppSettings["useS3TempFolder"]);
        public static string statisticUpdatedTime = ConfigurationManager.AppSettings["statisticUpdatedTime"] ?? "16:00";
        public static int listingPastDays = Convert.ToInt32(ConfigurationManager.AppSettings["listingPastDays"] ?? "2");
        public static int spamDuration = Convert.ToInt32(ConfigurationManager.AppSettings["spamDuration"] ?? "10");
        public static int maxStoredErrorDescriptions = Convert.ToInt32(ConfigurationManager.AppSettings["maxStoredErrorDescriptions"] ?? "100");
        public static string environment = ConfigurationManager.AppSettings["environment"];
        public const string BE_LongDateFormat = "HH:mm dd MMMM yyyy";
        public const string BE_ShortDateFormat = "dd MMMM yyyy";
        public const string Morris_ShortDateFormat = "yyyy-MM-dd";
        public static readonly int ServerUtcOffset = Convert.ToInt32(TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalHours);
    }
}
