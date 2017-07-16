using System.Web.Hosting;
using System.IO;

namespace System.Web.Mvc
{
    public static class HtmlExtensions
    {
        /// <summary>
        /// HTML extension that returns the ABSOLUTE value of a local url.  to be used instead of Url.Content for example
        /// </summary>
        /// <param name="url">url to resolve</param>
        /// <param name="relativeOrAbsolute"></param>
        /// <returns></returns>
        public static string Absolute(this UrlHelper url, string relativeOrAbsolute)
        {
            Uri uri = new Uri(url.RequestContext.HttpContext.Request.Url, url.RequestContext.HttpContext.Request.RawUrl);
            UriBuilder builder = new UriBuilder(uri);
            builder.Path = url.RequestContext.HttpContext.Request.ApplicationPath;
            builder.Query = null;
            builder.Fragment = null;
            return new Uri(builder.Uri, url.Content(relativeOrAbsolute)).AbsoluteUri;
        }
        
    }
}