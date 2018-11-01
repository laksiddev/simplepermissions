using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Open.SPF.Permissions
{
    public class PermissionRuleContext
    {
        public PermissionRuleContext(string ruleName, string permissionName, PermissionRules.IsAuthorizedMethod ruleDelegate)
            : this(ruleName, permissionName, null, ruleDelegate) { }

        public PermissionRuleContext(string ruleName, string permissionName, Dictionary<string, object> propertyBag, PermissionRules.IsAuthorizedMethod ruleDelegate)
            : this(ruleName, permissionName, null, propertyBag, ruleDelegate) { }

        public PermissionRuleContext(string ruleName, string permissionName, object contextObject, Dictionary<string, object> propertyBag, PermissionRules.IsAuthorizedMethod ruleDelegate)
        {
            ContextId = Guid.NewGuid();
            RuleName = ruleName;
            PermissionName = permissionName;
            ContextObject = contextObject;
            PropertyBag = propertyBag ?? new Dictionary<string, object>();
            RuleDelegate = ruleDelegate;
        }

        public PermissionRuleContext(PermissionRuleContext clonedContext) : this(clonedContext, null, null) { }

        public PermissionRuleContext(PermissionRuleContext clonedContext, object contextObject, Dictionary<string, object> additionalProperties)
        {
            ContextId = Guid.NewGuid();
            RuleName = clonedContext.RuleName;
            PermissionName = clonedContext.PermissionName;
            ContextObject = contextObject ?? clonedContext.ContextObject;
            PropertyBag = MergeProperties(clonedContext.PropertyBag, additionalProperties);
            RuleDelegate = clonedContext.RuleDelegate;
        }

        public Guid ContextId { get; protected set; }
        public string RuleName { get; protected set; }
        public string PermissionName { get; protected set; }
        public object ContextObject { get; protected set; }
        public Dictionary<string, object> PropertyBag { get; protected set; }
        public PermissionRules.IsAuthorizedMethod RuleDelegate { get; protected set; }

        protected Dictionary<string, object> MergeProperties(Dictionary<string, object> destPropertyBag, Dictionary<string, object> srcPropertyBag)
        {
            Dictionary<string, object> mergedPropertyBag;
            if (srcPropertyBag != null)
            {
                mergedPropertyBag = new Dictionary<string, object>(srcPropertyBag);

                if (destPropertyBag != null)
                {
                    foreach (string key in destPropertyBag.Keys)
                    {
                        // If there is a conflict, the source property bag is the authority
                        // Last in wins
                        if (!mergedPropertyBag.ContainsKey(key))
                        {
                            mergedPropertyBag.Add(key, destPropertyBag[key]);
                        }
                    }
                }
            }
            else if (destPropertyBag != null)
            {
                mergedPropertyBag = new Dictionary<string, object>(destPropertyBag);
            }
            else
            {
                mergedPropertyBag = new Dictionary<string, object>();
            }

            return mergedPropertyBag;
        }
    }
}
