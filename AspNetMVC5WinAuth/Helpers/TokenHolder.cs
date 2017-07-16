using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace AspNetMVC5WinAuth.Helpers
{
    public interface ITokenHolder
    {
        string GetTokenHeader();
        Task SetBearerTokenFromOAuth(string siteUrl, string Username, string Password);
        Task SetBearerTokenFromOAuth(Uri siteUrl, string Username, string Password);
    }

    /// <summary>
    /// An idea to pass tokens to all my web api calls.  Testing out the idea of saving them in session
    /// </summary>
    public class TokenHolder : ITokenHolder
    {
        internal const string tokenKey = "TokenClaim";

        HttpContext _context;
        public TokenHolder(HttpContext context)
        {
            _context = context;
        }

        public string GetTokenHeader()
        {
            string ret = string.Empty;
            ClaimsIdentity claimsIdentity = System.Web.HttpContext.Current.User.Identity as ClaimsIdentity;
            if ((claimsIdentity != null) && (claimsIdentity.FindFirst(tokenKey) != null))
            {
                ret = claimsIdentity.FindFirst(tokenKey).Value;
            }
            if (string.IsNullOrEmpty(ret))
            {
                return _context.Session[tokenKey].ToString();
            }
            return ret;
        }


        private static HttpClient client = new HttpClient();
        internal async Task<OAuthTokenObject> GetBearerTokenFromOAuth(string siteUrl, string Username, string Password)
        {
            return await GetBearerTokenFromOAuth(new Uri(siteUrl), Username, Password);
        }
        internal async Task<OAuthTokenObject> GetBearerTokenFromOAuth(Uri siteUrl, string Username, string Password)
        {
            try
            {
                HttpContent requestContent = new StringContent("grant_type=password&username=" + Username + "&password=" + Password, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");

                HttpResponseMessage responseMessage = await client.PostAsync(siteUrl, requestContent);
                if (responseMessage.IsSuccessStatusCode)
                {
                    OAuthTokenObject response = await responseMessage.Content.ReadAsAsync<OAuthTokenObject>();
                    return response;
                }
            }
            catch (Exception ex)
            {
                InvalidOperationException iex = new InvalidOperationException("error with token retrieval for user " + Username, ex);
                throw iex;
            }
            return null;
        }

        public async Task SetBearerTokenFromOAuth(string siteUrl, string Username, string Password)
        {
            await SetBearerTokenFromOAuth(new Uri(siteUrl), Username, Password);
        }

        public async Task SetBearerTokenFromOAuth(Uri siteUrl, string Username, string Password)
        {
            OAuthTokenObject response = await GetBearerTokenFromOAuth(siteUrl, Username, Password);
            if (response != null)
            {
                SaveTokenToSession(response);
            }
        }

        private void SaveTokenToSession(OAuthTokenObject token)
        {
            if (System.Web.HttpContext.Current.Session != null)
            {
                token.issued = System.DateTime.UtcNow;
                token.expires = token.issued.AddSeconds(token.expires_in);
                //System.Web.HttpContext.Current.Session[tokenKey] = token;
                _context.Session.Add(tokenKey, token.access_token);
            }
        }

    }

    public class OAuthTokenObject
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string userName { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public DateTime issued { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public DateTime expires { get; set; }
    }

}