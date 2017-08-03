using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BGP.Utils.Common
{
    public static class ImageHelper
    {
        public static string GetMimeType(this System.Drawing.Image image)
        {
            return image.RawFormat.GetMimeType();
        }

        public static string GetMimeType(this ImageFormat imageFormat)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            return codecs.First(codec => codec.FormatID == imageFormat.Guid).MimeType;
        }

        public static string GetImageNameFromUrl(string imageUrl)
        {
            string fileName = "";
            Uri uri = new Uri(imageUrl);
            fileName = Path.GetFileName(uri.AbsolutePath);
            return fileName;
        }

        public static byte[] GetBinaryFromUrl(string imageUrl)
        {
            byte[] raw;
            using (var client = new WebClient())
            {
                raw = client.DownloadData(imageUrl);
            }
            return raw;
        }

        public static Image GetImageFromUrl(string imageUrl)
        {
            var image = new Image();

            WebClient client = new WebClient();
            Stream stream = client.OpenRead(imageUrl);

            System.Drawing.Image bitmap;
            bitmap = new Bitmap(stream);

            image.ContentType = bitmap.GetMimeType();
            ImageConverter converter = new ImageConverter();
            image.Content = (byte[])converter.ConvertTo(bitmap, typeof(byte[]));
            image.ImageName = GetImageNameFromUrl(imageUrl);

            stream.Flush();
            stream.Close();
            client.Dispose();

            return image;
        }

        public static bool IsValidImage(string fileExtension, byte[] bytes)
        {
            try
            {
                if (!IsValidExtension(fileExtension))
                {
                    return false;
                }

                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    System.Drawing.Image.FromStream(ms);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static bool IsValidImage(string fileExtension, Stream stream)
        {
            try
            {
                if (!IsValidExtension(fileExtension))
                {
                    return false;
                }

                System.Drawing.Image.FromStream(stream);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static bool IsValidExtension(string extension)
        {
            extension = extension.ToLower().Replace(".", "");
            if (Configuration.validImageExtension.Split(',').Contains(extension))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class Image
    {
        public const string DEFAULT_IMAGE_CONTENTTYPE = "image/png";

        [JsonIgnore]
        public readonly string DEFAULT_IMAGE_NAME = string.Format("{0}.png", Guid.NewGuid().ToString());
        public Image()
        {
            this.ImageName = DEFAULT_IMAGE_NAME;
            this.ContentType = DEFAULT_IMAGE_CONTENTTYPE;
        }
        public Image(long byteLength)
        {
            this.Content = new byte[byteLength];
            this.ImageName = DEFAULT_IMAGE_NAME;
            this.ContentType = DEFAULT_IMAGE_CONTENTTYPE;
        }
        public Image(long byteLength, string contentType)
        {
            this.Content = new byte[byteLength];
            this.ContentType = contentType;
            this.ImageName = DEFAULT_IMAGE_NAME;
        }
        public Image(long byteLength, string imageName, string contentType)
        {
            this.Content = new byte[byteLength];
            this.ImageName = imageName;
            this.ContentType = contentType;
        }
        public string ImageName { get; set; }

        [JsonIgnore]
        public string ContentType { get; set; }
        [JsonIgnore]
        public byte[] Content { get; set; }
    }
}
