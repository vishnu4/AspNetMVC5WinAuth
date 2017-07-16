using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIndowsAuthCommon.Models;

namespace WIndowsAuthCommon.Identity
{
    public class CustomUserStore<T> : UserStore<T> where T : CustomUser
    {
        AuthContext _context;

        /// <summary>
        /// database will not be found for usermanager if this constructor does not exist
        /// </summary>
        /// <param name="context"></param>
        public CustomUserStore(AuthContext context) : base(context)
        {
            _context = context;
        }
    }
}
