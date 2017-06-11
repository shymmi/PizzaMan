using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PizzaMan.Startup))]
namespace PizzaMan
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
