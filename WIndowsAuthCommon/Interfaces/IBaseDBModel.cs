using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIndowsAuthCommon.Interfaces
{
    public interface IBaseDBModel
    {
        DateTime DateCreated { get; set; }
        
        DateTime DateLastModified { get; set; }
    }
}
