using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AzureKienThao.Startup))]
namespace AzureKienThao
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
