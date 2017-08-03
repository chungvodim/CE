using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BGP.Utils.Common.AmazonLibrary
{
    #region JSON Serializer

    public class ApplyDateTimeConverter<T> : JsonConverter
    {
        private readonly T _dbObject;
        private readonly CultureInfo _culture;
        private static readonly DateTime _nullDate = new DateTime(1900, 1, 1);

        public ApplyDateTimeConverter(T objectFromDB, CultureInfo culture)
        {
            _dbObject = objectFromDB;
            _culture = culture;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == _dbObject.GetType();
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because CanWrite is false. The type will skip the converter.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartObject)
            {
                serializer.Populate(reader, _dbObject);
            }
            else
            {
                JObject jo = JObject.Load(reader);
                // loop each json property in json string
                setJsonValueForTargetObject(jo, _dbObject);
            }

            return _dbObject;
        }

        private Nullable<DateTime> safeDateTimeCulture(object inputObj, bool outputIsNullable)
        {
            string input = inputObj != null ? inputObj.ToString() : "";
            if (string.IsNullOrEmpty(input))
            {
                return outputIsNullable ? new Nullable<DateTime>() : _nullDate;
            }
            else
            {
                // declare a pattern for datetime input
                var match = Regex.Match(input, @"(\d{1,2})/(\d{1,2})/(\d{4})( \d{1,2}:\d{1,2}:\d{1,2})?( [AP]M)?");
                if (!match.Success)
                {
                    throw new Exception(input + " is invalid DateTime");
                }

                var grps = match.Groups;
                string day = grps[1].Value;
                string month = grps[2].Value;
                string year = grps[3].Value;
                string time = grps[4].Value + grps[5].Value;

                // if not culture US and there're a typo that made day and month switched position
                if (_culture.Name != "en-US" && int.Parse(month) > 12)
                {
                    // then we switch it back to make it right
                    return DateTime.Parse(string.Format("{0}/{1}/{2}{3}", month, day, year, time), _culture.DateTimeFormat);
                }

                return DateTime.Parse(match.ToString(), _culture.DateTimeFormat);
            }
        }

        private void setJsonValueForTargetObject(JObject jo, object targetObject)
        {
            // loop each KeyValuePair(Of jsonPropertyName, jsonPropertyValue) in JsonObject
            foreach (var kv in jo)
            {
                var objType = targetObject.GetType();
                PropertyInfo[] props = objType.GetProperties();
                PropertyInfo p = props.Where(x => x.Name == kv.Key).FirstOrDefault();
                // we only set value for target object's property that exists in json object's property
                if (p != null && p.CanRead && p.CanWrite)
                {
                    if (kv.Value.Type == JTokenType.Object)
                    {
                        // check current targetObject is instantiated (created)
                        object visitingObject = p.GetValue(targetObject);
                        object newInstance = null;
                        if (visitingObject == null) // did not instantiate yet
                        {
                            // instantiate object
                            newInstance = Activator.CreateInstance(p.PropertyType);
                            p.SetValue(targetObject, newInstance);
                        }

                        setJsonValueForTargetObject((JObject)kv.Value, newInstance);
                    }
                    else if (kv.Value.Type == JTokenType.Array)
                    {
                        var arr = kv.Value.ToObject(p.PropertyType);
                        p.SetValue(targetObject, arr);
                    }
                    else
                    {
                        JValue jv = (JValue)kv.Value;
                        object v = null;
                        Type t = p.PropertyType;

                        // since Convert.ChangeType func does not know Nullable
                        // so we have to check if current property of object from DB is Nullable<T>
                        // then specify it for the Convert.ChangeType
                        if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            // check property is DateTime?
                            if (t == typeof(Nullable<DateTime>))
                            {
                                v = safeDateTimeCulture(jv.Value, true);
                            }
                            else if (jv.Value != null)
                            {
                                if (jv.Value.ToString() != "") // this data type is not "empty"
                                {
                                    // try to convert any data types that are not empty except string. Otherwise it should be null 
                                    v = jv.ToObject(t);
                                }
                                else if (t == typeof(string)) // special case for string since it can be empty
                                {
                                    v = string.Empty;
                                }
                            }
                            // otherwise, default value should be null for Nullable of Type data
                        }
                        else
                        {
                            // check property is DateTime
                            if (t == typeof(DateTime))
                            {
                                v = safeDateTimeCulture(jv.Value, false);
                            }
                            else if (jv.Value != null)
                            {
                                if (jv.Value.ToString() != "")
                                {
                                    v = jv.ToObject(t);
                                }
                                else if (t == typeof(string))
                                {
                                    v = string.Empty;
                                }
                            }
                            else
                            {
                                v = getDefaultValue(t);
                            }
                        }

                        p.SetValue(targetObject, v, null);

                    }
                }
            }
        }

        private object getDefaultValue(Type t)
        {
            if (t.IsValueType) // not reference type
            {
                return Activator.CreateInstance(t);
            }
            else if (t == typeof(string)) // special case: string is not ValueType
            {
                // default string value should be null anyway
            }
            return null;
        }

        private void walkNode(JToken node, Action<JObject> action)
        {
            if (node.Type == JTokenType.Object)
            {
                action((JObject)node);

                foreach (JProperty child in node.Children<JProperty>())
                {
                    walkNode(child.Value, action);
                }
            }
            else if (node.Type == JTokenType.Array)
            {
                foreach (JToken child in node.Children())
                {
                    walkNode(child, action);
                }
            }
        }
    }
    #endregion

    #region RenderThumbAndTotalPhotoNum
    public class ImageRenderer
    {
        HttpContext _ctx = null;
        private string _src = "";
        private string _mark = "";

        // Constructor
        public ImageRenderer(HttpContext context, string imgURL, string waterMark)
        {
            _ctx = context;
            _src = imgURL;
            _mark = waterMark;
            if (_mark == "0") _mark = "";
        }

        #region Private Methods

        private Bitmap getStreamBitmap(string imgURL)
        {
            WebRequest request = WebRequest.Create(imgURL);
            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            return new Bitmap(responseStream);
        }

        private Bitmap getThumb()
        {
            string url = "";
            if (string.IsNullOrEmpty(_src) && _ctx != null)
            {
                return (Bitmap)Bitmap.FromFile(_ctx.Server.MapPath("~/images/no-image-thumb.jpg"));
            }
            else if (_src.Contains("s3.amazonaws.com") && _src.Contains("protool"))
            {
                string ex = Path.GetExtension(_src);
                url = _src.Replace(ex, "_th" + ex);
            }

            if (url != "") // has thumb
            {
                return getStreamBitmap(url);
            }
            else
            {
                Bitmap bmp = getStreamBitmap(_src);
                //return bmp;
                int height = bmp.Height > 70 ? 70 : bmp.Height;

                int sourceWidth = bmp.Width;
                int sourceHeight = bmp.Height;
                float nPercent = ((float)height / (float)sourceHeight);
                int destHeight = height;
                int destWidth = (int)(Math.Round(sourceWidth * nPercent, 0));

                int width = (int)(Math.Round(sourceWidth * nPercent, 0));

                return new Bitmap(bmp.GetThumbnailImage(width, height, null, IntPtr.Zero));
            }
        }

        private Bitmap drawWatermark(Bitmap bmp)
        {
            // create new empty frame with width and height
            Bitmap newFrame = new Bitmap(bmp.Width, bmp.Height);
            newFrame.SetResolution(200f, 200f);
            // manupilate the frame using Graphic
            using (Graphics g = Graphics.FromImage(newFrame))
            {
                // set quality
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                g.TextContrast = 12;

                // draw bmp onto the empty frame, they will fit together
                g.DrawImage(bmp, new Rectangle(0, 0, newFrame.Width, newFrame.Height), 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel);
                // determine the size of the Watermark text.
                Font f = new Font("Tahoma", 11, FontStyle.Bold, GraphicsUnit.Pixel);
                SizeF textSize = g.MeasureString(_mark, f);
                // set x and y position at the right corner of the bitmap
                float xPos = bmp.Width - textSize.Width - 8; //  (float)(bmp.Width * 0.2);
                float yPos = bmp.Height - textSize.Height - 8; //(float)(bmp.Height * 0.3);
                // draw a rectangle around text
                RectangleF rf = new RectangleF(xPos, yPos, textSize.Width + 3, textSize.Height + 2);
                fillRoundedRectangle(g, Rectangle.Round(rf), 10, new SolidBrush(Color.FromArgb(231, 76, 60)));
                // draw wartermark text at the center of rectangle
                g.DrawString(_mark, f, new SolidBrush(Color.White), rf, new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }
            return newFrame;
        }

        private void fillRoundedRectangle(Graphics g, Rectangle r, int d, Brush b)
        {
            g.FillPie(b, r.X, r.Y, d, d, 180, 90);
            g.FillPie(b, r.X + r.Width - d, r.Y, d, d, 270, 90);
            g.FillPie(b, r.X, r.Y + r.Height - d, d, d, 90, 90);
            g.FillPie(b, r.X + r.Width - d, r.Y + r.Height - d, d, d, 0, 90);
            g.FillRectangle(b, Convert.ToInt32(r.X + d / 2), r.Y, r.Width - d, Convert.ToInt32(d / 2));
            g.FillRectangle(b, r.X, Convert.ToInt32(r.Y + d / 2), r.Width, Convert.ToInt32(r.Height - d));
            g.FillRectangle(b, Convert.ToInt32(r.X + d / 2), Convert.ToInt32(r.Y + r.Height - d / 2), Convert.ToInt32(r.Width - d), Convert.ToInt32(d / 2));
        }

        #endregion

        public Bitmap GetImage()
        {
            // 1. create a bitmap from src
            Bitmap bmp = getThumb();
            // 2. draw watermark if available
            if (!string.IsNullOrEmpty(_mark))
            {
                bmp = drawWatermark(bmp);
            }
            return bmp;
        }

        #region Async Methods

        public async Task<Bitmap> GetImageAsync()
        {
            var t = Task<Bitmap>.Factory.StartNew(() => { return getThumb(); });
            if (!string.IsNullOrEmpty(_mark))
            {
                return await t.ContinueWith<Bitmap>(x => { return drawWatermark(x.Result); }, TaskContinuationOptions.OnlyOnRanToCompletion);
            }
            return await t;
        }

        #endregion

    }
    #endregion

    #region Verify Image Integrity

    public class S3ImageVerifier
    {
        private readonly Stream _stream;
        private readonly string _filePathBeforeUpload;
        private readonly string _uploadedFileUrl;

        public S3ImageVerifier(string filePathBeforeUpload, string uploadedFileURL)
        {
            _filePathBeforeUpload = filePathBeforeUpload;
            _uploadedFileUrl = uploadedFileURL;
        }

        public S3ImageVerifier(Stream stream , string uploadedFileURL)
        {
            _stream = stream;
            _uploadedFileUrl = uploadedFileURL;
        }

        public bool IsIntegrity()
        {
            string hash = "";
            string eTag = "";
            // hash md5 file
            using (var md5 = MD5.Create())
            {
                if(_stream == null)
                {
                    using (var stream = File.OpenRead(_filePathBeforeUpload))
                    {
                        hash = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                    }

                }
                else
                {
                    hash = BitConverter.ToString(md5.ComputeHash(_stream)).Replace("-", "").ToLower();
                }
            }

            if (!string.IsNullOrEmpty(hash))
            {
                // get ETag from HEAD Request on Amazon S3
                WebRequest webRequest = WebRequest.Create(_uploadedFileUrl);
                webRequest.Method = "HEAD";
                using (WebResponse webResponse = webRequest.GetResponse())
                {
                    eTag = webResponse.Headers["ETag"];
                }
            }

            if (!string.IsNullOrEmpty(eTag))
            {
                // remove double quote if have
                eTag = eTag.Replace("\"", "");
                return eTag.ToLower() == hash;
            }

            return false;
        }

    }

    #endregion

    #region Caching
    /// <summary>
    /// Base class for caching data using HttpContext.Current.Cache
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CachingData<T>
    {
        public string Key;
        protected abstract string _CachePrefix { get; }
        protected int _id;
        protected HttpContext _Context
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    throw new Exception("Could not access HttpContext.Current");
                }
                return HttpContext.Current;
            }
        }

        public CachingData(int id)
        {
            _id = id;
            Key = string.Format("{0}_{1}", _CachePrefix, id);
        }

        public abstract T GetCacheItem();
        public bool RemoveCacheItem()
        {
            return _Context.Cache.Remove(Key) != null;
        }
    }

    public class CachingPropertyImage : CachingData<KeyValuePair<string, int>>
    {
        public CachingPropertyImage(int propertyID) : base(propertyID) { }

        protected override string _CachePrefix
        {
            get { return "PROPERTY_IMAGE"; }
        }

        public override KeyValuePair<string, int> GetCacheItem()
        {
            // try get id from cache first
            var obj = base._Context.Cache.Get(base.Key);
            if (obj != null)
            {
                return (KeyValuePair<string, int>)obj;
            }
            else
            {
                var result = new KeyValuePair<string, int>("", 0);
                return result;
            }
        }
    }
    #endregion

    #region DateTime
    public static class DateTimeExtension
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }
            return dt.AddDays(-1 * diff).Date;
        }
        public static DateTime EndOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            return dt.StartOfWeek(startOfWeek).AddDays(6);
        }

        public static DateTime StartOfMonth(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1);
        }

        public static DateTime EndOfMonth(this DateTime dt)
        {
            return dt.StartOfMonth().AddMonths(1).AddDays(-1);
        }

        public static DateTime StartOfYear(this DateTime dt)
        {
            return new DateTime(dt.Year, 1, 1);
        }

        public static DateTime EndOfYear(this DateTime dt)
        {
            return dt.StartOfYear().AddYears(1).AddDays(-1);
        }
    }

    public struct DateTimeWithZone
    {
        private readonly DateTime utcDateTime;
        private readonly TimeZoneInfo timeZone;

        public DateTimeWithZone(DateTime dateTime, TimeZoneInfo timeZone)
        {
            utcDateTime = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified), timeZone);
            this.timeZone = timeZone;
        }

        public DateTime UniversalTime { get { return utcDateTime; } }

        public TimeZoneInfo TimeZone { get { return timeZone; } }

        public DateTime LocalTime
        {
            get
            {
                return TimeZoneInfo.ConvertTime(utcDateTime, timeZone);
            }
        }
    }
    #endregion
}