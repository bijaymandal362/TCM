using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Exceptions
{
    [Serializable]
    public class TestCaseIdNotFoundException : Exception
    {
        public TestCaseIdNotFoundException() : base("TestCaseId not found")
        {

        }
    }
}
