using AspNetMVC5WinAuth.Helpers;
using AspNetMVC5WinAuth.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WIndowsAuthCommon.Identity;
using WIndowsAuthCommon.Interfaces;

namespace AspNetMVC5WinAuth.Controllers
{
    public partial class AccountController : baseController
    {
        private CustomSignInManager SignInManager;
        private CustomUserManager UserManager;
        private IAuthenticationManager AuthenticationManager;
        private ITokenHolder TokenHolder;
        public AccountController(CustomUserManager userManager, CustomSignInManager signInManager, IAuthenticationManager authenticationManager, ITokenHolder tokenHolder)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            AuthenticationManager = authenticationManager;
            TokenHolder = tokenHolder;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> LogOn(string returnUrl)
        {
            if (Helpers.WebConfigSettings.UseWindowsAuthentication)
            {
                return await WindowsLogin(Url.Action("Index", "Home")); //honestly not sure if this is right with Josh's setup. to look up
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {

#if !DEBUG
                if (Request.IsSecureConnection)
                {
#endif
                string tokenURL = Url.Absolute(Url.Content("~/token"));
                await TokenHolder.SetBearerTokenFromOAuth(tokenURL, model.UserName, model.Password);
#if !DEBUG
                }
#endif

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, change to shouldLockout: true
                var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
                switch (result)
                {
                    case SignInStatus.Success:
                        ICustomUser usr = await UserManager.FindByNameAsync(model.UserName);
                        return HandleAfterPasswordSuccess(usr, model, returnUrl);
                    //case SignInStatus.LockedOut:
                    //return View("Lockout");
                    //case SignInStatus.RequiresVerification:
                    //return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Invalid login attempt.");
                        return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }

        private ActionResult HandleAfterPasswordSuccess(ICustomUser usr, LogOnModel mdl, string returnUrl)
        {
            //random section here to do things to flush out the user.
            return RedirectToLocal(returnUrl);
        }

        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ApplicationCookie);
            System.Web.HttpContext.Current.Session.Clear();
            return RedirectToAction("LogOn", "Account");
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

    }
}