using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspNetMVC5WinAuth.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class APIAuthorization : System.Web.Http.AuthorizeAttribute
    {
        protected override bool IsAuthorized(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            //insert custom API authorization here
            return base.IsAuthorized(actionContext);
        }
    }
}