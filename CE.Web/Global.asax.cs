using CE.Bootstrapper;
using BGP.Utils.Common;
using CE.Web.Base.Binder;
using DryIoc;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using DryIoc.Mvc;

namespace CE.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            log4net.Config.XmlConfigurator.Configure();

            AutoMapper.Mapper.Initialize(cfg =>
            {
                Bootstrapper.AutoMapper.Configure(cfg);
            });

            var container = new Container();
            Bootstrapper.DryIoC.Configure(container, Reuse.InWebRequest);

            container.WithMvc();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            string message = "Something went wrong";
            int errorCode = 0;

            var httpException = ex as HttpException;

            if (httpException != null)
            {
                message = httpException.Message;
                errorCode = httpException.GetHttpCode();
                switch (errorCode)
                {
                    case 404:
                        // page not found
                        message = "We can't seem to find the page you're looking for.";
                        break;
                    case 500:
                        // server error
                        message = "server error";
                        break;
                    default:
                        break;
                }
            }
            ErrorHelper.Logger.Error(message, ex);
            Server.ClearError();
            Response.Redirect(string.Format("/Error/Index?errorCode={0}&message={1}", errorCode, message));
        }

        public override string GetVaryByCustomString(HttpContext context, string arg)
        {
            if (arg.Equals("User", StringComparison.InvariantCultureIgnoreCase))
            {
                return string.Format("{0}@{1}", User.Identity.GetUserId<int>(), context.User.Identity.Name);
            }

            return base.GetVaryByCustomString(context, arg);
        }
    }
}
