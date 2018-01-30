using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Open.SPF.Core
{
    public interface ICoreRulesContainer
    {
        Dictionary<string, object> PrepareStateForRoleRule(string roleName);
        Dictionary<string, object> PrepareStateForUnrestrictedRule(bool isUnrestricted);

        //PermissionRules.IsAuthorizedMethod RuleDelegateFromMethodName(string typeName, string methodName);
        //Task<bool> TaskFromInvocationState(RuleInvocationState invocationState, IIdentity userIdentity, IEnumerable<string> userRoles);
    }
}
