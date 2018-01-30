using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Open.SPF.Core;

namespace Open.SPF.Web.Mvc
{
    public class PermissionAuthorizeAttribute : AuthorizeAttribute
    {
        private string[] _permissions { get; set; }

        public PermissionAuthorizeAttribute(string Permissions)
        {
            _permissions = Permissions.Split(',');            
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (PermissionManager.AssertPermission(_permissions))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);
            filterContext.Result = new RedirectResult(Properties.Settings.Default.UmauthorizedRedirectPage);
        }
    }
}
