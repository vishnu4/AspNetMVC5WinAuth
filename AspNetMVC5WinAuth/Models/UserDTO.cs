using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspNetMVC5WinAuth.Models
{
    public class UserDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }
        public string Id { get; set; }
        public string UserName { get; set; }
    }
}