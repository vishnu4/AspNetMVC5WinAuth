using AspNetMVC5WinAuth.App_Start;
using AspNetMVC5WinAuth.DependencyResolution.Custom;
using AspNetMVC5WinAuth.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WIndowsAuthCommon.Identity;
using WIndowsAuthCommon.Models;

[assembly: OwinStartup(typeof(AspNetMVC5WinAuth.Startup))]
namespace AspNetMVC5WinAuth
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var container = App_Start.StructuremapMvc.StructureMapDependencyScope.Container;
            ConfigureAuth(app);
            ConfigureOAuth(app, container);
            HttpConfiguration config = new HttpConfiguration();

            WebApiConfig.Register(config);
            //http://cdroulers.com/blog/2015/03/03/structuremap-3-and-asp-net-web-api-2/
            config.Services.Replace(typeof(System.Web.Http.Dispatcher.IHttpControllerActivator), new StructureMapWebApiControllerActivator(container));
            app.UseCors(CorsOptions.AllowAll);
            app.UseWebApi(config);
        }

        private void ConfigureAuth(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            CookieAuthenticationOptions cookieOpt = new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                AuthenticationMode = AuthenticationMode.Active,
                SlidingExpiration = true,
                ExpireTimeSpan = TimeSpan.FromMinutes(525600),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<CustomUserManager, CustomUser>(
                            validateInterval: TimeSpan.FromMinutes(30),
                            regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager, DefaultAuthenticationTypes.ApplicationCookie)),
                    OnApplyRedirect = ctx =>
                    {
                        if (!IsAjaxRequest(ctx.Request))
                        {
                            ctx.Response.Redirect(ctx.RedirectUri);
                        }
                    }
                }
            };

            if (!Helpers.WebConfigSettings.UseWindowsAuthentication)        //no need for any of this part if we're authenticating via windows
            {
                cookieOpt.LogoutPath = new PathString("/Account/Logoff");
                cookieOpt.LoginPath = new PathString("/Account/LogOn");

                app.UseCookieAuthentication(cookieOpt);

                app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

                // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
                app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

                // Enables the application to remember the second login verification factor such as phone or email.
                // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
                // This is similar to the RememberMe option when you log in.
                app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
            }
        }

        public static string PublicClientId { get; private set; }
        public void ConfigureOAuth(IAppBuilder app, IContainer container)
        {
            PublicClientId = "self";
            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
#if DEBUG
                AllowInsecureHttp = true,
#else
                AllowInsecureHttp = false,
#endif
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                Provider = container.GetInstance<ApplicationOAuthProvider>(),
                ApplicationCanDisplayErrors = true
            };

            app.UseOAuthBearerTokens(OAuthServerOptions);
        }

        public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
        {
            private CustomUserManager m_UserManager;

            public ApplicationOAuthProvider(CustomUserManager usrManager)
            {
                m_UserManager = usrManager;
            }

            //http://bitoftech.net/2014/06/01/token-based-authentication-asp-net-web-api-2-owin-asp-net-identity/
            public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
            {
                if (m_UserManager != null)
                {
                    CustomUser user = null;
                    user = await m_UserManager.FindByNameAsync(context.UserName);
                    string hashedPassword = CustomPasswordHasher.GetPasswordAfterHashing(context.Password, user);
                    if (string.Compare(user.PasswordHash, hashedPassword, System.StringComparison.Ordinal) != 0)
                    {
                        user = null;
                    }

                    if (user == null)
                    {
                        context.SetError("invalid_grant", "The user name or password is incorrect.");
                        return;
                    }

                    ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(m_UserManager, OAuthDefaults.AuthenticationType);


                    AuthenticationProperties properties = CreateProperties(user.UserName);
                    AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
                    context.Validated(ticket);

                    //what is this for exactly?
                    ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(m_UserManager, CookieAuthenticationDefaults.AuthenticationType);
                    context.Request.Context.Authentication.SignIn(cookiesIdentity);
                }
            }
        }
        private static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }
        private static bool IsAjaxRequest(IOwinRequest request)
        {
            IReadableStringCollection query = request.Query;
            if ((query != null) && (query["X-Requested-With"] == "XMLHttpRequest"))
            {
                return true;
            }
            IHeaderDictionary headers = request.Headers;
            return ((headers != null) && (headers["X-Requested-With"] == "XMLHttpRequest"));
        }
    }
}