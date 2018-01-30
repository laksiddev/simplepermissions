using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Open.SPF.Core
{
    public class PermissionResult
    {
        public PermissionResult(bool? isAuthorized, string ruleName)
        {
            IsAuthorized = isAuthorized;
            RuleName = ruleName;
        }

        public bool? IsAuthorized { get; protected set; }
        public string RuleName { get; protected set; }
    }
}
