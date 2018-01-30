using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

using Open.SPF.Core;
using Open.SPF.Utility;

namespace Open.SPF.Web
{
    public class MembershipPermissionManager : PermissionManager
    {
        protected internal override List<string> DoGetRolesForUser(IPrincipal currentUser)
        {
            TraceUtility.WriteTrace(this.GetType(), "DoGetRolesForUser", TraceUtility.TraceType.Begin);
            List<string> userRoles = null;
            try
            {
                string[] roleArray = Roles.GetRolesForUser();
                userRoles = new List<string>(roleArray);
            }
            catch (Exception ex)
            {
                EventLogUtility.LogWarningMessage(String.Format("There was an error reading the roles for the current user: {0}.\r\n\r\n{1}", CurrentUser.Identity.Name, EventLogUtility.FormatExceptionMessage(ex)));
            }

            if (userRoles == null)
                userRoles = base.DoGetRolesForUser(currentUser);

            TraceUtility.WriteTrace(this.GetType(), "DoGetRolesForUser", null, "from Roles.GetRolesForUser()", TraceUtility.TraceType.End);
            return userRoles;
        }
    }
}
