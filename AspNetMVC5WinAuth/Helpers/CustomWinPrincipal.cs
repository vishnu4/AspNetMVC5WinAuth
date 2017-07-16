using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace AspNetMVC5WinAuth.Helpers
{
    /// <summary>
    /// custom windows principal class.  mostly created so i can add extra values to the principal when the user is assigned
    /// </summary>
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