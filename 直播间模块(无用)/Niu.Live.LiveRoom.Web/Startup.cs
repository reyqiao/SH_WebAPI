using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Niu.Live.LiveRoom.Web.Startup))]
namespace Niu.Live.LiveRoom.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
