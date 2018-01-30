using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using Microsoft.Owin;
using System.Net.Http;
using System.Security.Claims;
using Microsoft.Owin.Security;

using Open.SPF.Core;
using Open.SPF.Utility;

namespace Open.SPF.Web
{
    public class OwinPermissionManager : WebPermissionManager
    {
        public override IPrincipal CurrentUser
        {
            get 
            {
                TraceUtility.WriteTrace(this.GetType(), "get_CurrentUser", TraceUtility.TraceType.Begin);
                if ((HttpContext.Current != null) && (HttpContext.Current.GetOwinContext() != null))
                {
                    IAuthenticationManager authenticationManager = System.Web.HttpContext.Current.GetOwinContext().Authentication;
                    if (authenticationManager != null)
                    {
                        TraceUtility.WriteTrace(this.GetType(), "get_CurrentUser", null, "AuthenticationManager was found.", TraceUtility.TraceType.End);
                        return authenticationManager.User;
                    }
                }
                TraceUtility.WriteTrace(this.GetType(), "get_CurrentUser", null, "AuthenticationManager was not found.", TraceUtility.TraceType.End);
                return null; 
            }
        }
    }
}
