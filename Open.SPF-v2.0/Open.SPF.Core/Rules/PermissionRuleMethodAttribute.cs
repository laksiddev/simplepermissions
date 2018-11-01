using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Open.SPF.Permissions
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PermissionRuleMethodAttribute : Attribute
    {
        public PermissionRuleMethodAttribute() : this(null)
        {

        }

        public PermissionRuleMethodAttribute(string name)
        {
            _name = name;
        }

        private string _name;
        public string Name
        {
            get { return _name; }
        }
    }
}
