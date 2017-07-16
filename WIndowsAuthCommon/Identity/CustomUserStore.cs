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
        public CustomUserStore(AuthContext context) : base(context)
        {
            _context = context;
        }
    }
}
