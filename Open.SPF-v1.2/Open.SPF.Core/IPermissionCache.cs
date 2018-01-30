using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Open.SPF.Core
{
    public interface IPermissionCache
    {
        void SaveRolesForUser(IPrincipal currentUser, List<string> userRoles);

        List<string> GetRolesForUser(IPrincipal currentUser);
    }
}
