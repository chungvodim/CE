using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Net;

namespace BGP.Utils.Common.AmazonLibrary
{
    public class AmazonS3
    {
        private ILog _log = log4net.LogManager.GetLogger(typeof(AmazonS3));

        /// <summary>
        /// Add file to S3
        /// </summary>
        /// <param name="keyName">S3 file key. Use "/" to hierarchy to folder structure</param>
        /// <param name="filePath">Path of file to submit</param>
        /// <returns>url of image from Amazon</returns>
        public string AddFile(string keyName, string filePath)
        {
            string fileUrl = string.Empty;
            // check local filePath
            if (UrlHelper.IsLocalPath(filePath) && !File.Exists(filePath))
                return fileUrl;

            try
            {
                // upload to S3
                uploadFileToS3(filePath, keyName);

                fileUrl = string.Format(AmazonHelper.AWS_ImageBucketUrl, keyName);
            }
            catch (Exception ex)
            {
                _log.Error("Fail to upload file to Amazon S3", ex);
            }
            
            return fileUrl;
        }

        /// <summary>
        /// Add file to S3
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public string AddFile(string keyName, Stream stream)
        {
            try
            {
                // upload to S3
                uploadFileToS3(stream, keyName);

                return string.Format(AmazonHelper.AWS_ImageBucketUrl, keyName);
            }
            catch (Exception ex)
            {
                _log.Error("Fail to upload file to Amazon S3", ex);
                return null;
            }
        }

        public bool RemoveFile(string key)
        {
            bool result = false;

            try
            {
                removeFileFromS3(key);

                result = true;
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("Fail to delete {0} from {1}", key, AmazonHelper.AWS_ImageBucketName), ex);
            }

            return result;
        }

        public bool CopyFile(string sourceKey, string destKey)
        {
            bool result = false;

            try
            {
                copyingObject(sourceKey, destKey);

                result = true;
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("Fail to copy {0} to {1} on {2}", sourceKey, destKey, AmazonHelper.AWS_ImageBucketName), ex);
            }

            return result;
        }

        public string AddImportArchive(string keyName, string filePath)
        {
            string fileUrl = string.Empty;
            try
            {
                if (UrlHelper.IsLocalPath(filePath) && !File.Exists(filePath))
                    return fileUrl;

                uploadFileToS3(filePath, keyName);
                fileUrl = string.Format(AmazonHelper.AWS_ImageBucketUrl, keyName);
            }
            catch (Exception ex)
            {
                _log.Error("Fail to upload file to Amazon S3", ex);
            }

            return fileUrl;
        }

        #region Private section
        private void uploadFileToS3(string filePath, string keyName)
        {
            // prepare aws settings
            string AWSAccessKey = AmazonHelper.AWS_AccessKey,
                AWSSecretKey = AmazonHelper.AWS_SecretKey,
                AWSBucketName = AmazonHelper.AWS_ImageBucketName;

            var config = new AmazonS3Config
            {
                ServiceURL = AmazonHelper.AWS_ServiceURL
            };

            if (UrlHelper.IsLocalPath(filePath))
            {
                using (FileStream fileToUpload = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (IAmazonS3 client = Amazon.AWSClientFactory.CreateAmazonS3Client(AWSAccessKey, AWSSecretKey, config))
                    {
                        using (TransferUtility fileTransferUtility = new TransferUtility(client))
                        {
                            fileTransferUtility.Upload(new TransferUtilityUploadRequest
                            {
                                BucketName = AWSBucketName,
                                InputStream = fileToUpload,
                                Key = keyName,
                                PartSize = 6291456, // 6 MB.
                                StorageClass = S3StorageClass.Standard,
                                CannedACL = S3CannedACL.PublicRead
                            });
                        }
                    }
                }
            }
            else
            {
                var bytes = ImageHelper.GetBinaryFromUrl(filePath);
                var stream = new MemoryStream(bytes);

                using (IAmazonS3 client = Amazon.AWSClientFactory.CreateAmazonS3Client(AWSAccessKey, AWSSecretKey, config))
                {
                    using (TransferUtility fileTransferUtility = new TransferUtility(client))
                    {
                        fileTransferUtility.Upload(new TransferUtilityUploadRequest
                        {
                            BucketName = AWSBucketName,
                            InputStream = stream,
                            Key = keyName,
                            PartSize = 6291456, // 6 MB.
                            StorageClass = S3StorageClass.Standard,
                            CannedACL = S3CannedACL.PublicRead
                        });
                    }
                }
            }
        }

        private void uploadFileToS3(Stream stream, string keyName)
        {
            // prepare aws settings
            string AWSAccessKey = AmazonHelper.AWS_AccessKey,
                AWSSecretKey = AmazonHelper.AWS_SecretKey,
                AWSBucketName = AmazonHelper.AWS_ImageBucketName;

            var config = new AmazonS3Config
            {
                ServiceURL = AmazonHelper.AWS_ServiceURL
            };

            using (IAmazonS3 client = Amazon.AWSClientFactory.CreateAmazonS3Client(AWSAccessKey, AWSSecretKey, config))
            {
                using (TransferUtility fileTransferUtility = new TransferUtility(client))
                {
                    fileTransferUtility.Upload(new TransferUtilityUploadRequest
                    {
                        BucketName = AWSBucketName,
                        InputStream = stream,
                        Key = keyName,
                        PartSize = 6291456, // 6 MB.
                        StorageClass = S3StorageClass.Standard,
                        CannedACL = S3CannedACL.PublicRead
                    });
                }
            }
        }

        private void removeFileFromS3(string keyName)
        {
            DeleteObjectRequest deleteRequest = new DeleteObjectRequest
            {
                BucketName = AmazonHelper.AWS_ImageBucketName,
                Key = keyName
            };

            AmazonS3Config config = new AmazonS3Config
            {
                ServiceURL = AmazonHelper.AWS_ServiceURL
            };

            using (IAmazonS3 client = Amazon.AWSClientFactory.CreateAmazonS3Client(AmazonHelper.AWS_AccessKey, AmazonHelper.AWS_SecretKey, config))
            {
                client.DeleteObject(deleteRequest);
                _log.DebugFormat("Deleted {0} on {1}", keyName, AmazonHelper.AWS_ImageBucketName);
            }
        }

        private void copyingObject(string sourceKey, string destKey)
        {
            CopyObjectRequest request = new CopyObjectRequest
            {
                SourceBucket = AmazonHelper.AWS_ImageBucketName,
                SourceKey = sourceKey,
                DestinationBucket = AmazonHelper.AWS_ImageBucketName,
                DestinationKey = destKey
            };

            AmazonS3Config config = new AmazonS3Config
            {
                ServiceURL = AmazonHelper.AWS_ServiceURL
            };

            using (IAmazonS3 client = Amazon.AWSClientFactory.CreateAmazonS3Client(AmazonHelper.AWS_AccessKey, AmazonHelper.AWS_SecretKey, config))
            {
                client.CopyObject(request);
                _log.DebugFormat("Copy {0} to {1} on {2}", sourceKey, destKey, AmazonHelper.AWS_ImageBucketName);
            }
        }

        #endregion
    }
}
