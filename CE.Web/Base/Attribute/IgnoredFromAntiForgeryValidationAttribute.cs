using System;

namespace CE.Web.Base.Attribute
{
    [AttributeUsage(AttributeTargets.Method)]
    public class IgnoredFromAntiForgeryValidationAttribute : System.Attribute
    {
    }
}