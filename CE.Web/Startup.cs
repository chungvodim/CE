using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CE.Web.Startup))]
namespace CE.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
