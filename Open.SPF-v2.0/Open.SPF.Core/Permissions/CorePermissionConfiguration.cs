using System;
using System.Collections.Generic;
using System.Text;

namespace Open.SPF.Permissions
{
    public class CorePermissionConfiguration
    {
        public string PermissionName { get; set; }
        public string PermittedRole { get; set; }
        public string IsUnrestricted { get; set; }
    }
}
