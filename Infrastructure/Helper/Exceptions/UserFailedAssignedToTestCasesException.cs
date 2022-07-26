using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Exceptions
{
	[Serializable]
	public class UserFailedAssignedToTestCasesException : Exception
	{
		public UserFailedAssignedToTestCasesException() : base("User failed to assign to Test Cases")
		{
				
		}
	}
}

