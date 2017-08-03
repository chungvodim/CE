using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BGP.Utils.Common
{
    public static class Reflection
    {
        public static object GetProperty(this object obj, string propertyName)
        {
            string[] props = propertyName.Split('.');
            object value = obj;
            foreach (var p in props)
            {
                if (value == null)
                    return null;
                if (String.IsNullOrEmpty(p))
                    continue;
                var prop = value.GetType().GetProperty(p);
                if (prop == null)
                    throw new Exception(string.Format("Property \"{0}\" is not found in type \"{1}\"", propertyName, obj.GetType().FullName));
                value = prop.GetValue(value, null);
            }
            return value;
        }

        public static void SetProperty(this object obj, string propertyName, object propertyValue)
        {
            var property = obj.GetType().GetProperty(propertyName);

            if (property == null)
            {
                throw new Exception(string.Format("Property \"{0}\" is not found in type \"{1}\"", propertyName, obj.GetType().FullName));
            }

            var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

            if (propertyType.IsEnum)
            {
                propertyValue = Enum.ToObject(propertyType, propertyValue);
            }

            //var safeValue = (propertyValue == null) ? null : Convert.ChangeType(propertyValue, propertyType);

            property.SetValue(obj, propertyValue);
        }

        public static bool IsDerivedFromGenericInterface(this object obj, Type genericType)
        {
            return obj.GetType().GetInterfaces().Any(x =>
                x.IsGenericType &&
                x.GetGenericTypeDefinition() == genericType);
        }

        public static string GetUniqueKey<T>(Expression<Func<T>> propertyExpression)
        {
            var member = (propertyExpression.Body as MemberExpression).Member;
            var typeName = member.ReflectedType.Name;
            var propertyName = member.Name;
            return typeName + "_" + propertyName;
        }
    }
}
