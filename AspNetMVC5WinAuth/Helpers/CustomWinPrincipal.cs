using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace AspNetMVC5WinAuth.Helpers
{
    public class CustomWinPrincipal : WindowsPrincipal
    {

        private WindowsIdentity principalSource;

        public WIndowsAuthCommon.Models.CustomUser User { get; protected set; }

        public CustomWinPrincipal(WindowsIdentity source, WIndowsAuthCommon.Models.CustomUser baseUser) :
                                base(source)
        {
            principalSource = source;
            User = baseUser;
        }
    }
}