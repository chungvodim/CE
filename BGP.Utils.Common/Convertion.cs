using Newtonsoft.Json;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using System.Xml;

namespace BGP.Utils.Common
{
    public static class Convertion
    {
        public static T To<T>(this object obj, bool defaultOnFailure = true) where T : struct
        {
            try
            {
                return (T)Convert.ChangeType(obj, typeof(T));
            }
            catch
            {
                if (defaultOnFailure)
                {
                    return default(T);
                }
                else
                {
                    throw;
                }
            }
        }

        public static Nullable<T> ToNullable<T>(this object obj, bool treatDefaultAsNull = false) where T : struct
        {
            try
            {
                Nullable<T> result = (T)Convert.ChangeType(obj, typeof(T));
                return (treatDefaultAsNull && object.Equals(result, default(T))) ? null : result;
            }
            catch
            {
                return null;
            }
        }

        public static string ToStringOrDefault(this object obj, string defaultValue = "")
        {
            return obj == null ? defaultValue : obj.ToString();
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static DataTable ConvertToDatatable<T>(List<T> data)
        {
            PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    table.Columns.Add(prop.Name, prop.PropertyType.GetGenericArguments()[0]);
                else
                    table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }

        public static string ToRoman(this int number)
        {
            if ((number < 0) || (number > 3999)) throw new ArgumentOutOfRangeException("insert value betwheen 1 and 3999");
            if (number < 1) return string.Empty;
            if (number >= 1000) return "M" + ToRoman(number - 1000);
            if (number >= 900) return "CM" + ToRoman(number - 900);
            if (number >= 500) return "D" + ToRoman(number - 500);
            if (number >= 400) return "CD" + ToRoman(number - 400);
            if (number >= 100) return "C" + ToRoman(number - 100);
            if (number >= 90) return "XC" + ToRoman(number - 90);
            if (number >= 50) return "L" + ToRoman(number - 50);
            if (number >= 40) return "XL" + ToRoman(number - 40);
            if (number >= 10) return "X" + ToRoman(number - 10);
            if (number >= 9) return "IX" + ToRoman(number - 9);
            if (number >= 5) return "V" + ToRoman(number - 5);
            if (number >= 4) return "IV" + ToRoman(number - 4);
            if (number >= 1) return "I" + ToRoman(number - 1);
            throw new ArgumentOutOfRangeException("something bad happened");
        }

        public static string SerializeToCsv<T>(IEnumerable<T> source, bool useResource = false)
        {
            try
            {
                if (!useResource)
                {
                    return CsvSerializer.SerializeToCsv<T>(source);
                }
                else
                {
                    var json = JsonConvert.SerializeObject(source, new JsonSerializerSettings()
                    {
                        ContractResolver = new ResourceNameContractResolver()
                    });
                    return JsonToCSV(json);
                }
            }
            catch (Exception ex)
            {
                ErrorHelper.Logger.Error("Unable to serialize list to csv", ex);
                return ex.Message;
            }
        }

        public static string JsonToCSV(string jsonContent)
        {
            //used NewtonSoft json nuget package
            XmlNode xml = JsonConvert.DeserializeXmlNode("{records:{record:" + jsonContent + "}}");
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(xml.InnerXml);
            XmlReader xmlReader = new XmlNodeReader(xml);
            DataSet dataSet = new DataSet();
            dataSet.ReadXml(xmlReader);
            var dataTable = dataSet.Tables[0];

            //Datatable to CSV
            StringBuilder sb = new StringBuilder();
            //var lines = new List<string>();
            string[] columnNames = dataTable.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToArray();
            var header = string.Join(",", columnNames);
            sb.AppendLine(header);
            //lines.Add(header);
            var valueLines = dataTable.AsEnumerable().Select(row => string.Join(",", row.ItemArray));
            foreach (var item in valueLines)
            {
                sb.AppendLine(item);
            }
            
            //lines.AddRange(valueLines);
            return sb.ToString(); ;
        }

    }

    public class ResourceNameContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var list = base.CreateProperties(type, memberSerialization);

            foreach (JsonProperty p in list)
            {
                p.PropertyName = this.GetRealNameFromResourceFile(type, p.PropertyName);
            }

            return list;
        }

        private string GetRealNameFromResourceFile(Type type, string propertyName)
        {
            try
            {
                var resource = propertyName;

                var firstChar = Char.ToUpper(propertyName[0]);
                var originalPropertyName = propertyName.Remove(0, 1).Insert(0, firstChar.ToString());

                var property = type.GetProperty(originalPropertyName);
                if (property != null)
                {
                    var propertyInfo = property.GetAttribute<PropertyInformationAttribute>();
                    if (propertyInfo != null)
                    {
                        if (propertyInfo.ResourceType != null && !string.IsNullOrEmpty(propertyInfo.ResourceName))
                        {
                            var resourceProperty = propertyInfo.ResourceType.GetProperty(propertyInfo.ResourceName);

                            if (resourceProperty == null)
                            {
                                throw new Exception(string.Format("Resource name \"{0}\" is not found in type \"{1}\"", propertyInfo.ResourceName, propertyInfo.ResourceType.FullName));
                            }

                            resource = resourceProperty.GetValue(null).ToString();
                        }
                    }
                }

                return resource;
            }
            catch (Exception)
            {
                return propertyName;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class PropertyInformationAttribute : Attribute
    {
        public Type ResourceType;
        public string ResourceName;
    }

    public static class DateTimeHelper
    {
        public static DateTime ToServerTime(this DateTime dateTime, int utcOffset)
        {
            return dateTime.AddHours(-utcOffset + Configuration.ServerUtcOffset);
        }
        public static DateTime ToClientTime(this DateTime dateTime, int utcOffset)
        {
            return dateTime.AddHours(utcOffset - Configuration.ServerUtcOffset);
        }
    }
}
