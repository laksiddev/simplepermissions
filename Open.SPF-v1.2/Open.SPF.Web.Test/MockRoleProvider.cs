using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace Open.SPF.Web.Test
{
    public class MockRoleProvider : RoleProvider
    {
        private List<string> _userRoles;
        private List<string> _allRoles;

        public MockRoleProvider()
        {
            _userRoles = new List<string>();
            _userRoles.Add("FirstRole");
            _userRoles.Add("ThirdRole");

            _allRoles = new List<string>();
            _allRoles.AddRange(_userRoles);
            _allRoles.Add("SecondRole");
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            return _userRoles.Contains(roleName);
        }

        public override bool RoleExists(string roleName)
        {
            return _allRoles.Contains(roleName);
        }

        public override string[] GetAllRoles()
        {
            return _allRoles.ToArray();
        }

        public override string[] GetRolesForUser(string username)
        {
            return _userRoles.ToArray();
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            
        }

        public override void CreateRole(string roleName)
        {
            
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            return true;
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
 	        return new string[0];
        }

        public override string Name
        {
            get
            {
                return "MockProvider";
            }
        }

        public override string ApplicationName
        {
            get
            {
                return "Open.SPF.Web.Test";
            }
            set
            {
                
            }
        }

        public override string[] GetUsersInRole(string roleName)
        {
            return new string[0];
        }

        public override string Description
        {
            get
            {
                return ApplicationName + "." + Name;
            }
        }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {

        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            
        }
    }
}
