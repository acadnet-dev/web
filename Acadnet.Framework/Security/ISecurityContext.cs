using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acadnet.Data.Identity;

namespace Acadnet.Framework.Security
{
    public interface ISecurityContext
    {
        bool IsAuthenticated { get; }

        User? User { get; }
    }
}