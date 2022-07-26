using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Helper.Exceptions
{
	[Serializable]
	public class ProjectTestCaseNameAlreadyExistException : Exception
	{
		public ProjectTestCaseNameAlreadyExistException() : base("TestCase Name already exists, please enter a unique name")
		{

		}
	}
}
