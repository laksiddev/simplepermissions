using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using System.Net.Http;
using System.Security.Claims;

using Open.SPF.Core;
using Open.SPF.Utility;

namespace Open.SPF.Web
{
    public class WebPermissionManager : PermissionManager
    {
        protected internal override List<string> DoGetRolesForUser(IPrincipal currentUser)
        {
            TraceUtility.WriteTrace(this.GetType(), "DoGetRolesForUser", TraceUtility.TraceType.Begin);
            // use the base IsInRole crawl, but use the current Owin User
            List<string> results = base.DoGetRolesForUser(this.CurrentUser);

            TraceUtility.WriteTrace(this.GetType(), "DoGetRolesForUser", TraceUtility.TraceType.End);
            return results;
        }

        public override IPrincipal CurrentUser
        {
            get 
            {
                TraceUtility.WriteTrace(this.GetType(), "get_CurrentUser", TraceUtility.TraceType.Begin);
                if (HttpContext.Current != null) 
                {
                    TraceUtility.WriteTrace(this.GetType(), "get_CurrentUser", null, "Current HttpContext was found.", TraceUtility.TraceType.End);
                    return HttpContext.Current.User;
                }
                TraceUtility.WriteTrace(this.GetType(), "get_CurrentUser", null, "Current HttpContext was not found.", TraceUtility.TraceType.End);
                return null; 
            }
        }
    }
}
