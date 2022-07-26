using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Exceptions
{
    [Serializable]
    public class TestCaseStepDetailDataNullOrEmptyException : Exception
    {
        public TestCaseStepDetailDataNullOrEmptyException() : base("Either  Step Description Or Expected Result  is empty")
        {

        }
    }
}
