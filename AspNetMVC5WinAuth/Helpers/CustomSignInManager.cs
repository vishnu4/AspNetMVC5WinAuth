using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WIndowsAuthCommon.Identity;
using WIndowsAuthCommon.Models;

namespace AspNetMVC5WinAuth.Helpers
{
    public class CustomSignInManager : SignInManager<CustomUser, string>
    {
        public CustomSignInManager(CustomUserManager userManager, IAuthenticationManager authenticationManager) : base(userManager, authenticationManager)
        {
            
        }


        public override async Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
        {
            var usr = await UserManager.FindByNameAsync(userName);
            string providedPasswordAfterHash = CustomPasswordHasher.GetPasswordAfterHashing(password, usr);
            return await base.PasswordSignInAsync(userName, providedPasswordAfterHash, isPersistent, shouldLockout);
        }

    }
}