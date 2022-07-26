using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Exceptions
{
    [Serializable]
    public class FailedToDeleteTestPlanTestCaseException : Exception
    {
        public FailedToDeleteTestPlanTestCaseException() : base("Failed to Delete testplantestcase")
        {

        }
    }
}
