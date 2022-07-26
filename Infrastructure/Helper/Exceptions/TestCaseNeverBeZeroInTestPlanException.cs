using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Exceptions
{
    [Serializable]
    public class TestCaseNeverBeZeroInTestPlanException : Exception
    {
        public TestCaseNeverBeZeroInTestPlanException() : base("Please add atleast one testcase for testplan")
        {

        }
    }
}
