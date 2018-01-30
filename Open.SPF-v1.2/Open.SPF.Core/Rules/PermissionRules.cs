using Open.SPF.Configuration;
using Open.SPF.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Open.SPF.Core
{
    public class PermissionRules : ICoreRulesContainer
    {
        public delegate bool? IsAuthorizedMethod(IIdentity userIdentity, IEnumerable<string> userRoles, object contextObject, Dictionary<string, object> contextProperyBag);

        private const string __permittedRolePropertyTag = "PermittedRole";
        private const string __isUnrestrictedPropertyTag = "IsUnrestricted";

        public Dictionary<string, object> PrepareStateForRoleRule(string roleName)
        {
            Dictionary<string, object> contextPropertyBag = new Dictionary<string, object>();
            contextPropertyBag.Add(__permittedRolePropertyTag, roleName);

            return contextPropertyBag;
        }

        public Dictionary<string, object> PrepareStateForUnrestrictedRule(bool isUnrestricted)
        {
            Dictionary<string, object> contextPropertyBag = new Dictionary<string, object>();
            contextPropertyBag.Add(__isUnrestrictedPropertyTag, isUnrestricted);

            return contextPropertyBag;
        }

        public ICustomRuleContainer CustomRuleContainerFromTypeName(string typeName)
        {
            Type ruleType = Type.GetType(typeName);
            if ((ruleType == null) || (!(typeof(ICustomRuleContainer).IsAssignableFrom(ruleType))))
            {
                TraceUtility.WriteTrace(this.GetType(), "CustomRuleContainerFromTypeName", null, String.Format("Could not obtain Type information from the Type name. TypeName: {0}", typeName), TraceUtility.TraceType.Warning);
                return null;
            }

            ConstructorInfo constructorMethod = ruleType.GetConstructor(new Type[0]);
            if (constructorMethod == null)
            {
                TraceUtility.WriteTrace(this.GetType(), "CustomRuleContainerFromTypeName", null, String.Format("Could not obtain Contructor infomration about the Type. TypeName: {0}", typeName), TraceUtility.TraceType.Warning);
                return null;
            }

            ICustomRuleContainer customContainer; // = new TestCustomRules();
            customContainer = (ICustomRuleContainer)constructorMethod.Invoke(new object[0]);
            if (customContainer == null)
            {
                TraceUtility.WriteTrace(this.GetType(), "CustomRuleContainerFromTypeName", null, String.Format("Could not obtain an instance of the Type from the constructor. TypeName: {0}", typeName), TraceUtility.TraceType.Warning);
            }

            return customContainer;
        }

        public IsAuthorizedMethod RuleDelegateFromMethodName(ICustomRuleContainer customContainer, string methodName)
        {
            Type[] methodSignature = new Type[] { typeof(IIdentity), typeof(IEnumerable<string>), typeof(object), typeof(Dictionary<string, object>) };
            MethodInfo ruleMethodInfo = customContainer.GetType().GetMethod(methodName, methodSignature);
            if (ruleMethodInfo == null)
            {
                TraceUtility.WriteTrace(this.GetType(), "RuleDelegateFromMethodName", null, String.Format("Could not obtain Method information from the Method name. TypeName: {0}, MethodName: {1}", customContainer.GetType().FullName, methodName), TraceUtility.TraceType.Warning);
                return null;
            }

            PermissionRuleMethodAttribute permissionAttribute = ruleMethodInfo.GetCustomAttribute<PermissionRuleMethodAttribute>();
            if (permissionAttribute == null)
            {
                TraceUtility.WriteTrace(this.GetType(), "RuleDelegateFromMethodName", null, String.Format("The {1} method in the {0} class was marked as a Permission Rule Method, but it exhibited an incorrect method signature for rule processing. A Permission Rule Method should accept the following parameters: (IIdentity userIdentity, IEnumerable<string> userRoles, object contextObject, Dictionary<string, object> contextPropertyBag).", customContainer.GetType().FullName, ruleMethodInfo.Name), TraceUtility.TraceType.Warning);
                return null;
            }

            IsAuthorizedMethod ruleDelegate = 
                (IsAuthorizedMethod)ruleMethodInfo.CreateDelegate(typeof(IsAuthorizedMethod), customContainer);

            return ruleDelegate;
        }

        public string RuleNameFromMethodName(ICustomRuleContainer customContainer, string methodName)
        {
            Type[] methodSignature = new Type[] { typeof(IIdentity), typeof(IEnumerable<string>), typeof(object), typeof(Dictionary<string, object>) };
            MethodInfo ruleMethodInfo = customContainer.GetType().GetMethod(methodName, methodSignature);
            if (ruleMethodInfo == null)
            {
                TraceUtility.WriteTrace(this.GetType(), "RuleNameFromMethodName", null, String.Format("Could not obtain Method information from the Method name. TypeName: {0}, MethodName: {1}", customContainer.GetType().FullName, methodName), TraceUtility.TraceType.Warning);
                return null;
            }

            PermissionRuleMethodAttribute permissionAttribute = ruleMethodInfo.GetCustomAttribute<PermissionRuleMethodAttribute>();
            if (permissionAttribute == null)
            {
                TraceUtility.WriteTrace(this.GetType(), "RuleNameFromMethodName", null, String.Format("The {1} method in the {0} class was marked as a Permission Rule Method, but it exhibited an incorrect method signature for rule processing. A Permission Rule Method should accept the following parameters: (IIdentity userIdentity, IEnumerable<string> userRoles, Dictionary<string, object> contextPropertyBag).", customContainer.GetType().FullName, ruleMethodInfo.Name), TraceUtility.TraceType.Warning);
                return null;
            }

            return ((!String.IsNullOrWhiteSpace(permissionAttribute.Name)) ? permissionAttribute.Name : ruleMethodInfo.Name);
        }

        public Task<bool?> TaskFromPermissionRuleContext(PermissionRuleContext ruleContext, IIdentity userIdentity, IEnumerable<string> userRoles)
        {
            Task<bool?> ruleTask = new Task<bool?>(a => ruleContext.RuleDelegate.Invoke(userIdentity, userRoles, ruleContext.ContextObject, ruleContext.PropertyBag), ruleContext.ContextId);
            return ruleTask;
        }

        public Dictionary<string, PermissionRuleContextCollection> ReadRulesFromConfiguration()
        {
            Dictionary<string, PermissionRuleContextCollection> ruleContextLookup = new Dictionary<string, PermissionRuleContextCollection>();

            ApplicationPermissionConfigurationSettings config = null;

            try
            {
                config = (ApplicationPermissionConfigurationSettings)System.Configuration.ConfigurationManager.GetSection
                ("appPermissionConfiguration");
            }
            catch (Exception) { }

            if (config != null)
            {
                // process the core rules
                foreach (CorePermissionConfigurationElement configElement in config.CorePermissionConfigurationItems)
                {
                    PermissionRuleContext ruleContext = ConvertFromConfiguration(configElement);
                    AddRuleStateToLookup(ruleContextLookup, ruleContext);
                }

                // process the custom rules
                foreach (CustomPermissionConfigurationElement configElement in config.CustomPermissionConfigurationItems)
                {
                    PermissionRuleContext ruleContext = ConvertFromConfiguration(configElement);
                    AddRuleStateToLookup(ruleContextLookup, ruleContext);
                }
            }

            return ruleContextLookup;
        }

        private PermissionRuleContext ConvertFromConfiguration(CorePermissionConfigurationElement configElement)
        {
            if (String.IsNullOrWhiteSpace(configElement.PermissionName))
                return null;

            IsAuthorizedMethod ruleDelegate;
            string ruleName;
            Dictionary<string, object> propertyBag;
            if (!String.IsNullOrWhiteSpace(configElement.PermittedRole))
            {
                ruleDelegate = this.IsUserRoleAuthorizedRule;
                ruleName = "IsUserRoleAuthorizedRule";
                propertyBag = PrepareStateForRoleRule(configElement.PermittedRole);
            }
            else
            {
                ruleDelegate = this.IsUnrestrictedRule;
                ruleName = "IsUnrestrictedRule";
                propertyBag = PrepareStateForUnrestrictedRule(configElement.IsUnrestricted);
            }
            PermissionRuleContext ruleState = new PermissionRuleContext(ruleName, configElement.PermissionName, propertyBag, ruleDelegate);
            return ruleState;
        }

        private void AddRuleStateToLookup(Dictionary<string, PermissionRuleContextCollection> ruleContextLookup, PermissionRuleContext ruleContext)
        {
            if (ruleContext != null)
            {
                PermissionRuleContextCollection contextCollection;
                if (ruleContextLookup.ContainsKey(ruleContext.PermissionName))
                {
                    contextCollection = ruleContextLookup[ruleContext.PermissionName];
                }
                else
                {
                    contextCollection = new PermissionRuleContextCollection();
                    ruleContextLookup.Add(ruleContext.PermissionName, contextCollection);
                }
                contextCollection.Add(ruleContext);
            }
        }

        private PermissionRuleContext ConvertFromConfiguration(CustomPermissionConfigurationElement configElement)
        {
            if (String.IsNullOrWhiteSpace(configElement.PermissionName))
                return null;

            IsAuthorizedMethod ruleDelegate;
            Dictionary<string, object> propertyBag;
            ICustomRuleContainer customContainer = CustomRuleContainerFromTypeName(configElement.RuleTypeName);
            if (customContainer == null)
                return null;

            ruleDelegate = this.RuleDelegateFromMethodName(customContainer, configElement.RuleMethodName);
            if (ruleDelegate == null)
                return null;
            string ruleName = RuleNameFromMethodName(customContainer, configElement.RuleMethodName);

            propertyBag = customContainer.PreparePropertiesForRule(configElement.RuleMethodName, new string[] { configElement.Argument1, configElement.Argument2, configElement.Argument3, configElement.Argument4, configElement.Argument5 });
            if (propertyBag == null)
                propertyBag = new Dictionary<string, object>();

            PermissionRuleContext ruleState = new PermissionRuleContext(ruleName, configElement.PermissionName, propertyBag, ruleDelegate);
            return ruleState;
        }

        [PermissionRuleMethod]
        public bool? IsUserRoleAuthorizedRule(IIdentity userIdentity, IEnumerable<string> userRoles, object contextObject, Dictionary<string, object> contextPropertyBag)
        {
            if ((contextPropertyBag == null) || 
                (contextPropertyBag.Count == 0) || 
                (!contextPropertyBag.ContainsKey(__permittedRolePropertyTag)) || 
                (!(contextPropertyBag[__permittedRolePropertyTag] is string)) ||
                (String.IsNullOrWhiteSpace((string)contextPropertyBag[__permittedRolePropertyTag])))
                throw new ArgumentException("Invalid state for IsUserRoleAuthorized method.", "contextPropertyBag");

            string permittedRole = (string)contextPropertyBag[__permittedRolePropertyTag];
            foreach(string userRole in userRoles)
            {
                if (String.Compare(permittedRole, userRole, StringComparison.CurrentCultureIgnoreCase) == 0)
                    return true;
            }

            return false;
        }

        [PermissionRuleMethod]
        public bool? IsUnrestrictedRule(IIdentity userIdentity, IEnumerable<string> userRoles, object contextObject, Dictionary<string, object> contextPropertyBag)
        {
            if ((contextPropertyBag == null) ||
                (contextPropertyBag.Count == 0) ||
                (!contextPropertyBag.ContainsKey(__isUnrestrictedPropertyTag)) ||
                (!(contextPropertyBag[__isUnrestrictedPropertyTag] is bool)))
                throw new ArgumentException("Invalid state for IsUserRoleAuthorized method.", "contextPropertyBag");
                       
            return (bool)contextPropertyBag[__isUnrestrictedPropertyTag];
        }
    }
}
