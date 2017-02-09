using IdentityServer3.Core.Configuration;
using Owin;

namespace IdentityServer
{
    public sealed class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Map(
                "/core",
                coreApp => {
                    coreApp.UseIdentityServer(new IdentityServerOptions
                    {
                        SiteName = "Standalone Identity Server",
                        SigningCertificate = Cert.Load(),
                        Factory = new IdentityServerServiceFactory()
                                .UseInMemoryClients(Clients.Get())
                                .UseInMemoryScopes(Scopes.Get())
                                .UseInMemoryUsers(Users.Get()),
                        RequireSsl = true
                    });
                });
        }
    }
}