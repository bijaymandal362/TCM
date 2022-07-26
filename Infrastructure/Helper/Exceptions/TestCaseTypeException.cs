using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Exceptions
{
    [Serializable]
    public class TestCaseTypeException : Exception
    {
        public TestCaseTypeException(): base("Please check testcasetype")
        {

        }
    }
}
