using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace Open.SPF.Core
{
    public interface IPermissionManager
    {
        //bool AssertPermissionFromUserRoles(string permissionName, System.Collections.Generic.IEnumerable<string> userRoles);

        bool DoAssertPermission(string permissionName);

        bool DoAssertPermission(string permissionName, object contextObject, Dictionary<string, object> contextProperties);

        bool DoAssertPermission(string permissionName, IIdentity userIdentity, IEnumerable<string> userRoles, object contextObject, Dictionary<string, object> contextProperties);

        bool InquirePermission(string permissionName, IIdentity userIdentity, IEnumerable<string> userRoles);

        PermissionResultCollection InquirePermission(string permissionName, IIdentity userIdentity, IEnumerable<string> userRoles, object contextObject, Dictionary<string, object> contextProperties);

        IPrincipal CurrentUser { get; }

        List<string> GetUserRoles(IPrincipal currentUser);
    
        List<string> GetUserRoles();
    }
}
