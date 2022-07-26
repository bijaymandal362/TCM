using System;

namespace Data.Exceptions
{
    [Serializable]
    public class TestRunTestCaseHistoryIdNotFoundException : Exception
    {
        public TestRunTestCaseHistoryIdNotFoundException() : base("TestRunTestCaseHistoryId not found")
        {

        }
    }
}
