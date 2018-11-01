using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;


namespace Open.SPF.Permissions
{
    public class SimplePermissionRequirement : AuthorizationHandler<SimplePermissionRequirement>, IAuthorizationRequirement
    {
        private string _expectedPermssion;
        public SimplePermissionRequirement(string expectedPermission)
        {
            _expectedPermssion = expectedPermission;
        }

        public string ExpectedPermission
        {
            get { return _expectedPermssion; }
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SimplePermissionRequirement requirement)
        {
            IPrincipal currentUser = context.User;

            IPermissionManager permissionManager = PermissionManager.Instance;

            List<string> userRoles = permissionManager.GetUserRoles(currentUser);

            bool isPermitted = permissionManager.InquirePermission(requirement.ExpectedPermission, currentUser.Identity, userRoles);

            if (isPermitted)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.FromResult(0);
        }
    }
}
