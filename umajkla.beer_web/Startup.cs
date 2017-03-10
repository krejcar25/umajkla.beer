using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(umajkla.beer.Startup))]
namespace umajkla.beer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
