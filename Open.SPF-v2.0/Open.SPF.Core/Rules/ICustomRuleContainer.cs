using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Open.SPF.Permissions
{
    public interface ICustomRuleContainer
    {
        Dictionary<string, object> PreparePropertiesForRule(string methodName, string[] args);
        //List<object> CustomRuleTaskList { get; }
    }
}
