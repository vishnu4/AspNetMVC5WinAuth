using System.Web.Hosting;
using System.IO;

namespace System.Web.Mvc
{
    public static class HtmlExtensions
    {
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