using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Helper.Exceptions
{
	[Serializable]
	public class ProjectFunctionNameAlreadyExistException : Exception
	{
		public ProjectFunctionNameAlreadyExistException() : base("Function Name already exists, please enter a unique name")
		{

		}
	}
}
