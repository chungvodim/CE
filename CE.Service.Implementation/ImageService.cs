using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CE.Dto;
using CE.Entity.Main;
using CE.Enum;
using CE.Service;
using CE.Repository.Main;
using BGP.Utils.Service;
using log4net;
using System.Web;
using System.IO;
using BGP.Utils.Common.AmazonLibrary;
using System.Linq;

namespace CE.Service.Implementation
{
    public class ImageService : BaseService, IImageService
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ImageService));

        public ImageService(BGP.Utils.Service.EntityFramework.BaseService<MainEntityFrameworkRepository> mainService) : base(mainService, _logger) { }

        public async Task<int> AddImageAsync(ImageDto image)
        {
            await _mainService.CreateAsync<Image, ImageDto>(image);
            return image.ImageID;
        }

        public async Task AddImagesAsync(IEnumerable<ImageDto> images)
        {
            await _mainService.CreateManyAsync<Image, ImageDto>(images);
        }

        public Task<ImageDto> GetImageByIdAsync(int imageId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ImageDto>> GetCompanyStaticImages(int companyId, int branchId)
        {
            return await ProcessingAsync(_mainService, async (svc) => await svc.FilterAsync<Image, ImageDto>(x =>
                            (x.ObjectID == branchId || x.ObjectID == companyId) &&
                            x.ObjectType == ImageType.Company &&
                            x.IsDeleted == false));
        }

        public Task UpdateImageAsync(int imageId, ImageDto image)
        {
            throw new NotImplementedException();
        }

        public int AddImage(ImageDto image)
        {
            _mainService.Create<Image, ImageDto>(image);
            return image.ImageID;
        }

        public ImageDto GetImageById(int imageId)
        {
            return _mainService.FindById<Image, ImageDto>(imageId);
        }

        public async Task<List<ImageDto>> GetImageByListingIdAsync(int ListingId)
        {
            return await ProcessingAsync(_mainService, async (svc) =>
            {
                var images = await svc.FilterAsync<Image, ImageDto>(x => x.ListingID == ListingId && x.IsDeleted == false);
                return images;
            });
        }

        public async Task<ImageDto> GetFirstImageByListingIdAsync(int ListingId)
        {
            return await ProcessingAsync(_mainService, async (svc) =>
            {
                var images = await svc.FilterAsync<Image, ImageDto>(x => x.ListingID == ListingId && x.IsDeleted == false);
                var firstImage = images.OrderBy(x => x.ImageIndex).FirstOrDefault();
                return firstImage;
            });
        }

        public ImageDto GetFirstImageByListingId(int ListingId)
        {
            return Processing(_mainService, (svc) =>
            {
                var images = svc.Filter<Image, ImageDto>(x => x.ListingID == ListingId && x.IsDeleted == false);
                var firstImage = images.OrderBy(x => x.ImageIndex).FirstOrDefault();
                return firstImage;
            });
        }

        public async Task DeleteImageByListingIdAsync(int ListingId)
        {
            await ProcessingAsync(_mainService, async (svc) =>
            {
                var images = await svc.FilterAsync<Image, ImageDto>(x => x.ListingID == ListingId && x.IsDeleted == false);
                foreach (var image in images)
                {
                    image.IsDeleted = true;
                    svc.Update<Image, ImageDto>(image.ImageID, image);
                }
            });
        }

        public void UpdateImage(int imageId, ImageDto image)
        {
            _mainService.Update<Image, ImageDto>(imageId, image);
        }

        public ImageDto PutImageToS3(string photoUrl, int objectId, ImageType imageType, int companyID)
        {
            var urlWithoutDomain = new Uri(photoUrl).AbsolutePath;
            if (urlWithoutDomain.StartsWith("/"))
            {
                urlWithoutDomain = urlWithoutDomain.Remove(0, 1);
            }
            var filePath = AppDomain.CurrentDomain.BaseDirectory +
                           urlWithoutDomain.Replace("/", "\\");
            var fileExtension = Path.GetExtension(photoUrl);

            if (objectId == 0)
            {
                throw new Exception("Invalid listing ID.");
            }

            AmazonS3 s3 = new AmazonS3();

            // generate S3 keyName and uirl
            var key = "";
            switch (imageType)
            {
                case ImageType.Listing:
                    key = S3ObjectPathFactory.GetAdsImageFolder(companyID, objectId);
                    break;

                case ImageType.Company:
                    key = S3ObjectPathFactory.GetCompanyImageFolder(companyID);
                    break;
            }

            var s3KeyName = key + S3ObjectPathFactory.GenerateFileName(fileExtension);
            var s3Url = S3ObjectPathFactory.GetUrl(s3KeyName);

            _logger.InfoFormat("Upload file {0} to {1}", filePath, s3Url);

            // Ad file to Amazon S3
            string uploadedFileUrl = s3.AddFile(s3KeyName, filePath);

            // check file integrity
            S3ImageVerifier verifier = new S3ImageVerifier(filePath, uploadedFileUrl);
            if (!verifier.IsIntegrity())
            {
                // throw exception right away
                throw new Exception("File uploaded on S3 is not integrity");
            }

            // Delete local file
            System.IO.File.Delete(filePath);

            _logger.InfoFormat("Uploaded successfull");

            return new ImageDto()
            {
                S3ID = s3KeyName,
                URL = uploadedFileUrl
            };
        }

        public ImageDto PutImageToTempS3(HttpPostedFileBase file, ImageType imageType, int companyID)
        {
            var fileName = file.FileName;
            var fileExtension = Path.GetExtension(fileName);
            var fileSize = file.ContentLength;

            AmazonS3 s3 = new AmazonS3();

            // generate S3 keyName and uirl
            var key = "";
            switch (imageType)
            {
                case ImageType.Listing:
                    key = S3ObjectPathFactory.GetAdsImageTempFolder(companyID);
                    break;

                case ImageType.Company:
                    key = S3ObjectPathFactory.GetCompanyImageTempFolder(companyID);
                    break;
            }

            var s3KeyName = key + S3ObjectPathFactory.GenerateFileName(fileExtension);
            var s3Url = S3ObjectPathFactory.GetUrl(s3KeyName);

            _logger.InfoFormat("Upload file {0} to {1}", fileName, s3Url);

            // Ad file to Amazon S3
            string uploadedFileUrl = s3.AddFile(s3KeyName, file.InputStream);

            // check file integrity
            //S3ImageVerifier verifier = new S3ImageVerifier(file.InputStream, uploadedFileUrl);
            //if (!verifier.IsIntegrity())
            //{
            //    // throw exception right away
            //    throw new Exception("File uploaded on S3 is not integrity");
            //}

            _logger.InfoFormat("Uploaded successfull");

            return new ImageDto()
            {
                S3ID = s3KeyName,
                URL = uploadedFileUrl,
                ObjectType = imageType,
                DateCreated = DateTime.Now
            };
        }

        public ImageDto LeechImageFromS3ToS3(string photoUrl, int objectId, ImageType imageType, int companyID)
        {
            var fileExtension = Path.GetExtension(photoUrl);

            if (objectId == 0)
            {
                throw new Exception("Invalid listing ID.");
            }

            AmazonS3 s3 = new AmazonS3();

            // generate S3 keyName and uirl
            var key = "";
            switch (imageType)
            {
                case ImageType.Listing:
                    key = S3ObjectPathFactory.GetAdsImageFolder(companyID, objectId);
                    break;

                case ImageType.Company:
                    key = S3ObjectPathFactory.GetCompanyImageFolder(companyID);
                    break;
            }

            var s3KeyName = key + S3ObjectPathFactory.GenerateFileName(fileExtension);
            var s3Url = S3ObjectPathFactory.GetUrl(s3KeyName);

            _logger.InfoFormat("Upload file {0} to {1}", photoUrl, s3Url);

            // Ad file to Amazon S3
            string uploadedFileUrl = s3.AddFile(s3KeyName, photoUrl);

            // check file integrity
            //S3ImageVerifier verifier = new S3ImageVerifier(photoUrl, uploadedFileUrl);
            //if (!verifier.IsIntegrity())
            //{
            //    // throw exception right away
            //    throw new Exception("File uploaded on S3 is not integrity");
            //}

            // Remove file from S3
            s3.RemoveFile(S3ObjectPathFactory.GetKeyFromUrl(photoUrl));

            _logger.InfoFormat("Uploaded successfull");

            return new ImageDto()
            {
                S3ID = s3KeyName,
                URL = uploadedFileUrl,
                ObjectType = imageType,
                DateCreated = DateTime.Now
            };
        }
    }
}