using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Exceptions
{
    [Serializable]
    public class TestPlanTestCaseCountZeroException : Exception
    {
        public TestPlanTestCaseCountZeroException():base("Please add testcase to add testplan")
        {

        }
    }
}
