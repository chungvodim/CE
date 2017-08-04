using CE.Dto;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace CE.Web.Base
{
    public class CustomActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            
        }
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var modelState = filterContext.Controller.ViewData.ModelState;
            var model = filterContext.Controller.ViewData.Model;
            if(model != null)
            {
                var baseModel = model as BaseDto;
                if (!modelState.IsValid && baseModel != null)
                {
                    baseModel.HttpStatusCode = HttpStatusCode.BadRequest;
                    baseModel.AppendMessage(CE.Web.Base.Utils.GetErrorsFromModelState(modelState));
                }
            }
        }
    }
}