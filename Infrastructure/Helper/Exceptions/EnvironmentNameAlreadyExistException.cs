using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Helper.Exceptions
{
	[Serializable]
	public class EnvironmentNameAlreadyExistException :Exception
	{
		public EnvironmentNameAlreadyExistException() : base("Environment name already exists,please enter a unique name.")
		{

		}
	}
}
