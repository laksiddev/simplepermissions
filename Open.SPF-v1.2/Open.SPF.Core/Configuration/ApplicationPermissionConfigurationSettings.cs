using System;
using System.Configuration;

namespace Open.SPF.Configuration
{
    [Serializable]
    public class ApplicationPermissionConfigurationSettings : ConfigurationSection
    {
        public ApplicationPermissionConfigurationSettings()
        {
        }

        [ConfigurationProperty("corePermissionConfigurationItems", IsRequired = true)]
        [ConfigurationCollection(typeof(CorePermissionConfigurationCollection))]
        public CorePermissionConfigurationCollection CorePermissionConfigurationItems
        {
            get { return (CorePermissionConfigurationCollection)this["corePermissionConfigurationItems"]; }
            set { this["corePermissionConfigurationItems"] = value; }
        }

        [ConfigurationProperty("customPermissionConfigurationItems", IsRequired = false)]
        [ConfigurationCollection(typeof(CustomPermissionConfigurationCollection))]
        public CustomPermissionConfigurationCollection CustomPermissionConfigurationItems
        {
            get { return (CustomPermissionConfigurationCollection)this["customPermissionConfigurationItems"]; }
            set { this["customPermissionConfigurationItems"] = value; }
        }
    }
}
