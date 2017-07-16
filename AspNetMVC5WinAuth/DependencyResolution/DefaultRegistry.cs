using AspNetMVC5WinAuth.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using StructureMap;
using System.Web;
using WIndowsAuthCommon;
using WIndowsAuthCommon.Identity;
using WIndowsAuthCommon.Models;

namespace AspNetMVC5WinAuth.DependencyResolution {
    
	
    public class DefaultRegistry : Registry {

        public DefaultRegistry() {
     //       Scan(
     //           scan => {
     //               scan.TheCallingAssembly();
     //               scan.WithDefaultConventions();
					//scan.With(new ControllerConvention());
     //           });

            For<HttpContext>()
                .AlwaysUnique().Use(ctx => HttpContext.Current);

            For<System.Web.Routing.RequestContext>()
                .AlwaysUnique()
                .Use(ctx => ctx.GetInstance<HttpContext>().Request.RequestContext);

            For<System.Web.Mvc.UrlHelper>()
                .AlwaysUnique()
                .Use(ctx => new System.Web.Mvc.UrlHelper(ctx.GetInstance<System.Web.Routing.RequestContext>()));

            For<AuthContext>().Add<AuthContext>()
                .Ctor<string>("nameOrConnectionString")
                .Is(Helpers.WebConfigSettings.SQLConnectionString)
                .SelectConstructor(() => new AuthContext());//force use of default constructor!
            For<System.Data.Entity.DbContext>().Use(c => c.GetInstance<AuthContext>());
            For<IOwinContext>().Transient().Use(() => System.Web.HttpContext.Current.GetOwinContext());
            For<IAuthenticationManager>().Transient().Use(c => c.GetInstance<IOwinContext>().Authentication);
            For<IUserStore<CustomUser>>().Add<CustomUserStore<CustomUser>>();
            For<UserManager<CustomUser>>().Use<CustomUserManager>();
            For<ITokenHolder>().Use<TokenHolder>();
            For<CustomSignInManager>().Use<CustomSignInManager>();
        }
        
    }
}