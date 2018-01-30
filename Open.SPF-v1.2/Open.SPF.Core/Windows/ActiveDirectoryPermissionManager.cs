using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices.AccountManagement;

using Open.SPF.Core;
using System.Security.Principal;

namespace Open.SPF.Windows
{
    public class ActiveDirectoryPermissionManager : PermissionManager
    {
        protected internal override List<string> DoGetRolesForUser(IPrincipal currentUser)
        {
            Open.SPF.Utility.TraceUtility.WriteTrace(this.GetType(), "DoGetRolesForUser", Open.SPF.Utility.TraceUtility.TraceType.Begin);
            List<string> userRoles = null;
            try
            {
                UserPrincipal user = UserPrincipal.Current;
                userRoles = new List<string>();
                foreach (GroupPrincipal group in user.GetGroups())
                {
                    userRoles.Add(group.Name);
                }
            }
            catch (Exception ex)
            {
                Open.SPF.Utility.EventLogUtility.LogWarningMessage(String.Format("There was an error reading the roles for the current user: {0}.\r\n\r\n{1}", CurrentUser.Identity.Name, Open.SPF.Utility.EventLogUtility.FormatExceptionMessage(ex)));
            }

            if ((userRoles == null) || (userRoles.Count == 0))
                userRoles = base.DoGetRolesForUser(currentUser);

            Open.SPF.Utility.TraceUtility.WriteTrace(this.GetType(), "DoGetRolesForUser", null, "from UserPrincipal.Current.GetGroups()", Open.SPF.Utility.TraceUtility.TraceType.End);
            return userRoles;
        }
    }
}
