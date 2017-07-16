using AspNetMVC5WinAuth.App_Start;
using AspNetMVC5WinAuth.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WIndowsAuthCommon.Identity;
using WIndowsAuthCommon.Models;

namespace AspNetMVC5WinAuth
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            MvcHandler.DisableMvcResponseHeader = true;
        }

        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            if ((Helpers.WebConfigSettings.UseWindowsAuthentication) && (User.Identity.GetType() == typeof(System.Security.Principal.WindowsIdentity)))   
            {
                //not a fan of this, I'm not sure of the best way to get the usermanager DI instance from structuremap in global.asax
                CustomUserManager userManager = App_Start.StructuremapMvc.StructureMapDependencyScope.Container.GetInstance<CustomUserManager>();
                CustomUser user = userManager.FindByNameAsync(User.Identity.Name).Result;  
                //ApplicationUser user = userManager.FindByName(User.Identity.Name);  
                HttpContext.Current.User = new CustomWinPrincipal((System.Security.Principal.WindowsIdentity)User.Identity, user);
            }
        }
    }
}
