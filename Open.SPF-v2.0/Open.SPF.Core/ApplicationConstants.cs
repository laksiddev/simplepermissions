using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Open.SPF.Core
{
    public class ApplicationConstants
    {
        public class AppSettings
        {
            public const string ApplicationEventLogSource = "appSettings:applicationEventLogSource";
            public const string DiagnosticLoggingLevel = "appSettings:diagnosticLoggingLevel";
            public const string ContactInformation = "appSettings:contactInformation";
            public const string ServiceRegistrationType = "appSettings:serviceRegistrationType";
            public const string ApplicationLogSource = "appSettings:applicationLogSource";
            public const string PermissionCacheTimeoutMinutes = "appSettings:permissionCacheTimeoutMinutes";
            public const string PermissionCacheIsRollingTimeout = "appSettings:permissionCacheIsRollingTimeout";
        }
    }
}
