using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Open.SPF.Permissions
{
    public class PermissionResultCollection
    {
        private List<PermissionResult> _resultList;

        public PermissionResultCollection()
        {
            _resultList = new List<PermissionResult>();
        }

        public bool IsAuthorized
        {
            get
            {
                if ((_resultList != null) && (_resultList.Count != 0))
                {
                    foreach (PermissionResult result in _resultList)
                    {
                        if ((result.IsAuthorized.HasValue) && (result.IsAuthorized.Value))
                            return true;
                    }
                }

                return false;
            }
        }

        public IEnumerable<PermissionResult> Items
        {
            get { return _resultList;  }
        }

        public PermissionResult this[int index]
        {
            get
            {
                return _resultList[index];
            }
        }

        public PermissionResult this[string ruleName]
        {
            get
            {
                PermissionResult result = null;
                foreach (PermissionResult testResult in _resultList)
                {
                    if (testResult.RuleName == ruleName)
                    {
                        result = testResult;
                        break;
                    }
                }

                return result;
            }
        }

        public void Add(PermissionResult result)
        {
            _resultList.Add(result);
        }
    }
}
