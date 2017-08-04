using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CE.Web.Base.Attribute
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ValidateAntiForgeryTokenWrapperAttribute : FilterAttribute, IAuthorizationFilter
    {
        private readonly ValidateAntiForgeryTokenAttribute _validator;
        private readonly AcceptVerbsAttribute _verbs;
        public ValidateAntiForgeryTokenWrapperAttribute(HttpVerbs verbs)
        {
            this._verbs = new AcceptVerbsAttribute(verbs);
            this._validator = new ValidateAntiForgeryTokenAttribute()
            {
                //Salt = salt
            };
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if(BGP.Utils.Common.Configuration.turnOnValidateAntiForgery == true)
            {
                bool shouldValidate = !filterContext
                .ActionDescriptor
                .GetCustomAttributes(typeof(IgnoredFromAntiForgeryValidationAttribute), true)
                .Any();

                string httpMethodOverride = filterContext.HttpContext.Request.GetHttpMethodOverride();
                if (this._verbs.Verbs.Contains(httpMethodOverride, StringComparer.OrdinalIgnoreCase) && shouldValidate)
                {
                    try
                    {
                        this._validator.OnAuthorization(filterContext);
                    }
                    catch (System.Web.Mvc.HttpAntiForgeryException)
                    {
                        BGP.Utils.Common.ErrorHelper.Logger.InfoFormat("HttpAntiForgeryException - Controller: {0} / Action: {1}", filterContext.Controller.ToString(), filterContext.ActionDescriptor.ActionName);
                        throw;
                    }
                }
            }
        }
    }
}