using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DCRSystem.Startup))]
namespace DCRSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
