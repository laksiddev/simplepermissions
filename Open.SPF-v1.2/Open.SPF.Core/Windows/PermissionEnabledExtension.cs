using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

using Open.SPF.Core;

namespace Open.SPF.Windows
{
    [MarkupExtensionReturnType(typeof(bool))]
    public class PermissionEnabledExtension : MarkupExtension
    {
        private string[] _permissions { get; set; }

        public PermissionEnabledExtension()
        {
            _permissions = null;
        }

        public PermissionEnabledExtension(string permissions)
        {
            _permissions = permissions.Split(','); ;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return (PermissionManager.AssertPermission(_permissions));
        }
    }
}
