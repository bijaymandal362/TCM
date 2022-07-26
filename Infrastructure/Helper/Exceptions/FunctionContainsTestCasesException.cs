using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Helper.Exceptions
{
	[Serializable]
	public class FunctionContainsTestCasesException : Exception
	{
		public FunctionContainsTestCasesException() : base("Function cannnot be deleted,  it contains TestCases")
		{

		}
	}
}


