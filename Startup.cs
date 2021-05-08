using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Red_Lake_Hospital_Redesign_Team6.Startup))]
namespace Red_Lake_Hospital_Redesign_Team6
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
