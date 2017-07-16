using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspNetMVC5WinAuth.Models
{

    /// <summary>
    /// basic class to show the user information about their user object.  good opportunity for automapper in the future.
    /// </summary>
    public class UserDTO
    {
        public string UserName { get; set; }
    }
}