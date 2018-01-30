using System.Configuration;

namespace Open.SPF.Configuration
{
    public class CorePermissionConfigurationElement : ConfigurationElement
    {
        public CorePermissionConfigurationElement()
        {
        }

        // There is no key because there are many-to-many relationships between Permission Name and Permitted Role
        [ConfigurationProperty("permissionName", IsRequired = true, IsKey = false)]
        public string PermissionName
        {
            get { return (string)this["permissionName"]; }
            set { this["permissionName"] = value; }
        }

        [ConfigurationProperty("permittedRole", IsRequired = false, IsKey = false)]
        public string PermittedRole
        {
            get { return (string)this["permittedRole"]; }
            set { this["permittedRole"] = value; }
        }

        [ConfigurationProperty("isUnrestricted", IsRequired = false, IsKey = false)]
        public bool IsUnrestricted
        {
            get 
            {
                bool? isUnrestricted = (bool)this["isUnrestricted"];
                return ((isUnrestricted.HasValue) ? isUnrestricted.Value : false);
            }
            set { this["isUnrestricted"] = value; }
        }
    }
}
