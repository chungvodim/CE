using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGP.Utils.Common.AmazonLibrary
{
    public class AmazonHelper
    {
        #region S3 section
        /// <summary>
        /// Get AWS.AccessKey from config file
        /// </summary>
        public static string AWS_AccessKey
        {
            get
            {
                return ConfigurationManager.AppSettings["AWS.AccessKey"];
            }
        }

        /// <summary>
        /// Get AWS.SecretKey from config file
        /// </summary>
        public static string AWS_SecretKey
        {
            get
            {
                return ConfigurationManager.AppSettings["AWS.SecretKey"];
            }
        }

        /// <summary>
        /// Get AWS.ImageBucketName from config file
        /// </summary>
        public static string AWS_ImageBucketName
        {
            get
            {
                return ConfigurationManager.AppSettings["AWS.ImageBucketName"];
            }
        }

        /// <summary>
        /// Get AWS.ImageBucketUrl from config file
        /// </summary>
        public static string AWS_ImageBucketUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["AWS.ImageBucketUrl"];
            }
        }

        /// <summary>
        /// Get AWS.ServiceURL from config file
        /// </summary>
        public static string AWS_ServiceURL
        {
            get
            {
                return ConfigurationManager.AppSettings["AWS.ServiceURL"];
            }
        }

        /// <summary>
        /// Get AWS.ServiceURL from config file
        /// </summary>
        public static string AWS_ServiceUrlPath
        {
            get
            {
                var url = ConfigurationManager.AppSettings["AWS.ServiceURL"];
                if (!string.IsNullOrEmpty(url))
                {
                    url = url.Replace("http://", "").Replace("https://", "");
                }
                return url;
            }
        }
        #endregion

        #region SES section
        /// <summary>
        /// Get AWS.FromAddress from config file
        /// </summary>
        public static string AWS_FromAddress
        {
            get
            {
                return ConfigurationManager.AppSettings["AWS.FromAddress"];
            }
        }

        /// <summary>
        /// Get AWS.FromDomains from config file
        /// </summary>
        public static string AWS_FromDomains
        {
            get
            {
                return ConfigurationManager.AppSettings["AWS.FromDomains"];
            }
        }

        /// <summary>
        /// Get AWS.SmtpUsername from config file
        /// </summary>
        public static string AWS_SmtpUsername
        {
            get
            {
                return ConfigurationManager.AppSettings["AWS.SmtpUsername"];
            }
        }

        /// <summary>
        /// Get AWS.SmtpPassword from config file
        /// </summary>
        public static string AWS_SmtpPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["AWS.SmtpPassword"];
            }
        }

        /// <summary>
        /// Get AWS.SmtpHost from config file
        /// </summary>
        public static string AWS_SmtpHost
        {
            get
            {
                return ConfigurationManager.AppSettings["AWS.SmtpHost"];
            }
        }

        /// <summary>
        /// Get AWS.SmtpPort from config file
        /// </summary>
        public static int AWS_SmtpPort
        {
            get
            {
                int port = 465;
                int.TryParse(ConfigurationManager.AppSettings["AWS.SmtpPort"], out port);
                return port; 
            }
        }
        #endregion
    }
}
