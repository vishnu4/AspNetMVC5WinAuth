using AspNetMVC5WinAuth.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AspNetMVC5WinAuth.App_Start
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //adding custom authorization for MVC controllers
            filters.Add(new MVCAuthorization());
        }
    }
}