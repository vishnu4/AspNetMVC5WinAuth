using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WIndowsAuthCommon.Utilities
{
    public class CustomEncrypt
    {
        /// <summary>
        /// Gives us a random salt string using RNGCryptoServiceProvider
        /// </summary>
        /// <returns>uses the default minimum salt length</returns>
        public static byte[] PBKDF2GetRandomSalt()
        {
            return PBKDF2GetRandomSalt(minSaltLength);
        }

        /// <summary>
        /// Gives us a random salt string using RNGCryptoServiceProvider
        /// </summary>
        /// <param name="saltByteLength">the length of the salt</param>
        /// <returns></returns>
        public static byte[] PBKDF2GetRandomSalt(int saltByteLength)
        {
            RNGCryptoServiceProvider csprng = new RNGCryptoServiceProvider();
            if (saltByteLength < minSaltLength)
            {
                saltByteLength = minSaltLength;
            }
            byte[] salt = new byte[saltByteLength];
            csprng.GetBytes(salt);
            return salt;
        }

        //must be at least 128 bits.  this is 192
        private const int minSaltLength = 24;
        public const int minimumIterationCount = 1000;

        private const int derivedHashKeyLength = 24;
        /// <summary>
        /// hashes a string using Rfc2898DeriveBytes 
        /// </summary>
        /// <param name="originalPassword">the password you want hashed</param>
        /// <param name="saltValue">a random byte amount to 'salt' the value</param>
        /// <param name="iterationCount">the iteration to step through the encryption</param>
        /// <returns></returns>
        /// <remarks>https://crackstation.net/hashing-security.htm#aspsourcecode , http://lockmedown.com/hash-right-implementing-pbkdf2-net/</remarks>
        public static string PBKDF2HashedPassword(string originalPassword, byte[] saltValue, int iterationCount)
        {
            if (!string.IsNullOrEmpty(originalPassword) && saltValue != null)
            {
                if (saltValue.Length < minSaltLength)
                {
                    throw new InvalidOperationException("salt value must be longer than " + minSaltLength);
                }
                if (iterationCount < minimumIterationCount)
                {
                    iterationCount = minimumIterationCount;
                }
                using (Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(originalPassword, saltValue, iterationCount))
                {
                    return Convert.ToBase64String(key.GetBytes(derivedHashKeyLength));
                }
            }
            throw new InvalidOperationException("Cannot hash without a password or valid salt value");
        }

    }
}

