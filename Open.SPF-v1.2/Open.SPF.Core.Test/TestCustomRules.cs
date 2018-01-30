using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Open.SPF.Core;
using System.Security.Principal;
namespace Open.SPF.Core.Test
{
    public class TestCustomRules : ICustomRuleContainer
    {
        public Dictionary<string, object> PreparePropertiesForRule(string methodName, string[] args)
        {
            Dictionary<string, object> propertyBag = new Dictionary<string, object>();

            if (args != null)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (!String.IsNullOrWhiteSpace(args[i]))
                        propertyBag.Add(String.Format("Argument{0}", i + 1), args[i]);
                }
            }

            return propertyBag;
        }

        [PermissionRuleMethod]
        public bool? CustomRuleIsAlwaysAuthorized(IIdentity userIdentity, IEnumerable<string> userRoles, object contextObject, Dictionary<string, object> contextPropertyBag)
        {
            return true;
        }

        [PermissionRuleMethod]
        public bool? CustomRuleIsNeverAuthorized(IIdentity userIdentity, IEnumerable<string> userRoles, object contextObject, Dictionary<string, object> contextPropertyBag)
        {
            return false;
        }

        //[PermissionRuleMethod]
        public bool? CustomRuleIsNotValidRule(IIdentity userIdentity, IEnumerable<string> userRoles, object contextObject, Dictionary<string, object> contextPropertyBag)
        {
            return false;
        }

        [PermissionRuleMethod]
        public bool? CustomRuleWrongSignatureRule(IIdentity userIdentity, IEnumerable<string> userRoles, Dictionary<string, object> contextPropertyBag)
        {
            return false;
        }

        [PermissionRuleMethod("CustomRuleAreEqualRule-CustomName")]
        public bool? CustomRuleAreEqualRule(IIdentity userIdentity, IEnumerable<string> userRoles, object contextObject, Dictionary<string, object> contextPropertyBag)
        {
            if (!contextPropertyBag.ContainsKey("ContextCompareObject"))
                throw new ArgumentException("No value was provided to compare.", "ContextCompareObject");

            object compareObject = contextPropertyBag["ContextCompareObject"];
            if (compareObject == null)
                throw new ArgumentException("The value that was provided to compare was empty.", "ContextCompareObject");

            return (contextObject == compareObject);
        }

        [PermissionRuleMethod]
        public bool? CheckArgument1(IIdentity userIdentity, IEnumerable<string> userRoles, object contextObject, Dictionary<string, object> contextPropertyBag)
        {
            if (!contextPropertyBag.ContainsKey("Argument1"))
                return false;

            return ((string)contextPropertyBag["Argument1"] == "Value1");
        }

        [PermissionRuleMethod]
        public bool? CheckArgument2(IIdentity userIdentity, IEnumerable<string> userRoles, object contextObject, Dictionary<string, object> contextPropertyBag)
        {
            if (!contextPropertyBag.ContainsKey("Argument2"))
                return false;

            return ((string)contextPropertyBag["Argument2"] == "Value2");
        }

        [PermissionRuleMethod]
        public bool? CheckArgument3(IIdentity userIdentity, IEnumerable<string> userRoles, object contextObject, Dictionary<string, object> contextPropertyBag)
        {
            if (contextPropertyBag.ContainsKey("Argument3"))
                return false;

            return true;
        }

        [PermissionRuleMethod]
        public bool? CheckCustomArgument(IIdentity userIdentity, IEnumerable<string> userRoles, object contextObject, Dictionary<string, object> contextPropertyBag)
        {
            if (!contextPropertyBag.ContainsKey("CustomArgument"))
                return false;

            return ((string)contextPropertyBag["CustomArgument"] == "CustomValue");
        }

        [PermissionRuleMethod]
        public bool? CheckCustomObject(IIdentity userIdentity, IEnumerable<string> userRoles, object contextObject, Dictionary<string, object> contextPropertyBag)
        {
            if (contextObject == null)
                return false;
            if (!(contextObject is StringBuilder))
                return false;

            StringBuilder sb = (StringBuilder)contextObject;

            return (sb.ToString() == "CustomValue");
        }

        [PermissionRuleMethod]
        public bool? RecursivePermissionCheck(IIdentity userIdentity, IEnumerable<string> userRoles, object contextObject, Dictionary<string, object> contextPropertyBag)
        {
            // this assumes the following line was added to the app or web config file under <corePermissionConfigurationItems>
            // <add permissionName="ThirdPermission" isUnrestricted="true"/>
            // We may want to test core permissions from within a custom permission

            return PermissionManager.AssertPermission("ThirdPermission");
        }
    }
}
