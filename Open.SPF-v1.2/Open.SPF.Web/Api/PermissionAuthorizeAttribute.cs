using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Net;
using System.Net.Http;

using Open.SPF.Core;

namespace Open.SPF.Web.Api
{
    public class PermissionAuthorizeAttribute : AuthorizeAttribute
    {
        private string[] _permissions { get; set; }

        public PermissionAuthorizeAttribute(string Permissions)
        {
            _permissions = Permissions.Split(',');            
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
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

        //protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        //{
        //    base.HandleUnauthorizedRequest(actionContext);
        //    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
        //}
    }
}
