using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Diplom.Startup))]
namespace Diplom
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
