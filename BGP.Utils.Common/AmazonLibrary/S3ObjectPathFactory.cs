using System;
using System.Configuration;

namespace BGP.Utils.Common.AmazonLibrary
{
    public class S3ObjectPathFactory
    {
        public const string RootFolder = "Companies";

        public static readonly string TempFolder = ConfigurationManager.AppSettings["UploadPath"] ?? "~/Temp";
        public static readonly string ImageBucketUrl = ConfigurationManager.AppSettings["AWS.ImageBucketUrl"];
        public static readonly string ImageBucketName = ConfigurationManager.AppSettings["AWS.ImageBucketName"];
        public static readonly string ServiceURL = ConfigurationManager.AppSettings["AWS.ServiceURL"];

        /// <summary>
        /// Generate S3Key for Company's images.
        /// <para></para>
        /// Companies/{CompanyId}/Images
        /// </summary>
        /// <param name="companyID"></param>
        /// <returns></returns>
        public static string GetCompanyImageFolder(int companyID)
        {
            return string.Format("{0}/{1}/Images", RootFolder, companyID);
        }

        /// <summary>
        /// Get temporary folder for company image
        /// </summary>
        /// <param name="companyID"></param>
        /// <returns></returns>
        public static string GetCompanyImageTempFolder(int companyID)
        {
            return string.Format("{0}/{1}/Temp/Images", RootFolder, companyID);
        }

        /// <summary>
        /// Generate S3Key for Company's temp images.
        /// <para></para>
        /// Companies/{CompanyId}/Temp
        /// </summary>
        /// <param name="companyID"></param>
        /// <returns></returns>
        public static string GetCompanyTempFolder(int companyID)
        {
            return string.Format("{0}/{1}/Temp", RootFolder, companyID);
        }

        /// <summary>
        /// Generate S3Key for User's profile image.
        /// <para></para>
        /// Companies/{CompanyId}/Users/{UserId}/Images
        /// </summary>
        /// <param name="companyID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static string GetUserImageFolder(int companyID, int userID)
        {
            return string.Format("{0}/{1}/Users/{2}/Images", RootFolder, companyID, userID);
        }

        public static string GetUserImageFolder(int companyID)
        {
            return string.Format("{0}/{1}/Users/", RootFolder, companyID);
        }

        /// <summary>
        /// Generate S3Key for Ad's image.
        /// </summary>
        /// <para></para>
        /// Companies/{CompanyId}/Ads/{AdId}/Images
        /// <param name="companyID"></param>
        /// <param name="adID"></param>
        /// <returns></returns>
        public static string GetAdsImageFolder(int companyID, int adID)
        {
            return string.Format("{0}/{1}/Ads/{2}/Images", RootFolder, companyID, adID);
        }
        /// <summary>
        /// Get tempoary folder for advert image
        /// </summary>
        /// <param name="companyID"></param>
        /// <returns></returns>
        public static string GetAdsImageTempFolder(int companyID)
        {
            return string.Format("{0}/{1}/Ads/Temp/Images", RootFolder, companyID);
        }

        public static string GetAdsImageFolder(int companyID)
        {
            return string.Format("{0}/{1}/Ads/", RootFolder, companyID);
        }

        /// <summary>
        /// Generate S3Key for image overlay.
        /// <para></para>
        /// Companies/{CompanyId}/ImageOverlay/Images
        /// </summary>
        /// <param name="companyID"></param>        
        /// <returns></returns>
        public static string GetImageOverlayFolder(int companyID)
        {
            return string.Format("{0}/{1}/ImageOverlay/Images", RootFolder, companyID);
        }

        /// <summary>
        /// Generate S3 file name like [Guid].jpg
        /// </summary>
        /// <param name="fileExtension">file extension name with DOT</param>
        /// <returns></returns>
        public static string GenerateFileName(string fileExtension)
        {
            return string.Format("/{0}{1}", Guid.NewGuid().ToString().Replace("-", ""), fileExtension);
        }

        /// <summary>
        /// Get S3 file full URL
        /// </summary>
        /// <param name="s3KeyName">S3 path with Folder and File Name</param>
        /// <returns></returns>
        public static string GetUrl(string s3KeyName)
        {
            return string.Format(ImageBucketUrl, s3KeyName);
        }

        /// <summary>
        /// Get key from S3 URL
        /// </summary>
        /// <param name="s3KeyName"></param>
        /// <returns></returns>
        public static string GetKeyFromUrl(string s3URL)
        {
            return s3URL.Replace(AmazonHelper.AWS_ImageBucketUrl.Replace("{0}",""), "");
        }
    }
}