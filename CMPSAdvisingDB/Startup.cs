using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CMPSAdvisingDB.Startup))]
namespace CMPSAdvisingDB
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
