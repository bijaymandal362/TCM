using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Exceptions
{
	[Serializable]
	public class UserFailedUnAssignedToTestCasesException : Exception
	{
		public UserFailedUnAssignedToTestCasesException() : base("User failed to unassign to Test Cases")
		{

		}
	}
}

