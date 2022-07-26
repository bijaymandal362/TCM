using System;

namespace Infrastructure.Helper.Exceptions
{
    public class TestRunIdTestPlanIdNotFoundException : Exception
    {
        public TestRunIdTestPlanIdNotFoundException():base("TestRunId and TestPlanId not found")
        {

        }
    }
}
