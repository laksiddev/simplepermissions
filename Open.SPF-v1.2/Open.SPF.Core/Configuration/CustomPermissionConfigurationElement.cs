using System.Configuration;

namespace Open.SPF.Configuration
{
    public class CustomPermissionConfigurationElement : ConfigurationElement
    {
        // There is no key because there are many-to-many relationships between Permission Name and Permitted Role
        [ConfigurationProperty("permissionName", IsRequired = true, IsKey = false)]
        public string PermissionName
        {
            get { return (string)this["permissionName"]; }
            set { this["permissionName"] = value; }
        }

        [ConfigurationProperty("ruleType", IsRequired = true, IsKey = false)]
        public string RuleTypeName
        {
            get { return (string)this["ruleType"]; }
            set { this["ruleType"] = value; }
        }

        [ConfigurationProperty("ruleMethod", IsRequired = true, IsKey = false)]
        public string RuleMethodName
        {
            get { return (string)this["ruleMethod"]; }
            set { this["ruleMethod"] = value; }
        }

        [ConfigurationProperty("argument1", IsRequired = false, IsKey = false)]
        public string Argument1
        {
            get { return (string)this["argument1"]; }
            set { this["argument1"] = value; }
        }

        [ConfigurationProperty("argument2", IsRequired = false, IsKey = false)]
        public string Argument2
        {
            get { return (string)this["argument2"]; }
            set { this["argument2"] = value; }
        }

        [ConfigurationProperty("argument3", IsRequired = false, IsKey = false)]
        public string Argument3
        {
            get { return (string)this["argument3"]; }
            set { this["argument3"] = value; }
        }

        [ConfigurationProperty("argument4", IsRequired = false, IsKey = false)]
        public string Argument4
        {
            get { return (string)this["argument4"]; }
            set { this["argument4"] = value; }
        }

        [ConfigurationProperty("argument5", IsRequired = false, IsKey = false)]
        public string Argument5
        {
            get { return (string)this["argument5"]; }
            set { this["argument5"] = value; }
        }
    }
}
