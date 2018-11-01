using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Open.SPF.Core.Test
{
    public class UnitTestServiceImpl : IUnitTestService
    {
        public string UnitTestMethod()
        {
            return "SUCCESS";
        }
    }
}
