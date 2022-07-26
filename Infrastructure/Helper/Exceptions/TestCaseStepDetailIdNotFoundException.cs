using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Exceptions
{
    [Serializable]
    public   class TestCaseStepDetailIdNotFoundException : Exception
    {
        public TestCaseStepDetailIdNotFoundException() : base("TestCaseStepDetailId not found exception")
        {

        }
    }
}
