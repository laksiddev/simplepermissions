using System;
using System.Collections.Generic;
using System.Text;

namespace Open.SPF.Permissions
{
    public class CustomPermissionConfiguration
    {
        public string PermissionName { get; set; }
        public string RuleTypeName { get; set; }
        public string RuleMethodName { get; set; }

        public string[] Arguments { get; set; }
    }
}
