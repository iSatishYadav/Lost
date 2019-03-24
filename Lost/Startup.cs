using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Lost.Startup))]

namespace Lost
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //
        }
    }
}
