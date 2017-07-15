using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspNetMVC5WinAuth.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class MVCAuthorization : System.Web.Mvc.AuthorizeAttribute
    {

        public MVCAuthorization()
        {
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //in case i want to do extra checks here.
            return base.AuthorizeCore(httpContext);
        }

    }
}