using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WIndowsAuthCommon.Utilities;

namespace AspNetMVC5WinAuth.Helpers
{
    public sealed class WebConfigSettings
    {
        public static string SQLConnectionString
        {
            get
            {
                if (System.Configuration.ConfigurationManager.ConnectionStrings["WindowsAuthSQL"] != null)
                {
                    return System.Configuration.ConfigurationManager.ConnectionStrings["WindowsAuthSQL"].ConnectionString;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public static bool UseWindowsAuthentication
        {
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("UseWindowsAuthentication"))
                {
                    bool? b = System.Configuration.ConfigurationManager.AppSettings["UseWindowsAuthentication"].ToNullableBoolean();
                    if (b.HasValue)
                    {
                        return b.Value;
                    }
                }
                return false;
            }
        }

    }
}