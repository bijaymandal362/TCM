using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Exceptions
{
    [Serializable]
    public class TestCaseDetailIdNotFoundException : Exception
    {
        public TestCaseDetailIdNotFoundException():base("TestCaseDetailId is not found")
        {

        }
    }
}
