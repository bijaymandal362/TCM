using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Exceptions
{
    [Serializable]
    public class TestRunTitleCannotBeEmptyException : Exception
    {
        public TestRunTitleCannotBeEmptyException(): base("TestRunTitle cannot be empty")
        {

        }
    }
}
