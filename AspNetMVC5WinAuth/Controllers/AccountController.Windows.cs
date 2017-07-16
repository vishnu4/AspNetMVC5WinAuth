﻿using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WIndowsAuthCommon.Models;
using WIndowsAuthCommon.Utilities;

namespace AspNetMVC5WinAuth.Controllers
{
    /// <summary>
    /// separate partial class for account controller to handle all of the windows auth methods
    /// </summary>
    public partial class AccountController : baseController
    {
        [AllowAnonymous]
        private async Task<ActionResult> WindowsLogin(string returnUrl)
        {
            if (!Helpers.WebConfigSettings.UseWindowsAuthentication)
            {
                throw new InvalidOperationException("Not a valid server for windows authentication");
            }
            bool b = await BuildWindowsUser();
            if (b)  //user was successfully created, move on
            {
                return RedirectToHome(returnUrl);
            }
            else
            {
                throw new InvalidOperationException("Could not log on windows user");
            }
        }

        /// <summary>
        /// default username based on actual windows login information
        /// </summary>
        private string windowsUserName
        {
            get
            {
                return System.Environment.UserDomainName + @"\" + System.Environment.UserName;
            }
        }

        /// <summary>
        /// Finds out if the user already exists in the database, if not then it gets created
        /// </summary>
        /// <returns></returns>
        internal async Task<bool> BuildWindowsUser()
        {
            CustomUser user = await UserManager.FindByNameAsync(windowsUserName);
            if ((user != null) && (!string.IsNullOrEmpty(user.Id)))
            {
                //user already exists
                if (!user.IsUserAutoGenerated)
                {
                    //weird circumstance.  existing user in the database was NOT auto created, but entered in explicitly.  
                    throw new InvalidOperationException("User with login name " + windowsUserName + " exists, but not as a windows authorized user");
                }
                else
                {
#if !DEBUG
                if (Request.IsSecureConnection)
                {
#endif
                    string tokenURL = Url.Absolute(Url.Content("~/token"));
                    await TokenHolder.SetBearerTokenFromOAuth(tokenURL, user.UserName, windowsAuthPassword);
#if !DEBUG
                }
#endif
                    await SignInManager.SignInAsync(user, true, true);
                    return true;
                }
            }
            else
            {
                //user does NOT exist in database, create user default details
                user = GetAutoCreateUser();
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    //not ideal.  we don't know the user ID, so we have to search the user manager again to make sure it got created
                    user = await UserManager.FindByNameAsync(user.UserName);
                    //add user role if we like
                    //await UserManager.AddToRoleAsync(user.Id, "WhateverRoleIsInYourDatabase");
                    return await BuildWindowsUser();    //return back through the method, it should just pass through the 'does user exist' check this time without issue
                }
                else
                {
                    throw new InvalidOperationException(string.Join("|", result.Errors));
                }
            }
            //return false;
        }

        private const string windowsAuthPassword = "whateveryoufeellikeputtinghereitsnotused";

        private CustomUser GetAutoCreateUser()
        {
            CustomUser usr = new CustomUser();
            usr.UserName = windowsUserName;
            //passwords will never get used for a windows auth user, so this is mostly just gibberish, but added so that i don't have to allow nulls for passwords in the database
            //extra amount of gibberish to potentially avoid a security issue
            usr.PasswordIterationCount = CustomEncrypt.minimumIterationCount;
            usr.PasswordSalt = CustomEncrypt.PBKDF2GetRandomSalt();
            usr.PasswordHash = CustomEncrypt.PBKDF2HashedPassword(windowsAuthPassword, usr.PasswordSalt, usr.PasswordIterationCount);
            //***************************************************************
            usr.IsUserAutoGenerated = true;
            usr.DateCreated = System.DateTime.UtcNow;
            usr.DateLastModified = System.DateTime.UtcNow;

            //this section is intended on connecting to the domain controller and getting some information about the user to add to our user object
            //doesn't always work based on the security of the DC.  based this attempt on https://stackoverflow.com/questions/20156913/get-active-directory-user-information-with-windows-authentication-in-mvc-4
            try
            {
                UserPrincipalExtended windowsUser = UserPrincipalExtended.FindByIdentity(new PrincipalContext(ContextType.Domain), User.Identity.Name);
                if (windowsUser != null)
                {
                    usr.LastName = windowsUser.Surname;
                    usr.FirstName = windowsUser.GivenName;
                    //windowsUser.Title;
                    //windowsUser.Department;

                    usr.PhoneNumber = windowsUser.VoiceTelephoneNumber;
                    usr.Email = windowsUser.EmailAddress;
                }
            }
            catch (Exception)// ex)
            {
                //data was not retrieved successfully from the domain controller.  not a good enough reason to cancel the user create, so just move on.
            }

            //to avoid empty fields, but that's just a personal choice.
            if (string.IsNullOrEmpty(usr.FirstName))
            {
                usr.FirstName = windowsUserName;
            }
            if (string.IsNullOrEmpty(usr.LastName))
            {
                usr.LastName = windowsUserName;
            }
            return usr;
        }
    }

    [DirectoryRdnPrefix("CN")]
    [DirectoryObjectClass("user")]
    internal class UserPrincipalExtended : UserPrincipal
    {
        internal UserPrincipalExtended(PrincipalContext context)
            : base(context)
        {
        }

        internal UserPrincipalExtended(PrincipalContext context, string samAccountName, string password, bool enabled)
            : base(context, samAccountName, password, enabled)
        {
        }

        [DirectoryProperty("title")]
        internal string Title
        {
            get
            {
                if (ExtensionGet("title").Length != 1)
                    return null;

                return (string)ExtensionGet("title")[0];
            }

            set
            {
                ExtensionSet("title", value);
            }
        }

        [DirectoryProperty("department")]
        internal string Department
        {
            get
            {
                if (ExtensionGet("department").Length != 1)
                    return null;

                return (string)ExtensionGet("department")[0];
            }

            set
            {
                ExtensionSet("department", value);
            }
        }

        internal static new UserPrincipalExtended FindByIdentity(PrincipalContext context, string identityValue)
        {
            return (UserPrincipalExtended)FindByIdentityWithType(context, typeof(UserPrincipalExtended), identityValue);
        }

        internal static new UserPrincipalExtended FindByIdentity(PrincipalContext context, IdentityType identityType, string identityValue)
        {
            return (UserPrincipalExtended)FindByIdentityWithType(context, typeof(UserPrincipalExtended), identityType, identityValue);
        }
    }
}