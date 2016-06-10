using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Ko.PoC.Web.Startup))]
namespace Ko.PoC.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
