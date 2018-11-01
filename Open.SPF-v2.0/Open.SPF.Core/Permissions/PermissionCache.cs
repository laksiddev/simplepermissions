﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Open.Common.DependencyInjection;
using System.Security.Principal;
using CommonServiceLocator;

namespace Open.SPF.Permissions
{
    public class PermissionCache : IPermissionCache
    {
        protected static IServiceLocator _locator = null;
        protected static IPermissionCache _instance = null;
        
        protected Dictionary<string, List<string>> _userRoleLookup;
 
        public PermissionCache()
        {
            _userRoleLookup = new Dictionary<string, List<string>>();
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

            _userRoleLookup.Add(currentUser.Identity.Name, userRoles);
        }

        public virtual List<string> GetRolesForUser(IPrincipal currentUser)
        {
            if (_userRoleLookup.ContainsKey(currentUser.Identity.Name))
                return _userRoleLookup[currentUser.Identity.Name];

            return null;
        }
    }
}
