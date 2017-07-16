using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AspNetMVC5WinAuth.Models
{

    /// <summary>
    /// default MVC5 logon model
    /// </summary>
    public class LogOnModel
    {
        [Required]
        [DisplayName("Username")]
        public string UserName { get; set; }

        //[Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayName("Remember Me")]
        public bool RememberMe { get; set; }
    }

}