using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TowerSite.Startup))]
namespace TowerSite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
