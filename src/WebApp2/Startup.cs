using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Logging;
using IdentityServer3.Core.Services;
using Microsoft.Owin;
using Owin;
using Serilog;
using System;
using System.Security.Cryptography.X509Certificates;

[assembly: OwinStartupAttribute(typeof(WebApp2.Startup))]
namespace WebApp2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            ConfigureAuth(app);
            app.Use(typeof(RequireAuthenticationMiddleware));
            app.Map("/identity", idSrvApp =>
            {
                var factory = new IdentityServerServiceFactory()
                        .UseInMemoryClients(InMemoryConfig.Clients)
                        .UseInMemoryUsers(InMemoryConfig.Users)
                        .UseInMemoryScopes(InMemoryConfig.Scopes);

                //var userService = new UserService();
                factory.UserService = new Registration<IUserService>(typeof(UserService));

                idSrvApp.UseIdentityServer(new IdentityServerOptions
                {
                    SiteName = "TugaIT 2016 IdentityServer3",
                    SigningCertificate = LoadCertificate(),
                    Factory = factory,                            
                });

            });
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Trace()
                .CreateLogger();            
        }

        X509Certificate2 LoadCertificate()
        {
            return new X509Certificate2(
                string.Format(@"{0}\bin\idsrv3test.pfx", AppDomain.CurrentDomain.BaseDirectory), "idsrv3test");
        }
    }
}
