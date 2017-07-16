using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIndowsAuthCommon.Models;

namespace WIndowsAuthCommon.Identity
{
    public class CustomUserManager : UserManager<CustomUser>
    {
        public CustomUserManager(CustomUserStore<CustomUser> userStore) : base(userStore)
        {
            //since i'm making the username the domainname\windowsname, this setting needs to be changed
            this.UserValidator = new UserValidator<CustomUser>(this) { AllowOnlyAlphanumericUserNames = false };
        }
    }
}
