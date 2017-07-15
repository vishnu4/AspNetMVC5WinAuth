using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AspNetMVC5WinAuth.Controllers
{
    public class HomeController : baseController
    {
        // GET: Home
        public ActionResult Index()
        {
            return View(System.Web.HttpContext.Current.User.Identity.Name);
        }
    }
}