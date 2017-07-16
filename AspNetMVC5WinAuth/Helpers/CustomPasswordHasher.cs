using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WIndowsAuthCommon.Interfaces;
using WIndowsAuthCommon.Utilities;

namespace AspNetMVC5WinAuth.Helpers
{
    /// <summary>
    /// Custom hasher created to force our own PBKDF2 methodology
    /// </summary>
    public class CustomPasswordHasher : PasswordHasher
    {
        public override string HashPassword(string password)
        {
            throw new NotImplementedException("we do not hash via the password hasher object");
        }

        public override PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedHashedPassword)
        {
            if ((string.IsNullOrEmpty(hashedPassword) && string.IsNullOrEmpty(providedHashedPassword)) || (string.Compare(hashedPassword, providedHashedPassword, System.StringComparison.Ordinal) == 0))
            {
                return PasswordVerificationResult.Success;
            }
            return PasswordVerificationResult.Failed;
        }

        public static string GetPasswordAfterHashing(string passwordToHash, ICustomUser usr)
        {
            string providedPasswordAfterHash = passwordToHash;
            if (!string.IsNullOrEmpty(passwordToHash))
            {
                if (usr != null)
                {
                    if (usr.PasswordIterationCount > 0 && usr.PasswordSalt != null)
                    {
                        providedPasswordAfterHash = CustomEncrypt.PBKDF2HashedPassword(passwordToHash, usr.PasswordSalt, usr.PasswordIterationCount);
                    }
                    else
                    {
                        throw new InvalidOperationException("no salt or password iteration found");
                    }
                }
                else
                {
                    throw new InvalidOperationException("usr not declared");
                }
            }
            return providedPasswordAfterHash;
        }

    }
}