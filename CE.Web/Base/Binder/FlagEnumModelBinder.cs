using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;

namespace CE.Web.Base.Binder
{
    public class FlagEnumModelBinder : DefaultModelBinder
    {
        /// <summary>
        /// Fix for the default model binder's failure to decode flag enum types from chexbox list.
        /// </summary>
        protected override object GetPropertyValue(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, IModelBinder propertyBinder)
        {
            var propertyType = propertyDescriptor.PropertyType;

            // Check if the property type is an enum with the flag attribute
            if (propertyType.IsEnum && propertyType.GetCustomAttributes<FlagsAttribute>().Any())
            {
                var providerValue = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
                if (providerValue != null)
                {
                    var value = providerValue.RawValue;
                    if (value != null)
                    {
                        // In case it is a checkbox list/dropdownlist/radio button list
                        if (value is string[] && (value as string[]).All(x => !string.IsNullOrEmpty(x)))
                        {
                            // Create flag value from posted values
                            var flagValue = ((string[])value).Aggregate(0, (current, v) => current | (int)System.Enum.Parse(propertyType, v));

                            return System.Enum.ToObject(propertyType, flagValue);
                        }
                        else
                        {
                            // In case it is a single value
                            if (value.GetType().IsEnum)
                            {
                                return System.Enum.ToObject(propertyType, value);
                            }
                        }
                    }
                }
            }
            return base.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor, propertyBinder);
        }
    }
}