using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Niu.Live.Video.Web.Startup))]
namespace Niu.Live.Video.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
