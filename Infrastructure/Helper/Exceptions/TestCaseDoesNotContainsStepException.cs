using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Helper.Exceptions
{
	[Serializable]
	public class TestCaseDoesNotContainsStepException : Exception
	{
		public TestCaseDoesNotContainsStepException() : base("Testcase does not contain steps")
		{

		}
	}
}
