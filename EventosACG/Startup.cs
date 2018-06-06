using EventosACG.Models;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EventosACG.Startup))]
namespace EventosACG
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

       
    }
}
