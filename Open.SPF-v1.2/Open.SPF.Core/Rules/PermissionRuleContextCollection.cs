using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Open.SPF.Core
{
    public class PermissionRuleContextCollection
    {
          private List<PermissionRuleContext> _contextList;

        public PermissionRuleContextCollection()
        {
            _contextList = new List<PermissionRuleContext>();
        }

        public IEnumerable<PermissionRuleContext> Items
        {
            get { return _contextList; }
        }

        public int Count
        {
            get { return _contextList.Count; }
        }

        public PermissionRuleContext this[Guid contextId]
        {
            get
            {
                PermissionRuleContext context = null;
                foreach (PermissionRuleContext testContext in _contextList)
                {
                    if (testContext.ContextId == contextId)
                    {
                        context = testContext;
                        break;
                    }
                }

                return context;
            }
        }

        public void Add(PermissionRuleContext context)
        {
            _contextList.Add(context);
        }
    }
}
