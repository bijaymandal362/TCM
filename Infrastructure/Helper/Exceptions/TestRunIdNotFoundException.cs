using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Exceptions
{
    [Serializable]
    public class TestRunIdNotFoundException : Exception
    {
        public TestRunIdNotFoundException() : base("TestRunId not found")
        {

        }
    }
}
