using Microsoft.Owin;
using Owin;
using BGP.Utils.Common;

[assembly: OwinStartupAttribute(typeof(CE.Web.Startup))]
namespace CE.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            // Check first login
            app.Use(async (context, next) =>
            {
                var isSuperAdmin = context.Authentication.User.IsInRole(CE.Enum.UserRole.SuperAdmin.GetDescription())
                || context.Authentication.User.Identity.IsImpersonated();
                if (isSuperAdmin)
                {
                    await next.Invoke();
                }
                else
                {
                    var agreeTermsPath = "/Account/AgreeTerms";
                    var isCurrentAgreeTermsPath = context.Request.Path.Value == agreeTermsPath;
                    var identity = context.Authentication.User.Identity;
                    if (identity.IsAuthenticated && identity.GetConsent() == false)
                    {
                        if (isCurrentAgreeTermsPath && context.Request.Method == "POST")
                        {
                            await next.Invoke();
                        }
                        else if (isCurrentAgreeTermsPath == false)
                        {
                            context.Response.Redirect(agreeTermsPath);
                        }
                        else
                        {
                            await next.Invoke();
                        }
                    }
                    else
                    {
                        await next.Invoke();
                    }
                }
            });
        }
    }
}
