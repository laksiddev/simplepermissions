﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Claims;
using System.Security.Principal;
using System.Reflection;

using Open.Common.DependencyInjection;
//using Open.Common.Utility;
using System.Threading.Tasks;
using CommonServiceLocator;
using Microsoft.Extensions.DependencyInjection;

namespace Open.SPF.Permissions
{
    public class PermissionManager : IPermissionManager
    {
        protected static IServiceLocator _locator = null;
        protected static IPermissionManager _instance = null;

        protected IPermissionCache _cache = null;
        protected Dictionary<string, PermissionRuleContextCollection> _permissionRulesLookup;
        protected Dictionary<string, Dictionary<string, bool>> _authorizationFlagLookup;
        protected List<string> _permissionRoles;
        protected PermissionRules _coreRules;

        protected List<MethodInfo> _availableRules;

        public PermissionManager()
        {
        }

        public static IPermissionManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    if (_locator == null)
                    {
                        _locator = DependencyInjectionServiceLocator.LocatorInstance;
                    }

                    _instance = _locator.GetInstance<IPermissionManager>();
                    if (_instance is PermissionManager)
                        ((PermissionManager)_instance).Initialze();
                }

                return _instance;
            }
        }

        public static bool AssertPermission(string[] permissionNames)
        {
            if ((permissionNames == null) || (permissionNames.Length == 0))
                return false;

            return (permissionNames.Any(permission => AssertPermission(permission)));
        }

        public static bool AssertPermission(string permissionName)
        {
            return Instance.DoAssertPermission(permissionName);
        }

        public static bool AssertPermission(string permissionName, object contextObject, Dictionary<string, object> contextProperties)
        {
            return Instance.DoAssertPermission(permissionName, contextObject, contextProperties);
        }

        public bool DoAssertPermission(string permissionName)
        {
            bool isPermitted;

            IPrincipal currentUser = CurrentUser;
            Dictionary<string, bool> authorizationFlags;
            List<string> userRoles;

            lock (_authorizationFlagLookup)
            {
                userRoles = GetUserRoles(currentUser);

                if (_authorizationFlagLookup.ContainsKey(currentUser.Identity.Name))
                {
                    authorizationFlags = _authorizationFlagLookup[currentUser.Identity.Name];
                    if (authorizationFlags.ContainsKey(permissionName))
                        return authorizationFlags[permissionName];
                }
                else
                {
                    authorizationFlags = new Dictionary<string, bool>();
                    try
                    {
                        _authorizationFlagLookup.Add(currentUser.Identity.Name, authorizationFlags);
                    }
                    catch (System.ArgumentException argEx)
                    {
                        // Record the error, then rethrow it so that it is made obvious that an error occurred
                        Open.Common.Utility.TraceUtility.LogException(argEx);

                        string itemsAlreadyInList = String.Join(", ", _authorizationFlagLookup.Keys.ToArray());
                        string detailMessage = String.Format("Here are details surrounding the previous error. Key attempted to be added: {0}. Items already in the list: {1}.", currentUser.Identity.Name, itemsAlreadyInList);
                        Open.Common.Utility.TraceUtility.LogWarningMessage(detailMessage);

                        throw new ApplicationException("An error occurred while attempting to check permissions on a new user. A duplicate user name was found. The inner exception will have more details.", argEx);
                    }
                }
            }

            // else need to check permission
            isPermitted = DoAssertPermission(permissionName, currentUser.Identity, userRoles, null, null);

            lock (_authorizationFlagLookup)
            {
                // if permission existed, would have returned it around line 83. Therefore we need to add it now.
                try
                {
                    authorizationFlags.Add(permissionName, isPermitted);
                }
                catch (System.ArgumentException argEx)
                {
                    // Record the error, then rethrow it so that it is made obvious that an error occurred
                    Open.Common.Utility.TraceUtility.LogException(argEx);

                    if (authorizationFlags != null)
                    {
                        string itemsAlreadyInList = String.Join(", ", authorizationFlags.Keys.ToArray());
                        string detailMessage = String.Format("Here are details surrounding the previous error. Key attempted to be added: {0}. Items already in the list: {1}.", permissionName, itemsAlreadyInList);
                        Open.Common.Utility.TraceUtility.LogWarningMessage(detailMessage);
                    }

                    throw new ApplicationException("An error occurred while attempting to check a new permission on an existing user. A duplicate permission name was found. The inner exception will have more details.", argEx);
                }
            }

            return isPermitted;
        }

        public bool DoAssertPermission(string permissionName, object contextObject, Dictionary<string, object> contextProperties)
        {
            // This version CANNOT cache authorization flags becasue the rule context may change with each invocation
            IPrincipal currentUser = CurrentUser;

            // else need to check permission
            List<string> userRoles = GetUserRoles(currentUser);

            bool isPermitted = DoAssertPermission(permissionName, currentUser.Identity, userRoles, contextObject, contextProperties);

            return isPermitted;
        }

        public bool DoAssertPermission(string permissionName, IIdentity userIdentity, IEnumerable<string> userRoles, object contextObject, Dictionary<string, object> contextProperties)
        {
            PermissionResultCollection permissionResults = InquirePermission(permissionName, userIdentity, userRoles, contextObject, contextProperties);
            return permissionResults.IsAuthorized;
        }

        public bool InquirePermission(string permissionName, IIdentity userIdentity, IEnumerable<string> userRoles)
        {
            PermissionResultCollection results = InquirePermission(permissionName, userIdentity, userRoles, null, null);

            return results.IsAuthorized;
        }

        public PermissionResultCollection InquirePermission(string permissionName, IIdentity userIdentity, IEnumerable<string> userRoles, object contextObject, Dictionary<string, object> contextProperties)
        {
            if (!_permissionRulesLookup.ContainsKey(permissionName))
                return new PermissionResultCollection();   // There are no know rules for the requested permission, therefore the permission CANNOT be granted

            PermissionRuleContextCollection ruleContextCollection = _permissionRulesLookup[permissionName];
            List<Task<bool?>> taskList = new List<Task<bool?>>();
            PermissionRuleContextCollection invocationContextCollection = new PermissionRuleContextCollection();
            foreach (PermissionRuleContext configurationContext in ruleContextCollection.Items)
            {
                PermissionRuleContext invocationContext = new PermissionRuleContext(configurationContext, contextObject, contextProperties);
                invocationContextCollection.Add(invocationContext);
                Task<bool?> ruleTask = _coreRules.TaskFromPermissionRuleContext(invocationContext, userIdentity, userRoles);
                ruleTask.Start();
                taskList.Add(ruleTask);
            }

            Task.WaitAll(taskList.ToArray());

            PermissionResultCollection results = new PermissionResultCollection(); 
            foreach(Task<bool?> ruleTask in taskList)
            {
                PermissionRuleContext invocationContext = invocationContextCollection[(Guid)ruleTask.AsyncState];
                if (invocationContext == null)
                    throw new InvalidOperationException("An unexpected contition occurred while processing the permission rules. Could not identify the proper rule context.");

                results.Add(new PermissionResult(ruleTask.Result, invocationContext.RuleName));
            }

            return results;
        }

        public virtual IPrincipal CurrentUser
        {
            get { return ClaimsPrincipal.Current; }
        }

        public List<string> GetUserRoles()
        {
            return GetUserRoles(CurrentUser);
        }

        public List<string> GetUserRoles(IPrincipal currentUser)
        {
            Open.Common.Utility.TraceUtility.WriteInformationTrace(this.GetType(), "GetUerRoles", Open.Common.Utility.TraceUtility.TraceType.Begin);
            List<string> userRoles = _cache.GetRolesForUser(currentUser);
            if (userRoles == null)
            {
                userRoles = DoGetRolesForUser(currentUser);
                _cache.SaveRolesForUser(currentUser, userRoles);
                // clear authorization flags in case they were cached previously
                _authorizationFlagLookup.Remove(CurrentUser.Identity.Name);
                Open.Common.Utility.TraceUtility.WriteInformationTrace(this.GetType(), "GetUerRoles", null, "from DoGetRolesForUser()", Open.Common.Utility.TraceUtility.TraceType.End);
            }
            else
            {
                Open.Common.Utility.TraceUtility.WriteInformationTrace(this.GetType(), "GetUerRoles", null, "from cache", Open.Common.Utility.TraceUtility.TraceType.End);
            }

            return userRoles;
        }

        public virtual List<string> DoGetRolesForUser(IPrincipal currentUser)
        {
            Open.Common.Utility.TraceUtility.WriteInformationTrace(this.GetType(), "DoGetRolesForUser", Open.Common.Utility.TraceUtility.TraceType.Begin);

            Exception roleCheckException = null;
            List<string> userRoles = new List<string>();
            Open.Common.Utility.TraceUtility.WriteInformationTrace(this.GetType(), "DoGetRolesForUser", "foreach(permissionRole)", String.Format("username: {0}, authenticationType: {1}", currentUser.Identity.Name, ((currentUser.Identity.IsAuthenticated) ? currentUser.Identity.AuthenticationType : "NOT AUTHENTICATED")), Open.Common.Utility.TraceUtility.TraceType.Watch);
            foreach (string permissionRole in _permissionRoles)
            {
                try
                {
                    bool isUserInRole = currentUser.IsInRole(permissionRole);
                    Open.Common.Utility.TraceUtility.WriteInformationTrace(this.GetType(), "DoGetRolesForUser", "currentUser.IsInRole()", String.Format("role: {0}, isInRole: {1}", permissionRole, isUserInRole.ToString().ToLower()), Open.Common.Utility.TraceUtility.TraceType.Watch);
                    if (isUserInRole)
                        userRoles.Add(permissionRole);
                }
                catch (Exception ex)
                {
                    // This could fail and if it does, there's nothing we can do.
                    roleCheckException = ex;
                }
            }

            if (roleCheckException != null)
            {
                Open.Common.Utility.TraceUtility.LogWarningMessage(String.Format("There was an error reading the roles for the current user: {0}.\r\n\r\n{1}", currentUser.Identity.Name, Open.Common.Utility.TraceUtility.FormatExceptionMessage(roleCheckException)));
            }

            Open.Common.Utility.TraceUtility.WriteInformationTrace(this.GetType(), "DoGetRolesForUser", null, "from currentUser.IsInRole(permissionRole)", Open.Common.Utility.TraceUtility.TraceType.End);
            return userRoles;
        }

        public void Initialze()
        {
            Open.Common.Utility.TraceUtility.WriteInformationTrace(this.GetType(), "Initialze", Open.Common.Utility.TraceUtility.TraceType.Begin);
            _cache = PermissionCache.Instance;

            _coreRules = new PermissionRules();
            _permissionRulesLookup = _coreRules.ReadRulesFromConfiguration();
            _authorizationFlagLookup = new Dictionary<string, Dictionary<string, bool>>();
            _permissionRoles = ReadPermissionRolesFromRules(_permissionRulesLookup);

            Open.Common.Utility.TraceUtility.WriteInformationTrace(this.GetType(), "Initialze", Open.Common.Utility.TraceUtility.TraceType.End);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            Open.Common.Utility.TraceUtility.WriteInformationTrace(this.GetType(), "ConfigureServices(IServiceCollection)", Open.Common.Utility.TraceUtility.TraceType.Begin);
            if (_cache == null)
            {
                Initialze();
            }

            foreach (string permissionName in _permissionRulesLookup.Keys)
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy(permissionName,
                        policy => policy.Requirements.Add(new SimplePermissionRequirement(permissionName)));
                });
            }

            Open.Common.Utility.TraceUtility.WriteInformationTrace(this.GetType(), "ConfigureServices(IServiceCollection)", Open.Common.Utility.TraceUtility.TraceType.End);
        }

        protected List<string> ReadPermissionRolesFromRules(Dictionary<string, PermissionRuleContextCollection> permissionRulesLookup)
        {
            List<string> permissionRoles = new List<string>();
            foreach(string permissionName in permissionRulesLookup.Keys)
            {
                PermissionRuleContextCollection contextCollection = permissionRulesLookup[permissionName];
                foreach(PermissionRuleContext rule in contextCollection.Items)
                {
                    if (!rule.PropertyBag.ContainsKey("PermittedRole"))
                        continue;

                    string permittedRole = (string)rule.PropertyBag["PermittedRole"];
                    if (!permissionRoles.Contains(permittedRole))
                        permissionRoles.Add(permittedRole);
                }
            }

            return permissionRoles;
        }
    }
}
