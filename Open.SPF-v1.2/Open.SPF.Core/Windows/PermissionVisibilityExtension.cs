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
    [MarkupExtensionReturnType(typeof(Visibility))]
    public class PermissionVisibilityExtension : MarkupExtension
    {
        private string[] _permissions { get; set; }

        public PermissionVisibilityExtension()
        {
            _permissions = null;
        }

        public PermissionVisibilityExtension(string permissions)
        {
            _permissions = permissions.Split(','); ;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (PermissionManager.AssertPermission(_permissions))
                return Visibility.Visible;

            return Visibility.Collapsed;
        }
    }
}
