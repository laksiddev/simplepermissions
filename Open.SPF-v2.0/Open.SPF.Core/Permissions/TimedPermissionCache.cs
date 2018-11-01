using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Open.Common.DependencyInjection;
using System.Security.Principal;
using Open.SPF.Configuration;
using Open.SPF.Core;
using CommonServiceLocator;

namespace Open.SPF.Permissions
{
    public class TimedPermissionCache : IPermissionCache
    {
        protected static IServiceLocator _locator = null;
        protected static IPermissionCache _instance = null;
        
        protected Dictionary<string, TimedUserRoles> _userRoleLookup;
 
        public TimedPermissionCache()
        {
            _userRoleLookup = new Dictionary<string, TimedUserRoles>();
        }

        public static IPermissionCache Instance
        {
            get
            {
                if (_instance == null)
                {
                    if (_locator == null)
                    {
                        _locator = DependencyInjectionServiceLocator.LocatorInstance;
                    }

                    _instance = _locator.GetInstance<IPermissionCache>();
                }

                return _instance;
            }
        }
       
        public virtual void SaveRolesForUser(IPrincipal currentUser, List<string> userRoles)
        {
            if (_userRoleLookup.ContainsKey(currentUser.Identity.Name))
                _userRoleLookup.Remove(currentUser.Identity.Name);

            TimedUserRoles timedUserRoles = new TimedUserRoles(DateTime.Now, userRoles);
            _userRoleLookup.Add(currentUser.Identity.Name, timedUserRoles);
        }

        public virtual List<string> GetRolesForUser(IPrincipal currentUser)
        {
            if (_userRoleLookup.ContainsKey(currentUser.Identity.Name))
            {
                TimedUserRoles timedUserRoles = _userRoleLookup[currentUser.Identity.Name];
                if (timedUserRoles != null)
                {
                    string permissionCacheTimeoutMinutesString = SpfConfiguration.Item(ApplicationConstants.AppSettings.PermissionCacheTimeoutMinutes);
                    string permissionCacheIsRollingTimeoutString = SpfConfiguration.Item(ApplicationConstants.AppSettings.PermissionCacheIsRollingTimeout);

                    int permissionCacheTimeoutMinutes;
                    bool isPermissionCacheIsRollingTimeout;
                    if ((Int32.TryParse(permissionCacheTimeoutMinutesString, out permissionCacheTimeoutMinutes)) &&
                        (Boolean.TryParse(permissionCacheIsRollingTimeoutString, out isPermissionCacheIsRollingTimeout)))
                    {
                        if (timedUserRoles.LastAccess.AddMinutes(permissionCacheTimeoutMinutes) > DateTime.Now)
                        {
                            if (isPermissionCacheIsRollingTimeout)
                                timedUserRoles.LastAccess = DateTime.Now;
                            return timedUserRoles.UserRoles;
                        }
                    }

                    _userRoleLookup.Remove(currentUser.Identity.Name);
                }
            }

            return null;
        }

        protected class TimedUserRoles
        {
            public TimedUserRoles(DateTime lastAccess, List<string> userRoles)
            {
                LastAccess = lastAccess;
                UserRoles = userRoles;
            }

            public DateTime LastAccess { get; set; }

            public List<string> UserRoles { get; set; }
        }
    }
}
