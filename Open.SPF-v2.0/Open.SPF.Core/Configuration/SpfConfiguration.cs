using Microsoft.Extensions.Configuration;
using Open.SPF.Permissions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Open.SPF.Configuration
{
    public class SpfConfiguration : Open.Common.Configuration.CommonConfiguration
    {
        //private static IConfiguration __configuration;

        //public static string Item(string value)
        //{
        //    return Configuration[value];
        //}

        //private static IConfiguration Configuration
        //{
        //    get
        //    {
        //        if (__configuration == null)
        //        {
        //            IConfigurationBuilder builder = new ConfigurationBuilder()
        //            .SetBasePath(Directory.GetCurrentDirectory())
        //            .AddJsonFile("appsettings.json");

        //            __configuration = builder.Build();
        //        }

        //        return __configuration;
        //    }
        //}

        public static List<CorePermissionConfiguration> CorePermissions
        {
            get
            {
                IConfigurationSection corePermissionSection = Configuration.GetSection("corePermissions");
                if (corePermissionSection == null)
                    return null;

                List<CorePermissionConfiguration> corePermissions = new List<CorePermissionConfiguration>();
                foreach (IConfigurationSection permissionConfigurationItem in corePermissionSection.GetChildren())
                {
                    CorePermissionConfiguration corePermission = new CorePermissionConfiguration()
                    {
                        PermissionName = permissionConfigurationItem["permissionName"],
                        PermittedRole = permissionConfigurationItem["permittedRole"],
                        IsUnrestricted = permissionConfigurationItem["isUnrestricted"]
                    };

                    corePermissions.Add(corePermission);
                }

                return corePermissions;
            }
        }

        public static List<CustomPermissionConfiguration> CustomPermissions
        {
            get
            {
                IConfigurationSection customPermissionSection = Configuration.GetSection("customPermissions");
                if (customPermissionSection == null)
                    return null;

                List<CustomPermissionConfiguration> customPermissions = new List<CustomPermissionConfiguration>();
                foreach (IConfigurationSection permissionConfigurationItem in customPermissionSection.GetChildren())
                {
                    CustomPermissionConfiguration customPermission = new CustomPermissionConfiguration()
                    {
                        PermissionName = permissionConfigurationItem["permissionName"],
                        RuleTypeName = permissionConfigurationItem["ruleType"],
                        RuleMethodName = permissionConfigurationItem["ruleMethod"]
                    };

                    IConfigurationSection argumentConfiguration = permissionConfigurationItem.GetSection("arguments");
                    if (argumentConfiguration != null)
                    {
                        List<string> argumentList = new List<string>();
                        for (int i=0; i<100; i++)
                        {
                            string argumentString = argumentConfiguration[i.ToString()];
                            if (argumentString != null)
                            {
                                argumentList.Add(argumentString);
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (argumentList.Count > 0)
                            customPermission.Arguments = argumentList.ToArray();
                    }
 
                    customPermissions.Add(customPermission);
                }

                return customPermissions;
            }
        }
    }
}
