using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BGP.Utils.Common
{
    public static class EnumHelper
    {
        public static string GetDescription(this object obj)
        {
            var des = obj.GetAttribute<DescriptionAttribute>();

            return des == null ? null : des.Description;
        }

        public static TAttribute GetAttribute<TAttribute>(this object obj) where TAttribute : Attribute
        {
            var descriptionAttr = obj.GetType().GetField(obj.ToString()).GetCustomAttributes(typeof(TAttribute), false).FirstOrDefault() as TAttribute;

            return descriptionAttr;
        }

        public static TAttribute GetEnumAttribute<TAttribute>(this Enum enumObj) where TAttribute : Attribute
        {
            MemberInfo memberInfo = enumObj.GetType().GetMember(enumObj.ToString()).FirstOrDefault();

            if (memberInfo != null)
            {
                var attribute = (TAttribute)memberInfo.GetCustomAttributes(typeof(TAttribute), false).FirstOrDefault();
                return attribute;
            }
            return null;
        }

        public static List<TEnum> GetEnumsByDescription<TEnum>(string description) where TEnum : struct
        {
            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Where(x => x.GetDescription() == description)
                .ToList();
        }

        public static string GetEnumValue(this Enum enumObj)
        {
            var enumInfo = enumObj.GetEnumAttribute<EnumInformationAttribute>();

            if (enumInfo != null)
            {
                return enumInfo.Value;
            }
            else
            {
                throw new Exception("Enum is not marked with EnumInformationAttribute");
            }
        }

        public static IEnumerable<Enum> GetFlags(this Enum input)
        {
            foreach (Enum value in Enum.GetValues(input.GetType()))
                if (input.HasFlag(value))
                    yield return value;
        }

        public static string GetEnumDisplayText(this Enum enumVal)
        {
            string displayText = enumVal.ToString();

            var enumInfo = enumVal.GetEnumAttribute<EnumInformationAttribute>();

            if (enumInfo != null)
            {
                if (enumInfo.ResourceType != null && !string.IsNullOrEmpty(enumInfo.ResourceName))
                {
                    var property = enumInfo.ResourceType.GetProperty(enumInfo.ResourceName);

                    if (property == null)
                    {
                        throw new Exception(string.Format("Resource name \"{0}\" is not found in type \"{1}\"", enumInfo.ResourceName, enumInfo.ResourceType.FullName));
                    }

                    displayText = property.GetValue(null).ToString();
                }
                else if (!string.IsNullOrEmpty(enumInfo.Description))
                {
                    displayText = enumInfo.Description;
                }
            }

            return displayText;
        }

        public static TEnum ToEnum<TEnum>(this string str)
        {
            return (TEnum)Enum.Parse(typeof(TEnum), str);
        }

        public static TEnum ToEnumByResource<TEnum>(this string str)
        {
            foreach (var enumValue in Enum.GetValues(typeof(TEnum)).OfType<TEnum>())
            {
                var text = (enumValue as System.Enum).GetEnumDisplayText();
                if (text.Equals(str, StringComparison.CurrentCultureIgnoreCase))
                {
                    return enumValue;
                }
            }

            return str.ToEnum<TEnum>();
        }

        public static TEnum ToEnumByDescription<TEnum>(this string str)
        {
            foreach (var enumValue in Enum.GetValues(typeof(TEnum)).OfType<TEnum>())
            {
                var text = (enumValue as System.Enum).GetDescription();
                if (text.Equals(str, StringComparison.CurrentCultureIgnoreCase))
                {
                    return enumValue;
                }
            }

            return str.ToEnum<TEnum>();
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class EnumInformationAttribute : Attribute
    {
        public string Value;
        public string Description;
        public Type ResourceType;
        public string ResourceName;
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class IgnoredEnumAttribute : Attribute
    {
    }
}
