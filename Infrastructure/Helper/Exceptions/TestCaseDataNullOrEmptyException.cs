using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Exceptions
{
    [Serializable]
    public class TestCaseDataNullOrEmptyException : Exception
    {
        public TestCaseDataNullOrEmptyException() : base("Either Pre-Condition Or Expected Result  is empty")
        {

        }
    }
}
